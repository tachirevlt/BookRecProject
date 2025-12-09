import { useEffect, useState } from 'react';
import { useParams, useNavigate, Link } from 'react-router-dom';
import { bookService } from '../services/bookService';
import { authService } from '../services/authService';
import { userService } from '../services/userService';
import type { Book } from '../types/Book';

export const BookDetailPage = () => {
  const { id } = useParams(); 
  const navigate = useNavigate();
  
  const [book, setBook] = useState<Book | null>(null);
  const [loading, setLoading] = useState(true);
  
  const [isLiked, setIsLiked] = useState(false);
  const [isAdmin, setIsAdmin] = useState(false); // State kiểm tra quyền Admin
  
  const user = authService.getCurrentUser();

  useEffect(() => {
    const loadData = async () => {
      if (!id) return;

      try {
        // 1. Tải thông tin sách
        const data = await bookService.getById(id);
        setBook(data);

        // 2. Nếu đã đăng nhập: Kiểm tra Like và Role Admin
        if (user && user.userId) {
          const profile = await userService.getProfile(user.userId);
          
          // Check Admin
          if (profile.role === 'Admin') {
              setIsAdmin(true);
          }

          // Check Like
          const found = profile.favoriteBooks.some((b: any) => {
             const favId = b.bookId || b.BookId || b.id;
             return String(favId).toLowerCase() === String(id).toLowerCase();
          });
          setIsLiked(found);
        }

      } catch (err) {
        console.error("Lỗi tải trang chi tiết:", err);
      } finally {
        setLoading(false);
      }
    };

    loadData();
  }, [id, user]);

  const handleToggleLike = async () => {
    if (!user || !user.userId) {
      alert("Bạn cần đăng nhập để thêm vào yêu thích!");
      navigate('/login');
      return;
    }
    if (!id) return;

    try {
      if (isLiked) {
        await userService.removeFavorite(user.userId, id);
        setIsLiked(false);
      } else {
        await userService.addFavorite(user.userId, id);
        setIsLiked(true);
      }
    } catch (error) {
      alert("Lỗi khi thả tim. Có thể API bị lỗi.");
    }
  };

  if (loading) return <div style={{padding: '100px', textAlign: 'center', color: '#666'}}>⏳ Đang tải thông tin sách...</div>;
  if (!book) return <div style={{padding: '100px', textAlign: 'center', color: 'red'}}>❌ Không tìm thấy sách hoặc sách đã bị xóa.</div>;

  return (
    <div style={{ background: '#F4F7FF', minHeight: '100vh', padding: '40px 20px' }}>
      
      {/* Container giới hạn 1440px giống HomePage */}
      <div style={{ maxWidth: '1440px', margin: '0 auto' }}>
        
        {/* Breadcrumb / Back Button */}
        <button 
            onClick={() => navigate(-1)} 
            style={{ 
                marginBottom: '20px', cursor: 'pointer', 
                padding: '10px 20px', background: 'white', 
                border: '1px solid #ddd', borderRadius: '8px',
                display: 'flex', alignItems: 'center', gap: '5px',
                fontWeight: '600', color: '#555'
            }}
        >
            ⬅ Quay lại
        </button>

        <div style={{ 
            display: 'flex', gap: '40px', 
            background: 'white', padding: '40px', borderRadius: '20px',
            boxShadow: '0 4px 20px rgba(0,0,0,0.05)',
            flexWrap: 'wrap'
        }}>
            
            {/* CỘT TRÁI: ẢNH BÌA (Placeholder) */}
            <div style={{ flex: '0 0 300px', display: 'flex', flexDirection: 'column', gap: '20px' }}>
                <div style={{ 
                    width: '100%', height: '450px', 
                    background: '#eee', borderRadius: '12px',
                    display: 'flex', alignItems: 'center', justifyContent: 'center',
                    fontSize: '60px', color: '#ccc', overflow: 'hidden'
                }}>
                    {/* Nếu có link ảnh thật thì dùng thẻ img ở đây */}
                    {/* {book.coverImage ? (
                        <img src={book.coverImage} alt={book.title} style={{width:'100%', height:'100%', objectFit:'cover'}} />
                    ) : (
                        <span>📖</span>
                    )} */}
                </div>

                {/* Nút hành động */}
                <button 
                    onClick={handleToggleLike}
                    style={{ 
                        width: '100%', padding: '15px', 
                        borderRadius: '10px', cursor: 'pointer',
                        fontWeight: 'bold', fontSize: '16px',
                        display: 'flex', alignItems: 'center', justifyContent: 'center', gap: '10px',
                        background: isLiked ? '#FFF0F1' : '#F4F7FF',
                        color: isLiked ? '#D9534F' : '#3056D3',
                        border: isLiked ? '1px solid #FFCDD2' : '1px solid #E0E7FF',
                        transition: 'all 0.2s'
                    }}
                >
                    {isLiked ? '❤️ Đã yêu thích' : '🤍 Thêm vào yêu thích'}
                </button>
            </div>

            {/* CỘT PHẢI: THÔNG TIN CHI TIẾT */}
            <div style={{ flex: 1, minWidth: '300px' }}>
                
                {/* Genres Tags */}
                <div style={{ display: 'flex', gap: '8px', flexWrap: 'wrap', marginBottom: '15px' }}>
                    {book.genres && book.genres.length > 0 ? (
                        book.genres.map((g, index) => (
                            <span key={index} style={{ 
                                background: '#E0E7FF', color: '#3056D3', 
                                padding: '6px 14px', borderRadius: '20px', 
                                fontSize: '13px', fontWeight: 'bold', textTransform: 'uppercase' 
                            }}>
                                {g}
                            </span>
                        ))
                    ) : (
                        <span style={{ background: '#eee', color: '#666', padding: '6px 14px', borderRadius: '20px', fontSize: '13px' }}>
                            Unknown Genre
                        </span>
                    )}
                </div>

                <h1 style={{ fontSize: '42px', color: '#090E34', margin: '0 0 10px 0', lineHeight: '1.2' }}>
                    {book.title}
                </h1>
                
                <h3 style={{ fontSize: '20px', color: '#637381', margin: '0 0 30px 0', fontWeight: '500' }}>
                    by <span style={{ color: '#090E34', fontWeight: 'bold' }}>{book.author}</span> • {book.year}
                </h3>

                {/* Chỉ số Rating */}
                <div style={{ display: 'flex', alignItems: 'center', gap: '30px', padding: '20px', background: '#FAFAFA', borderRadius: '12px', marginBottom: '30px' }}>
                    <div>
                        <div style={{ fontSize: '28px', fontWeight: 'bold', color: '#F59E0B' }}>
                            {book.average_rating} <span style={{fontSize:'16px', color:'#ccc'}}>/ 5</span>
                        </div>
                        <div style={{ fontSize: '13px', color: '#637381' }}>Average Rating</div>
                    </div>
                    <div style={{ width: '1px', height: '40px', background: '#ddd' }}></div>
                    <div>
                        <div style={{ fontSize: '28px', fontWeight: 'bold', color: '#090E34' }}>
                            {book.ratings?.toLocaleString() || 0}
                        </div>
                        <div style={{ fontSize: '13px', color: '#637381' }}>Total Ratings</div>
                    </div>
                    <div style={{ width: '1px', height: '40px', background: '#ddd' }}></div>
                    <div>
                        <div style={{ fontSize: '28px', fontWeight: 'bold', color: '#090E34' }}>
                            {book.language_code?.toUpperCase() || 'ENG'}
                        </div>
                        <div style={{ fontSize: '13px', color: '#637381' }}>Language</div>
                    </div>
                </div>

                {/* Mô tả */}
                <div style={{ marginBottom: '40px' }}>
                    <h4 style={{ fontSize: '18px', fontWeight: 'bold', color: '#090E34', marginBottom: '10px' }}>About this book</h4>
                    <p style={{ lineHeight: '1.8', color: '#555', fontSize: '16px' }}>
                        {/* {book.description || "Chưa có mô tả cho cuốn sách này."} */}
                        { "Chưa có mô tả cho cuốn sách này."}
                    </p>
                </div>

                {/* ISBN */}
                <div style={{ fontSize: '14px', color: '#999' }}>
                    ISBN: {book.isbn || 'N/A'} • Work ID: {book.work_id || 'N/A'}
                </div>

                {/* NÚT SỬA (CHỈ HIỆN CHO ADMIN) */}
                {isAdmin && (
                    <div style={{ marginTop: '40px', borderTop: '1px solid #eee', paddingTop: '20px' }}>
                        <Link to={`/edit/${id}`} style={{ textDecoration: 'none' }}>
                            <button style={{ 
                                background: '#090E34', color: 'white', 
                                padding: '12px 24px', borderRadius: '8px', 
                                border: 'none', cursor: 'pointer',
                                fontWeight: 'bold', fontSize: '15px',
                                display: 'inline-flex', alignItems: 'center', gap: '8px'
                            }}>
                                ✏️ Admin: Chỉnh sửa thông tin
                            </button>
                        </Link>
                    </div>
                )}

            </div>
        </div>
      </div>
    </div>
  );
};