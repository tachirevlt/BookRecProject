import type { Book } from '../types/Book';

interface Props {
  data: Book;
}

export const BookCard = ({ data }: Props) => {
  return (
    <div style={{ 
      border: '1px solid #e0e0e0',
      borderRadius: '12px',
      padding: '20px',
      backgroundColor: '#fff',
      boxShadow: '0 4px 12px rgba(0,0,0,0.05)',
      height: '100%'
    }}>
      <div style={{ display: 'flex', justifyContent: 'space-between', marginBottom: '10px' }}>
        <span style={{ background: '#e3f2fd', color: '#1976d2', padding: '4px 12px', borderRadius: '20px', fontSize: '12px', fontWeight: 'bold' }}>
          {data.tag_name}
        </span>
        <span style={{ color: '#f57c00', fontWeight: 'bold' }}>⭐ {data.average_rating}</span>
      </div>
      
      <h2 style={{ fontSize: '18px', margin: '10px 0', color: '#333', minHeight: '50px' }}>
        {data.title}
      </h2>
      
      <p style={{ margin: '5px 0', color: '#666', fontSize: '14px' }}>
        Tác giả: <strong>{data.author}</strong>
      </p>
      
      <div style={{ borderTop: '1px solid #eee', marginTop: '15px', paddingTop: '10px', fontSize: '13px', color: '#888', display: 'flex', justifyContent: 'space-between' }}>
        <span>Năm: {data.year}</span>
        <span>{data.ratings?.toLocaleString()} đánh giá</span>
      </div>
    </div>
  );
};