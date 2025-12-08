import { useEffect, useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { bookService } from '../services/bookService';
import { authService } from '../services/authService';
import { userService } from '../services/userService'; // Import UserService
import type { Book } from '../types/Book';

export const BookDetailPage = () => {
  const { id } = useParams(); 
  const navigate = useNavigate();
  
  const [book, setBook] = useState<Book | null>(null);
  const [loading, setLoading] = useState(true);
  
  // State quản lý nút Tim
  const [isLiked, setIsLiked] = useState(false);
  
  // Lấy User hiện tại
  const user = authService.getCurrentUser();

  useEffect(() => {
    const loadData = async () => {
      if (!id) return;

      try {
        // 1. Tải thông tin sách
        const data = await bookService.getById(id);
        console.log("Chi tiết sách:", data);
        setBook(data);

        // 2. Kiểm tra xem User đã like cuốn này chưa (Nếu đã đăng nhập)
        if (user && user.userId) {
          const profile = await userService.getProfile(user.userId);
          
          // Tìm xem ID sách hiện tại có trong danh sách yêu thích không
          // (Dùng some để trả về true/false)
          const found = profile.favoriteBooks.some((b: any) => {
             const favId = b.bookId || b.BookId || b.id;
             // So sánh ID sách yêu thích với ID sách hiện tại (id từ URL)
             // Lưu ý: ép kiểu String để so sánh cho chắc ăn (vì có khi 1 cái là số, 1 cái là chữ)
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
  }, [id]);

  // Hàm xử lý bấm Tim
  const handleToggleLike = async () => {
    if (!user || !user.userId) {
      alert("Bạn cần đăng nhập để thêm vào yêu thích!");
      navigate('/login');
      return;
    }

    if (!id) return;

    try {
      if (isLiked) {
        // Đang thích -> Bấm là XÓA
        await userService.removeFavorite(user.userId, id);
        setIsLiked(false);
      } else {
        // Chưa thích -> Bấm là THÊM
        await userService.addFavorite(user.userId, id);
        setIsLiked(true);
      }
    } catch (error) {
      alert("Lỗi khi thả tim. Có thể API bị lỗi.");
    }
  };

  if (loading) return <div style={{padding: '50px', textAlign: 'center'}}>⏳ Đang tải...</div>;
  if (!book) return <div style={{padding: '50px', textAlign: 'center'}}>❌ Không tìm thấy sách</div>;

  return (
    <div style={{ padding: '40px', maxWidth: '900px', margin: '0 auto', fontFamily: 'Arial' }}>
      
      {/* Nút quay lại */}
      <button onClick={() => navigate('/')} style={{ marginBottom: '20px', cursor: 'pointer', padding: '8px 15px', background: '#eee', border: 'none', borderRadius: '5px' }}>
        ⬅ Quay lại danh sách
      </button>

      <div style={{ 
        border: '1px solid #ddd', padding: '40px', borderRadius: '15px', 
        backgroundColor: '#fff', boxShadow: '0 5px 20px rgba(0,0,0,0.05)',
        position: 'relative' // Để đặt nút Tim
      }}>
        
        {/* --- NÚT TIM (GÓC TRÊN PHẢI) --- */}
        <button 
            onClick={handleToggleLike}
            title={isLiked ? "Bỏ yêu thích" : "Thêm vào yêu thích"}
            style={{ 
              position: 'absolute', top: '30px', right: '30px',
              background: isLiked ? '#ffebee' : '#f5f5f5', 
              border: isLiked ? '1px solid #ffcdd2' : '1px solid #ddd',
              borderRadius: '50%', width: '50px', height: '50px',
              cursor: 'pointer', fontSize: '24px',
              display: 'flex', alignItems: 'center', justifyContent: 'center',
              transition: 'all 0.2s'
            }}
        >
            {isLiked ? '❤️' : '🤍'}
        </button>

        {/* Header: Tag và Năm */}
        <div style={{ display: 'flex', gap: '10px', marginBottom: '15px' }}>
          <span style={{ background: '#e3f2fd', color: '#0d47a1', padding: '5px 12px', borderRadius: '20px', fontSize: '14px', fontWeight: 'bold' }}>
            {book.tag_name || "Book"}
          </span>
          <span style={{ background: '#f3e5f5', color: '#7b1fa2', padding: '5px 12px', borderRadius: '20px', fontSize: '14px' }}>
            Năm: {book.year}
          </span>
        </div>

        {/* Tiêu đề & Tác giả */}
        <h1 style={{ fontSize: '36px', color: '#2c3e50', margin: '10px 0', paddingRight: '60px' }}>
            {book.title}
        </h1>
        <h3 style={{ color: '#555', fontWeight: 'normal', fontSize: '20px' }}>
            Tác giả: <strong style={{color: '#333'}}>{book.author}</strong>
        </h3>

        <hr style={{ border: '0', borderTop: '1px solid #eee', margin: '30px 0' }} />

        {/* Thông tin chi tiết */}
        <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '30px' }}>
          <div style={{ background: '#fafafa', padding: '20px', borderRadius: '10px' }}>
            <p style={{ margin: '10px 0' }}><strong>⭐ Đánh giá:</strong> {book.average_rating} / 5</p>
            <p style={{ margin: '10px 0' }}><strong>👥 Lượt đánh giá:</strong> {book.ratings?.toLocaleString()}</p>
          </div>
          <div style={{ background: '#fafafa', padding: '20px', borderRadius: '10px' }}>
             <p style={{ margin: '10px 0' }}><strong>🔢 Mã ISBN:</strong> {book.isbn || '---'}</p>
             <p style={{ margin: '10px 0' }}><strong>🌐 Ngôn ngữ:</strong> {book.language_code || '---'}</p>
          </div>
        </div>

        {/* Nút hành động */}
        {user && (
            <div style={{ marginTop: '40px', textAlign: 'right', display: 'flex', justifyContent: 'flex-end', gap: '15px' }}>
            <button 
                onClick={() => navigate(`/edit/${id}`)} 
                style={{ padding: '12px 24px', backgroundColor: '#ffc107', border: 'none', borderRadius: '8px', cursor: 'pointer', fontWeight: 'bold', fontSize: '16px', color: '#333' }}
            >
                ✏️ Chỉnh sửa thông tin
            </button>
            </div>
        )}

      </div>
    </div>
  );
};