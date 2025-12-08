import { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import { userService, type UserProfile } from '../services/userService';
import { authService } from '../services/authService';
import { BookCard } from '../components/BookCard';

export const UserDetailPage = () => {
  const [profile, setProfile] = useState<UserProfile | null>(null);
  const [loading, setLoading] = useState(true);

  // 1. Lấy User hiện tại đang đăng nhập
  const currentUser = authService.getCurrentUser();

  useEffect(() => {
    const loadData = async () => {
      // Kiểm tra xem đã có userId chưa
      if (!currentUser || !currentUser.userId) {
        alert("Không tìm thấy ID người dùng. Vui lòng đăng xuất và đăng nhập lại!");
        setLoading(false);
        return;
      }

      try {
        console.log("Đang gọi API User với ID:", currentUser.userId);
        
        // 👇 GỌI API VỚI ID CHÍNH XÁC
        const data = await userService.getProfile(currentUser.userId);
        
        setProfile(data);
      } catch (error) {
        console.error("Lỗi tải profile:", error);
      } finally {
        setLoading(false);
      }
    };

    loadData();
  }, []);

  if (loading) return <div style={{padding:'50px', textAlign:'center'}}>⏳ Đang tải thông tin...</div>;
  
  if (!profile) return (
    <div style={{padding:'50px', textAlign:'center'}}>
      ❌ Không tải được hồ sơ.<br/> 
      <small>Hãy kiểm tra lại xem Token có chứa userId chưa (Thử đăng xuất/đăng nhập lại).</small>
    </div>
  );

  return (
    <div style={{ padding: '40px', background: '#f8f9fa', minHeight: '100vh' }}>
      {/* HEADER PROFILE */}
      <div style={{ background: 'white', padding: '30px', borderRadius: '15px', maxWidth: '800px', margin: '0 auto 30px auto', textAlign: 'center', boxShadow: '0 4px 10px rgba(0,0,0,0.05)' }}>
         <h1 style={{margin: '0', color: '#2c3e50'}}>{profile.username}</h1>
         <p style={{color: '#777'}}>{profile.email}</p>
         <span style={{background: '#e3f2fd', color: '#0d47a1', padding: '5px 15px', borderRadius: '15px', fontSize: '12px', fontWeight: 'bold'}}>{profile.role}</span>
      </div>

      {/* DANH SÁCH YÊU THÍCH */}
      <div style={{ maxWidth: '1200px', margin: '0 auto' }}>
        <h3 style={{ borderLeft: '5px solid #ff4757', paddingLeft: '15px', color: '#333' }}>
           ❤️ Sách yêu thích ({profile.favoriteBooks?.length || 0})
        </h3>

        {profile.favoriteBooks?.length > 0 ? (
           <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fill, minmax(280px, 1fr))', gap: '20px' }}>
              {profile.favoriteBooks.map((book: any) => {
                 const realId = book.bookId || book.BookId || book.id;
                 return (
                   <Link key={realId} to={`/book/${realId}`} style={{textDecoration: 'none'}}>
                      <BookCard data={book} />
                   </Link>
                 )
              })}
           </div>
        ) : (
           <p style={{textAlign: 'center', color: '#999', padding: '20px'}}>Chưa có sách yêu thích nào.</p>
        )}
      </div>
    </div>
  );
};