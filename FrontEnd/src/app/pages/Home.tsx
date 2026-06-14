import React, { useMemo, useState, useEffect } from 'react';
import { useOutletContext } from 'react-router';
import { HeroCarousel } from '../components/HeroCarousel';
import { FeaturedSection } from '../components/FeaturedSection';
import { BookShelf } from '../components/BookShelf';
import { TrendingSection } from '../components/TrendingSection';
import { MoodSection } from '../components/MoodSection';
import { ReadingStreak } from '../components/ReadingStreak';
import { useBooks, useBookCollections } from '../hooks/useBooks';
import { genreInfo } from '../data/books'; 
import { ImageWithFallback } from '../components/figma/ImageWithFallback';
import { userService } from '../../services/userService'; // Import userService
import type { OutletContextType } from '../components/RootLayout';
import type { Book } from '../data/books'; // Import type Book

export function Home() {
  const { books, isLoading } = useBooks();
  const { 
    fantasyBooks, 
    sciFiBooks, 
    classicsBooks, 
    mysteryBooks, 
    romanceBooks 
  } = useBookCollections();
  
  const { searchQuery, onOpenBook } = useOutletContext<OutletContextType>();

  // --- THÊM STATE CHO SÁCH ĐỀ XUẤT CÁ NHÂN ---
  const [personalizedBooks, setPersonalizedBooks] = useState<Book[]>([]);
  const isLoggedIn = userService.isLoggedIn();

  // --- GỌI API LẤY SÁCH KHI ĐÃ ĐĂNG NHẬP ---
  useEffect(() => {
    const fetchPersonalizedBooks = async () => {
      if (isLoggedIn) {
        const userId = userService.getCurrentUserId();
        if (userId) {
          try {
            // Lấy 8 cuốn sách đề xuất
            const recommended = await userService.getRecommendationsByUserkId(userId, 8);
            if (recommended && recommended.length > 0) {
              setPersonalizedBooks(recommended);
            }
          } catch (error) {
            console.error("Lỗi khi tải sách đề xuất cá nhân:", error);
          }
        }
      }
    };

    fetchPersonalizedBooks();
  }, [isLoggedIn]);
  
  const filtered = useMemo(() => {
    if (!searchQuery) return null;
    const lowerQuery = searchQuery.toLowerCase();
    return books.filter(b =>
      b.title.toLowerCase().includes(lowerQuery) ||
      b.author.toLowerCase().includes(lowerQuery) ||
      b.genres?.some(g => g.toLowerCase().includes(lowerQuery))
    );
  }, [books, searchQuery]);

  // THÊM CÁC DEPENDENCY MỚI VÀO USEMEMO (isLoggedIn, personalizedBooks)
  const homeContent = useMemo(() => (
    <>
      <HeroCarousel onOpenBook={onOpenBook} />

      <div className="space-y-16 py-14">
        
        {/* CHỈ CẦN isLoggedIn LÀ RENDER LUÔN ĐỂ BOOKSHELF LO VIỆC HIỂN THỊ LOADING */}
        {isLoggedIn && (
          <BookShelf
            title="✨ Dành Riêng Cho Bạn"
            subtitle="Đề xuất dựa trên sở thích và lịch sử đọc của bạn"
            emoji="🎯"
            genre="Personalized"
            books={personalizedBooks} 
            onOpenBook={onOpenBook}
            accentColor="from-indigo-500 to-purple-600"
          />
        )}

        <FeaturedSection onOpenBook={onOpenBook} />
        <ReadingStreak />
        <MoodSection onOpenBook={onOpenBook} />

        <BookShelf
          title="Thế giới Giả tưởng"
          subtitle={genreInfo['fantasy'].description}
          emoji={genreInfo['fantasy'].emoji}
          genre="Fantasy"
          onOpenBook={onOpenBook}
          accentColor={genreInfo['fantasy'].accentColor}
        />

        <TrendingSection onOpenBook={onOpenBook} />

        <BookShelf
          title="Khoa học Viễn tưởng"
          subtitle={genreInfo['science-fiction'].description}
          emoji={genreInfo['science-fiction'].emoji}
          genre="Science-Fiction"
          onOpenBook={onOpenBook}
          accentColor={genreInfo['science-fiction'].accentColor}
        />

        <BookShelf
          title="Tác phẩm Kinh điển"
          subtitle={genreInfo['classics'].description}
          emoji={genreInfo['classics'].emoji}
          genre="Classics"
          onOpenBook={onOpenBook}
          accentColor={genreInfo['classics'].accentColor}
        />

        <BookShelf
          title="Bí ẩn & Trinh thám"
          subtitle={genreInfo['mystery'].description}
          emoji={genreInfo['mystery'].emoji}
          genre="Mystery"
          onOpenBook={onOpenBook}
          accentColor={genreInfo['mystery'].accentColor}
        />

        <BookShelf
          title="Tiểu thuyết Lãng mạn"
          subtitle={genreInfo['romance'].description}
          emoji={genreInfo['romance'].emoji}
          genre="Romance"
          onOpenBook={onOpenBook}
          accentColor={genreInfo['romance'].accentColor}
        />
      </div>
    </>
  ), [books, onOpenBook, isLoggedIn, personalizedBooks]); 
  // Nhớ giữ mảng dependency ở trên để React biết khi nào cần render lại

  if (filtered) {
    return (
      <main className="min-h-screen bg-[#F8F7F4] dark:bg-[#0D0C14] pt-24 pb-16">
        <div className="max-w-[1400px] mx-auto px-4 sm:px-6 lg:px-8">
          <h2 className="text-gray-900 dark:text-white mb-6" style={{ fontFamily: 'var(--font-serif)', fontSize: '1.4rem', fontWeight: 700 }}>
            Kết quả tìm kiếm "{searchQuery}" ({filtered.length} kết quả)
          </h2>
          {filtered.length === 0 ? (
            <div className="text-center py-20">
              <span className="text-5xl mb-4 block">📚</span>
              <p className="text-gray-500 dark:text-gray-400">Không tìm thấy sách phù hợp. Hãy thử từ khóa khác!</p>
            </div>
          ) : (
            <div className="grid grid-cols-2 sm:grid-cols-3 md:grid-cols-4 lg:grid-cols-5 xl:grid-cols-6 gap-5">
              {filtered.map(book => (
                <div key={book.id} onClick={() => onOpenBook(book)} className="cursor-pointer">
                  <div className="rounded-2xl overflow-hidden shadow-sm hover:shadow-md transition-shadow" style={{ aspectRatio: '2/3' }}>
                    <ImageWithFallback src={book.cover} alt={book.title} className="w-full h-full object-cover hover:scale-105 transition-transform duration-300" />
                  </div>
                  <div className="mt-2">
                    <p className="text-gray-900 dark:text-white font-semibold text-sm line-clamp-1">{book.title}</p>
                    <p className="text-gray-500 dark:text-gray-400 text-xs">{book.author}</p>
                    <p className="text-gray-900 dark:text-white font-bold text-sm mt-1">{book.price.toLocaleString('vi-VN')}₫</p>
                  </div>
                </div>
              ))}
            </div>
          )}
        </div>
      </main>
    );
  }

  return (
    <main className="bg-[#F8F7F4] dark:bg-[#0D0C14]">
      {homeContent}
    </main>
  );
}