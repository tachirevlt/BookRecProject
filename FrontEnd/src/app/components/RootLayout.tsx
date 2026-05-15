import { useState, useEffect } from 'react';
import { Outlet } from 'react-router';
import { Navbar } from './Navbar';
import { Footer } from './Footer';
import { BookDetailModal } from './BookDetailModal';
import type { Book } from '../data/books';

export interface OutletContextType {
  isDark: boolean;
  searchQuery: string;
  onOpenBook: (book: Book) => void;
}

export function RootLayout() {
  const [isDark, setIsDark] = useState(false);
  const [selectedBookId, setSelectedBookId] = useState<number | null>(null);
  const [searchQuery, setSearchQuery] = useState('');

  useEffect(() => {
    const pref = window.matchMedia('(prefers-color-scheme: dark)').matches;
    setIsDark(pref);
  }, []);

  useEffect(() => {
    document.documentElement.classList.toggle('dark', isDark);
  }, [isDark]);

  const onOpenBook = (book: Book) => setSelectedBookId(book.id);

  const ctx: OutletContextType = { isDark, searchQuery, onOpenBook };

  return (
    <div className="min-h-screen bg-[#F8F7F4] dark:bg-[#0D0C14] transition-colors duration-300">
      <Navbar
        isDark={isDark}
        onToggleDark={() => setIsDark(d => !d)}
        onSearch={setSearchQuery}
        onOpenBook={(id) => setSelectedBookId(id)}
      />
      <Outlet context={ctx} />
      <Footer />
      <BookDetailModal
        bookId={selectedBookId}
        onClose={() => setSelectedBookId(null)}
        onOpenBook={(book) => setSelectedBookId(book.id)}
      />
    </div>
  );
}
