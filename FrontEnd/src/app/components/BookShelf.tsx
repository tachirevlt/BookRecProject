import { useRef, useState, useEffect } from 'react';
import { motion, AnimatePresence } from 'motion/react';
import { ChevronLeft, ChevronRight, ArrowRight } from 'lucide-react';
import { Book } from '../data/books';
import { BookCard } from './BookCard';
import { Skeleton } from './ui/skeleton';

interface BookShelfProps {
  title: string;
  subtitle?: string;
  emoji?: string;
  books: Book[];
  onOpenBook: (book: Book) => void;
  accentColor?: string;
}

export function BookShelf({ title, subtitle, emoji, books, onOpenBook, accentColor = '#4F46E5' }: BookShelfProps) {
  const scrollRef = useRef<HTMLDivElement>(null);
  const [canScrollLeft, setCanScrollLeft] = useState(false);
  const [canScrollRight, setCanScrollRight] = useState(true);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const timer = setTimeout(() => setLoading(false), 600);
    return () => clearTimeout(timer);
  }, []);

  const updateScrollButtons = () => {
    const el = scrollRef.current;
    if (!el) return;
    setCanScrollLeft(el.scrollLeft > 8);
    setCanScrollRight(el.scrollLeft < el.scrollWidth - el.clientWidth - 8);
  };

  const scroll = (dir: 'left' | 'right') => {
    const el = scrollRef.current;
    if (!el) return;
    el.scrollBy({ left: dir === 'left' ? -360 : 360, behavior: 'smooth' });
  };

  return (
    <motion.section
      initial={{ opacity: 0, y: 32 }}
      whileInView={{ opacity: 1, y: 0 }}
      viewport={{ once: true, margin: '-60px' }}
      transition={{ duration: 0.55, ease: [0.4, 0, 0.2, 1] }}
      className="relative"
    >
      {/* Shelf header */}
      <div className="flex items-center justify-between mb-5 px-4 sm:px-6 lg:px-8 max-w-[1400px] mx-auto">
        <div className="flex items-center gap-3">
          {emoji && (
            <span className="text-2xl">{emoji}</span>
          )}
          <div>
            <h2
              className="text-gray-900 dark:text-white"
              style={{ fontFamily: 'var(--font-serif)', fontSize: 'clamp(1.1rem, 2vw, 1.4rem)', fontWeight: 700 }}
            >
              {title}
            </h2>
            {subtitle && <p className="text-sm text-gray-500 dark:text-gray-400 mt-0.5">{subtitle}</p>}
          </div>
        </div>

        <div className="flex items-center gap-2">
          {/* Scroll arrows */}
          <AnimatePresence>
            {canScrollLeft && (
              <motion.button
                initial={{ opacity: 0, scale: 0.8 }}
                animate={{ opacity: 1, scale: 1 }}
                exit={{ opacity: 0, scale: 0.8 }}
                onClick={() => scroll('left')}
                className="w-8 h-8 rounded-full bg-white dark:bg-white/10 border border-gray-200 dark:border-white/10 flex items-center justify-center text-gray-600 dark:text-gray-300 hover:bg-gray-50 dark:hover:bg-white/15 transition-colors shadow-sm"
              >
                <ChevronLeft className="w-4 h-4" />
              </motion.button>
            )}
          </AnimatePresence>
          <button
            onClick={() => scroll('right')}
            disabled={!canScrollRight}
            className="w-8 h-8 rounded-full bg-white dark:bg-white/10 border border-gray-200 dark:border-white/10 flex items-center justify-center text-gray-600 dark:text-gray-300 hover:bg-gray-50 dark:hover:bg-white/15 transition-colors shadow-sm disabled:opacity-40"
          >
            <ChevronRight className="w-4 h-4" />
          </button>

          <a href="#" className="flex items-center gap-1 text-sm font-medium text-indigo-600 dark:text-indigo-400 hover:text-indigo-500 transition-colors ml-1">
            Tất cả
            <ArrowRight className="w-3.5 h-3.5" />
          </a>
        </div>
      </div>

      {/* Shelf strip */}
      <div
        ref={scrollRef}
        onScroll={updateScrollButtons}
        className="overflow-x-auto scrollbar-none flex gap-5 px-4 sm:px-6 lg:px-8 pb-4"
        style={{ scrollbarWidth: 'none', msOverflowStyle: 'none', maxWidth: '100vw' }}
      >
        <div className="flex gap-5">
          {loading
            ? [...Array(6)].map((_, i) => (
                <div key={i} className="shrink-0 w-40 sm:w-44">
                  <Skeleton className="w-full rounded-2xl" style={{ aspectRatio: '2/3' }} />
                  <Skeleton className="h-3 mt-3 rounded w-3/4" />
                  <Skeleton className="h-3 mt-1.5 rounded w-1/2" />
                </div>
              ))
            : books.map((book, idx) => (
                <motion.div
                  key={book.id}
                  initial={{ opacity: 0, y: 20 }}
                  animate={{ opacity: 1, y: 0 }}
                  transition={{ delay: idx * 0.07, duration: 0.4 }}
                >
                  <BookCard book={book} onOpen={onOpenBook} />
                </motion.div>
              ))
          }
        </div>
      </div>

      {/* Shelf bottom line decoration */}
      <div className="h-px max-w-[1400px] mx-auto mt-2 px-4 sm:px-6 lg:px-8">
        <div className="h-px bg-gradient-to-r from-transparent via-gray-200 dark:via-white/10 to-transparent" />
      </div>
    </motion.section>
  );
}
