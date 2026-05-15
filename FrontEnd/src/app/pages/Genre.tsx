import { useState, useEffect } from 'react';
import { motion, AnimatePresence } from 'motion/react';
import { useParams, useNavigate, useOutletContext } from 'react-router';
import {
  Star, ShoppingCart, ChevronRight, TrendingUp, Sparkles, BookOpen,
  Eye, ArrowLeft, Award, Clock, Zap, Filter
} from 'lucide-react';
import { genreInfo, genreColors, badgeColors, GENRES } from '../data/books';
import { useBooks } from '../hooks/useBooks';
import type { Book } from '../data/books';
import { ImageWithFallback } from '../components/figma/ImageWithFallback';
import { BookCard } from '../components/BookCard';
import { BookShelf } from '../components/BookShelf';
import type { OutletContextType } from '../components/RootLayout';

export function Genre() {
  const { genre } = useParams<{ genre: string }>();
  const navigate = useNavigate();
  const { onOpenBook } = useOutletContext<OutletContextType>();
  const { books } = useBooks();
  const [selectedSubgenre, setSelectedSubgenre] = useState<string | null>(null);
  const [selectedMood, setSelectedMood] = useState<string | null>(null);
  const [scrollY, setScrollY] = useState(0);

  useEffect(() => {
    window.scrollTo(0, 0);
    const handler = () => setScrollY(window.scrollY);
    window.addEventListener('scroll', handler, { passive: true });
    return () => window.removeEventListener('scroll', handler);
  }, [genre]);

  if (!genre) {
    return <NotFound navigate={navigate} />;
  }

  const info = genreInfo[genre];
  const genreBooks = books.filter(b => b.genres.map(g => g.toLowerCase()).includes(genre.toLowerCase()));

  if (genreBooks.length === 0) {
    return <NotFound navigate={navigate} message={`Chưa có sách trong thể loại "${genre}"`} />;
  }

  const sortedGenreBooks = [...genreBooks].sort((a, b) => b.popularity - a.popularity);
  const featuredBooks = sortedGenreBooks.slice(0, 3);
  const editorChoice = genreBooks.filter(b => b.badge === "Editor's Choice" || b.badge === 'Bestseller');
  const newArrivals = genreBooks.filter(b => b.badge === 'New' || b.releaseYear === 2024);
  const trendingInGenre = genreBooks.filter(b => b.badge === 'Trending' || b.popularity >= 85);
  const hiddenGems = genreBooks.filter(b => b.rating >= 4.3 && !b.badge).slice(0, 6);
  const allBooks = sortedGenreBooks;

  const accentColor = info?.accentColor ?? '#4F46E5';
  const emoji = info?.emoji ?? '📚';

  return (
    <div className="min-h-screen bg-[#F8F7F4] dark:bg-[#0D0C14]">

      {/* ── Genre Hero ── */}
      <div className="relative min-h-[75vh] flex items-end overflow-hidden">

        {/* Background */}
        <div className="absolute inset-0" style={{ transform: `translateY(${scrollY * 0.2}px)` }}>
          {featuredBooks[0] && (
            <img
              src={featuredBooks[0].cover}
              alt=""
              className="w-full h-full object-cover scale-125"
              style={{ filter: 'blur(65px)', opacity: 0.55 }}
            />
          )}
        </div>

        {/* Gradient overlays */}
        <div className="absolute inset-0 bg-gradient-to-b from-[#F8F7F4]/50 dark:from-[#0D0C14]/50 via-[#F8F7F4]/55 dark:via-[#0D0C14]/65 to-[#F8F7F4] dark:to-[#0D0C14]" />

        {/* Genre-specific tinted overlay */}
        {info && (
          <div
            className="absolute inset-0 opacity-15"
            style={{ background: `radial-gradient(ellipse at 50% 40%, ${info.accentColor}70, transparent 65%)` }}
          />
        )}

        {/* Ambient orbs */}
        <motion.div
          animate={{ y: [0, -18, 0], opacity: [0.12, 0.22, 0.12] }}
          transition={{ duration: 6, repeat: Infinity, ease: 'easeInOut' }}
          className="absolute top-24 right-24 w-72 h-72 rounded-full blur-3xl opacity-15 pointer-events-none hidden lg:block"
          style={{ background: accentColor }}
        />

        {/* Back button */}
        <div className="absolute top-20 left-0 right-0 max-w-[1400px] mx-auto px-4 sm:px-6 lg:px-8">
          <motion.button
            initial={{ opacity: 0, x: -20 }}
            animate={{ opacity: 1, x: 0 }}
            onClick={() => navigate(-1)}
            className="flex items-center gap-2 text-gray-600 dark:text-gray-400 hover:text-gray-900 dark:hover:text-white transition-colors group"
          >
            <ArrowLeft className="w-4 h-4 group-hover:-translate-x-1 transition-transform" />
            <span className="text-sm font-medium">Quay lại</span>
          </motion.button>
        </div>

        <div className="relative max-w-[1400px] mx-auto px-4 sm:px-6 lg:px-8 pb-14 pt-8 w-full">
          <div className="flex flex-col lg:flex-row gap-10 lg:gap-16 items-end">

            {/* Left: Genre info */}
            <div className="flex-1 space-y-5">
              <motion.div
                initial={{ opacity: 0, x: -20 }}
                animate={{ opacity: 1, x: 0 }}
                transition={{ duration: 0.5 }}
                className="flex items-center gap-2 w-fit px-4 py-2 rounded-full bg-white/60 dark:bg-white/8 backdrop-blur-sm border border-gray-100/80 dark:border-white/10"
              >
                <span className="text-xl">{emoji}</span>
                <span className="text-gray-600 dark:text-gray-300 font-semibold text-sm">Thể loại</span>
              </motion.div>

              <motion.h1
                initial={{ opacity: 0, y: 20 }}
                animate={{ opacity: 1, y: 0 }}
                transition={{ duration: 0.6, delay: 0.1 }}
                className="text-gray-900 dark:text-white"
                style={{
                  fontFamily: 'var(--font-serif)',
                  fontSize: 'clamp(2.5rem, 5vw, 4rem)',
                  fontWeight: 800,
                  lineHeight: 1.1,
                }}
              >
                {genre}
              </motion.h1>

              {info && (
                <motion.p
                  initial={{ opacity: 0, y: 15 }}
                  animate={{ opacity: 1, y: 0 }}
                  transition={{ duration: 0.5, delay: 0.2 }}
                  className="text-gray-600 dark:text-gray-300 max-w-xl"
                  style={{ fontSize: '1.05rem', lineHeight: 1.7 }}
                >
                  {info.description}
                </motion.p>
              )}

              {/* Stats */}
              <motion.div
                initial={{ opacity: 0, y: 15 }}
                animate={{ opacity: 1, y: 0 }}
                transition={{ duration: 0.5, delay: 0.25 }}
                className="flex flex-wrap gap-3"
              >
                {[
                  { icon: <BookOpen className="w-4 h-4 text-indigo-500" />, value: `${genreBooks.length}`, label: 'sách' },
                  { icon: <Star className="w-4 h-4 fill-amber-400 text-amber-400" />, value: `${(genreBooks.reduce((s, b) => s + b.rating, 0) / genreBooks.length).toFixed(1)}`, label: 'đánh giá TB' },
                  { icon: <TrendingUp className="w-4 h-4 text-emerald-500" />, value: `${genreBooks.filter(b => b.badge).length}`, label: 'sách nổi bật' },
                ].map(s => (
                  <div key={s.label} className="flex items-center gap-2 px-4 py-2.5 bg-white/60 dark:bg-white/8 backdrop-blur-sm rounded-2xl border border-gray-100/80 dark:border-white/8">
                    {s.icon}
                    <span className="text-gray-900 dark:text-white font-bold text-sm">{s.value}</span>
                    <span className="text-gray-500 dark:text-gray-400 text-xs">{s.label}</span>
                  </div>
                ))}
              </motion.div>
            </div>

            {/* Right: Featured books showcase */}
            <motion.div
              initial={{ opacity: 0, y: 30 }}
              animate={{ opacity: 1, y: 0 }}
              transition={{ duration: 0.7, delay: 0.15, ease: [0.22, 1, 0.36, 1] }}
              className="hidden lg:flex items-end gap-4 shrink-0"
            >
              {featuredBooks.slice(0, 3).map((book, i) => (
                <motion.div
                  key={book.id}
                  whileHover={{ y: -12 }}
                  onClick={() => onOpenBook(book)}
                  className="cursor-pointer group"
                  style={{
                    width: i === 1 ? '120px' : '96px',
                    transform: i === 0 ? 'translateY(16px)' : i === 2 ? 'translateY(24px)' : undefined,
                  }}
                >
                  <div className="relative rounded-2xl overflow-hidden shadow-xl" style={{ aspectRatio: '2/3' }}>
                    <ImageWithFallback
                      src={book.cover}
                      alt={book.title}
                      className="w-full h-full object-cover group-hover:scale-110 transition-transform duration-500"
                    />
                    <div
                      className="absolute -inset-2 -z-10 blur-2xl opacity-40 rounded-full"
                      style={{ background: book.accentColor }}
                    />
                    <motion.div
                      className="absolute inset-0 rounded-2xl pointer-events-none"
                      whileHover={{ boxShadow: `0 0 24px 4px ${book.accentColor}60` }}
                    />
                    {book.badge && (
                      <span className={`absolute top-2 left-2 px-2 py-0.5 rounded-full text-[10px] font-bold ${badgeColors[book.badge]}`}>
                        {book.badge}
                      </span>
                    )}
                  </div>
                </motion.div>
              ))}
            </motion.div>

          </div>
        </div>
      </div>

      {/* ── Subgenre & Mood Tags ── */}
      {info && (
        <div className="sticky top-16 z-30 bg-[#F8F7F4]/90 dark:bg-[#0D0C14]/90 backdrop-blur-xl border-b border-gray-100/80 dark:border-white/8">
          <div className="max-w-[1400px] mx-auto px-4 sm:px-6 lg:px-8 py-3 space-y-2">
            {/* Subgenres */}
            <div className="flex items-center gap-2 overflow-x-auto" style={{ scrollbarWidth: 'none' }}>
              <Filter className="w-3.5 h-3.5 text-gray-400 shrink-0" />
              <span className="text-gray-400 text-xs shrink-0 mr-1">Phụ thể loại:</span>
              <button
                onClick={() => setSelectedSubgenre(null)}
                className={`shrink-0 px-3 py-1.5 rounded-full text-xs font-semibold transition-all ${selectedSubgenre === null ? 'bg-gray-900 dark:bg-white text-white dark:text-gray-900' : 'bg-white/70 dark:bg-white/8 text-gray-600 dark:text-gray-400 border border-gray-200 dark:border-white/10 hover:bg-gray-100 dark:hover:bg-white/12'}`}
              >
                Tất cả
              </button>
              {info.subgenres.map(sg => (
                <motion.button
                  key={sg}
                  onClick={() => setSelectedSubgenre(selectedSubgenre === sg ? null : sg)}
                  whileTap={{ scale: 0.95 }}
                  className="relative shrink-0 px-3 py-1.5 rounded-full text-xs font-semibold transition-all overflow-hidden"
                >
                  {selectedSubgenre === sg && (
                    <motion.div
                      layoutId={`subgenre-bg-${genre}`}
                      className="absolute inset-0 rounded-full"
                      style={{ background: `linear-gradient(135deg, ${accentColor}, ${accentColor}dd)` }}
                    />
                  )}
                  <span className={`relative z-10 ${selectedSubgenre === sg ? 'text-white' : 'text-gray-600 dark:text-gray-400 hover:text-gray-900 dark:hover:text-white'}`}>
                    {sg}
                  </span>
                  {selectedSubgenre !== sg && (
                    <div className="absolute inset-0 rounded-full bg-white/70 dark:bg-white/8 border border-gray-200 dark:border-white/10 -z-10" />
                  )}
                </motion.button>
              ))}
            </div>

            {/* Mood tags */}
            <div className="flex items-center gap-2 overflow-x-auto" style={{ scrollbarWidth: 'none' }}>
              <span className="text-gray-400 text-xs shrink-0 ml-4 mr-1">Tâm trạng:</span>
              {info.moodTags.map(tag => (
                <motion.button
                  key={tag}
                  onClick={() => setSelectedMood(selectedMood === tag ? null : tag)}
                  whileTap={{ scale: 0.95 }}
                  className={`shrink-0 px-3 py-1 rounded-full text-xs font-medium transition-all border ${selectedMood === tag
                    ? 'text-white border-transparent'
                    : 'bg-white/60 dark:bg-white/8 text-gray-600 dark:text-gray-400 border-gray-200/60 dark:border-white/10 hover:bg-gray-100 dark:hover:bg-white/12'
                  }`}
                  style={selectedMood === tag ? { background: accentColor, borderColor: accentColor } : undefined}
                >
                  {tag}
                </motion.button>
              ))}
            </div>
          </div>
        </div>
      )}

      {/* ── Main Content ── */}
      <div className="space-y-14 py-12">

        {/* ── Featured Books (cinematic large cards) ── */}
        {featuredBooks.length > 0 && (
          <motion.section
            initial={{ opacity: 0, y: 28 }}
            whileInView={{ opacity: 1, y: 0 }}
            viewport={{ once: true, margin: '-60px' }}
            className="max-w-[1400px] mx-auto px-4 sm:px-6 lg:px-8"
          >
            <div className="flex items-center justify-between mb-6">
              <div className="flex items-center gap-3">
                <div className="w-10 h-10 rounded-2xl flex items-center justify-center shadow-lg text-xl"
                  style={{ background: `linear-gradient(135deg, ${accentColor}30, ${accentColor}15)`, border: `1px solid ${accentColor}30` }}>
                  {emoji}
                </div>
                <div>
                  <h2 className="text-gray-900 dark:text-white" style={{ fontFamily: 'var(--font-serif)', fontSize: 'clamp(1.1rem, 2vw, 1.4rem)', fontWeight: 700 }}>
                    Nổi bật nhất trong {genre}
                  </h2>
                  <p className="text-gray-500 dark:text-gray-400 text-xs mt-0.5">Được cộng đồng yêu thích nhất</p>
                </div>
              </div>
            </div>

            <div className="grid grid-cols-1 md:grid-cols-3 gap-5">
              {featuredBooks.map((book, i) => (
                <motion.div
                  key={book.id}
                  initial={{ opacity: 0, y: 20 }}
                  whileInView={{ opacity: 1, y: 0 }}
                  viewport={{ once: true }}
                  transition={{ delay: i * 0.1 }}
                  whileHover={{ y: -8 }}
                  onClick={() => onOpenBook(book)}
                  className="cursor-pointer group relative bg-white/80 dark:bg-[#16152B]/60 backdrop-blur-sm rounded-3xl overflow-hidden border border-gray-100/80 dark:border-white/8 hover:shadow-2xl transition-all"
                >
                  {/* Accent glow behind card */}
                  <div
                    className="absolute inset-0 opacity-0 group-hover:opacity-8 transition-opacity rounded-3xl"
                    style={{ background: book.accentColor }}
                  />

                  <div className="flex gap-4 p-5">
                    <div className="relative w-24 shrink-0">
                      <div
                        className="absolute -inset-2 rounded-2xl blur-xl opacity-0 group-hover:opacity-40 transition-opacity"
                        style={{ background: book.accentColor }}
                      />
                      <div className="relative rounded-2xl overflow-hidden shadow-xl" style={{ aspectRatio: '2/3' }}>
                        <ImageWithFallback src={book.cover} alt={book.title} className="w-full h-full object-cover group-hover:scale-110 transition-transform duration-500" />
                      </div>
                    </div>

                    <div className="flex-1 min-w-0 space-y-2">
                      {book.badge && (
                        <span className={`inline-block px-2.5 py-0.5 rounded-full text-xs font-bold ${badgeColors[book.badge]}`}>
                          {book.badge}
                        </span>
                      )}
                      {i === 0 && (
                        <span className="inline-block ml-1.5 px-2 py-0.5 rounded-full text-xs font-bold bg-amber-400 text-amber-900">
                          #1 {genre}
                        </span>
                      )}
                      <h3 className="text-gray-900 dark:text-white font-bold text-sm leading-snug group-hover:text-indigo-600 dark:group-hover:text-indigo-400 transition-colors"
                        style={{ fontFamily: 'var(--font-serif)' }}>
                        {book.title}
                      </h3>
                      <p className="text-gray-500 dark:text-gray-400 text-xs">{book.author}</p>
                      <div className="flex items-center gap-1">
                        <Star className="w-3.5 h-3.5 fill-amber-400 text-amber-400" />
                        <span className="text-xs font-semibold text-gray-700 dark:text-gray-300">{book.rating}</span>
                        <span className="text-xs text-gray-400">({book.ratingCount.toLocaleString()})</span>
                      </div>
                      <p className="text-gray-500 dark:text-gray-400 text-xs leading-relaxed line-clamp-2">{book.description}</p>
                      <div className="flex items-center justify-between pt-1">
                        <span className="text-gray-900 dark:text-white font-black text-base">{book.price.toLocaleString('vi-VN')}₫</span>
                        <motion.button
                          whileTap={{ scale: 0.93 }}
                          onClick={e => { e.stopPropagation(); }}
                          className="flex items-center gap-1.5 px-3 py-1.5 bg-gradient-to-r from-indigo-600 to-violet-600 text-white rounded-xl text-xs font-bold shadow-md hover:opacity-90 transition-opacity"
                        >
                          <ShoppingCart className="w-3 h-3" /> Mua
                        </motion.button>
                      </div>
                    </div>
                  </div>
                  {/* Popularity bar */}
                  <div className="h-0.5 bg-gray-100 dark:bg-white/5">
                    <motion.div
                      initial={{ width: 0 }}
                      whileInView={{ width: `${book.popularity}%` }}
                      viewport={{ once: true }}
                      transition={{ duration: 0.8, delay: i * 0.1 + 0.3 }}
                      className="h-full"
                      style={{ background: `linear-gradient(to right, ${book.accentColor}, ${book.accentColor}99)` }}
                    />
                  </div>
                </motion.div>
              ))}
            </div>
          </motion.section>
        )}

        {/* ── Editor's Choice in Genre ── */}
        {editorChoice.length > 0 && (
          <BookShelf
            title={`Editor's Choice · ${genre}`}
            subtitle="Được các biên tập viên InkShelf chọn lọc kỹ càng"
            emoji="✨"
            books={editorChoice}
            onOpenBook={onOpenBook}
            accentColor={accentColor}
          />
        )}

        {/* ── Discovery Grid (all books in genre) ── */}
        <motion.section
          initial={{ opacity: 0, y: 28 }}
          whileInView={{ opacity: 1, y: 0 }}
          viewport={{ once: true, margin: '-60px' }}
          className="max-w-[1400px] mx-auto px-4 sm:px-6 lg:px-8"
        >
          <div className="flex items-center justify-between mb-6">
            <div className="flex items-center gap-3">
              <div className="w-10 h-10 rounded-2xl bg-gradient-to-br from-indigo-100 to-violet-100 dark:from-indigo-900/40 dark:to-violet-900/30 flex items-center justify-center shadow-sm">
                <BookOpen className="w-5 h-5 text-indigo-600 dark:text-indigo-400" />
              </div>
              <div>
                <h2 className="text-gray-900 dark:text-white" style={{ fontFamily: 'var(--font-serif)', fontSize: 'clamp(1.1rem, 2vw, 1.4rem)', fontWeight: 700 }}>
                  Thư viện {genre}
                </h2>
                <p className="text-gray-500 dark:text-gray-400 text-xs mt-0.5">{allBooks.length} tựa sách · Cập nhật liên tục</p>
              </div>
            </div>
            {selectedSubgenre && (
              <button
                onClick={() => setSelectedSubgenre(null)}
                className="text-indigo-600 dark:text-indigo-400 text-sm font-medium hover:underline"
              >
                Xoá bộ lọc
              </button>
            )}
          </div>

          <div className="grid grid-cols-2 sm:grid-cols-3 md:grid-cols-4 lg:grid-cols-5 xl:grid-cols-6 gap-5">
            {allBooks.map((book, i) => (
              <motion.div
                key={book.id}
                initial={{ opacity: 0, y: 20 }}
                whileInView={{ opacity: 1, y: 0 }}
                viewport={{ once: true }}
                transition={{ delay: Math.min(i * 0.04, 0.4) }}
              >
                <BookCard book={book} onOpen={onOpenBook} />
              </motion.div>
            ))}
          </div>
        </motion.section>

        {/* ── Trending in Genre ── */}
        {trendingInGenre.length > 0 && (
          <BookShelf
            title={`Trending trong ${genre}`}
            subtitle="Được đọc nhiều nhất tuần này"
            emoji="🔥"
            books={trendingInGenre}
            onOpenBook={onOpenBook}
            accentColor={accentColor}
          />
        )}

        {/* ── New Arrivals ── */}
        {newArrivals.length > 0 && (
          <BookShelf
            title={`Mới thêm · ${genre}`}
            subtitle="Những tựa sách vừa được thêm vào InkShelf"
            emoji="✨"
            books={newArrivals}
            onOpenBook={onOpenBook}
            accentColor={accentColor}
          />
        )}

        {/* ── Hidden Gems ── */}
        {hiddenGems.length > 0 && (
          <motion.section
            initial={{ opacity: 0, y: 28 }}
            whileInView={{ opacity: 1, y: 0 }}
            viewport={{ once: true, margin: '-60px' }}
            className="max-w-[1400px] mx-auto px-4 sm:px-6 lg:px-8"
          >
            <div className="flex items-center gap-3 mb-6">
              <div className="w-10 h-10 rounded-2xl bg-gradient-to-br from-amber-100 to-orange-100 dark:from-amber-900/30 dark:to-orange-900/20 flex items-center justify-center shadow-sm">
                <Zap className="w-5 h-5 text-amber-600 dark:text-amber-400" />
              </div>
              <div>
                <h2 className="text-gray-900 dark:text-white" style={{ fontFamily: 'var(--font-serif)', fontSize: 'clamp(1.1rem, 2vw, 1.4rem)', fontWeight: 700 }}>
                  Hidden Gems · {genre}
                </h2>
                <p className="text-gray-500 dark:text-gray-400 text-xs mt-0.5">Ít được biết đến nhưng chất lượng cao</p>
              </div>
            </div>

            <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-4">
              {hiddenGems.map((book, i) => (
                <motion.div
                  key={book.id}
                  initial={{ opacity: 0, y: 16 }}
                  whileInView={{ opacity: 1, y: 0 }}
                  viewport={{ once: true }}
                  transition={{ delay: i * 0.08 }}
                  whileHover={{ y: -4 }}
                  onClick={() => onOpenBook(book)}
                  className="cursor-pointer group flex gap-4 bg-white/80 dark:bg-[#16152B]/60 backdrop-blur-sm rounded-2xl p-4 border border-gray-100/80 dark:border-white/8 hover:shadow-lg transition-all"
                >
                  <div className="relative w-16 shrink-0 rounded-xl overflow-hidden shadow-md" style={{ aspectRatio: '2/3' }}>
                    <ImageWithFallback src={book.cover} alt={book.title} className="w-full h-full object-cover group-hover:scale-110 transition-transform duration-500" />
                  </div>
                  <div className="flex-1 min-w-0">
                    <h4 className="text-gray-900 dark:text-white font-semibold text-sm line-clamp-2 group-hover:text-indigo-600 dark:group-hover:text-indigo-400 transition-colors mb-1">{book.title}</h4>
                    <p className="text-gray-500 dark:text-gray-400 text-xs mb-2">{book.author}</p>
                    <div className="flex items-center gap-1 mb-2">
                      <Star className="w-3 h-3 fill-amber-400 text-amber-400" />
                      <span className="text-xs text-gray-600 dark:text-gray-400 font-medium">{book.rating}</span>
                      <span className="text-gray-400 text-[10px]">· {book.readTime}</span>
                    </div>
                    <span className="text-indigo-600 dark:text-indigo-400 font-bold text-sm">{book.price.toLocaleString('vi-VN')}₫</span>
                  </div>
                </motion.div>
              ))}
            </div>
          </motion.section>
        )}

        {/* ── AI Recommendation for Genre ── */}
        <motion.section
          initial={{ opacity: 0, y: 28 }}
          whileInView={{ opacity: 1, y: 0 }}
          viewport={{ once: true, margin: '-60px' }}
          className="max-w-[1400px] mx-auto px-4 sm:px-6 lg:px-8"
        >
          <div
            className="relative overflow-hidden rounded-3xl p-8 sm:p-10 border"
            style={{
              background: `linear-gradient(135deg, ${accentColor}10, ${accentColor}05)`,
              borderColor: `${accentColor}25`,
            }}
          >
            <div
              className="absolute top-0 right-0 w-72 h-72 rounded-full blur-3xl opacity-15 translate-x-1/3 -translate-y-1/3 pointer-events-none"
              style={{ background: accentColor }}
            />
            <div className="relative flex flex-col lg:flex-row gap-8 items-start">
              <div className="flex-1">
                <div className="flex items-center gap-2 mb-3">
                  <Sparkles className="w-5 h-5" style={{ color: accentColor }} />
                  <span className="font-semibold text-sm" style={{ color: accentColor }}>Gợi ý AI · Dành cho bạn</span>
                </div>
                <h3 className="text-gray-900 dark:text-white font-bold mb-3" style={{ fontFamily: 'var(--font-serif)', fontSize: '1.4rem' }}>
                  Vì bạn yêu thích {genre}
                </h3>
                <p className="text-gray-600 dark:text-gray-300 text-sm leading-relaxed mb-5">
                  Những độc giả yêu thích thể loại {genre} cũng thường khám phá những thể loại liên quan. Mở rộng thế giới đọc sách của bạn!
                </p>
                <div className="flex flex-wrap gap-2">
                  {GENRES.filter(g => g !== genre).slice(0, 4).map(g => (
                    <motion.button
                      key={g}
                      whileHover={{ scale: 1.05 }}
                      whileTap={{ scale: 0.95 }}
                      onClick={() => navigate(`/genre/${g}`)}
                      className="flex items-center gap-1.5 px-4 py-2 rounded-xl text-sm font-semibold bg-white/60 dark:bg-white/8 text-gray-700 dark:text-gray-300 border border-gray-200/60 dark:border-white/10 hover:shadow-md transition-all"
                    >
                      Khám phá {g} <ChevronRight className="w-3.5 h-3.5" />
                    </motion.button>
                  ))}
                </div>
              </div>

              {/* Mini book preview grid */}
              <div className="flex gap-3 shrink-0">
                {sortedGenreBooks.slice(0, 4).map((book, i) => (
                  <motion.div
                    key={book.id}
                    whileHover={{ y: -8 }}
                    onClick={() => onOpenBook(book)}
                    className="cursor-pointer w-16 sm:w-20"
                    style={{ transform: `translateY(${i % 2 === 0 ? '0' : '12px'})` }}
                  >
                    <div className="relative rounded-xl overflow-hidden shadow-md hover:shadow-xl transition-shadow" style={{ aspectRatio: '2/3' }}>
                      <ImageWithFallback src={book.cover} alt={book.title} className="w-full h-full object-cover" />
                      <motion.div
                        className="absolute inset-0 rounded-xl pointer-events-none"
                        whileHover={{ boxShadow: `0 0 14px 2px ${book.accentColor}50` }}
                      />
                    </div>
                  </motion.div>
                ))}
              </div>
            </div>
          </div>
        </motion.section>

      </div>
    </div>
  );
}

function NotFound({ navigate, message = 'Không tìm thấy thể loại này' }: { navigate: (p: string) => void; message?: string }) {
  return (
    <div className="min-h-screen bg-[#F8F7F4] dark:bg-[#0D0C14] pt-24 flex items-center justify-center">
      <div className="text-center">
        <span className="text-5xl mb-4 block">📚</span>
        <p className="text-gray-600 dark:text-gray-400 text-lg mb-4">{message}</p>
        <button onClick={() => navigate('/')}
          className="px-6 py-2.5 bg-gradient-to-r from-indigo-600 to-violet-600 text-white rounded-xl font-medium hover:opacity-90 transition-all">
          Về trang chủ
        </button>
      </div>
    </div>
  );
}
