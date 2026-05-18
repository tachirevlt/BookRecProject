# -*- coding: utf-8 -*-
import sys, io
sys.stdout = io.TextIOWrapper(sys.stdout.buffer, encoding='utf-8')
sys.stderr = io.TextIOWrapper(sys.stderr.buffer, encoding='utf-8')

"""
Script import du lieu tu books_with_tags_enhanced_2.csv vao SQL Server.
- Doc tat ca 32 cot bao gom 8 cot moi: views_7d, favorite_7d, purchases_7d,
  views_30d, favorite_30d, purchases_30d, total_ratings, average_rating
- book_id trong CSV la int, duoc convert thanh GUID xac dinh (uuid.UUID(int=...))
- tags trong CSV la string, luu dang JSON string
- Xoa du lieu cu trong bang Books truoc khi import
"""

import pandas as pd
import pyodbc
import uuid
import json
import math
import os
import ast

# --- Doc connection string tu appsettings.json ---
APPSETTINGS_PATH = os.path.join(os.path.dirname(os.path.abspath(__file__)), "Api", "appsettings.json")
with open(APPSETTINGS_PATH, "r", encoding="utf-8") as f:
    cfg = json.load(f)

# Lay DefaultConnection trong ConnectionStringOptions
conn_str_ef = cfg.get("ConnectionStringOptions", {}).get("DefaultConnection", "")
if not conn_str_ef:
    print("Khong tim thay DefaultConnection trong appsettings.json")
    sys.exit(1)

print(f"Connection string EF: {conn_str_ef}")

# --- Phan tich EF connection string ---
parts = {}
for item in conn_str_ef.split(";"):
    if "=" in item:
        k, v = item.split("=", 1)
        parts[k.strip()] = v.strip()

server   = parts.get("Server", parts.get("server", ""))
database = parts.get("Database", parts.get("database", ""))
trusted  = parts.get("Trusted_Connection", parts.get("trusted_connection", "False"))

print(f"Server: {server}")
print(f"Database: {database}")

if trusted.lower() in ("true", "yes"):
    auth = "Trusted_Connection=yes;TrustServerCertificate=yes;"
else:
    uid = parts.get("User Id", parts.get("user id", ""))
    pwd = parts.get("Password", parts.get("password", ""))
    auth = f"UID={uid};PWD={pwd};TrustServerCertificate=yes;"

drivers = [
    "{ODBC Driver 17 for SQL Server}",
    "{ODBC Driver 18 for SQL Server}",
    "{SQL Server}",
]

conn = None
for drv in drivers:
    try:
        cs = f"DRIVER={drv};SERVER={server};DATABASE={database};{auth}"
        print(f"Thu ket noi: {cs}")
        conn = pyodbc.connect(cs, timeout=15)
        print(f"Ket noi thanh cong voi driver: {drv}")
        break
    except Exception as e:
        print(f"  That bai: {e}")

if conn is None:
    print("Khong the ket noi SQL Server.")
    sys.exit(1)

cursor = conn.cursor()

# --- Doc CSV ---
CSV_PATH = os.path.join(os.path.dirname(os.path.abspath(__file__)), "books_with_tags_enhanced_2.csv")
print(f"\nDoc CSV: {CSV_PATH}")
df = pd.read_csv(CSV_PATH, encoding="utf-8")
print(f"Tong so dong: {len(df)}")
print(f"Cac cot: {list(df.columns)}")

# --- Helper functions ---
def int_to_guid(n) -> str:
    return str(uuid.UUID(int=int(n)))

def safe_str(val):
    if val is None or (isinstance(val, float) and math.isnan(val)):
        return None
    return str(val)

def safe_int(val, default=0):
    if val is None or (isinstance(val, float) and math.isnan(val)):
        return default
    try:
        return int(val)
    except:
        return default

def safe_float(val):
    if val is None or (isinstance(val, float) and math.isnan(val)):
        return None
    try:
        v = float(val)
        return None if math.isnan(v) else v
    except:
        return None

def safe_decimal(val, default=0):
    if val is None or (isinstance(val, float) and math.isnan(val)):
        return default
    try:
        return float(val)
    except:
        return default

def safe_double(val, default=0.0):
    if val is None or (isinstance(val, float) and math.isnan(val)):
        return default
    try:
        v = float(val)
        return default if math.isnan(v) else v
    except:
        return default

