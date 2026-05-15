import { useState, useEffect, useRef } from 'react';
import { motion, AnimatePresence } from 'motion/react';
import { useParams, useNavigate, useOutletContext } from 'react-router';
import {
  Star, ShoppingCart, BookOpen, Heart, Share2, ArrowLeft, ChevronRight,
  Clock, Tag, Award, MessageSquare, ThumbsUp, Play, Globe, CheckCircle,
  Layers, Calendar, TrendingUp, Sparkles, ChevronLeft, Eye
} from 'lucide-react';
import { extendedReviews, genreColors, badgeColors, genreInfo, MOODS } from '../data/books';
import { useBooks } from '../hooks/useBooks';
import { ImageWithFallback } from '../components/figma/ImageWithFallback';
import type { Book } from '../data/books';
import type { OutletContextType } from '../components/RootLayout';



export function BookDetail() {
  const { id } = useParams();
  const navigate = useNavigate();
  const { onOpenBook } = useOutletContext<OutletContextType>();
  const [isWishlisted, setIsWishlisted] = useState(false);
  const [purchased, setPurchased] = useState(false);
  const [showFullPreview, setShowFullPreview] = useState(false);
  const [purchaseSticky, setPurchaseSticky] = useState(false);
  const [scrollY, setScrollY] = useState(0);
  const heroRef = useRef<HTMLDivElement>(null);

  const { books } = useBooks();
  const book = books.find(b => String(b.id) === String(id));

  useEffect(() => {
    window.scrollTo(0, 0);
  }, [id]);

  useEffect(() => {
    const handler = () => {
      setScrollY(window.scrollY);
      if (heroRef.current) {
        const rect = heroRef.current.getBoundingClientRect();
        setPurchaseSticky(rect.bottom < 60);
      }
    };
    window.addEventListener('scroll', handler, { passive: true });
    return () => window.removeEventListener('scroll', handler);
  }, []);

  if (!book) {
    return (
      <div className="min-h-screen bg-[#F8F7F4] dark:bg-[#0D0C14] pt-24 flex items-center justify-center">
        <div className="text-center">
          <p className="text-gray-600 dark:text-gray-400 text-lg mb-4">Không tìm thấy sách</p>
          <button onClick={() => navigate('/')}
            className="px-6 py-2.5 bg-gradient-to-r from-indigo-600 to-violet-600 text-white rounded-xl font-medium hover:opacity-90 transition-all">
            Về trang chủ
          </button>
        </div>
      </div>
    );
  }

  const relatedBooks = books.filter(b => b.genres.some(g => book.genres.includes(g)) && b.id !== book.id).slice(0, 8);
  const discount = book.originalPrice ? Math.round((1 - book.price / book.originalPrice) * 100) : 0;
  const genreData = genreInfo[book.genres[0]?.toLowerCase() || ''];

  const ratingBreakdown = book.ratingBreakdown ? [
    { stars: 5, pct: book.ratingCount > 0 ? Math.round((book.ratingBreakdown[5] / book.ratingCount) * 100) : 0 },
    { stars: 4, pct: book.ratingCount > 0 ? Math.round((book.ratingBreakdown[4] / book.ratingCount) * 100) : 0 },
    { stars: 3, pct: book.ratingCount > 0 ? Math.round((book.ratingBreakdown[3] / book.ratingCount) * 100) : 0 },
    { stars: 2, pct: book.ratingCount > 0 ? Math.round((book.ratingBreakdown[2] / book.ratingCount) * 100) : 0 },
    { stars: 1, pct: book.ratingCount > 0 ? Math.round((book.ratingBreakdown[1] / book.ratingCount) * 100) : 0 },
  ] : [
    { stars: 5, pct: 68 },
    { stars: 4, pct: 20 },
    { stars: 3, pct: 8 },
    { stars: 2, pct: 2 },
    { stars: 1, pct: 2 },
  ];

  const handleBuy = () => {
    setPurchased(true);
    setTimeout(() => setPurchased(false), 2500);
  };

  return (
    <div className="min-h-screen bg-[#F8F7F4] dark:bg-[#0D0C14]">

      {/* ── Sticky Action Panel ── */}
      <AnimatePresence>
        {purchaseSticky && (
          <motion.div
            initial={{ y: -100, opacity: 0 }}
            animate={{ y: 0, opacity: 1 }}
            exit={{ y: -100, opacity: 0 }}
            transition={{ type: 'spring', stiffness: 300, damping: 30 }}
            className="fixed top-16 left-0 right-0 z-40 bg-white/95 dark:bg-[#16152B]/95 backdrop-blur-xl border-b border-gray-100/80 dark:border-white/8 shadow-lg"
          >
            <div className="max-w-[1400px] mx-auto px-4 sm:px-6 lg:px-8 py-3 flex items-center gap-4">
              <div className="w-10 h-14 rounded-lg overflow-hidden shadow-sm shrink-0">
                <ImageWithFallback src={book.cover} alt={book.title} className="w-full h-full object-cover" />
              </div>
              <div className="flex-1 min-w-0">
                <h3 className="text-gray-900 dark:text-white font-semibold text-sm truncate">{book.title}</h3>
                <p className="text-gray-500 dark:text-gray-400 text-xs">{book.author}</p>
              </div>
              <div className="hidden sm:flex items-center gap-2">
                <button className="flex items-center gap-2 px-4 py-2 bg-white dark:bg-white/10 border border-gray-200 dark:border-white/10 text-gray-700 dark:text-gray-300 rounded-xl text-sm font-semibold hover:bg-gray-50 transition-all">
                  <Eye className="w-3.5 h-3.5" /> Đọc thử
                </button>
                <button
                  onClick={() => setIsWishlisted(!isWishlisted)}
                  className={`w-9 h-9 rounded-xl flex items-center justify-center border transition-all ${isWishlisted ? 'bg-rose-50 border-rose-200 text-rose-500' : 'border-gray-200 dark:border-white/10 text-gray-500 hover:bg-gray-50 dark:hover:bg-white/5'}`}
                >
                  <Heart className={`w-4 h-4 ${isWishlisted ? 'fill-rose-500' : ''}`} />
                </button>
                <button className="w-9 h-9 rounded-xl border border-gray-200 dark:border-white/10 flex items-center justify-center text-gray-500 hover:bg-gray-50 dark:hover:bg-white/5 transition-all">
                  <Share2 className="w-4 h-4" />
                </button>
              </div>
              <div className="flex items-center gap-3">
                <div className="text-right">
                  <div className="text-indigo-600 dark:text-indigo-400 font-bold text-lg">{book.price.toLocaleString('vi-VN')}₫</div>
                  {book.originalPrice && <div className="text-gray-400 line-through text-xs">{book.originalPrice.toLocaleString('vi-VN')}₫</div>}
                </div>
                <motion.button
                  whileTap={{ scale: 0.96 }}
                  onClick={handleBuy}
                  className={`px-5 py-2.5 rounded-xl font-semibold text-sm shadow-md transition-all ${purchased ? 'bg-emerald-500 text-white' : 'bg-gradient-to-r from-indigo-600 to-violet-600 text-white shadow-indigo-200/60 dark:shadow-indigo-900/30 hover:opacity-90'}`}
                >
                  {purchased ? '✓ Đã mua!' : 'Mua ngay'}
                </motion.button>
              </div>
            </div>
          </motion.div>
        )}
      </AnimatePresence>

      {/* ── Cinematic Hero ── */}
      <div ref={heroRef} className="relative min-h-[90vh] flex items-end overflow-hidden">

        {/* Background: blurred cover */}
        <div
          className="absolute inset-0"
          style={{ transform: `translateY(${scrollY * 0.25}px)` }}
        >
          <img
            src={book.cover}
            alt=""
            className="w-full h-full object-cover scale-125"
            style={{ filter: 'blur(70px)', opacity: 0.6 }}
          />
        </div>

        {/* Gradient overlays */}
        <div className="absolute inset-0 bg-gradient-to-b from-[#F8F7F4]/40 dark:from-[#0D0C14]/40 via-[#F8F7F4]/50 dark:via-[#0D0C14]/60 to-[#F8F7F4] dark:to-[#0D0C14]" />
        <div className="absolute inset-0 bg-gradient-to-r from-[#F8F7F4]/20 dark:from-[#0D0C14]/20 to-transparent" />

        {/* Accent glow */}
        <div
          className="absolute inset-0 opacity-20"
          style={{ background: `radial-gradient(ellipse at 55% 35%, ${book.accentColor}90, transparent 65%)` }}
        />

        {/* Floating ambient orbs */}
        <motion.div
          animate={{ y: [0, -20, 0], opacity: [0.15, 0.25, 0.15] }}
          transition={{ duration: 6, repeat: Infinity, ease: 'easeInOut' }}
          className="absolute top-20 right-20 w-64 h-64 rounded-full blur-3xl pointer-events-none hidden lg:block"
          style={{ background: book.accentColor }}
        />
        <motion.div
          animate={{ y: [0, 15, 0], opacity: [0.08, 0.15, 0.08] }}
          transition={{ duration: 8, repeat: Infinity, ease: 'easeInOut', delay: 2 }}
          className="absolute bottom-40 left-10 w-48 h-48 rounded-full blur-3xl pointer-events-none hidden lg:block"
          style={{ background: book.accentColor }}
        />

        {/* Back button */}
        <div className="absolute top-20 left-0 right-0 max-w-[1400px] mx-auto px-4 sm:px-6 lg:px-8">
          <motion.button
            initial={{ opacity: 0, x: -20 }}
            animate={{ opacity: 1, x: 0 }}
            transition={{ duration: 0.4 }}
            onClick={() => navigate(-1)}
            className="flex items-center gap-2 text-gray-600 dark:text-gray-400 hover:text-gray-900 dark:hover:text-white transition-colors group"
          >
            <ArrowLeft className="w-4 h-4 group-hover:-translate-x-1 transition-transform" />
            <span className="text-sm font-medium">Quay lại</span>
          </motion.button>
        </div>

        {/* Main hero content */}
        <div className="relative max-w-[1400px] mx-auto px-4 sm:px-6 lg:px-8 pb-16 pt-8 w-full">
          <div className="flex flex-col lg:flex-row gap-10 lg:gap-16 items-end">

            {/* ── Cover Column ── */}
            <motion.div
              initial={{ opacity: 0, y: 40, scale: 0.95 }}
              animate={{ opacity: 1, y: 0, scale: 1 }}
              transition={{ duration: 0.7, ease: [0.22, 1, 0.36, 1] }}
              className="relative lg:w-72 xl:w-80 shrink-0 self-end"
            >
              {/* Glow rings */}
              <div
                className="absolute -inset-6 rounded-[40px] blur-3xl opacity-40"
                style={{ background: book.accentColor }}
              />
              <div
                className="absolute -inset-3 rounded-[36px] blur-xl opacity-20"
                style={{ background: book.accentColor }}
              />

              {/* Cover image */}
              <div
                className="relative rounded-3xl overflow-hidden shadow-2xl"
                style={{ aspectRatio: '2/3', filter: `drop-shadow(0 25px 50px ${book.accentColor}60)` }}
              >
                <ImageWithFallback
                  src={book.cover}
                  alt={book.title}
                  className="w-full h-full object-cover"
                />
                <div className="absolute inset-0 rounded-3xl ring-1 ring-white/20 pointer-events-none" />

                {/* Badge */}
                {book.badge && (
                  <div className={`absolute top-4 left-4 px-3 py-1 rounded-full text-xs font-bold shadow-lg ${badgeColors[book.badge]}`}>
                    {book.badge}
                  </div>
                )}

                {/* Status chip */}
                {book.status && (
                  <div className={`absolute bottom-4 right-4 px-3 py-1 rounded-full text-xs font-bold ${book.status === 'complete' ? 'bg-emerald-500/90 text-white' : 'bg-amber-500/90 text-white'}`}>
                    {book.status === 'complete' ? '✓ Hoàn thành' : '📖 Đang ra'}
                  </div>
                )}
              </div>

              {/* Quick stats under cover */}
              <div className="mt-4 grid grid-cols-3 gap-2">
                {[
                  { label: 'Trang', value: `${book.pages}` },
                  { label: 'Chương', value: `${book.chapters ?? Math.round(book.pages / 18)}` },
                  { label: 'Thời gian', value: book.readTime },
                ].map(s => (
                  <div key={s.label} className="bg-white/70 dark:bg-white/8 backdrop-blur-sm rounded-2xl px-3 py-2.5 text-center border border-gray-100/80 dark:border-white/8">
                    <p className="text-gray-900 dark:text-white font-bold text-sm">{s.value}</p>
                    <p className="text-gray-400 text-[10px]">{s.label}</p>
                  </div>
                ))}
              </div>
            </motion.div>

            {/* ── Info Column ── */}
            <div className="flex-1 min-w-0 space-y-5 pb-2">

              {/* Genre + mood badges */}
              <motion.div
                initial={{ opacity: 0, y: 20 }}
                animate={{ opacity: 1, y: 0 }}
                transition={{ duration: 0.5, delay: 0.1 }}
                className="flex flex-wrap gap-2"
              >
                <div className="flex gap-2">
                  {book.genres.slice(0, 3).map((g, idx) => (
                    <span key={idx} className={`px-3 py-1 rounded-full text-xs font-semibold ${genreColors[g.toLowerCase()] || 'bg-gray-100 text-gray-700'}`}>
                      {idx === 0 && genreData?.emoji ? `${genreData.emoji} ` : ''}{g}
                    </span>
                  ))}
                </div>
                {book.language && (
                  <span className="flex items-center gap-1 px-3 py-1 rounded-full text-xs font-medium bg-gray-100/80 dark:bg-white/8 text-gray-600 dark:text-gray-300 border border-gray-200/60 dark:border-white/8">
                    <Globe className="w-3 h-3" /> {book.language}
                  </span>
                )}
                <span className="flex items-center gap-1 px-3 py-1 rounded-full text-xs font-medium bg-gray-100/80 dark:bg-white/8 text-gray-600 dark:text-gray-300 border border-gray-200/60 dark:border-white/8">
                  <Calendar className="w-3 h-3" /> {book.releaseYear}
                </span>
              </motion.div>

              {/* Title */}
              <motion.h1
                initial={{ opacity: 0, y: 20 }}
                animate={{ opacity: 1, y: 0 }}
                transition={{ duration: 0.6, delay: 0.15 }}
                className="text-gray-900 dark:text-white"
                style={{
                  fontFamily: 'var(--font-serif)',
                  fontSize: 'clamp(2rem, 4vw, 3.2rem)',
                  fontWeight: 800,
                  lineHeight: 1.15,
                  letterSpacing: '-0.02em',
                }}
              >
                {book.title}
              </motion.h1>

              {/* Author */}
              <motion.p
                initial={{ opacity: 0, y: 15 }}
                animate={{ opacity: 1, y: 0 }}
                transition={{ duration: 0.5, delay: 0.2 }}
                className="text-gray-500 dark:text-gray-400 text-lg"
              >
                bởi <span className="text-gray-700 dark:text-gray-300 font-semibold">{book.author}</span>
              </motion.p>

              {/* Rating + reads */}
              <motion.div
                initial={{ opacity: 0, y: 15 }}
                animate={{ opacity: 1, y: 0 }}
                transition={{ duration: 0.5, delay: 0.25 }}
                className="flex flex-wrap items-center gap-4"
              >
                <div className="flex items-center gap-1.5">
                  {[1, 2, 3, 4, 5].map(s => (
                    <Star key={s} className={`w-5 h-5 ${s <= Math.round(book.rating) ? 'fill-amber-400 text-amber-400' : 'text-gray-300 dark:text-gray-600'}`} />
                  ))}
                  <span className="text-gray-900 dark:text-white font-bold text-lg ml-1">{book.rating}</span>
                  <span className="text-gray-500 dark:text-gray-400 text-sm">({book.ratingCount.toLocaleString()} đánh giá)</span>
                </div>
                <div className="flex items-center gap-1.5 text-gray-500 dark:text-gray-400 text-sm">
                  <TrendingUp className="w-4 h-4 text-indigo-500" />
                  Top {100 - book.popularity}% phổ biến
                </div>
              </motion.div>

              {/* Short description */}
              <motion.p
                initial={{ opacity: 0, y: 15 }}
                animate={{ opacity: 1, y: 0 }}
                transition={{ duration: 0.5, delay: 0.3 }}
                className="text-gray-600 dark:text-gray-300 leading-relaxed max-w-2xl"
                style={{ fontSize: '1.05rem' }}
              >
                {book.description} {book.longDescription.slice(0, 120)}...
              </motion.p>

              {/* Discount banner */}
              {book.originalPrice && discount > 0 && (
                <motion.div
                  initial={{ opacity: 0, scale: 0.95 }}
                  animate={{ opacity: 1, scale: 1 }}
                  transition={{ duration: 0.4, delay: 0.35 }}
                  className="inline-flex items-center gap-2.5 px-4 py-2.5 bg-gradient-to-r from-rose-50 to-orange-50 dark:from-rose-950/40 dark:to-orange-950/20 rounded-2xl border border-rose-100 dark:border-rose-800/30"
                >
                  <Tag className="w-4 h-4 text-rose-600 dark:text-rose-400" />
                  <span className="text-rose-600 dark:text-rose-400 font-bold text-sm">
                    Giảm {discount}% · Tiết kiệm {(book.originalPrice - book.price).toLocaleString('vi-VN')}₫
                  </span>
                </motion.div>
              )}

              {/* Price + CTA */}
              <motion.div
                initial={{ opacity: 0, y: 20 }}
                animate={{ opacity: 1, y: 0 }}
                transition={{ duration: 0.5, delay: 0.4 }}
                className="flex flex-wrap items-center gap-4 pt-2"
              >
                <div className="flex items-baseline gap-2">
                  <span className="text-3xl font-black text-gray-900 dark:text-white">{book.price.toLocaleString('vi-VN')}₫</span>
                  {book.originalPrice && (
                    <span className="text-gray-400 line-through text-lg">{book.originalPrice.toLocaleString('vi-VN')}₫</span>
                  )}
                </div>

                <div className="flex flex-wrap gap-3">
                  <motion.button
                    whileHover={{ scale: 1.03 }}
                    whileTap={{ scale: 0.97 }}
                    onClick={handleBuy}
                    className={`flex items-center gap-2 px-7 py-3.5 rounded-2xl font-bold shadow-lg transition-all text-sm ${purchased
                      ? 'bg-emerald-500 text-white shadow-emerald-200/60'
                      : 'bg-gradient-to-r from-indigo-600 to-violet-600 text-white shadow-indigo-200/60 dark:shadow-indigo-900/40 hover:from-indigo-500 hover:to-violet-500'
                    }`}
                  >
                    <ShoppingCart className="w-4 h-4" />
                    {purchased ? '✓ Đã mua!' : 'Mua ngay'}
                  </motion.button>

                  <motion.button
                    whileTap={{ scale: 0.97 }}
                    className="flex items-center gap-2 px-5 py-3.5 bg-white/80 dark:bg-white/10 backdrop-blur-sm text-indigo-600 dark:text-indigo-400 border border-indigo-200 dark:border-indigo-800/50 rounded-2xl font-semibold hover:bg-indigo-50 dark:hover:bg-indigo-950/30 transition-all text-sm"
                  >
                    <Play className="w-4 h-4" /> Đọc thử
                  </motion.button>

                  <motion.button
                    whileTap={{ scale: 0.97 }}
                    onClick={() => setIsWishlisted(!isWishlisted)}
                    className={`flex items-center gap-2 px-5 py-3.5 rounded-2xl font-semibold transition-all text-sm ${isWishlisted
                      ? 'bg-rose-50 dark:bg-rose-950/30 text-rose-600 dark:text-rose-400 border border-rose-200 dark:border-rose-800/50'
                      : 'bg-white/80 dark:bg-white/10 backdrop-blur-sm text-gray-600 dark:text-gray-400 border border-gray-200/80 dark:border-white/10 hover:border-gray-300 dark:hover:border-white/15'
                    }`}
                  >
                    <Heart className={`w-4 h-4 ${isWishlisted ? 'fill-rose-500 text-rose-500' : ''}`} />
                    {isWishlisted ? 'Đã lưu' : 'Wishlist'}
                  </motion.button>

                  <motion.button
                    whileTap={{ scale: 0.97 }}
                    className="w-12 h-12 bg-white/80 dark:bg-white/10 backdrop-blur-sm text-gray-600 dark:text-gray-400 border border-gray-200/80 dark:border-white/10 rounded-2xl flex items-center justify-center hover:border-gray-300 dark:hover:border-white/15 transition-all"
                  >
                    <Share2 className="w-4 h-4" />
                  </motion.button>
                </div>
              </motion.div>

            </div>
          </div>
        </div>
      </div>

      {/* ── Main Content Area ── */}
      <div className="max-w-[1400px] mx-auto px-4 sm:px-6 lg:px-8 py-12 space-y-10">
        <div className="flex flex-col lg:flex-row gap-8 lg:gap-12">

          {/* ── Left Sidebar (sticky) ── */}
          <div className="lg:w-72 xl:w-80 shrink-0">
            <div className="sticky top-24 space-y-4">

              {/* Book meta card */}
              <motion.div
                initial={{ opacity: 0, y: 20 }}
                whileInView={{ opacity: 1, y: 0 }}
                viewport={{ once: true }}
                className="bg-white/80 dark:bg-[#16152B]/60 backdrop-blur-sm rounded-3xl border border-gray-100/80 dark:border-white/8 p-5 space-y-3"
              >
                <h3 className="text-gray-900 dark:text-white font-bold text-sm flex items-center gap-2">
                  <Layers className="w-4 h-4 text-indigo-500" />
                  Thông tin sách
                </h3>
                {[
                  { label: 'Tác giả', value: book.author },
                  { label: 'Thể loại', value: book.genres.join(', ') },
                  { label: 'Ngôn ngữ', value: book.language ?? 'Tiếng Việt' },
                  { label: 'Số trang', value: `${book.pages} trang` },
                  { label: 'Số chương', value: `${book.chapters ?? Math.round(book.pages / 18)} chương` },
                  { label: 'Thời gian đọc', value: book.readTime },
                  { label: 'Năm xuất bản', value: String(book.releaseYear) },
                  { label: 'Trạng thái', value: book.status === 'complete' ? 'Hoàn thành' : 'Đang ra' },
                ].map(item => (
                  <div key={item.label} className="flex items-center justify-between py-1.5 border-b border-gray-100/80 dark:border-white/5 last:border-0">
                    <span className="text-gray-400 text-xs">{item.label}</span>
                    <span className="text-gray-900 dark:text-white text-xs font-semibold max-w-[60%] text-right">{item.value}</span>
                  </div>
                ))}
              </motion.div>

              {/* Mood tags */}
              {book.mood.length > 0 && (
                <motion.div
                  initial={{ opacity: 0, y: 20 }}
                  whileInView={{ opacity: 1, y: 0 }}
                  viewport={{ once: true }}
                  transition={{ delay: 0.1 }}
                  className="bg-white/80 dark:bg-[#16152B]/60 backdrop-blur-sm rounded-3xl border border-gray-100/80 dark:border-white/8 p-5"
                >
                  <h3 className="text-gray-900 dark:text-white font-bold text-sm mb-3">Tâm trạng phù hợp</h3>
                  <div className="flex flex-wrap gap-2">
                    {book.mood.map(m => {
                      const moodData = MOODS.find(md => md.id === m);
                      if (!moodData) return null;
                      return (
                        <span key={m} className={`px-3 py-1.5 rounded-xl text-xs font-semibold flex items-center gap-1.5 ${moodData.color}`}>
                          {moodData.emoji} {moodData.label}
                        </span>
                      );
                    })}
                  </div>
                </motion.div>
              )}

              {/* Author card */}
              <motion.div
                initial={{ opacity: 0, y: 20 }}
                whileInView={{ opacity: 1, y: 0 }}
                viewport={{ once: true }}
                transition={{ delay: 0.15 }}
                className="bg-white/80 dark:bg-[#16152B]/60 backdrop-blur-sm rounded-3xl border border-gray-100/80 dark:border-white/8 p-5"
              >
                <h3 className="text-gray-900 dark:text-white font-bold text-sm mb-3 flex items-center gap-2">
                  <Award className="w-4 h-4 text-indigo-500" /> Về tác giả
                </h3>
                <div className="flex items-center gap-3 mb-3">
                  <div className="w-10 h-10 rounded-full bg-gradient-to-br from-indigo-500 to-violet-600 flex items-center justify-center text-white font-bold text-sm shrink-0">
                    {book.author.split(' ').map(w => w[0]).join('').slice(0, 2)}
                  </div>
                  <div>
                    <p className="text-gray-900 dark:text-white font-semibold text-sm">{book.author}</p>
                    <p className="text-gray-400 text-xs">Tác giả nổi tiếng</p>
                  </div>
                </div>
                <p className="text-gray-500 dark:text-gray-400 text-xs leading-relaxed">
                  {book.author} là tác giả được yêu thích trong thể loại {book.genres[0]}, với văn phong độc đáo và khả năng xây dựng nhân vật sâu sắc. Cuốn sách "{book.title}" là một trong những tác phẩm tiêu biểu nhất.
                </p>
              </motion.div>
            </div>
          </div>

          {/* ── Right Content ── */}
          <div className="flex-1 min-w-0 space-y-8">

            {/* Full description */}
            <motion.div
              initial={{ opacity: 0, y: 20 }}
              whileInView={{ opacity: 1, y: 0 }}
              viewport={{ once: true }}
              className="bg-white/80 dark:bg-[#16152B]/60 backdrop-blur-sm rounded-3xl border border-gray-100/80 dark:border-white/8 p-6 sm:p-8"
            >
              <h2 className="text-gray-900 dark:text-white font-bold mb-4 flex items-center gap-2">
                <BookOpen className="w-5 h-5 text-indigo-600 dark:text-indigo-400" />
                Giới thiệu
              </h2>
              <p className="text-gray-600 dark:text-gray-300 leading-relaxed mb-4 text-[0.95rem]">{book.longDescription}</p>
              <p className="text-gray-500 dark:text-gray-400 text-sm italic">{book.description}</p>
            </motion.div>

            {/* ── Preview Reading Section ── */}
            {book.previewText && (
              <motion.div
                initial={{ opacity: 0, y: 20 }}
                whileInView={{ opacity: 1, y: 0 }}
                viewport={{ once: true, margin: '-80px' }}
                className="bg-white/80 dark:bg-[#16152B]/60 backdrop-blur-sm rounded-3xl border border-gray-100/80 dark:border-white/8 overflow-hidden"
              >
                {/* Header */}
                <div className="px-6 sm:px-8 pt-6 pb-4 border-b border-gray-100/80 dark:border-white/8 flex items-center justify-between">
                  <div className="flex items-center gap-2">
                    <div className="w-8 h-8 rounded-xl bg-gradient-to-br from-indigo-100 to-violet-100 dark:from-indigo-900/40 dark:to-violet-900/40 flex items-center justify-center">
                      <Eye className="w-4 h-4 text-indigo-600 dark:text-indigo-400" />
                    </div>
                    <div>
                      <h2 className="text-gray-900 dark:text-white font-bold text-base">Đọc thử</h2>
                      <p className="text-gray-400 text-xs">Chương mở đầu · Miễn phí</p>
                    </div>
                  </div>
                  <span className="px-3 py-1 rounded-full text-xs font-semibold bg-indigo-50 dark:bg-indigo-900/30 text-indigo-600 dark:text-indigo-400 border border-indigo-100 dark:border-indigo-800/50">
                    Preview
                  </span>
                </div>

                {/* Reading content */}
                <div className="relative px-6 sm:px-8 pt-6">
                  <div
                    className="prose max-w-none"
                    style={{
                      fontFamily: 'var(--font-serif)',
                      fontSize: '1.05rem',
                      lineHeight: 1.85,
                      color: 'inherit',
                    }}
                  >
                    {book.previewText.split('\n\n').map((para, i) => (
                      <p
                        key={i}
                        className="text-gray-700 dark:text-gray-300 mb-4 last:mb-0"
                        style={{ fontFamily: 'var(--font-serif)', lineHeight: 1.85 }}
                      >
                        {para}
                      </p>
                    ))}
                  </div>

                  {/* Fade overlay */}
                  {!showFullPreview && (
                    <div className="absolute bottom-0 left-0 right-0 h-28 bg-gradient-to-t from-white dark:from-[#16152B] to-transparent pointer-events-none" />
                  )}
                </div>

                {/* Continue reading CTA */}
                <div className="px-6 sm:px-8 py-6 text-center">
                  {!showFullPreview ? (
                    <div className="space-y-3">
                      <p className="text-gray-500 dark:text-gray-400 text-sm">Tiếp tục đọc với toàn bộ nội dung sách</p>
                      <div className="flex flex-wrap items-center justify-center gap-3">
                        <motion.button
                          whileHover={{ scale: 1.03 }}
                          whileTap={{ scale: 0.97 }}
                          onClick={handleBuy}
                          className="flex items-center gap-2 px-6 py-3 bg-gradient-to-r from-indigo-600 to-violet-600 text-white rounded-2xl font-semibold text-sm shadow-lg shadow-indigo-200/60 dark:shadow-indigo-900/30 hover:opacity-90"
                        >
                          <ShoppingCart className="w-4 h-4" />
                          Mua để đọc tiếp · {book.price.toLocaleString('vi-VN')}₫
                        </motion.button>
                        <button
                          onClick={() => setShowFullPreview(true)}
                          className="flex items-center gap-1.5 text-indigo-600 dark:text-indigo-400 font-semibold text-sm hover:underline"
                        >
                          Xem thêm preview <ChevronRight className="w-4 h-4" />
                        </button>
                      </div>
                    </div>
                  ) : (
                    <div className="space-y-3">
                      <p className="text-gray-500 dark:text-gray-400 text-sm">Đây là toàn bộ nội dung preview. Mua sách để đọc tiếp!</p>
                      <motion.button
                        whileHover={{ scale: 1.03 }}
                        whileTap={{ scale: 0.97 }}
                        onClick={handleBuy}
                        className="flex items-center gap-2 px-6 py-3 bg-gradient-to-r from-indigo-600 to-violet-600 text-white rounded-2xl font-semibold text-sm shadow-lg shadow-indigo-200/60 dark:shadow-indigo-900/30 hover:opacity-90 mx-auto"
                      >
                        <ShoppingCart className="w-4 h-4" />
                        Mua ngay · {book.price.toLocaleString('vi-VN')}₫
                      </motion.button>
                    </div>
                  )}
                </div>
              </motion.div>
            )}

            {/* ── Reviews & Rating ── */}
            <motion.div
              initial={{ opacity: 0, y: 20 }}
              whileInView={{ opacity: 1, y: 0 }}
              viewport={{ once: true, margin: '-80px' }}
              className="bg-white/80 dark:bg-[#16152B]/60 backdrop-blur-sm rounded-3xl border border-gray-100/80 dark:border-white/8 p-6 sm:p-8"
            >
              <div className="flex items-center gap-2 mb-6">
                <MessageSquare className="w-5 h-5 text-indigo-600 dark:text-indigo-400" />
                <h2 className="text-gray-900 dark:text-white font-bold">Đánh giá từ độc giả</h2>
              </div>

              {/* Rating overview */}
              <div className="flex flex-col sm:flex-row gap-6 sm:gap-10 mb-8 p-5 bg-gradient-to-br from-gray-50 to-gray-100/50 dark:from-white/5 dark:to-white/3 rounded-2xl border border-gray-100/80 dark:border-white/8">
                {/* Big rating number */}
                <div className="text-center sm:shrink-0">
                  <div
                    className="text-gray-900 dark:text-white mb-1"
                    style={{ fontSize: '3.5rem', fontWeight: 900, fontFamily: 'var(--font-serif)', lineHeight: 1 }}
                  >
                    {book.rating}
                  </div>
                  <div className="flex justify-center gap-0.5 mb-1">
                    {[1, 2, 3, 4, 5].map(s => (
                      <Star key={s} className={`w-4 h-4 ${s <= Math.round(book.rating) ? 'fill-amber-400 text-amber-400' : 'text-gray-300 dark:text-gray-600'}`} />
                    ))}
                  </div>
                  <p className="text-gray-400 text-xs">{book.ratingCount.toLocaleString()} đánh giá</p>
                </div>

                {/* Breakdown bars */}
                <div className="flex-1 space-y-2">
                  {ratingBreakdown.map(item => (
                    <div key={item.stars} className="flex items-center gap-3">
                      <div className="flex items-center gap-0.5 w-12 shrink-0 justify-end">
                        <span className="text-xs text-gray-500 dark:text-gray-400">{item.stars}</span>
                        <Star className="w-3 h-3 fill-amber-400 text-amber-400" />
                      </div>
                      <div className="flex-1 h-2 rounded-full bg-gray-200 dark:bg-white/10 overflow-hidden">
                        <motion.div
                          initial={{ width: 0 }}
                          whileInView={{ width: `${item.pct}%` }}
                          viewport={{ once: true }}
                          transition={{ duration: 0.8, delay: (5 - item.stars) * 0.06, ease: 'easeOut' }}
                          className="h-full rounded-full bg-gradient-to-r from-amber-400 to-amber-500"
                        />
                      </div>
                      <span className="text-xs text-gray-400 w-8 shrink-0">{item.pct}%</span>
                    </div>
                  ))}
                </div>
              </div>

              {/* Review cards */}
              <div className="space-y-4">
                {extendedReviews.map((review, i) => (
                  <motion.div
                    key={review.id}
                    initial={{ opacity: 0, y: 12 }}
                    whileInView={{ opacity: 1, y: 0 }}
                    viewport={{ once: true }}
                    transition={{ delay: i * 0.06 }}
                    whileHover={{ y: -2 }}
                    className="bg-gray-50/80 dark:bg-white/5 rounded-2xl p-4 sm:p-5 border border-gray-100/80 dark:border-white/8 hover:shadow-md transition-all"
                  >
                    <div className="flex items-start gap-3 mb-3">
                      <div className="w-10 h-10 rounded-full bg-gradient-to-br from-indigo-500 to-violet-600 flex items-center justify-center text-white font-bold text-sm shrink-0 shadow-md">
                        {review.avatar}
                      </div>
                      <div className="flex-1 min-w-0">
                        <div className="flex items-center justify-between gap-2 mb-1">
                          <p className="text-gray-900 dark:text-white font-semibold text-sm">{review.user}</p>
                          <p className="text-gray-400 text-xs shrink-0">{review.date}</p>
                        </div>
                        <div className="flex gap-0.5">
                          {[1, 2, 3, 4, 5].map(s => (
                            <Star key={s} className={`w-3.5 h-3.5 ${s <= review.rating ? 'fill-amber-400 text-amber-400' : 'text-gray-300 dark:text-gray-600'}`} />
                          ))}
                        </div>
                      </div>
                    </div>
                    <p className="text-gray-600 dark:text-gray-300 text-sm leading-relaxed mb-3">{review.text}</p>
                    <button className="flex items-center gap-1.5 text-gray-400 hover:text-indigo-600 dark:hover:text-indigo-400 text-xs font-medium transition-colors">
                      <ThumbsUp className="w-3 h-3" />
                      Hữu ích · {review.helpful}
                    </button>
                  </motion.div>
                ))}
              </div>

              <button className="mt-5 w-full py-3 bg-gray-50 dark:bg-white/5 hover:bg-gray-100 dark:hover:bg-white/8 text-gray-700 dark:text-gray-300 rounded-2xl font-medium text-sm transition-colors border border-gray-100/80 dark:border-white/8">
                Xem tất cả {book.ratingCount.toLocaleString()} đánh giá
              </button>
            </motion.div>

          </div>
        </div>
      </div>

      {/* ── Related Books Section ── */}
      {relatedBooks.length > 0 && (
        <div className="bg-white/60 dark:bg-[#16152B]/40 backdrop-blur-sm border-t border-gray-100/80 dark:border-white/8 py-12 mt-4">
          <div className="max-w-[1400px] mx-auto px-4 sm:px-6 lg:px-8">
            <div className="flex items-center justify-between mb-6">
              <div>
                <h2 className="text-gray-900 dark:text-white font-bold text-xl mb-1 flex items-center gap-2">
                  <Sparkles className="w-5 h-5 text-indigo-500" />
                  Sách liên quan
                </h2>
                <p className="text-gray-500 dark:text-gray-400 text-sm">Cùng thể loại {book.genres[0]} bạn có thể thích</p>
              </div>
              <div className="flex gap-2">
                <RelatedBooksCarousel books={relatedBooks} onOpenBook={onOpenBook} navigate={navigate} />
              </div>
            </div>
            <RelatedBooksGrid books={relatedBooks} onOpenBook={onOpenBook} navigate={navigate} />
          </div>
        </div>
      )}

      {/* ── AI Recommendation ── */}
      <div className="max-w-[1400px] mx-auto px-4 sm:px-6 lg:px-8 py-12">
        <motion.div
          initial={{ opacity: 0, y: 20 }}
          whileInView={{ opacity: 1, y: 0 }}
          viewport={{ once: true }}
          className="relative overflow-hidden rounded-3xl p-8 sm:p-10 bg-gradient-to-br from-indigo-50 via-violet-50 to-fuchsia-50 dark:from-indigo-950/40 dark:via-violet-950/30 dark:to-fuchsia-950/20 border border-indigo-100/80 dark:border-indigo-800/30"
        >
          <div className="absolute top-0 right-0 w-64 h-64 bg-gradient-to-br from-indigo-200/30 to-violet-200/20 dark:from-indigo-600/10 dark:to-violet-600/10 rounded-full blur-3xl translate-x-1/3 -translate-y-1/3 pointer-events-none" />
          <div className="relative">
            <div className="flex items-center gap-2 mb-3">
              <Sparkles className="w-5 h-5 text-indigo-600 dark:text-indigo-400" />
              <span className="text-indigo-600 dark:text-indigo-400 font-semibold text-sm">Gợi ý từ AI</span>
            </div>
            <h3 className="text-gray-900 dark:text-white font-bold text-xl mb-2" style={{ fontFamily: 'var(--font-serif)' }}>
              Vì bạn đang xem "{book.title}"
            </h3>
            <p className="text-gray-600 dark:text-gray-300 text-sm mb-6">
              Những độc giả yêu thích cuốn sách này cũng thường tìm đến những tác phẩm cùng thể loại {book.genres[0]} với phong cách tương tự.
            </p>
            <button
              onClick={() => navigate(`/genre/${book.genres[0].toLowerCase()}`)}
              className="flex items-center gap-2 px-5 py-3 bg-gradient-to-r from-indigo-600 to-violet-600 text-white rounded-2xl font-semibold text-sm shadow-lg shadow-indigo-200/60 dark:shadow-indigo-900/30 hover:opacity-90 transition-all"
            >
              Khám phá thêm {book.genres[0]} <ChevronRight className="w-4 h-4" />
            </button>
          </div>
        </motion.div>
      </div>

    </div>
  );
}

