import { useState, useRef, useEffect } from 'react';
import { motion, AnimatePresence } from 'motion/react';
import { useNavigate, useOutletContext } from 'react-router';
import {
  TrendingUp, Flame, Zap, Star, ShoppingCart, ChevronRight, ChevronLeft,
  Users, MessageCircle, Heart, Sparkles, Trophy, ArrowUpRight, Eye,
  BookOpen, Quote
} from 'lucide-react';
import { badgeColors, genreColors, communityQuotes, GENRES } from '../data/books';
import { useBooks, useBookCollections } from '../hooks/useBooks';
import type { Book } from '../data/books';
import { ImageWithFallback } from '../components/figma/ImageWithFallback';
import type { OutletContextType } from '../components/RootLayout';

const FILTERS = [
  { id: 'today', label: 'Hôm nay' },
  { id: 'week', label: 'Tuần này' },
  { id: 'month', label: 'Tháng này' },
  { id: 'alltime', label: 'Mọi thời' },
];

const BADGE_INFO: Record<string, { label: string; color: string; icon: string }> = {
  viral: { label: 'Viral', color: 'bg-rose-500 text-white', icon: '🔥' },
  hot: { label: 'Hot This Week', color: 'bg-orange-500 text-white', icon: '⚡' },
  purchased: { label: 'Most Purchased', color: 'bg-violet-500 text-white', icon: '🏆' },
};

const rankColors = [
  'from-amber-400 to-orange-500',
  'from-slate-300 to-slate-500',
  'from-amber-600 to-amber-800',
  'from-indigo-400 to-indigo-600',
  'from-rose-400 to-rose-600',
  'from-emerald-400 to-emerald-600',
];

