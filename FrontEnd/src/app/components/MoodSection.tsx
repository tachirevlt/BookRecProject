import { useState } from 'react';
import { motion, AnimatePresence } from 'motion/react';
import { Sparkles } from 'lucide-react';
import { MOODS } from '../data/books';
import type { Book } from '../data/books';
import { BookCard } from './BookCard';
import { useBooks } from '../hooks/useBooks';


interface MoodSectionProps {
  onOpenBook: (book: Book) => void;
}

export function MoodSection({ onOpenBook }: MoodSectionProps) {
  const { books } = useBooks();
  const [selectedMood, setSelectedMood] = useState(MOODS[0].id);
  // Lọc sách theo mood được chọn, tối đa 5 cuốn để hiển thị
  const filteredBooks = books.filter(b => b.mood?.some(m => m.toLowerCase() === selectedMood.toLowerCase())).slice(0, 5);

  return (
    <motion.section
      initial={{ opacity: 0, y: 32 }}
      whileInView={{ opacity: 1, y: 0 }}
      viewport={{ once: true, margin: '-80px' }}
      transition={{ duration: 0.6, ease: [0.4, 0, 0.2, 1] }}
      className="bg-gradient-to-br from-indigo-50 via-violet-50/50 to-purple-50 dark:from-indigo-950/30 dark:via-violet-950/20 dark:to-purple-950/30 rounded-3xl overflow-hidden mx-4 sm:mx-6 lg:mx-8 max-w-[calc(1400px-4rem)] xl:mx-auto"
    >
      <div className="px-6 sm:px-8 py-8">
        {/* Header */}
        <div className="flex items-center gap-3 mb-2">
          <div className="w-9 h-9 rounded-2xl bg-gradient-to-br from-violet-500 to-indigo-600 flex items-center justify-center shadow-md shadow-violet-200 dark:shadow-violet-900/30">
            <Sparkles className="w-4.5 h-4.5 text-white" />
          </div>
          <div>
            <h2
              className="text-gray-900 dark:text-white"
              style={{ fontFamily: 'var(--font-serif)', fontSize: 'clamp(1.1rem, 2vw, 1.4rem)', fontWeight: 700 }}
            >
              Hôm nay bạn muốn đọc gì?
            </h2>
            <p className="text-sm text-gray-500 dark:text-gray-400">Chọn trạng thái — chúng tôi sẽ gợi ý cuốn sách phù hợp</p>
          </div>
        </div>

        {/* Mood selector */}
        <div className="flex flex-wrap gap-2.5 mt-5 mb-6">
          {MOODS.map((mood) => (
            <motion.button
              key={mood.id}
              whileHover={{ scale: 1.05, y: -2 }}
              whileTap={{ scale: 0.96 }}
              onClick={() => setSelectedMood(mood.id)}
              className={`flex items-center gap-2 px-4 py-2.5 rounded-2xl text-sm font-semibold transition-all duration-200 ${
                selectedMood === mood.id
                  ? 'bg-indigo-600 text-white shadow-lg shadow-indigo-200 dark:shadow-indigo-900/30'
                  : 'bg-white dark:bg-white/10 text-gray-700 dark:text-gray-300 border border-gray-200 dark:border-white/10 hover:border-indigo-300 dark:hover:border-indigo-500/50'
              }`}
            >
              <span className="text-base leading-none">{mood.emoji}</span>
              <span>{mood.label}</span>
              {selectedMood === mood.id && (
                <motion.span
                  layoutId="mood-active"
                  className="w-1.5 h-1.5 rounded-full bg-white/70"
                />
              )}
            </motion.button>
          ))}
        </div>

        {/* Books */}
        <AnimatePresence mode="wait">
          <motion.div
            key={selectedMood}
            initial={{ opacity: 0, y: 16 }}
            animate={{ opacity: 1, y: 0 }}
            exit={{ opacity: 0, y: -16 }}
            transition={{ duration: 0.3, ease: [0.4, 0, 0.2, 1] }}
            className="flex gap-5 overflow-x-auto pb-2"
            style={{ scrollbarWidth: 'none' }}
          >
            {filteredBooks.length > 0 ? (
              filteredBooks.map((book, idx) => (
                <motion.div
                  key={book.id}
                  initial={{ opacity: 0, y: 16 }}
                  animate={{ opacity: 1, y: 0 }}
                  transition={{ delay: idx * 0.07, duration: 0.35 }}
                >
                  <BookCard book={book} onOpen={onOpenBook} />
                </motion.div>
              ))
            ) : (
              <p className="text-gray-500 dark:text-gray-400 text-sm py-8">
                Chưa có sách cho tâm trạng này. Hãy thử tâm trạng khác!
              </p>
            )}
          </motion.div>
        </AnimatePresence>
      </div>
    </motion.section>
  );
}
