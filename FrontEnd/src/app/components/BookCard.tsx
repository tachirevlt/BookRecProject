import { useState } from 'react';
import { motion } from 'motion/react';
import { Star, ShoppingCart, Eye } from 'lucide-react';
import { Book, badgeColors, genreColors } from '../data/books';
import { ImageWithFallback } from './figma/ImageWithFallback';

interface BookCardProps {
  book: Book;
  onOpen: (book: Book) => void;
  size?: 'sm' | 'md' | 'lg';
}

export function BookCard({ book, onOpen, size = 'md' }: BookCardProps) {
  const [hovered, setHovered] = useState(false);
  const [added, setAdded] = useState(false);

  const formatPrice = (p: number) => p.toLocaleString('vi-VN') + '₫';

  const widths = { sm: 'w-32', md: 'w-40 sm:w-44', lg: 'w-52 sm:w-60' };

  const handleAdd = (e: React.MouseEvent) => {
    e.stopPropagation();
    setAdded(true);
    setTimeout(() => setAdded(false), 1500);
  };

  return (
    <motion.div
      className={`relative shrink-0 ${widths[size]} cursor-pointer group`}
      onHoverStart={() => setHovered(true)}
      onHoverEnd={() => setHovered(false)}
      whileHover={{ y: -6 }}
      transition={{ type: 'spring', stiffness: 400, damping: 28 }}
      onClick={() => onOpen(book)}
    >
      {/* Cover */}
      <div className="relative rounded-2xl overflow-hidden shadow-md group-hover:shadow-xl transition-shadow duration-300" style={{ aspectRatio: '2/3' }}>
        <ImageWithFallback
          src={book.cover}
          alt={book.title}
          className="w-full h-full object-cover transition-transform duration-500 group-hover:scale-110"
        />

        {/* Gradient overlay */}
        <div className="absolute inset-0 bg-gradient-to-t from-black/70 via-transparent to-transparent opacity-0 group-hover:opacity-100 transition-opacity duration-300" />

        {/* Glow on hover */}
        <motion.div
          className="absolute inset-0 rounded-2xl pointer-events-none"
          animate={hovered ? { boxShadow: `0 0 24px 2px ${book.accentColor}60` } : { boxShadow: 'none' }}
          transition={{ duration: 0.3 }}
        />

        {/* Badge */}
        {book.badge && (
          <span className={`absolute top-2.5 left-2.5 px-2 py-0.5 rounded-full text-[10px] font-bold ${badgeColors[book.badge]}`}>
            {book.badge}
          </span>
        )}

        {/* Hover actions */}
        <motion.div
          initial={{ opacity: 0, y: 12 }}
          animate={hovered ? { opacity: 1, y: 0 } : { opacity: 0, y: 12 }}
          transition={{ duration: 0.2 }}
          className="absolute bottom-3 left-3 right-3 flex gap-2"
          onClick={e => e.stopPropagation()}
        >
          <button
            onClick={() => onOpen(book)}
            className="flex-1 flex items-center justify-center gap-1.5 py-2 bg-white/90 dark:bg-white/15 backdrop-blur-sm text-gray-800 dark:text-white rounded-xl text-xs font-semibold hover:bg-white dark:hover:bg-white/25 transition-colors"
          >
            <Eye className="w-3.5 h-3.5" />
            Chi tiết
          </button>
          <motion.button
            whileTap={{ scale: 0.92 }}
            onClick={handleAdd}
            className={`flex-1 flex items-center justify-center gap-1.5 py-2 rounded-xl text-xs font-semibold transition-colors ${
              added
                ? 'bg-emerald-500 text-white'
                : 'bg-indigo-600 text-white hover:bg-indigo-500'
            }`}
          >
            <ShoppingCart className="w-3.5 h-3.5" />
            {added ? '✓ Đã thêm' : 'Mua'}
          </motion.button>
        </motion.div>
      </div>

      {/* Info */}
      <div className="mt-3 px-0.5">
        <div className="flex flex-wrap gap-1 mb-1.5">
          {book.genres?.slice(0, 3).map((g, idx) => (
            <span key={idx} className={`inline-block px-2 py-0.5 rounded-full text-[10px] font-semibold ${genreColors[g.toLowerCase()] || 'bg-gray-100 dark:bg-gray-800 text-gray-600 dark:text-gray-300'}`}>
              {g}
            </span>
          ))}
        </div>
        <h3 className="text-gray-900 dark:text-white font-semibold text-sm leading-snug line-clamp-2 group-hover:text-indigo-600 dark:group-hover:text-indigo-400 transition-colors">
          {book.title}
        </h3>
        <p className="text-gray-500 dark:text-gray-400 text-xs mt-0.5 truncate">{book.author}</p>

        <div className="flex items-center justify-between mt-2">
          <div className="flex items-baseline gap-1.5">
            <span className="text-gray-900 dark:text-white font-bold text-sm">{formatPrice(book.price)}</span>
            {book.originalPrice && (
              <span className="text-gray-400 text-xs line-through">{formatPrice(book.originalPrice)}</span>
            )}
          </div>
          <div className="flex items-center gap-0.5">
            <Star className="w-3 h-3 fill-amber-400 text-amber-400" />
            <span className="text-xs text-gray-600 dark:text-gray-400 font-medium">{book.rating}</span>
          </div>
        </div>
      </div>
    </motion.div>
  );
}
