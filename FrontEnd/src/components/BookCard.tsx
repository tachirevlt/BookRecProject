// src/components/BookCard.tsx
import type { Book } from '../types/Book';

interface Props {
  data: Book;
}

export const BookCard = ({ data }: Props) => {
  // Ảnh mặc định nếu không có coverImage
  const randomImage =  `https://placehold.co/350x500/e2e8f0/3056D3?text=${encodeURIComponent(data.title.substring(0,10))}`;

  return (
    <div style={{ 
      display: 'flex', 
      flexDirection: 'column',
      background: '#fff', 
      borderRadius: '12px',
      overflow: 'hidden', 
      boxShadow: '0 4px 10px rgba(0,0,0,0.06)', 
      border: '1px solid #F0F0F0',
      transition: 'all 0.3s ease',
      height: '100%',
      minHeight: '320px',
      margin: '0 auto',   // Căn giữa thẻ trong ô grid
      width: '100%',
      minWidth: 0,
      cursor: 'pointer',
      position: 'relative'
    }}
    onMouseEnter={(e) => {
        e.currentTarget.style.transform = 'translateY(-5px)';
        e.currentTarget.style.boxShadow = '0 12px 20px rgba(0,0,0,0.12)';
    }}
    onMouseLeave={(e) => {
        e.currentTarget.style.transform = 'translateY(0)';
        e.currentTarget.style.boxShadow = '0 4px 10px rgba(0,0,0,0.06)';
    }}
    >
      {/* 1. PHẦN ẢNH BÌA (Tăng chiều cao) */}
      <div style={{ 
          width: '100%', 
          height: '350px', // ⬆️ Tăng từ 240px lên 280px
          overflow: 'hidden',
          position: 'relative',
          background: '#f9f9f9'
      }}>
        <img 
          src={randomImage} 
          alt={data.title}
          style={{ width: '100%', height: '100%', objectFit: 'cover' }}
        />
        {/* Đã bỏ Badge Năm ở đây để chuyển xuống dưới */}
      </div>

      {/* 2. PHẦN NỘI DUNG */}
      <div style={{ 
          padding: '16px', 
          display: 'flex', 
          flexDirection: 'column',
          flex: 1, 
          justifyContent: 'space-between' 
      }}>
        
        {/* Nội dung chính */}
        <div style={{ flex: 1, display: 'flex', flexDirection: 'column', gap: '5px' }}>
            
            {/* Rating & Genre */}
            <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '5px' }}>
                <div style={{ display: 'flex', alignItems: 'center', gap: '4px', fontSize: '13px', fontWeight: 'bold', color: '#F59E0B' }}>
                    ⭐ {data.average_rating}
                </div>
                {/* Genres */}
                <div style={{ display: 'flex', gap: '4px', overflow: 'hidden' }}>
                    {data.genres?.slice(0, 2).map((g, i) => (
                        <span key={i} style={{ 
                            fontSize: '10px', fontWeight: '700', textTransform: 'uppercase',
                            color: '#3056D3', background: '#F4F7FF', 
                            padding: '3px 6px', borderRadius: '4px'
                        }}>
                            {g}
                        </span>
                    ))}
                </div>
            </div>

            {/* Title (Tăng size + 1 dòng + dấu ...) */}
            <h3 title={data.title} style={{ 
                fontSize: '18px', // ⬆️ Tăng Font Size
                fontWeight: '800', 
                margin: '0', 
                color: '#111',
                lineHeight: '1.4',
                // 👇 Cấu hình hiển thị 1 dòng và dấu ...
                whiteSpace: 'nowrap', 
                overflow: 'hidden', 
                textOverflow: 'ellipsis' 
            }}>
                {data.title}
            </h3>

            {/* Author (Tăng size + 1 dòng + dấu ...) */}
            <p title={data.author} style={{ 
                margin: 0, 
                fontSize: '15px', // ⬆️ Tăng Font Size
                color: '#555',
                fontWeight: '600',
                // 👇 Cấu hình hiển thị 1 dòng và dấu ...
                whiteSpace: 'nowrap', 
                overflow: 'hidden', 
                textOverflow: 'ellipsis' 
            }}>
                by <span style={{ color: '#222' }}>{data.author}</span>
            </p>
        </div>

        {/* Footer: Votes & Năm (Góc phải dưới) */}
        <div style={{ 
            borderTop: '1px solid #f5f5f5', 
            paddingTop: '12px', 
            marginTop: '10px', 
            display: 'flex', 
            justifyContent: 'space-between', 
            alignItems: 'center' 
        }}>
            {/* Lượt vote */}
            <span style={{ fontSize: '12px', color: '#999', display: 'flex', alignItems: 'center', gap: '4px' }}>
                👥 {data.ratings?.toLocaleString() || 0}
            </span>

            {/* Năm xuất bản (Đẩy xuống đây) */}
            <span style={{ 
                fontSize: '14px', 
                fontWeight: 'bold', 
                color: '#333',
                background: '#eee',
                padding: '2px 8px',
                borderRadius: '4px'
            }}>
                {data.year}
            </span>
        </div>

      </div>
    </div>
  );
};