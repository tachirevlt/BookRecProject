import React, { createContext, useContext, useState, useEffect, ReactNode } from 'react';
import { Book } from '../data/books';
import { bookService } from '../../services/bookService';


interface BookContextType {
  books: Book[];
  isLoading: boolean;
  error: Error | null;
  refreshBooks: () => Promise<void>;
}

const BookContext = createContext<BookContextType | undefined>(undefined);

export const BookProvider: React.FC<{ children: ReactNode }> = ({ children }) => {
  // Khởi tạo rỗng — dữ liệu được tải từ API ngay khi mount
  const [books, setBooks] = useState<Book[]>([]);
  const [isLoading, setIsLoading] = useState<boolean>(true);
  const [error, setError] = useState<Error | null>(null);

  const fetchBooks = async () => {
    setIsLoading(true);
    setError(null);
    try {
      const pagedList = await bookService.getBooks({ PageNumber: 1, PageSize: 1000 }); // Lấy thêm sách để hiển thị trên UI
      if (pagedList.items && pagedList.items.length > 0) {
        setBooks(pagedList.items);
      }
    } catch (err) {
      console.error('[BookProvider] Lỗi khi tải sách từ API:', err);
      setError(err instanceof Error ? err : new Error('Unknown error fetching books'));
      // Không có fallback mock data — hiển thị trạng thái lỗi
    } finally {
      setIsLoading(false);
    }
  };

  useEffect(() => {
    fetchBooks();
  }, []);

  return (
    <BookContext.Provider value={{ books, isLoading, error, refreshBooks: fetchBooks }}>
      {children}
    </BookContext.Provider>
  );
};

export const useBooks = (): BookContextType => {
  const context = useContext(BookContext);
  if (context === undefined) {
    throw new Error('useBooks must be used within a BookProvider');
  }
  return context;
};

export const useBookCollections = () => {
  const { books } = useBooks();
  
  const fantasyBooks = books.filter(b => b.genres.some(g => g.toLowerCase().includes('fantasy')));
  const classicsBooks = books.filter(b => b.genres.some(g => ['classics', 'biography', 'philosophy', 'tech'].includes(g.toLowerCase())));
  const mysteryRomanceBooks = books.filter(b => b.genres.some(g => ['mystery', 'romance', 'adventure'].includes(g.toLowerCase())));
  const trendingBooks = [...books].sort((a, b) => b.popularity - a.popularity).slice(0, 6);
  const featuredBook = books[0] || null;
  const mosaicBooks = books.length >= 5 ? [books[3], books[1], books[4], books[2]] : books.slice(0, 4);
  const heroBooks = books.slice(0, 3);
  
  return { fantasyBooks, classicsBooks, mysteryRomanceBooks, trendingBooks, featuredBook, mosaicBooks, heroBooks };
};
