import os

dir_path = "e:/CodeFolder/Github_project/BookRecProject/Backend"

replacements = [
    ("UserId", "user_id"),
    ("Username", "user_name"),
    ("Email", "email"),
    ("HashedPassword", "hashed_password"),
    ("Role", "role"),
    ("Sex", "sex"),
    ("CurrentBalance", "current_balance"),
    ("favorite_books", "FavoriteBooks"),
    ("purchased_books", "PurchasedBooks")
]

review_replacements = [
    ("Id", "id")
]

for root, dirs, files in os.walk(dir_path):
    if "obj" in root or "bin" in root or "Migrations" in root:
        continue
    for file in files:
        if file.endswith(".cs"):
            filepath = os.path.join(root, file)
            with open(filepath, 'r', encoding='utf-8') as f:
                content = f.read()
            
            new_content = content
            
            # Context-aware replacement to avoid replacing things like Microsoft.AspNetCore.Http
            for old, new in replacements:
                # Replace property accesses like .UserId or object initialization like UserId = 
                new_content = new_content.replace(f".{old}", f".{new}")
                new_content = new_content.replace(f"{old} =", f"{new} =")
                new_content = new_content.replace(f"{old}=", f"{new}=")
                new_content = new_content.replace(f"{old} ", f"{new} ")
                new_content = new_content.replace(f"{old},", f"{new},")
                new_content = new_content.replace(f"{old};", f"{new};")
                new_content = new_content.replace(f"{old})", f"{new})")
                new_content = new_content.replace(f"{old}\n", f"{new}\n")
            
            if "Review" in file or "Rating" in file or "GetRatings" in file or "GetReviews" in file:
                new_content = new_content.replace(".Id", ".id")
                new_content = new_content.replace("Id =", "id =")
                new_content = new_content.replace("Id=", "id=")

            if new_content != content:
                with open(filepath, 'w', encoding='utf-8') as f:
                    f.write(new_content)
                print(f"Updated {filepath}")
