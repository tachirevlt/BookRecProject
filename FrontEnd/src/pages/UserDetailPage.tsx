import { useEffect, useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { userService, type UserProfile } from '../services/userService';
import { authService } from '../services/authService';
import { BookCard } from '../components/BookCard';

// --- ICONS ---
const SearchIcon = () => (
    <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round" style={{position:'absolute', left:'10px', top:'50%', transform:'translateY(-50%)', color:'#999'}}>
      <circle cx="11" cy="11" r="8"></circle><line x1="21" y1="21" x2="16.65" y2="16.65"></line>
    </svg>
);
const BellIcon = () => (
    <svg width="25" height="25" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round"><path d="M18 8A6 6 0 0 0 6 8c0 7-3 9-3 9h18s-3-2-3-9"></path><path d="M13.73 21a2 2 0 0 1-3.46 0"></path></svg>
);
const MailIcon = () => (
    <svg width="25" height="25" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round"><path d="M4 4h16c1.1 0 2 .9 2 2v12c0 1.1-.9 2-2 2H4c-1.1 0-2-.9-2-2V6c0-1.1.9-2 2-2z"></path><polyline points="22,6 12,13 2,6"></polyline></svg>
);
const EditIcon = () => (
    <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round"><path d="M11 4H4a2 2 0 0 0-2 2v14a2 2 0 0 0 2 2h14a2 2 0 0 0 2-2v-7"></path><path d="M18.5 2.5a2.121 2.121 0 0 1 3 3L12 15l-4 1 1-4 9.5-9.5z"></path></svg>
);

export const UserDetailPage = () => {
  const navigate = useNavigate();
  const [profile, setProfile] = useState<UserProfile | null>(null);
  const [loading, setLoading] = useState(true);
  const [searchTerm, setSearchTerm] = useState('');

  const currentUser = authService.getCurrentUser();

  useEffect(() => {
    const loadData = async () => {
      if (!currentUser || !currentUser.userId) {
        navigate('/login');
        return;
      }
      try {
        const data = await userService.getProfile(currentUser.userId);
        setProfile(data);
      } catch (error) {
        console.error("Lỗi tải profile:", error);
      } finally {
        setLoading(false);
      }
    };
    loadData();
  }, [currentUser, navigate]);

  const handleLogout = () => {
    authService.logout();
    navigate('/login');
  };

  const handleSearch = (e: React.KeyboardEvent) => {
    if (e.key === 'Enter' && searchTerm.trim()) {
        navigate(`/?genre=&search=${encodeURIComponent(searchTerm)}`);
    }
  };

  if (loading) return (
    <div style={{ minHeight: '100vh', display: 'flex', alignItems: 'center', justifyContent: 'center', background: '#F4F7FF', color: '#637381' }}>
        ⏳ Đang tải thông tin...
    </div>
  );

  if (!profile) return (
    <div style={{ padding: '50px', textAlign: 'center', background: '#F4F7FF', minHeight: '100vh' }}>
      ❌ Không tải được hồ sơ. <br/> 
      <Link to="/login" style={{ color: '#3056D3', fontWeight: 'bold' }}>Đăng nhập lại</Link>
    </div>
  );

  return (
    <div style={{ background: '#F4F7FF', minHeight: '100vh', display: 'flex', flexDirection: 'column' }}>
      
      {/* HEADER */}
      <header className="gr-header">
        <div className="gr-header-inner">
            <div className="header-left">
                <Link to="/" style={{ fontSize: '27px', fontWeight: '800', color: '#382110', marginRight: '10px' }}>
                    PageStack
                </Link>
                <nav className="nav-links">                          
                    <Link to="/" className="nav-item" style={{ fontSize: '19px'}}> Home</Link>
                    <Link to="/profile" className="nav-item active" style={{ fontSize: '19px'}}>My Books</Link>
                    <div className="dropdown-wrapper">
                        <div className="nav-item" style={{ fontSize: '19px'}}> Browse ▾</div>
                        <div className="dropdown-menu">
                            <Link to="/" className="dropdown-item" style={{ fontSize: '16px'}}> All Lists</Link>
                        </div>
                    </div>
                </nav>
            </div>

            <div className="search-box">
                <SearchIcon />
                <input 
                    type="text" 
                    placeholder="Search by title or author..." 
                    className="search-input" 
                    value={searchTerm}
                    onChange={(e) => setSearchTerm(e.target.value)}
                    onKeyDown={handleSearch}
                />
            </div>

            <div style={{ display: 'flex', alignItems: 'center', gap: '5px' }}>
                <button className="icon-btn"><BellIcon /></button>
                <button className="icon-btn"><MailIcon /></button>
                <div className="dropdown-wrapper" style={{ marginLeft: '10px' }}>
                    <div style={{ display: 'flex', alignItems: 'center', height: '100%' }}>
                        <div style={{ width: '32px', height: '32px', borderRadius: '50%', background: '#3056D3', color: 'white', display: 'flex', alignItems: 'center', justifyContent: 'center', fontWeight: 'bold' }}>
                            {profile.username.charAt(0).toUpperCase()}
                        </div>
                    </div>
                    <div className="dropdown-menu" style={{ left: 'auto', right: 0 }}>
                        <Link to="/settings" className="dropdown-item">Account Settings</Link>
                        <button onClick={handleLogout} className="dropdown-item" style={{ width: '100%', textAlign: 'left', color: '#d32f2f', background: 'none', border: 'none', cursor: 'pointer' }}>Sign Out</button>
                    </div>
                </div>
            </div>
        </div>
      </header>

      {/* MAIN CONTENT */}
      <main className="container" style={{ padding: '40px 20px', flex: 1, maxWidth: '1240px', margin: '0 auto', width: '100%' }}>
        
        {/* --- PROFILE CARD GỐC (ĐÃ CẬP NHẬT) --- */}
        <div style={{ 
            background: 'white', 
            borderRadius: '12px', 
            boxShadow: '0 1px 3px rgba(0,0,0,0.1)', 
            padding: '40px', 
            marginBottom: '40px',
            display: 'flex', 
            alignItems: 'center', 
            gap: '30px', 
            flexWrap: 'wrap' // Để responsive trên mobile
        }}>
            {/* 1. Avatar */}
            <div style={{ 
                width: '120px', height: '120px', borderRadius: '50%', 
                background: '#E0E7FF', color: '#3056D3', 
                fontSize: '48px', fontWeight: 'bold', 
                display: 'flex', alignItems: 'center', justifyContent: 'center',
                border: '4px solid white', boxShadow: '0 4px 10px rgba(0,0,0,0.1)'
            }}>
                {profile.username.charAt(0).toUpperCase()}
            </div>

            {/* 2. Thông tin chính (Giữa) */}
            <div style={{ flex: 1 }}>
                <div style={{ display: 'flex', alignItems: 'center', gap: '15px', marginBottom: '10px' }}>
                    <h1 style={{ margin: 0, fontSize: '32px', color: '#090E34', fontWeight: '800' }}>
                        {profile.username}
                    </h1>
                    <span style={{ background: '#3056D3', color: 'white', padding: '4px 12px', borderRadius: '20px', fontSize: '12px', fontWeight: '600' }}>
                        {profile.role || 'Member'}
                    </span>
                </div>
                
                <p style={{ color: '#637381', fontSize: '16px', margin: '0 0 20px 0' }}>
                    {profile.email} • Thành viên từ 2025
                </p>

                <Link to="/settings" style={{ 
                    display: 'inline-flex', alignItems: 'center', gap: '8px',
                    background: '#3056D3', color: 'white', 
                    padding: '10px 24px', borderRadius: '6px', 
                    fontWeight: '600', textDecoration: 'none', transition: 'background 0.3s'
                }}>
                    <EditIcon /> Edit Profile
                </Link>
            </div>
            
            {/* 3. Khung Thống kê (Phải) */}
            <div style={{ 
                display: 'flex', 
                gap: '40px', 
                borderLeft: '1px solid #eee', 
                paddingLeft: '40px',
                minWidth: '400px' // Đảm bảo không bị co quá nhỏ
            }}>
                <div style={{ flex: 1, display: 'flex', justifyContent: 'space-around', alignItems: 'center', background: '#FAFAFA', padding: '20px', borderRadius: '8px' }}>
                        <div style={{ textAlign: 'center' }}>
                            <div style={{ fontSize: '32px', fontWeight: 'bold', color: '#3056D3' }}>{profile.favoriteBooks?.length || 0}</div>
                            <div style={{ color: '#637381', fontSize: '14px', fontWeight: '500' }}>Books Liked</div>
                        </div>
                        <div style={{ width: '1px', height: '40px', background: '#ddd' }}></div>
                        <div style={{ textAlign: 'center' }}>
                            <div style={{ fontSize: '32px', fontWeight: 'bold', color: '#3056D3' }}>0</div>
                            <div style={{ color: '#637381', fontSize: '14px', fontWeight: '500' }}>Reviews</div>
                        </div>
                    </div>
            </div>
        </div>

        {/* DANH SÁCH YÊU THÍCH */}
        <div>
            <div style={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between', marginBottom: '30px' }}>
                <h3 style={{ fontSize: '24px', fontWeight: 'bold', color: '#090E34', margin: 0 }}>
                    My Favorite Books
                </h3>
                <Link to="/" style={{ color: '#3056D3', fontWeight: '600' }}>Browse more books &raquo;</Link>
            </div>

            {profile.favoriteBooks && profile.favoriteBooks.length > 0 ? (
               <div style={{ 
                  display: 'grid', 
                  // Grid 310px giúp chia cột đẹp mắt trên màn hình rộng
                  gridTemplateColumns: 'repeat(auto-fill, minmax(310px, 1fr))', 
                  gap: '20px' 
               }}>
                  {profile.favoriteBooks.map((book: any) => {
                     const realId = book.bookId || book.BookId || book.id;
                     return (
                       <Link key={realId} to={`/book/${realId}`} style={{ display: 'block', textDecoration: 'none' }}>
                          <BookCard data={book} />
                       </Link>
                     )
                  })}
               </div>
            ) : (
               <div style={{ 
                   textAlign: 'center', padding: '60px', 
                   background: 'white', borderRadius: '12px', border: '1px dashed #E0E0E0' 
               }}>
                   <div style={{ fontSize: '40px', marginBottom: '10px' }}>📚</div>
                   <h4 style={{ margin: '0 0 10px 0', color: '#090E34' }}>Danh sách yêu thích trống</h4>
                   <p style={{ color: '#637381', marginBottom: '20px' }}>Hãy khám phá và thả tim cho những cuốn sách bạn yêu thích nhé.</p>
                   <Link to="/" style={{ color: '#3056D3', fontWeight: 'bold' }}>Khám phá ngay</Link>
               </div>
            )}
        </div>

      </main>
    </div>
  );
};