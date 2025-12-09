import { useEffect, useState } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { bookService } from '../services/bookService';

export const BookFormPage = () => {
  const navigate = useNavigate();
  const { id } = useParams();
  const isEdit = Boolean(id); // Có ID là sửa, không có là thêm mới

  // 1. Khởi tạo State cho Form
  const [formData, setFormData] = useState({
    title: '',
    author: '',
    genres: ['classic'], // Giá trị mặc định
    year: new Date().getFullYear(),
    average_rating: 0,
    ratings: 0,
    // Các trường ẩn (Backend yêu cầu nhưng người dùng không cần nhập)
    books_count: 1,
    work_id: 'W-AUTO',
    isbn: 'AUTO',
    language_code: 'vi'
  });

  // 2. Nếu là chế độ Sửa -> Tải dữ liệu cũ đổ vào Form
  useEffect(() => {
    if (isEdit && id) {
      bookService.getById(id).then((data) => {
        // Map dữ liệu từ API vào Form (Lưu ý chữ hoa chữ thường của API nhé)
        setFormData({
            title: data.title,
            author: data.author,
            genres: data.genres,
            year: data.year,
            average_rating: data.average_rating,
            ratings: data.ratings,
            books_count: data.books_count || 1,
            work_id: data.work_id || '',
            isbn: data.isbn || '',
            language_code: data.language_code || 'vi'
        });
      }).catch(err => {
        console.error("Lỗi chi tiết:", err); 
        alert("Không tìm thấy sách!");
      });
    }
  }, [id, isEdit]);

  // 3. Hàm xử lý khi bấm LƯU
  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault(); // Chặn load lại trang
    
    try {
      if (isEdit && id) {
        // --- LOGIC SỬA ---
        await bookService.update(id, { ...formData, bookId: id });
        alert("Cập nhật thành công!");
      } else {
        // --- LOGIC THÊM MỚI ---
        // Backend tự tạo ID, nhưng nếu bắt buộc gửi lên thì ta gửi string rỗng hoặc null
        await bookService.create({ ...formData });
        alert("Thêm mới thành công!");
      }
      
      // Lưu xong thì quay về trang chủ
      navigate('/'); 
    } catch (error) {
      console.error(error);
      alert('Có lỗi xảy ra khi lưu! Kiểm tra console (F12).');
    }
  };

  // --- GIAO DIỆN FORM ---
  return (
    <div style={{ padding: '40px', backgroundColor: '#f4f6f8', minHeight: '100vh', display: 'flex', justifyContent: 'center' }}>
      <div style={{ background: 'white', padding: '30px', borderRadius: '10px', boxShadow: '0 4px 10px rgba(0,0,0,0.1)', width: '100%', maxWidth: '500px' }}>
        
        <h2 style={{ textAlign: 'center', color: '#333', marginBottom: '20px' }}>
          {isEdit ? '✏️ Cập nhật thông tin' : '✨ Thêm sách mới'}
        </h2>

        <form onSubmit={handleSubmit} style={{ display: 'flex', flexDirection: 'column', gap: '15px' }}>
          
          {/* Tên sách */}
          <div>
            <label style={{fontWeight: 'bold', display: 'block', marginBottom: '5px'}}>Tên sách (*)</label>
            <input 
              type="text" required
              value={formData.title}
              onChange={e => setFormData({...formData, title: e.target.value})}
              style={styles.input} placeholder="Ví dụ: Dế Mèn Phiêu Lưu Ký"
            />
          </div>

          {/* Tác giả */}
          <div>
            <label style={{fontWeight: 'bold', display: 'block', marginBottom: '5px'}}>Tác giả (*)</label>
            <input 
              type="text" required
              value={formData.author}
              onChange={e => setFormData({...formData, author: e.target.value})}
              style={styles.input} placeholder="Ví dụ: Tô Hoài"
            />
          </div>

          <div style={{ display: 'flex', gap: '15px' }}>
             {/* Năm xuất bản */}
            <div style={{ flex: 1 }}>
              <label style={{fontWeight: 'bold', display: 'block', marginBottom: '5px'}}>Năm XB</label>
              <input 
                type="number"
                value={formData.year}
                onChange={e => setFormData({...formData, year: Number(e.target.value)})}
                style={styles.input}
              />
            </div>

              <div style={{ flex: 1 }}>
                <label style={{fontWeight: 'bold', display: 'block', marginBottom: '5px'}}>
                    Thể loại (cách nhau dấu phẩy)
                </label>
                <input 
                  type="text"
                  // 1. HIỂN THỊ: Nối mảng ["A", "B"] thành chuỗi "A, B"
                  value={formData.genres.join(', ')} 
                  
                  // 2. CẬP NHẬT: Cắt chuỗi "A, B" ngược lại thành mảng ["A", "B"]
                  onChange={e => {
                      const value = e.target.value;
                      // Nếu xóa hết thì để mảng rỗng, ngược lại thì cắt dấu phẩy và xóa khoảng trắng thừa
                      const arrayValues = value ? value.split(',').map(item => item.trim()) : []; //
                      
                      setFormData({...formData, genres: arrayValues});
                  }}
                  
                  style={styles.input}
                  placeholder="Ví dụ: Classic, Fiction"
                />
              </div>
          </div>

          {/* Đánh giá */}
          <div>
            <label style={{fontWeight: 'bold', display: 'block', marginBottom: '5px'}}>Điểm đánh giá (0 - 5)</label>
            <input 
              type="number" step="0.1" min="0" max="5"
              value={formData.average_rating}
              onChange={e => setFormData({...formData, average_rating: Number(e.target.value)})}
              style={styles.input}
            />
          </div>

          {/* Các nút bấm */}
          <div style={{ display: 'flex', gap: '10px', marginTop: '20px' }}>
            <button type="button" onClick={() => navigate('/')} style={styles.btnCancel}>
              Hủy bỏ
            </button>
            <button type="submit" style={styles.btnSubmit}>
              {isEdit ? 'Lưu thay đổi' : 'Tạo sách mới'}
            </button>
          </div>

        </form>
      </div>
    </div>
  );
};

// CSS viết gọn tại đây
const styles = {
  input: {
    width: '100%', padding: '10px', borderRadius: '5px', border: '1px solid #ccc', fontSize: '16px', boxSizing: 'border-box' as 'border-box'
  },
  btnSubmit: {
    flex: 1, padding: '12px', background: '#007bff', color: 'white', border: 'none', borderRadius: '5px', cursor: 'pointer', fontWeight: 'bold', fontSize: '16px'
  },
  btnCancel: {
    flex: 1, padding: '12px', background: '#6c757d', color: 'white', border: 'none', borderRadius: '5px', cursor: 'pointer', fontWeight: 'bold', fontSize: '16px'
  }
};