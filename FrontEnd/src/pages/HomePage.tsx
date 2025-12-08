import { useEffect, useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { bookService } from '../services/bookService';
import { authService } from '../services/authService';
import { userService } from '../services/userService'; // Import service User
import { BookCard } from '../components/BookCard'; 
import type { Book } from '../types/Book';

export const HomePage = () => {
  const [books, setBooks] = useState<Book[]>([]);
  const navigate = useNavigate();
  
  // 1. Lấy User hiện tại (Quan trọng: Phải có userId)
  const user = authService.getCurrentUser();

  // State phân trang
  const [currentPage, setCurrentPage] = useState(1);
  const [totalPages, setTotalPages] = useState(0);
  const PAGE_SIZE = 4;

  // State lưu danh sách ID các cuốn sách đã like
  const [favoriteIds, setFavoriteIds] = useState<string[]>([]);

  // --- HÀM 1: Tải danh sách sách ---
  const loadBooks = async (page: number) => {
    try {
      const data = await bookService.getAll(page, PAGE_SIZE);
      if (data && data.items) {
        setBooks(data.items);
        setTotalPages(Math.ceil(data.totalCount / PAGE_SIZE));
      } else {
        setBooks([]);
      }
    } catch (error) {
      console.error("Lỗi tải sách:", error);
      setBooks([]);
    }
  };

  // --- HÀM 2: Tải danh sách Tim (Dựa trên API User mới) ---
  const loadFavorites = async () => {
    // Nếu chưa đăng nhập hoặc Token thiếu userId thì thôi
    if (!user || !user.userId) return;

    try {
      // Gọi đúng API: /api/Users/{userId}
      const profile = await userService.getProfile(user.userId);
      
      // Lấy ra danh sách ID sách để so sánh
      if (profile.favoriteBooks) {
        const ids = profile.favoriteBooks.map((b: any) => b.bookId || b.BookId || b.id);
        setFavoriteIds(ids);
      }
    } catch (error) {
      console.error("Lỗi tải danh sách yêu thích", error);
    }
  };

  // Chạy khi vào trang hoặc đổi trang
  useEffect(() => { 
    loadBooks(currentPage);
    loadFavorites(); // Gọi hàm tải tim
  }, [currentPage]);

  // --- HÀM 3: Xử lý Bấm Tim ---
  const handleToggleFavorite = async (bookId: string) => {
    if (!user || !user.userId) {
      alert("Vui lòng đăng nhập lại để lấy ID người dùng!");
      return;
    }

    try {
      if (favoriteIds.includes(bookId)) {
        // Đang thích -> Gọi API XÓA (truyền cả userId và bookId)
        await userService.removeFavorite(user.userId, bookId);
        setFavoriteIds(prev => prev.filter(id => id !== bookId));
      } else {
        // Chưa thích -> Gọi API THÊM
        await userService.addFavorite(user.userId, bookId);
        setFavoriteIds(prev => [...prev, bookId]);
      }
    } catch (error) {
      console.error(error);
      alert("Có lỗi khi thả tim. Kiểm tra lại API Backend.");
    }
  };

  const handleLogout = () => {
    authService.logout();
    navigate('/login');
  };

  const handleDelete = async (id: string) => {
    if (confirm("Xóa nhé?")) {
      await bookService.delete(id);
      loadBooks(currentPage);
    }
  };

  return (
    <div style={{ 
      minHeight: '100vh', 
      backgroundImage: "url('https://images.unsplash.com/photo-1524995997946-a1c2e315a42f?q=80&w=2000&auto=format&fit=crop')",
      backgroundSize: 'cover', backgroundAttachment: 'fixed', padding: '40px'
    }}>
      
      <div style={{ maxWidth: '1200px', margin: '0 auto', backgroundColor: 'rgba(255,255,255,0.95)', padding: '30px', borderRadius: '15px', boxShadow: '0 8px 32px rgba(0,0,0,0.1)' }}>

        {/* HEADER */}
        <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '30px', borderBottom: '2px solid #f0f0f0', paddingBottom: '20px' }}>
          <h1 style={{ color: '#2c3e50', margin: 0 }}>📚 Thư Viện Online</h1>
          
          <div style={{ display: 'flex', gap: '10px', alignItems: 'center' }}>
             {user ? (
                <>
                  
                  <Link to="/profile" style={styles.btnProfile}><span style={{fontWeight: 'bold', color: '#555'}}>Hi, {user.username || "User"}</span></Link>
                  <Link to="/create" style={styles.btnCreate}>+ Thêm sách</Link>
                  <button onClick={handleLogout} style={styles.btnLogout}>Thoát</button>
                </>
             ) : (
                <>
                  <Link to="/login" style={styles.btnLogin}>Đăng nhập</Link>
                  <Link to="/register" style={styles.btnRegister}>Đăng ký</Link>
                </>
             )}
          </div>
        </div>

        {/* DANH SÁCH SÁCH */}
        <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fill, minmax(280px, 1fr))', gap: '25px' }}>
          {books.map((book: any) => {
            const realId = book.bookId || book.BookId || book.id;
            const isLiked = favoriteIds.includes(realId);

            return (
              <div key={realId} className="book-card-container" style={{ position: 'relative', padding: '15px', display: 'flex', flexDirection: 'column' }}>
                
                {/* Nút Tim */}
                <button 
                  onClick={() => handleToggleFavorite(realId)}
                  style={{ 
                    position: 'absolute', top: '10px', right: '10px', zIndex: 10, 
                    background: 'white', border: 'none', borderRadius: '50%', 
                    width: '35px', height: '35px', cursor: 'pointer', 
                    boxShadow: '0 2px 5px rgba(0,0,0,0.2)', fontSize: '20px',
                    transition: 'transform 0.2s'
                  }}
                  title={isLiked ? "Bỏ thích" : "Yêu thích"}
                >
                  {isLiked ? '❤️' : '🤍'}
                </button>

                <Link to={`/book/${realId}`} style={{ textDecoration: 'none', color: 'inherit', flex: 1 }}>
                   <BookCard data={book} />
                </Link>

                {user && (
                   <div style={{ marginTop: '15px', display: 'flex', gap: '5px' }}>
                      <button onClick={() => navigate(`/edit/${realId}`)} style={styles.btnEdit}>Sửa</button>
                      <button onClick={() => handleDelete(realId)} style={styles.btnDelete}>Xóa</button>
                   </div>
                )}
              </div>
            );
          })}
        </div>

        {/* PHÂN TRANG */}
        <div style={{ display: 'flex', justifyContent: 'center', marginTop: '40px', gap: '15px', alignItems: 'center' }}>
           <button onClick={() => setCurrentPage(p => Math.max(1, p - 1))} disabled={currentPage === 1} style={styles.btnPage}>⬅ Trước</button>
           <span style={{ fontWeight: 'bold' }}>Trang {currentPage}</span>
           <button onClick={() => setCurrentPage(p => Math.min(totalPages, p + 1))} disabled={currentPage === totalPages} style={styles.btnPage}>Sau ➡</button>
        </div>

      </div>
    </div>
  );
};

