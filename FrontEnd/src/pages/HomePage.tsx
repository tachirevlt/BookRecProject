// src/pages/HomePage.tsx
import { useEffect, useState } from 'react';
import { Link, useNavigate, useSearchParams } from 'react-router-dom';
import { bookService } from '../services/bookService';
import { authService } from '../services/authService';
import { BookCard } from '../components/BookCard';
import type { Book } from '../types/Book';

// Icons
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

export const HomePage = () => {
  const [books, setBooks] = useState<Book[]>([]);
  const navigate = useNavigate();
  const user = authService.getCurrentUser();
  
  const [searchParams] = useSearchParams();
  const currentGenre = searchParams.get('genre');

  const [viewMode, setViewMode] = useState<'featured' | 'all'>('featured');
  const [currentPage, setCurrentPage] = useState(1);
  
  const [searchTerm, setSearchTerm] = useState(''); 
  const [appliedTerm, setAppliedTerm] = useState(''); 

  const GENRES = ['Classic', 'Fiction', 'History', 'Romance', 'Science'];

  // Effect 1: Chuyển chế độ xem
  useEffect(() => {
    if (currentGenre || appliedTerm) {
        setViewMode('all');
        setCurrentPage(1);
    } else {
        setViewMode('featured');
    }
  }, [currentGenre, appliedTerm]);

  // Effect 2: Gọi API lấy sách
  useEffect(() => {
    let isMounted = true; 

    const fetchBooks = async () => {
      try {
        const limit = viewMode === 'featured' ? 100 : 12;
        
        const apiGenre = viewMode === 'featured' ? undefined : (currentGenre || undefined);
        const apisearchTerm = viewMode === 'featured' ? undefined : (appliedTerm || undefined);

        const data = await bookService.getAll(
            currentPage, 
            limit, 
            apiGenre, 
            apisearchTerm,
        );
        
        if (isMounted) { 
            if (data && data.items) {
                setBooks(data.items);
            } else {
                setBooks([]); 
            }
        }
      } catch (error) {
        console.error("Lỗi tải sách:", error);
        if (isMounted) setBooks([]);
      }
    };

    fetchBooks();
    return () => { isMounted = false; };
  }, [currentPage, viewMode, currentGenre, appliedTerm]); 

  const handleLogout = () => {
    authService.logout();
    navigate('/login');
  };

  const handleKeyDown = (e: React.KeyboardEvent) => {
    if (e.key === 'Enter') {
        setAppliedTerm(searchTerm); 
        setCurrentPage(1);
    }
  };

  const getBooksByGenre = (genre: string) => 
    books.filter(b => 
      b.genres?.some(g => g.toLowerCase().includes(genre.toLowerCase()))
    );
  return (
    <div style={{ minHeight: '100vh', display: 'flex', flexDirection: 'column' }}>
      
      {/* HEADER */}
      <header className="gr-header">
        <div className="gr-header-inner">
            <div className="header-left">
                <Link to="/" style={{ fontSize: '27px', fontWeight: '800', color: '#382110', marginRight: '10px' }}>
                    PageStack
                </Link>
                <nav className="nav-links">
                    <Link to="/" className={`nav-item ${!currentGenre && viewMode === 'featured' ? 'active' : ''}`} 
                          style={{ fontSize: '19px'}}
                          onClick={() => { 
                             setViewMode('featured'); 
                             setAppliedTerm(''); setSearchTerm(''); 
                             setCurrentPage(1);
                             navigate('/'); 
                          }}>
                        Home
                    </Link>
                    {user && <Link to="/profile" className="nav-item"
                                                  style={{ fontSize: '19px'}}>My Books</Link>}
                    <div className="dropdown-wrapper">
                        <div className="nav-item"
                              style={{ fontSize: '19px'}}>
                                Browse ▾
                        </div>
                        <div className="dropdown-menu">
                            <div style={{ padding: '8px 20px', fontSize: '15px', fontWeight: 'bold', textTransform: 'uppercase', color: '#888' }}>
                                Favorite Genres
                            </div>
                            {GENRES.map(g => (
                                <Link key={g} to={`/?genre=${g}`} className="dropdown-item"
                                      style={{ fontSize: '16px'}}>{g}</Link>
                            ))}
                            <div style={{ borderTop: '1px solid #eee', margin: '5px 0' }}></div>
                            <Link to="/" className="dropdown-item" onClick={() => { setViewMode('all'); setCurrentPage(1); }}
                                  style={{ fontSize: '16px'}}>
                                All Lists
                            </Link>
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
                    onKeyDown={handleKeyDown}
                />
            </div>

            <div style={{ display: 'flex', alignItems: 'center', gap: '5px', flexShrink: 0 }}>
                {user ? (
                    <>
                        <button className="icon-btn"><BellIcon /></button>
                        <button className="icon-btn"><MailIcon /></button>
                        <div className="dropdown-wrapper" style={{ fontSize: '16px',marginLeft: '10px' }}>
                            <div style={{ display: 'flex', alignItems: 'center', height: '100%' }}>
                                <div style={{ width: '32px', height: '32px', borderRadius: '50%', background: '#3056D3', color: 'white', display: 'flex', alignItems: 'center', justifyContent: 'center', fontWeight: 'bold' }}>
                                    {user.username.charAt(0).toUpperCase()}
                                </div>
                            </div>
                            <div className="dropdown-menu" style={{ left: 'auto', right: 0 }}>
                                <Link to="/profile" style={{ textDecoration: 'none', display: 'block', color: 'inherit' }}>
                                    <div style={{ padding: '10px 20px', borderBottom: '1px solid #eee', cursor: 'pointer' }}>
                                        <div style={{ fontWeight: 'bold', color: '#333' }}>{user.username}</div>
                                        <div style={{ fontSize: '12px', color: '#666', marginTop: '4px' }}>View Profile</div>
                                    </div>
                                </Link>
                                <Link to="/settings" className="dropdown-item" style={{ fontSize: '16px' }}>Account Settings</Link>
                                <button onClick={handleLogout} className="dropdown-item" style={{ fontSize: '16px', width: '100%', textAlign: 'left', border: 'none', background: 'none', cursor: 'pointer', color: '#d32f2f' }}>
                                    Sign Out
                                </button>
                            </div>
                        </div>
                    </>
                ) : (
                    <div style={{ display: 'flex', gap: '10px' }}>                        
                        <Link to="/login" className="nav-item" style={{ border: '1px solid #ccc', padding: '0 15px', borderRadius: '4px', height: '36px', lineHeight: '36px' }}>Login</Link>
                        <Link to="/register" className="nav-item" style={{ border: '1px solid #ccc', padding: '0 15px', borderRadius: '4px', height: '36px', lineHeight: '36px' }}>Sign Up</Link>
                    </div>
                )}
            </div>
        </div>
      </header>

      {/* CONTENT */}
      <main className="container" style={{ padding: '30px 20px', flex: 1, maxWidth: '1440px', margin: '0 auto', width: '100%' }}>
        
        {/* VIEW 1: FEATURED (List Ngang) */}
        {viewMode === 'featured' && !currentGenre && !appliedTerm ? (
            <div style={{ display: 'flex', flexDirection: 'column', gap: '40px' }}>
                {GENRES.map((genre) => {
                    const genreBooks = getBooksByGenre(genre);
                    if (genreBooks.length === 0) return null;
                    return (
                        <div key={genre}>
                            <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'end', marginBottom: '15px', borderBottom: '1px solid #ddd', paddingBottom: '8px' }}>
                                <h3 style={{ margin: 0, fontSize: '18px', color: '#382110', textTransform: 'uppercase' }}>{genre}</h3>
                                <Link to={`/?genre=${genre}`} style={{ color: '#3056D3', fontSize: '13px' }}>More {genre} &raquo;</Link>
                            </div>
                            <div className="horizontal-scroll">
                                {genreBooks.map((book) => {
                                    const realId = book.bookId ;
                                    return (
                                        <div key={realId} className="book-slide-item">
                                            <Link to={`/book/${realId}`} style={{ display: 'block', textDecoration: 'none' }}>
                                                <BookCard data={book} />
                                            </Link>
                                        </div>
                                    )
                                })}
                            </div>
                        </div>
                    );
                })}
                <div style={{ textAlign: 'center', marginTop: '30px' }}>
                    <button onClick={() =>{ setViewMode('all');setCurrentPage(1);}} style={{ padding: '12px 30px', background: '#F4F1EA', border: '1px solid #ccc', borderRadius: '30px', cursor: 'pointer', fontWeight: 'bold', color: '#333' }}>
                        View All Books
                    </button>
                </div>
            </div>
        ) : (
            // VIEW 2: GRID (Grid View khi lọc Genre hoặc Search)
            <div>
                 <div style={{ marginBottom: '20px', display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
                    <h2 style={{ fontSize: '24px', margin: 0 }}>
                        {currentGenre 
                            ? `Category: ${currentGenre}` 
                            : appliedTerm 
                                ? `Search Results for "${appliedTerm}"` 
                                : 'All Books'}
                    </h2>
                    
                    <button onClick={() => { 
                        setViewMode('featured'); 
                        setAppliedTerm(''); setSearchTerm(''); 
                        setCurrentPage(1);
                        navigate('/'); 
                    }} style={{ cursor: 'pointer', border: 'none', background: 'none', color: '#3056D3', fontWeight: 'bold' }}>
                        &laquo; Back to Home
                    </button>
                 </div>

                 {books.length === 0 && <p style={{color: '#666', textAlign:'center', padding:'40px'}}>No books found matching your criteria.</p>}

                 {/* 👇 2. Sửa Grid thành minmax 310px để hiển thị nhiều cột */}
                 <div style={{ 
                     display: 'grid', 
                     gridTemplateColumns: 'repeat(auto-fill, minmax(130px, 1fr))', 
                     gap: '10px' 
                 }}>
                    {books.map((book: any) => {
                        const realId = book.bookId || book.BookId || book.id;
                        return (
                            // 👇 3. Thêm display: block để Link không bị lỗi
                            <Link key={realId} to={`/book/${realId}`} style={{ display: 'block', textDecoration: 'none' }}>
                                <BookCard data={book} />
                            </Link>
                        )
                    })}
                 </div>

                 {/* Pagination */}
                 <div style={{ display: 'flex', justifyContent: 'center', marginTop: '40px', gap: '10px' }}>
                    <button onClick={() => setCurrentPage(p => Math.max(1, p - 1))} disabled={currentPage === 1} style={{padding:'8px 15px', cursor: 'pointer'}}>Prev</button>
                    <span style={{ lineHeight: '35px', fontWeight: 'bold' }}>{currentPage}</span>
                    <button onClick={() => setCurrentPage(p => p + 1)} style={{padding:'8px 15px', cursor: 'pointer'}}>Next</button>
                 </div>
            </div>
        )}
      </main>
    </div>
  );
};