function TrendingCarousel({ title, books, onOpenBook, emoji, accentColor = '#4F46E5', showRank = false }: {
  title: string;
  books: Book[];
  onOpenBook: (b: Book) => void;
  emoji?: string;
  accentColor?: string;
  showRank?: boolean;
}) {
  const scrollRef = useRef<HTMLDivElement>(null);
  const [canLeft, setCanLeft] = useState(false);
  const [canRight, setCanRight] = useState(true);

  const updateButtons = () => {
    const el = scrollRef.current;
    if (!el) return;
    setCanLeft(el.scrollLeft > 8);
    setCanRight(el.scrollLeft < el.scrollWidth - el.clientWidth - 8);
  };

  const scroll = (dir: 'left' | 'right') =>
    scrollRef.current?.scrollBy({ left: dir === 'left' ? -340 : 340, behavior: 'smooth' });

  return (
    <motion.section
      initial={{ opacity: 0, y: 28 }}
      whileInView={{ opacity: 1, y: 0 }}
      viewport={{ once: true, margin: '-60px' }}
      transition={{ duration: 0.55 }}
    >
      <div className="flex items-center justify-between mb-5 px-4 sm:px-6 lg:px-8 max-w-[1400px] mx-auto">
        <div className="flex items-center gap-3">
          {emoji && <span className="text-2xl">{emoji}</span>}
          <h2 className="text-gray-900 dark:text-white" style={{ fontFamily: 'var(--font-serif)', fontSize: 'clamp(1rem, 2vw, 1.3rem)', fontWeight: 700 }}>
            {title}
          </h2>
        </div>
        <div className="flex items-center gap-2">
          <AnimatePresence>
            {canLeft && (
              <motion.button initial={{ opacity: 0 }} animate={{ opacity: 1 }} exit={{ opacity: 0 }}
                onClick={() => scroll('left')}
                className="w-8 h-8 rounded-full bg-white dark:bg-white/10 border border-gray-200 dark:border-white/10 flex items-center justify-center text-gray-600 dark:text-gray-300 hover:bg-gray-50 transition-colors shadow-sm">
                <ChevronLeft className="w-4 h-4" />
              </motion.button>
            )}
          </AnimatePresence>
          <button onClick={() => scroll('right')} disabled={!canRight}
            className="w-8 h-8 rounded-full bg-white dark:bg-white/10 border border-gray-200 dark:border-white/10 flex items-center justify-center text-gray-600 dark:text-gray-300 hover:bg-gray-50 transition-colors shadow-sm disabled:opacity-40">
            <ChevronRight className="w-4 h-4" />
          </button>
        </div>
      </div>

      <div ref={scrollRef} onScroll={updateButtons}
        className="flex gap-5 overflow-x-auto pb-4 px-4 sm:px-6 lg:px-8"
        style={{ scrollbarWidth: 'none' }}>
        {books.map((book, i) => (
          <motion.div
            key={book.id}
            initial={{ opacity: 0, y: 16 }}
            whileInView={{ opacity: 1, y: 0 }}
            viewport={{ once: true }}
            transition={{ delay: i * 0.06 }}
            whileHover={{ y: -8 }}
            onClick={() => onOpenBook(book)}
            className="group cursor-pointer shrink-0 w-36 sm:w-44"
          >
            <div className="relative rounded-2xl overflow-hidden shadow-md group-hover:shadow-xl transition-shadow mb-3" style={{ aspectRatio: '2/3' }}>
              <ImageWithFallback src={book.cover} alt={book.title} className="w-full h-full object-cover group-hover:scale-110 transition-transform duration-500" />
              <div className="absolute inset-0 bg-gradient-to-t from-black/60 via-transparent to-transparent opacity-0 group-hover:opacity-100 transition-opacity" />
              <motion.div
                className="absolute inset-0 rounded-2xl pointer-events-none"
                initial={false}
                whileHover={{ boxShadow: `0 0 20px 3px ${book.accentColor}60` }}
              />
              {showRank && (
                <div className={`absolute top-2.5 left-2.5 w-7 h-7 rounded-xl bg-gradient-to-br ${rankColors[i % rankColors.length]} flex items-center justify-center text-white font-black text-xs shadow-lg`}>
                  {i + 1}
                </div>
              )}
              {book.badge && !showRank && (
                <span className={`absolute top-2.5 left-2.5 px-2 py-0.5 rounded-full text-[10px] font-bold ${badgeColors[book.badge]}`}>
                  {book.badge}
                </span>
              )}
              {/* Quick action overlay */}
              <motion.div
                initial={{ opacity: 0, y: 10 }}
                whileHover={{ opacity: 1, y: 0 }}
                className="absolute bottom-3 left-2 right-2 flex gap-1.5"
                onClick={e => e.stopPropagation()}
              >
                <button onClick={() => onOpenBook(book)}
                  className="flex-1 flex items-center justify-center gap-1 py-1.5 bg-white/90 backdrop-blur-sm text-gray-800 rounded-lg text-[11px] font-semibold hover:bg-white transition-colors">
                  <Eye className="w-3 h-3" /> Xem
                </button>
                <button className="flex-1 flex items-center justify-center gap-1 py-1.5 bg-indigo-600 text-white rounded-lg text-[11px] font-semibold hover:bg-indigo-500 transition-colors">
                  <ShoppingCart className="w-3 h-3" /> Mua
                </button>
              </motion.div>
            </div>
            <span className={`inline-block px-2 py-0.5 rounded-full text-[10px] font-semibold mb-1 ${genreColors[book.genres?.[0]?.toLowerCase() || ''] || 'bg-gray-100 text-gray-600'}`}>
              {book.genres?.[0]}
            </span>
            <h3 className="text-gray-900 dark:text-white font-semibold text-xs leading-snug line-clamp-2 group-hover:text-indigo-600 dark:group-hover:text-indigo-400 transition-colors mb-1">{book.title}</h3>
            <div className="flex items-center justify-between">
              <div className="flex items-center gap-0.5">
                <Star className="w-3 h-3 fill-amber-400 text-amber-400" />
                <span className="text-xs text-gray-600 dark:text-gray-400">{book.rating}</span>
              </div>
              <span className="text-indigo-600 dark:text-indigo-400 text-xs font-bold">{book.price.toLocaleString('vi-VN')}₫</span>
            </div>
          </motion.div>
        ))}
      </div>
    </motion.section>
  );
}

