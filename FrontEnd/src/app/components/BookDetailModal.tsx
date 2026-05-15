import { useEffect, useState } from 'react';
import { motion, AnimatePresence } from 'motion/react';
import { useNavigate } from 'react-router';
import {
  X, Star, BookOpen, ShoppingCart, Play, Heart,
  Share2, ChevronRight, Clock, FileText, Calendar, Users, ExternalLink
} from 'lucide-react';
import { reviews, badgeColors, genreColors } from '../data/books';
import type { Book } from '../data/books';
import { useBooks } from '../hooks/useBooks';
import { ImageWithFallback } from './figma/ImageWithFallback';
import { BookCard } from './BookCard';

interface BookDetailModalProps {
  bookId: string | number | null;
  onClose: () => void;
  onOpenBook: (book: Book) => void;
}

export function BookDetailModal({ bookId, onClose, onOpenBook }: BookDetailModalProps) {
  const [liked, setLiked] = useState(false);
  const [purchased, setPurchased] = useState(false);
  const navigate = useNavigate();
  const { books } = useBooks();
  const book = books.find(b => String(b.id) === String(bookId));
  const related = books.filter(b => b.genres.some(g => book?.genres?.includes(g)) && String(b.id) !== String(book?.id)).slice(0, 5);

  useEffect(() => {
    if (bookId) {
      document.body.style.overflow = 'hidden';
    } else {
      document.body.style.overflow = '';
    }
    return () => { document.body.style.overflow = ''; };
  }, [bookId]);

  const formatPrice = (p: number) => p.toLocaleString('vi-VN') + '₫';

  const handleBuy = () => {
    setPurchased(true);
    setTimeout(() => setPurchased(false), 2000);
  };

  const metaItems = book ? [
    { icon: FileText, label: 'Số trang', value: `${book.pages} trang` },
    { icon: Clock, label: 'Thời gian đọc', value: book.readTime },
    { icon: Calendar, label: 'Năm xuất bản', value: String(book.releaseYear) },
    { icon: Users, label: 'Đánh giá', value: `${book.ratingCount.toLocaleString()}` },
  ] : [];

  return (
    <AnimatePresence>
      {bookId && book && (
        <>
          {/* Backdrop */}
          <motion.div
            key="backdrop"
            initial={{ opacity: 0 }}
            animate={{ opacity: 1 }}
            exit={{ opacity: 0 }}
            transition={{ duration: 0.25 }}
            onClick={onClose}
            className="fixed inset-0 z-50 bg-black/60 backdrop-blur-sm"
          />

          {/* Modal */}
          <motion.div
            key="modal"
            initial={{ opacity: 0, y: '100%' }}
            animate={{ opacity: 1, y: 0 }}
            exit={{ opacity: 0, y: '100%' }}
            transition={{ type: 'spring', stiffness: 260, damping: 32 }}
            className="fixed bottom-0 left-0 right-0 z-50 max-h-[92vh] flex flex-col bg-[#F8F7F4] dark:bg-[#0D0C14] rounded-t-3xl overflow-hidden shadow-2xl md:top-12 md:bottom-12 md:left-1/2 md:-translate-x-1/2 md:w-full md:max-w-4xl md:rounded-3xl"
          >
            {/* Close drag handle */}
            <div className="flex justify-center pt-3 pb-1 md:hidden shrink-0">
              <div className="w-10 h-1 bg-gray-300 dark:bg-gray-700 rounded-full" />
            </div>

            {/* Close button (desktop) */}
            <button
              onClick={onClose}
              className="absolute top-4 right-4 z-10 w-9 h-9 rounded-2xl bg-gray-100 dark:bg-white/10 flex items-center justify-center text-gray-600 dark:text-gray-300 hover:bg-gray-200 dark:hover:bg-white/15 transition-colors"
            >
              <X className="w-4.5 h-4.5" />
            </button>

            {/* Scrollable content */}
            <div className="overflow-y-auto flex-1">
              {/* Hero section */}
              <div className="relative overflow-hidden">
                {/* Ambient background */}
                <div
                  className="absolute inset-0 opacity-30"
                  style={{ background: `radial-gradient(ellipse at 70% 50%, ${book.accentColor}50, transparent)` }}
                />
                <div className="absolute inset-0 bg-gradient-to-b from-transparent to-[#F8F7F4] dark:to-[#0D0C14]" />

                <div className="relative p-6 sm:p-8 grid sm:grid-cols-[auto_1fr] gap-6 items-start">
                  {/* Cover */}
                  <motion.div
                    initial={{ opacity: 0, scale: 0.9 }}
                    animate={{ opacity: 1, scale: 1 }}
                    transition={{ delay: 0.1 }}
                    className="relative w-36 sm:w-44 shrink-0 rounded-2xl overflow-hidden shadow-2xl self-start"
                    style={{ aspectRatio: '2/3' }}
                  >
                    <ImageWithFallback
                      src={book.cover}
                      alt={book.title}
                      className="w-full h-full object-cover"
                    />
                    <div className="absolute inset-0 bg-gradient-to-t from-black/20 to-transparent" />
                    {/* Glow */}
                    <div
                      className="absolute -inset-4 -z-10 opacity-40 blur-2xl rounded-full"
                      style={{ background: book.accentColor }}
                    />
                  </motion.div>

                  {/* Info */}
                  <motion.div
                    initial={{ opacity: 0, x: 20 }}
                    animate={{ opacity: 1, x: 0 }}
                    transition={{ delay: 0.15 }}
                    className="space-y-3 pt-1"
                  >
                    <div className="flex flex-wrap gap-2 items-center">
                      {book.genres.slice(0, 3).map((g, idx) => (
                        <span key={idx} className={`px-2.5 py-0.5 rounded-full text-xs font-semibold ${genreColors[g.toLowerCase()] || 'bg-gray-100 text-gray-700'}`}>
                          {g}
                        </span>
                      ))}
                      {book.badge && (
                        <span className={`px-2.5 py-0.5 rounded-full text-xs font-bold ${badgeColors[book.badge]}`}>
                          {book.badge}
                        </span>
                      )}
                    </div>

                    <h2
                      className="text-gray-900 dark:text-white leading-tight"
                      style={{ fontFamily: 'var(--font-serif)', fontSize: 'clamp(1.3rem, 3vw, 1.8rem)', fontWeight: 700 }}
                    >
                      {book.title}
                    </h2>
                    <p className="text-gray-500 dark:text-gray-400 text-sm">
                      bởi <span className="text-gray-700 dark:text-gray-300 font-medium">{book.author}</span>
                    </p>

                    {/* Rating */}
                    <div className="flex items-center gap-2">
                      <div className="flex items-center gap-0.5">
                        {[...Array(5)].map((_, i) => (
                          <Star key={i} className={`w-4 h-4 ${i < Math.floor(book.rating) ? 'fill-amber-400 text-amber-400' : 'text-gray-300 dark:text-gray-600'}`} />
                        ))}
                      </div>
                      <span className="text-gray-700 dark:text-gray-300 font-semibold text-sm">{book.rating}</span>
                      <span className="text-gray-400 text-xs">({book.ratingCount.toLocaleString()} đánh giá)</span>
                    </div>

                    {/* Meta grid */}
                    <div className="grid grid-cols-2 sm:grid-cols-4 gap-2 mt-2">
                      {metaItems.map(item => (
                        <div key={item.label} className="bg-white/60 dark:bg-white/5 rounded-xl px-3 py-2">
                          <p className="text-[10px] text-gray-400 mb-0.5">{item.label}</p>
                          <p className="text-xs font-semibold text-gray-800 dark:text-gray-200">{item.value}</p>
                        </div>
                      ))}
                    </div>
                  </motion.div>
                </div>
              </div>

              {/* Content below */}
              <div className="px-6 sm:px-8 pb-8 space-y-6">

                {/* Price + Actions */}
                <div className="flex flex-wrap items-center gap-4 py-4 border-y border-gray-200 dark:border-white/8">
                  <div className="flex items-baseline gap-2">
                    <span className="text-gray-900 dark:text-white font-black text-2xl">{formatPrice(book.price)}</span>
                    {book.originalPrice && (
                      <span className="text-gray-400 line-through">{formatPrice(book.originalPrice)}</span>
                    )}
                    {book.originalPrice && (
                      <span className="px-2 py-0.5 rounded-full bg-rose-100 text-rose-600 text-xs font-bold">
                        -{Math.round((1 - book.price / book.originalPrice) * 100)}%
                      </span>
                    )}
                  </div>
                  <div className="flex gap-2 ml-auto flex-wrap justify-end">
                    <motion.button
                      whileTap={{ scale: 0.95 }}
                      onClick={() => { onClose(); navigate(`/book/${book.id}`); }}
                      className="flex items-center gap-2 px-4 py-2.5 bg-gray-100 dark:bg-white/10 border border-gray-200 dark:border-white/10 text-gray-700 dark:text-gray-200 rounded-2xl text-sm font-semibold hover:bg-gray-200 dark:hover:bg-white/15 transition-all"
                    >
                      <ExternalLink className="w-4 h-4" />
                      Xem chi tiết
                    </motion.button>
                    <motion.button
                      whileTap={{ scale: 0.95 }}
                      onClick={() => setLiked(!liked)}
                      className={`w-10 h-10 rounded-2xl flex items-center justify-center border transition-all ${liked ? 'bg-rose-50 border-rose-200 text-rose-500 dark:bg-rose-500/15 dark:border-rose-500/30' : 'border-gray-200 dark:border-white/10 text-gray-500 dark:text-gray-400 hover:bg-gray-50 dark:hover:bg-white/5'}`}
                    >
                      <Heart className={`w-4.5 h-4.5 ${liked ? 'fill-rose-500' : ''}`} />
                    </motion.button>
                    <motion.button
                      whileTap={{ scale: 0.95 }}
                      className="w-10 h-10 rounded-2xl border border-gray-200 dark:border-white/10 flex items-center justify-center text-gray-500 dark:text-gray-400 hover:bg-gray-50 dark:hover:bg-white/5 transition-colors"
                    >
                      <Share2 className="w-4.5 h-4.5" />
                    </motion.button>
                    <motion.button
                      whileTap={{ scale: 0.96 }}
                      className="flex items-center gap-2 px-4 py-2.5 bg-white dark:bg-white/10 border border-gray-200 dark:border-white/10 text-gray-700 dark:text-gray-300 rounded-2xl text-sm font-semibold hover:bg-gray-50 dark:hover:bg-white/15 transition-all"
                    >
                      <Play className="w-4 h-4 fill-current" />
                      Đọc thử
                    </motion.button>
                    <motion.button
                      whileHover={{ scale: 1.03 }}
                      whileTap={{ scale: 0.96 }}
                      onClick={handleBuy}
                      className={`flex items-center gap-2 px-5 py-2.5 rounded-2xl text-sm font-bold shadow-lg transition-all ${
                        purchased
                          ? 'bg-emerald-500 text-white shadow-emerald-200 dark:shadow-emerald-900/30'
                          : 'bg-gradient-to-r from-indigo-600 to-violet-600 text-white shadow-indigo-200 dark:shadow-indigo-900/30 hover:from-indigo-500 hover:to-violet-500'
                      }`}
                    >
                      {purchased ? (
                        <>✓ Đã mua!</>
                      ) : (
                        <>
                          <ShoppingCart className="w-4 h-4" />
                          Mua ngay
                        </>
                      )}
                    </motion.button>
                  </div>
                </div>

                {/* Description */}
                <div>
                  <h3 className="text-gray-900 dark:text-white font-semibold mb-3 flex items-center gap-2">
                    <BookOpen className="w-4.5 h-4.5 text-indigo-500" />
                    Giới thiệu sách
                  </h3>
                  <p className="text-gray-600 dark:text-gray-300 leading-relaxed text-sm">
                    {book.longDescription}
                  </p>
                </div>

                {/* Reviews */}
                <div>
                  <div className="flex items-center justify-between mb-4">
                    <h3 className="text-gray-900 dark:text-white font-semibold flex items-center gap-2">
                      <Star className="w-4.5 h-4.5 text-amber-400 fill-amber-400" />
                      Đánh giá người đọc
                    </h3>
                    <a href="#" className="text-sm text-indigo-600 dark:text-indigo-400 hover:text-indigo-500 flex items-center gap-1">
                      Xem tất cả <ChevronRight className="w-3.5 h-3.5" />
                    </a>
                  </div>
                  <div className="space-y-3">
                    {reviews.map((r) => (
                      <motion.div
                        key={r.id}
                        initial={{ opacity: 0, y: 12 }}
                        animate={{ opacity: 1, y: 0 }}
                        transition={{ delay: r.id * 0.08 }}
                        className="bg-white dark:bg-white/5 rounded-2xl p-4"
                      >
                        <div className="flex items-start gap-3">
                          <div className="w-8 h-8 rounded-full bg-gradient-to-br from-indigo-400 to-violet-500 flex items-center justify-center text-white text-xs font-bold shrink-0">
                            {r.avatar}
                          </div>
                          <div className="flex-1 min-w-0">
                            <div className="flex items-center justify-between">
                              <span className="text-sm font-semibold text-gray-800 dark:text-gray-200">{r.user}</span>
                              <span className="text-xs text-gray-400">{r.date}</span>
                            </div>
                            <div className="flex items-center gap-0.5 mt-0.5">
                              {[...Array(r.rating)].map((_, i) => (
                                <Star key={i} className="w-3 h-3 fill-amber-400 text-amber-400" />
                              ))}
                            </div>
                            <p className="text-sm text-gray-600 dark:text-gray-400 mt-2 leading-relaxed">{r.text}</p>
                          </div>
                        </div>
                      </motion.div>
                    ))}
                  </div>
                </div>

                {/* Related books */}
                {related.length > 0 && (
                  <div>
                    <h3 className="text-gray-900 dark:text-white font-semibold mb-4">
                      Sách cùng thể loại
                    </h3>
                    <div className="flex gap-4 overflow-x-auto pb-2" style={{ scrollbarWidth: 'none' }}>
                      {related.map(rb => (
                        <BookCard key={rb.id} book={rb} onOpen={onOpenBook} size="sm" />
                      ))}
                    </div>
                  </div>
                )}
              </div>
            </div>
          </motion.div>
        </>
      )}
    </AnimatePresence>
  );
}