def tags_to_json(val):
    if val is None or (isinstance(val, float) and math.isnan(val)):
        return "[]"
    s = str(val).strip()
    if s.startswith("["):
        try:
            lst = ast.literal_eval(s)
            return json.dumps([str(t).strip() for t in lst], ensure_ascii=False)
        except:
            pass
    lst = [t.strip() for t in s.split(",") if t.strip()]
    return json.dumps(lst, ensure_ascii=False)

# --- Xoa du lieu cu (theo thu tu FK) ---
print("\nXoa du lieu cu...")
for tbl in ["TrackingEvents", "UserFavoriteBooks", "UserPurchasedBooks", "Ratings", "Reviews", "Books"]:
    cursor.execute(f"DELETE FROM {tbl}")
    print(f"  Da xoa bang {tbl}")
conn.commit()
print("Da xoa xong.")

# --- Insert du lieu moi ---
INSERT_SQL = """
INSERT INTO Books (
    book_id, authors, original_publication_year, original_title,
    language_code, tags, ratings_1, ratings_2, ratings_3, ratings_4, ratings_5,
    image_url, small_image_url, price,
    mood, badge, description, longDescription, pages, readTime,
    status, chapters, previewText, accentColor,
    views_7d, favorite_7d, purchases_7d,
    views_30d, favorite_30d, purchases_30d,
    total_ratings, average_rating
) VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)
"""

BATCH_SIZE = 500
total = len(df)
success = 0
errors = 0

print(f"\nBat dau import {total} cuon sach...")

for batch_start in range(0, total, BATCH_SIZE):
    batch = df.iloc[batch_start : batch_start + BATCH_SIZE]
    rows = []
    for idx, row in batch.iterrows():
        try:
            rows.append((
                int_to_guid(row["book_id"]),
                safe_str(row.get("authors")) or "",
                safe_float(row.get("original_publication_year")),
                safe_str(row.get("original_title")) or "",
                safe_str(row.get("language_code")) or "",
                tags_to_json(row.get("tags")),
                safe_int(row.get("ratings_1")),
                safe_int(row.get("ratings_2")),
                safe_int(row.get("ratings_3")),
                safe_int(row.get("ratings_4")),
                safe_int(row.get("ratings_5")),
                safe_str(row.get("image_url")) or "",
                safe_str(row.get("small_image_url")) or "",
                safe_decimal(row.get("price")),
                safe_str(row.get("mood")),
                safe_str(row.get("badge")),
                safe_str(row.get("description")),
                safe_str(row.get("longDescription")),
                safe_int(row.get("pages")),
                safe_int(row.get("readTime")),
                safe_str(row.get("status")),
                safe_int(row.get("chapters")),
                safe_str(row.get("previewText")),
                safe_str(row.get("accentColor")),
                # 8 cot moi
                safe_int(row.get("views_7d")),
                safe_int(row.get("favorite_7d")),
                safe_int(row.get("purchases_7d")),
                safe_int(row.get("views_30d")),
                safe_int(row.get("favorite_30d")),
                safe_int(row.get("purchases_30d")),
                safe_int(row.get("total_ratings")),
                safe_double(row.get("average_rating")),
            ))
        except Exception as e:
            errors += 1
            print(f"  Loi parse dong {idx}: {e}")

    if rows:
        try:
            cursor.executemany(INSERT_SQL, rows)
            conn.commit()
            success += len(rows)
            print(f"  Da import: {success}/{total} ({success/total*100:.1f}%)")
        except Exception as e:
            conn.rollback()
            errors += len(rows)
            print(f"  Loi insert batch {batch_start}-{batch_start+BATCH_SIZE}: {e}")
            # Thu insert tung dong de xac dinh dong loi
            for r in rows:
                try:
                    cursor.execute(INSERT_SQL, r)
                    conn.commit()
                    success += 1
                    errors -= 1
                except Exception as e2:
                    conn.rollback()
                    print(f"    Dong loi (book_id={r[0]}): {str(e2)[:200]}")

cursor.close()
conn.close()

print(f"\n{'='*50}")
print(f"Hoan thanh import!")
print(f"  Thanh cong : {success} dong")
print(f"  Loi        : {errors} dong")
print(f"{'='*50}")
