import { useOutletContext } from 'react-router';
import { HeroCarousel } from '../components/HeroCarousel';
import { FeaturedSection } from '../components/FeaturedSection';
import { BookShelf } from '../components/BookShelf';
import { TrendingSection } from '../components/TrendingSection';
import { MoodSection } from '../components/MoodSection';
import { ReadingStreak } from '../components/ReadingStreak';
import { useBooks, useBookCollections } from '../hooks/useBooks';
import type { Book } from '../data/books';
import { ImageWithFallback } from '../components/figma/ImageWithFallback';
import type { OutletContextType } from '../components/RootLayout';

export function Home() {
  const { books, isLoading } = useBooks();
  const { fantasyBooks, classicsBooks, mysteryRomanceBooks } = useBookCollections();
  const { searchQuery, onOpenBook } = useOutletContext<OutletContextType>();
  
  const filtered = searchQuery
    ? books.filter(b =>
        b.title.toLowerCase().includes(searchQuery.toLowerCase()) ||
        b.author.toLowerCase().includes(searchQuery.toLowerCase()) ||
        b.genres?.some(g => g.toLowerCase().includes(searchQuery.toLowerCase()))
      )
    : null;

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
      {/* Hero */}
      <HeroCarousel onOpenBook={(id) => {
        const b = books.find(bk => bk.id === id);
        if (b) onOpenBook(b);
      }} />

      {/* Sections */}
      <div className="space-y-16 py-14">

        {/* Featured mosaic */}
        <FeaturedSection onOpenBook={onOpenBook} />

        {/* Reading streak */}
        <ReadingStreak />

        {/* Mood recommendations */}
        <MoodSection onOpenBook={onOpenBook} />

        {/* Fantasy shelf */}
        <BookShelf
          title="Thế giới Giả tưởng"
          subtitle="Phiêu lưu qua những vũ trụ chưa từng khám phá"
          emoji="🧙‍♂️"
          books={fantasyBooks}
          onOpenBook={onOpenBook}
          accentColor="#7C3AED"
        />

        {/* Trending */}
        <TrendingSection onOpenBook={onOpenBook} />

        {/* Classics shelf */}
        <BookShelf
          title="Tác phẩm kinh điển"
          subtitle="Vượt thời gian, định hình văn học nhân loại"
          emoji="🏛️"
          books={classicsBooks}
          onOpenBook={onOpenBook}
          accentColor="#57534E"
        />

        {/* Mystery & Romance shelf */}
        <BookShelf
          title="Bí ẩn & Lãng mạn"
          subtitle="Từ những bí ẩn chưa lời giải đến tình yêu đẹp như mơ"
          emoji="🌹"
          books={mysteryRomanceBooks}
          onOpenBook={onOpenBook}
          accentColor="#EC4899"
        />
      </div>
    </main>
  );
}