// Style
const styles = {
  btnProfile: { background: '#ebe6ecff', color: 'white', padding: '8px 15px', textDecoration: 'none', borderRadius: '20px', fontWeight: 'bold', fontSize: '14px' },
  btnCreate: { background: '#2980b9', color: 'white', padding: '8px 15px', textDecoration: 'none', borderRadius: '20px', fontWeight: 'bold', fontSize: '14px' },
  btnLogout: { padding: '8px 15px', cursor: 'pointer', border: '1px solid #ddd', background: 'white', borderRadius: '20px', fontSize: '14px' },
  btnLogin: { padding: '8px 15px', background: 'white', border: '1px solid #333', textDecoration: 'none', borderRadius: '20px', color: '#333', fontWeight: 'bold' },
  btnRegister: { padding: '8px 15px', background: '#27ae60', textDecoration: 'none', borderRadius: '20px', color: 'white', fontWeight: 'bold' },
  btnEdit: { flex: 1, padding: '8px', cursor: 'pointer', background: '#f1c40f', border: 'none', borderRadius: '5px', color: 'white', fontWeight: 'bold' },
  btnDelete: { flex: 1, padding: '8px', cursor: 'pointer', background: '#e74c3c', color: 'white', border: 'none', borderRadius: '5px', fontWeight: 'bold' },
  btnPage: { padding: '8px 20px', cursor: 'pointer', background: '#fff', border: '1px solid #ccc', borderRadius: '5px' }
};