export function Trending() {
  const { onOpenBook } = useOutletContext<OutletContextType>();
  const navigate = useNavigate();
  const [activeFilter, setActiveFilter] = useState('week');
  const [scrollY, setScrollY] = useState(0);
  const { books } = useBooks();
  const { trendingBooks } = useBookCollections();

  useEffect(() => {
    window.scrollTo(0, 0);
    const handler = () => setScrollY(window.scrollY);
    window.addEventListener('scroll', handler, { passive: true });
    return () => window.removeEventListener('scroll', handler);
  }, []);

  const topBook = trendingBooks[0];
  if (!topBook) return null;
  const sortedBooks = [...books].sort((a, b) => b.popularity - a.popularity);
  const hotBooks = sortedBooks.slice(0, 8);
  const purchasedBooks = [...books].sort((a, b) => b.ratingCount - a.ratingCount).slice(0, 8);
  const fastGrowingBooks = [...books].sort((a, b) => b.rating - a.rating).slice(0, 8);
  const discussedBooks = [...books].sort((a, b) => (b.ratingCount * b.rating) - (a.ratingCount * a.rating)).slice(0, 8);
  const editorPicks = books.filter(b => b.badge === "Editor's Choice" || b.badge === 'Bestseller').slice(0, 8);

  return (
    <div className="min-h-screen bg-[#F8F7F4] dark:bg-[#0D0C14]">

      {/* ── Trending Hero ── */}
      <div className="relative overflow-hidden min-h-[80vh] flex items-end">

        {/* Background layers */}
        <div className="absolute inset-0" style={{ transform: `translateY(${scrollY * 0.2}px)` }}>
          <img src={topBook.cover} alt="" className="w-full h-full object-cover scale-125" style={{ filter: 'blur(60px)', opacity: 0.5 }} />
        </div>
        <div className="absolute inset-0 bg-gradient-to-b from-[#F8F7F4]/50 dark:from-[#0D0C14]/50 via-[#F8F7F4]/60 dark:via-[#0D0C14]/65 to-[#F8F7F4] dark:to-[#0D0C14]" />
        <div className="absolute inset-0 opacity-25" style={{ background: `radial-gradient(ellipse at 40% 40%, #EF444460, transparent 60%), radial-gradient(ellipse at 80% 70%, #4F46E560, transparent 50%)` }} />

        {/* Floating particles */}
        {[...Array(8)].map((_, i) => (
          <motion.div
            key={i}
            className="absolute w-2 h-2 rounded-full bg-orange-400/30 dark:bg-orange-400/20"
            style={{ left: `${10 + i * 12}%`, top: `${20 + (i % 3) * 20}%` }}
            animate={{ y: [0, -20 - i * 5, 0], opacity: [0.3, 0.7, 0.3] }}
            transition={{ duration: 3 + i * 0.5, repeat: Infinity, ease: 'easeInOut', delay: i * 0.4 }}
          />
        ))}

        {/* Ambient orbs */}
        <motion.div
          animate={{ scale: [1, 1.1, 1], opacity: [0.15, 0.25, 0.15] }}
          transition={{ duration: 5, repeat: Infinity }}
          className="absolute top-24 right-24 w-80 h-80 bg-rose-400 rounded-full blur-3xl opacity-15 pointer-events-none hidden lg:block"
        />
        <motion.div
          animate={{ scale: [1, 1.08, 1], opacity: [0.1, 0.2, 0.1] }}
          transition={{ duration: 7, repeat: Infinity, delay: 1.5 }}
          className="absolute bottom-32 left-16 w-64 h-64 bg-indigo-500 rounded-full blur-3xl opacity-10 pointer-events-none hidden lg:block"
        />

        <div className="relative max-w-[1400px] mx-auto px-4 sm:px-6 lg:px-8 pb-14 pt-28 w-full">
          <div className="flex flex-col lg:flex-row gap-10 lg:gap-16 items-end">

            {/* Left: Hero text */}
            <div className="flex-1 space-y-6">
              {/* "Trending Now" animated badge */}
              <motion.div
                initial={{ opacity: 0, x: -20 }}
                animate={{ opacity: 1, x: 0 }}
                transition={{ duration: 0.5 }}
                className="flex items-center gap-2 w-fit"
              >
                <div className="relative flex items-center gap-2 px-4 py-2 rounded-full bg-gradient-to-r from-rose-500/20 to-orange-500/20 border border-rose-300/40 dark:border-rose-600/30 backdrop-blur-sm">
                  <span className="relative flex h-2.5 w-2.5">
                    <span className="animate-ping absolute inline-flex h-full w-full rounded-full bg-rose-500 opacity-75" />
                    <span className="relative inline-flex rounded-full h-2.5 w-2.5 bg-rose-500" />
                  </span>
                  <Flame className="w-4 h-4 text-rose-500" />
                  <span className="text-rose-700 dark:text-rose-400 font-bold text-sm tracking-wide">TRENDING NOW</span>
                </div>
              </motion.div>

              <motion.h1
                initial={{ opacity: 0, y: 20 }}
                animate={{ opacity: 1, y: 0 }}
                transition={{ duration: 0.6, delay: 0.1 }}
                className="text-gray-900 dark:text-white"
                style={{ fontFamily: 'var(--font-serif)', fontSize: 'clamp(2rem, 4.5vw, 3.5rem)', fontWeight: 800, lineHeight: 1.15 }}
              >
                Sách đang <br />
                <span className="bg-gradient-to-r from-rose-500 via-orange-500 to-amber-500 bg-clip-text text-transparent">
                  làm mưa làm gió
                </span>
              </motion.h1>

              <motion.p
                initial={{ opacity: 0, y: 15 }}
                animate={{ opacity: 1, y: 0 }}
                transition={{ duration: 0.5, delay: 0.2 }}
                className="text-gray-600 dark:text-gray-300 max-w-lg"
                style={{ fontSize: '1.05rem', lineHeight: 1.7 }}
              >
                Khám phá những tựa sách đang được cộng đồng quan tâm nhiều nhất. Cập nhật real-time từ hàng nghìn độc giả InkShelf.
              </motion.p>

              {/* Stats row */}
              <motion.div
                initial={{ opacity: 0, y: 15 }}
                animate={{ opacity: 1, y: 0 }}
                transition={{ duration: 0.5, delay: 0.3 }}
                className="flex flex-wrap gap-4"
              >
                {[
                  { icon: <BookOpen className="w-4 h-4 text-indigo-500" />, value: '45.2K', label: 'lượt đọc tuần này' },
                  { icon: <TrendingUp className="w-4 h-4 text-emerald-500" />, value: '+23%', label: 'so với tuần trước' },
                  { icon: <Users className="w-4 h-4 text-rose-500" />, value: '12.4K', label: 'độc giả đang online' },
                ].map(s => (
                  <div key={s.label} className="flex items-center gap-2 px-4 py-2.5 bg-white/60 dark:bg-white/8 backdrop-blur-sm rounded-2xl border border-gray-100/80 dark:border-white/8">
                    {s.icon}
                    <span className="text-gray-900 dark:text-white font-bold text-sm">{s.value}</span>
                    <span className="text-gray-500 dark:text-gray-400 text-xs">{s.label}</span>
                  </div>
                ))}
              </motion.div>
            </div>

            {/* Right: #1 Trending spotlight */}
            <motion.div
              initial={{ opacity: 0, y: 30, scale: 0.95 }}
              animate={{ opacity: 1, y: 0, scale: 1 }}
              transition={{ duration: 0.7, delay: 0.15, ease: [0.22, 1, 0.36, 1] }}
              onClick={() => onOpenBook(topBook)}
              className="cursor-pointer lg:w-80 xl:w-96 shrink-0"
            >
              <div className="relative bg-white/80 dark:bg-[#16152B]/80 backdrop-blur-xl rounded-3xl p-5 border border-gray-100/80 dark:border-white/10 shadow-2xl hover:shadow-3xl transition-shadow group">
                {/* Rank badge */}
                <div className="absolute -top-4 -left-4 w-12 h-12 rounded-2xl bg-gradient-to-br from-amber-400 to-orange-500 flex items-center justify-center shadow-lg shadow-amber-200/60 dark:shadow-amber-900/30 z-10">
                  <Trophy className="w-5 h-5 text-white" />
                </div>

                <div className="flex gap-4">
                  <div className="relative w-28 shrink-0">
                    <div
                      className="absolute -inset-2 rounded-2xl blur-xl opacity-50"
                      style={{ background: topBook.accentColor }}
                    />
                    <div className="relative rounded-2xl overflow-hidden shadow-xl" style={{ aspectRatio: '2/3' }}>
                      <ImageWithFallback src={topBook.cover} alt={topBook.title} className="w-full h-full object-cover group-hover:scale-105 transition-transform duration-500" />
                    </div>
                  </div>

                  <div className="flex-1 min-w-0 pt-2 space-y-2">
                    <div className="flex items-center gap-2 flex-wrap">
                      <span className="px-2.5 py-0.5 rounded-full text-[10px] font-bold bg-rose-500 text-white">🔥 Viral</span>
                      <span className="px-2.5 py-0.5 rounded-full text-[10px] font-bold bg-orange-500 text-white">⚡ Hot</span>
                    </div>
                    <h3 className="text-gray-900 dark:text-white font-bold text-base leading-tight" style={{ fontFamily: 'var(--font-serif)' }}>
                      {topBook.title}
                    </h3>
                    <p className="text-gray-500 dark:text-gray-400 text-xs">{topBook.author}</p>
                    <div className="flex items-center gap-1">
                      {[1, 2, 3, 4, 5].map(s => (
                        <Star key={s} className={`w-3.5 h-3.5 ${s <= Math.round(topBook.rating) ? 'fill-amber-400 text-amber-400' : 'text-gray-300'}`} />
                      ))}
                      <span className="text-xs text-gray-500 dark:text-gray-400 ml-1">({topBook.ratingCount.toLocaleString()})</span>
                    </div>

                    {/* Growth indicator */}
                    <div className="flex items-center gap-1.5 px-2 py-1 bg-emerald-50 dark:bg-emerald-900/20 rounded-xl w-fit">
                      <ArrowUpRight className="w-3 h-3 text-emerald-600 dark:text-emerald-400" />
                      <span className="text-emerald-700 dark:text-emerald-400 text-xs font-bold">+2.3K lượt đọc/ngày</span>
                    </div>

                    <div className="flex items-center justify-between">
                      <span className="text-gray-900 dark:text-white font-black text-lg">{topBook.price.toLocaleString('vi-VN')}₫</span>
                      <motion.button
                        whileTap={{ scale: 0.93 }}
                        onClick={e => { e.stopPropagation(); }}
                        className="px-3 py-1.5 bg-gradient-to-r from-indigo-600 to-violet-600 text-white rounded-xl text-xs font-bold shadow-md"
                      >
                        Mua ngay
                      </motion.button>
                    </div>
                  </div>
                </div>

                {/* Animated popularity bar */}
                <div className="mt-4">
                  <div className="flex items-center justify-between mb-1.5">
                    <span className="text-gray-500 dark:text-gray-400 text-[10px]">Độ phổ biến</span>
                    <span className="text-gray-700 dark:text-gray-300 text-[10px] font-bold">{topBook.popularity}%</span>
                  </div>
                  <div className="h-1.5 bg-gray-100 dark:bg-white/10 rounded-full overflow-hidden">
                    <motion.div
                      initial={{ width: 0 }}
                      animate={{ width: `${topBook.popularity}%` }}
                      transition={{ duration: 1.2, delay: 0.5, ease: 'easeOut' }}
                      className="h-full bg-gradient-to-r from-amber-400 to-orange-500 rounded-full"
                    />
                  </div>
                </div>
              </div>
            </motion.div>

          </div>
        </div>
      </div>

      {/* ── Filter Tabs ── */}
      <div className="sticky top-16 z-30 bg-[#F8F7F4]/90 dark:bg-[#0D0C14]/90 backdrop-blur-xl border-b border-gray-100/80 dark:border-white/8 shadow-sm">
        <div className="max-w-[1400px] mx-auto px-4 sm:px-6 lg:px-8 py-3 flex items-center gap-2 overflow-x-auto" style={{ scrollbarWidth: 'none' }}>
          <span className="text-gray-500 dark:text-gray-400 text-xs font-medium shrink-0 mr-1">Lọc theo:</span>
          {FILTERS.map(f => (
            <motion.button
              key={f.id}
              onClick={() => setActiveFilter(f.id)}
              className="relative px-4 py-1.5 rounded-full text-sm font-semibold transition-all shrink-0"
              style={{
                color: activeFilter === f.id ? 'white' : undefined,
              }}
            >
              {activeFilter === f.id && (
                <motion.div
                  layoutId="trending-filter-bg"
                  className="absolute inset-0 rounded-full bg-gradient-to-r from-indigo-600 to-violet-600 shadow-md shadow-indigo-200/60 dark:shadow-indigo-900/30"
                />
              )}
              <span className={`relative z-10 ${activeFilter === f.id ? 'text-white' : 'text-gray-600 dark:text-gray-400 hover:text-gray-900 dark:hover:text-white'}`}>
                {f.label}
              </span>
            </motion.button>
          ))}
        </div>
      </div>

      {/* ── Trending Sections ── */}
      <div className="space-y-14 py-14">

        {/* Hot Right Now */}
        <TrendingCarousel
          title="🔥 Hot Right Now"
          books={hotBooks}
          onOpenBook={onOpenBook}
          showRank
        />

        {/* Most Purchased */}
        <TrendingCarousel
          title="🏆 Most Purchased"
          books={purchasedBooks}
          onOpenBook={onOpenBook}
          emoji=""
          showRank
        />

        {/* Fastest Growing */}
        <TrendingCarousel
          title="⚡ Tăng trưởng nhanh nhất"
          books={fastGrowingBooks}
          onOpenBook={onOpenBook}
          showRank={false}
        />

        {/* ── Community Momentum ── */}
        <motion.section
          initial={{ opacity: 0, y: 32 }}
          whileInView={{ opacity: 1, y: 0 }}
          viewport={{ once: true, margin: '-60px' }}
          className="max-w-[1400px] mx-auto px-4 sm:px-6 lg:px-8"
        >
          <div className="flex items-center gap-3 mb-6">
            <div className="w-10 h-10 rounded-2xl bg-gradient-to-br from-rose-400 to-pink-500 flex items-center justify-center shadow-lg shadow-rose-200 dark:shadow-rose-900/30">
              <MessageCircle className="w-5 h-5 text-white" />
            </div>
            <div>
              <h2 className="text-gray-900 dark:text-white" style={{ fontFamily: 'var(--font-serif)', fontSize: 'clamp(1.1rem, 2vw, 1.4rem)', fontWeight: 700 }}>
                Cộng đồng đang nói gì
              </h2>
              <p className="text-gray-500 dark:text-gray-400 text-xs mt-0.5">Trending quotes & reader reactions</p>
            </div>
          </div>

          <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4">
            {communityQuotes.map((q, i) => {
              const relBook = books.find(b => b.id === q.bookId);
              if (!relBook) return null;
              return (
                <motion.div
                  key={q.id}
                  initial={{ opacity: 0, y: 20 }}
                  whileInView={{ opacity: 1, y: 0 }}
                  viewport={{ once: true }}
                  transition={{ delay: i * 0.08 }}
                  whileHover={{ y: -6 }}
                  onClick={() => onOpenBook(relBook)}
                  className="cursor-pointer group relative bg-white/80 dark:bg-[#16152B]/60 backdrop-blur-sm rounded-3xl p-5 border border-gray-100/80 dark:border-white/8 hover:shadow-xl transition-all"
                >
                  <div className="absolute inset-0 rounded-3xl opacity-0 group-hover:opacity-5 transition-opacity"
                    style={{ background: relBook.accentColor }} />

                  <Quote className="w-5 h-5 text-indigo-400 mb-3" />
                  <p className="text-gray-700 dark:text-gray-200 text-sm leading-relaxed mb-4 italic">
                    "{q.quote}"
                  </p>
                  <div className="flex items-center justify-between">
                    <div className="flex items-center gap-2">
                      <div className="w-7 h-7 rounded-full bg-gradient-to-br from-indigo-400 to-violet-500 flex items-center justify-center text-white text-xs font-bold">
                        {q.user[0]}
                      </div>
                      <span className="text-gray-600 dark:text-gray-400 text-xs font-medium">{q.user}</span>
                    </div>
                    <div className="flex items-center gap-1 text-rose-500">
                      <Heart className="w-3.5 h-3.5 fill-rose-500" />
                      <span className="text-xs font-bold">{q.reactions}</span>
                    </div>
                  </div>
                  <div className="mt-3 pt-3 border-t border-gray-100/80 dark:border-white/8 flex items-center gap-2">
                    <div className="w-6 h-8 rounded overflow-hidden shrink-0">
                      <ImageWithFallback src={relBook.cover} alt={relBook.title} className="w-full h-full object-cover" />
                    </div>
                    <span className="text-gray-500 dark:text-gray-400 text-xs truncate">{relBook.title}</span>
                  </div>
                </motion.div>
              );
            })}
          </div>
        </motion.section>

        {/* Most Discussed */}
        <TrendingCarousel
          title="💬 Được thảo luận nhiều nhất"
          books={discussedBooks}
          onOpenBook={onOpenBook}
          showRank={false}
        />

        {/* ── Trending by Genre ── */}
        <motion.section
          initial={{ opacity: 0, y: 32 }}
          whileInView={{ opacity: 1, y: 0 }}
          viewport={{ once: true, margin: '-60px' }}
          className="max-w-[1400px] mx-auto px-4 sm:px-6 lg:px-8"
        >
          <div className="flex items-center gap-3 mb-6">
            <div className="w-10 h-10 rounded-2xl bg-gradient-to-br from-violet-400 to-indigo-500 flex items-center justify-center shadow-lg shadow-violet-200 dark:shadow-violet-900/30">
              <Zap className="w-5 h-5 text-white" />
            </div>
            <div>
              <h2 className="text-gray-900 dark:text-white" style={{ fontFamily: 'var(--font-serif)', fontSize: 'clamp(1.1rem, 2vw, 1.4rem)', fontWeight: 700 }}>
                Trending theo thể loại
              </h2>
              <p className="text-gray-500 dark:text-gray-400 text-xs mt-0.5">Chọn thể loại để khám phá sâu hơn</p>
            </div>
          </div>

          <div className="grid grid-cols-2 sm:grid-cols-3 lg:grid-cols-4 xl:grid-cols-7 gap-3">
            {GENRES.map((genre, i) => {
              const booksInGenre = books.filter(b => b.genres?.some(g => g.toLowerCase() === genre.toLowerCase()));
              if (booksInGenre.length === 0) return null;
              const topInGenre = booksInGenre.sort((a, b) => b.popularity - a.popularity)[0];
              return (
                <motion.div
                  key={genre}
                  initial={{ opacity: 0, scale: 0.95 }}
                  whileInView={{ opacity: 1, scale: 1 }}
                  viewport={{ once: true }}
                  transition={{ delay: i * 0.05 }}
                  whileHover={{ y: -4, scale: 1.02 }}
                  onClick={() => navigate(`/genre/${genre}`)}
                  className="cursor-pointer group relative rounded-2xl overflow-hidden"
                  style={{ aspectRatio: '3/4' }}
                >
                  <ImageWithFallback
                    src={topInGenre.cover}
                    alt={genre}
                    className="w-full h-full object-cover group-hover:scale-110 transition-transform duration-500"
                  />
                  <div className="absolute inset-0 bg-gradient-to-t from-black/70 via-black/20 to-transparent" />
                  <div
                    className="absolute inset-0 opacity-30 group-hover:opacity-40 transition-opacity"
                    style={{ background: `linear-gradient(to top, ${topInGenre.accentColor}60, transparent)` }}
                  />
                  <div className="absolute bottom-3 left-3 right-3">
                    <p className="text-white font-bold text-sm leading-tight">{genre}</p>
                    <p className="text-white/70 text-[10px]">{booksInGenre.length} sách</p>
                  </div>
                  <div className="absolute top-3 right-3">
                    <ChevronRight className="w-5 h-5 text-white/70 group-hover:text-white group-hover:translate-x-0.5 transition-all" />
                  </div>
                </motion.div>
              );
            })}
          </div>
        </motion.section>

        {/* ── Editor Trending Picks ── */}
        <TrendingCarousel
          title="✨ Editor's Trending Picks"
          books={editorPicks}
          onOpenBook={onOpenBook}
          showRank={false}
        />

        {/* ── AI Trending Recommendation ── */}
        <motion.section
          initial={{ opacity: 0, y: 32 }}
          whileInView={{ opacity: 1, y: 0 }}
          viewport={{ once: true, margin: '-60px' }}
          className="max-w-[1400px] mx-auto px-4 sm:px-6 lg:px-8"
        >
          <div className="relative overflow-hidden rounded-3xl bg-gradient-to-br from-indigo-50 via-violet-50 to-rose-50 dark:from-indigo-950/40 dark:via-violet-950/30 dark:to-rose-950/20 border border-indigo-100/80 dark:border-indigo-800/30 p-8 sm:p-10">
            <div className="absolute top-0 right-0 w-72 h-72 bg-gradient-to-br from-violet-300/20 to-indigo-300/10 rounded-full blur-3xl translate-x-1/3 -translate-y-1/3 pointer-events-none" />
            <div className="absolute bottom-0 left-0 w-56 h-56 bg-gradient-to-tr from-rose-300/15 to-orange-300/10 rounded-full blur-3xl -translate-x-1/3 translate-y-1/3 pointer-events-none" />

            <div className="relative flex flex-col lg:flex-row gap-8 lg:gap-12">
              {/* Left: AI recommendation text */}
              <div className="lg:w-80 shrink-0">
                <div className="flex items-center gap-2 mb-3">
                  <Sparkles className="w-5 h-5 text-indigo-600 dark:text-indigo-400" />
                  <span className="text-indigo-600 dark:text-indigo-400 font-semibold text-sm">AI Recommendations</span>
                </div>
                <h3 className="text-gray-900 dark:text-white font-bold mb-3" style={{ fontFamily: 'var(--font-serif)', fontSize: '1.4rem' }}>
                  Vì thể loại này đang trending
                </h3>
                <p className="text-gray-600 dark:text-gray-300 text-sm leading-relaxed mb-4">
                  Cộng đồng đang nhanh chóng khám phá những cuốn sách này. AI của chúng tôi phân tích xu hướng đọc để gợi ý cho bạn.
                </p>
                <div className="space-y-2">
                  {['Fantasy đang tăng +45% lượt đọc', 'Self-Help được mua nhiều nhất', 'Romance hot nhất trong tháng'].map(item => (
                    <div key={item} className="flex items-center gap-2 text-sm text-gray-600 dark:text-gray-300">
                      <TrendingUp className="w-4 h-4 text-emerald-500 shrink-0" />
                      {item}
                    </div>
                  ))}
                </div>
              </div>

              {/* Right: Asymmetric book layout */}
              <div className="flex-1 flex items-start gap-4">
                {/* Featured large card */}
                {trendingBooks[1] && (
                  <motion.div
                    whileHover={{ y: -8 }}
                    onClick={() => onOpenBook(trendingBooks[1])}
                    className="cursor-pointer group shrink-0 w-40"
                  >
                    <div className="relative rounded-2xl overflow-hidden shadow-xl mb-3" style={{ aspectRatio: '2/3' }}>
                      <ImageWithFallback src={trendingBooks[1].cover} alt={trendingBooks[1].title} className="w-full h-full object-cover group-hover:scale-110 transition-transform duration-500" />
                      <motion.div
                        className="absolute inset-0 rounded-2xl pointer-events-none"
                        whileHover={{ boxShadow: `0 0 24px 4px ${trendingBooks[1].accentColor}60` }}
                      />
                    </div>
                    <h4 className="text-gray-900 dark:text-white text-sm font-semibold line-clamp-2 group-hover:text-indigo-600 dark:group-hover:text-indigo-400 transition-colors">{trendingBooks[1].title}</h4>
                    <p className="text-indigo-600 dark:text-indigo-400 font-bold text-sm mt-1">{trendingBooks[1].price.toLocaleString('vi-VN')}₫</p>
                  </motion.div>
                )}

                {/* Smaller cards grid */}
                <div className="flex-1 grid grid-cols-2 sm:grid-cols-3 gap-3">
                  {trendingBooks.slice(2, 8).map((book, i) => (
                    <motion.div
                      key={book.id}
                      whileHover={{ y: -4 }}
                      onClick={() => onOpenBook(book)}
                      className="cursor-pointer group"
                    >
                      <div className="relative rounded-xl overflow-hidden shadow-md mb-2" style={{ aspectRatio: '2/3' }}>
                        <ImageWithFallback src={book.cover} alt={book.title} className="w-full h-full object-cover group-hover:scale-110 transition-transform duration-500" />
                        <motion.div
                          className="absolute inset-0 rounded-xl pointer-events-none"
                          whileHover={{ boxShadow: `0 0 14px 2px ${book.accentColor}50` }}
                        />
                      </div>
                      <h5 className="text-gray-900 dark:text-white text-xs font-semibold line-clamp-2 group-hover:text-indigo-600 dark:group-hover:text-indigo-400 transition-colors">{book.title}</h5>
                    </motion.div>
                  ))}
                </div>
              </div>
            </div>
          </div>
        </motion.section>

      </div>
    </div>
  );
}