// Related books horizontal scroll
function RelatedBooksGrid({ books, onOpenBook, navigate }: { books: Book[], onOpenBook: (b: Book) => void, navigate: (path: string) => void }) {
  const scrollRef = useRef<HTMLDivElement>(null);

  const scroll = (dir: 'left' | 'right') => {
    scrollRef.current?.scrollBy({ left: dir === 'left' ? -320 : 320, behavior: 'smooth' });
  };

  return (
    <div className="relative">
      <div
        ref={scrollRef}
        className="flex gap-5 overflow-x-auto pb-4"
        style={{ scrollbarWidth: 'none' }}
      >
        {books.map((relBook, i) => (
          <motion.div
            key={relBook.id}
            initial={{ opacity: 0, y: 20 }}
            whileInView={{ opacity: 1, y: 0 }}
            viewport={{ once: true }}
            transition={{ delay: i * 0.06 }}
            whileHover={{ y: -8 }}
            onClick={() => navigate(`/book/${relBook.id}`)}
            className="cursor-pointer group shrink-0 w-36 sm:w-40"
          >
            <div className="relative rounded-2xl overflow-hidden shadow-md mb-3 group-hover:shadow-xl transition-shadow" style={{ aspectRatio: '2/3' }}>
              <ImageWithFallback
                src={relBook.cover}
                alt={relBook.title}
                className="w-full h-full object-cover group-hover:scale-110 transition-transform duration-500"
              />
              <div className="absolute inset-0 bg-black/0 group-hover:bg-black/20 transition-colors" />
              <motion.div
                className="absolute inset-0 rounded-2xl pointer-events-none"
                initial={false}
                whileHover={{ boxShadow: `0 0 20px 3px ${relBook.accentColor}50` }}
              />
              {relBook.badge && (
                <span className={`absolute top-2 left-2 px-2 py-0.5 rounded-full text-[10px] font-bold ${badgeColors[relBook.badge]}`}>
                  {relBook.badge}
                </span>
              )}
            </div>
            <h4 className="text-gray-900 dark:text-white text-xs font-semibold line-clamp-2 mb-1 px-0.5 group-hover:text-indigo-600 dark:group-hover:text-indigo-400 transition-colors">
              {relBook.title}
            </h4>
            <div className="flex items-center gap-1 px-0.5">
              <Star className="w-3 h-3 fill-amber-400 text-amber-400" />
              <span className="text-gray-600 dark:text-gray-400 text-xs">{relBook.rating}</span>
              <span className="text-indigo-600 dark:text-indigo-400 text-xs font-bold ml-auto">{relBook.price.toLocaleString('vi-VN')}₫</span>
            </div>
          </motion.div>
        ))}
      </div>
    </div>
  );
}

function RelatedBooksCarousel({ books, onOpenBook, navigate }: { books: Book[], onOpenBook: (b: Book) => void, navigate: (path: string) => void }) {
  return (
    <button
      onClick={() => navigate(`/genre/${books[0]?.genres[0]?.toLowerCase()}`)}
      className="flex items-center gap-1 text-indigo-600 dark:text-indigo-400 hover:gap-2 transition-all font-medium text-sm"
    >
      Xem tất cả <ChevronRight className="w-4 h-4" />
    </button>
  );
}
