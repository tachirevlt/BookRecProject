import { useEffect, useState, useMemo } from 'react';
import { motion, AnimatePresence } from 'motion/react';
import { useNavigate } from 'react-router';
import {
  X, Star, BookOpen, ShoppingCart, Play, Heart,
  Share2, ChevronRight, ChevronLeft, Clock, FileText, Calendar, Users, ExternalLink, MessageCircle
} from 'lucide-react';
import { badgeColors, genreColors } from '../data/books';
import type { Book } from '../data/books';
import { useBooks } from '../hooks/useBooks';
import { ImageWithFallback } from './figma/ImageWithFallback';
import { BookCard } from './BookCard';
import { userService } from '../../services/userService';
import { reviewService, ReviewData } from '../../services/reviewService';
import { trackingService } from '../../services/trackingService';
import { bookService } from '../../services/bookService'; 

interface BookDetailModalProps {
  bookId: string | number | null;
  bookData?: any;
  onClose: () => void;
  onOpenBook: (book: Book) => void;
}

export function BookDetailModal({ bookId, bookData, onClose, onOpenBook }: BookDetailModalProps) {
  const navigate = useNavigate();
  
  // STATES
  const [liked, setLiked] = useState(false);
  const [purchased, setPurchased] = useState(false);
  const [isAlreadyPurchased, setIsAlreadyPurchased] = useState(false);
  
  const [isBuying, setIsBuying] = useState(false);
  const [isLiking, setIsLiking] = useState(false);
  
  const [bookReviews, setBookReviews] = useState<ReviewData[]>([]);
  const [loadingReviews, setLoadingReviews] = useState(false);

  // STATE ĐỀ XUẤT SÁCH & ĐIỀU HƯỚNG VÒNG LẶP
  const [recommendedBooksList, setRecommendedBooksList] = useState<Book[]>([]);
  const [loadingRecommendations, setLoadingRecommendations] = useState(false);
  const [direction, setDirection] = useState(1); // 1: Trượt tới, -1: Trượt lùi

  // LẤY DỮ LIỆU SÁCH
  const { books, recommendedBooks } = useBooks();

  const book = useMemo(() => {
    if (bookData) return bookData;
    if (!bookId) return null;
    const targetId = String(bookId);
    return books.find(b => String(b.id) === targetId) || 
           recommendedBooks.find(b => String(b.id) === targetId);
  }, [books, recommendedBooks, bookId, bookData]);
  
  // ─── EFFECTS ────────────────────────────────────────────────────────────────
  useEffect(() => {
    if (bookId) {
      document.body.style.overflow = 'hidden';
    } else {
      document.body.style.overflow = '';
      setPurchased(false);
      setIsAlreadyPurchased(false);
    }
    return () => {
      document.body.style.overflow = '';
    };
  }, [bookId]);

  useEffect(() => {
    const checkUserStatus = async () => {
      if (!book?.id) return;
      
      setIsAlreadyPurchased(false);
      setLiked(false);

      const userId = userService.getCurrentUserId();
      if (!userId) return; 

      try {
        const [purchasedList, wishlist] = await Promise.all([
          userService.getPurchasedBooks(userId),
          userService.getWishlist()
        ]);

        const hasPurchased = purchasedList?.some(b => String(b.book_id) === String(book.id));
        if (hasPurchased) setIsAlreadyPurchased(true);

        const hasLiked = wishlist?.some(b => String(b.book_id) === String(book.id));
        if (hasLiked) setLiked(true);

      } catch (error) {
        console.error("Lỗi kiểm tra trạng thái sách của User:", error);
      }
    };

    checkUserStatus();
  }, [book?.id, bookId]);

  useEffect(() => {
    const fetchAdditionalData = async () => {
      if (!book?.id) return;
      
      setLoadingReviews(true);
      setLoadingRecommendations(true);
      
      try {
        const [reviewsData, recommendationsResponse] = await Promise.all([
          reviewService.getReviewsByBook(String(book.id), 1, 3).catch(() => []),
          bookService.getRecommendationsByBookId(String(book.id), 10).catch(() => []) // Lấy 10 cuốn
        ]);

        const sortedReviews = (reviewsData || []).sort((a, b) => new Date(b.time).getTime() - new Date(a.time).getTime());
        setBookReviews(sortedReviews.slice(0, 3));

        const rawRecList = Array.isArray(recommendationsResponse) 
          ? recommendationsResponse 
          : (recommendationsResponse as any)?.items || [];

        const normalizedRecs = rawRecList.map((rb: any) => ({
          ...rb,
          id: rb.id || rb.book_id,
          title: rb.title || rb.original_title,
          author: rb.author || rb.authors,
          cover: rb.cover || rb.image_url,
          rating: rb.rating || rb.average_rating || 0,
        }));

        const filteredRecommendations = normalizedRecs.filter((b: any) => String(b.id) !== String(book.id));
        
        setRecommendedBooksList(filteredRecommendations as Book[]);
        
      } catch (error) {
        console.error("Lỗi lấy dữ liệu phụ:", error);
      } finally {
        setLoadingReviews(false);
        setLoadingRecommendations(false);
      }
    };

    fetchAdditionalData();
  }, [book?.id]);

  // ─── HANDLERS MUA HÀNG ──────────────────────────────────────────────────────
  const handleSafeClose = () => {
    document.body.style.overflow = '';
    onClose();
  };

  const formatPrice = (p: number) => p.toLocaleString('vi-VN') + '₫';

  const handleBuy = async () => {
    if (isBuying || purchased || isAlreadyPurchased || !book) return;
    
    if (!userService.isLoggedIn()) {
      handleSafeClose();
      navigate('/login');
      return;
    }

    setIsBuying(true);
    try {
      await userService.purchaseBook(String(book.id));
      setPurchased(true);
      setIsAlreadyPurchased(true); 
    } catch (error: any) {
      alert(error.response?.data?.message || "Mua sách thất bại. Vui lòng thử lại.");
    } finally {
      setIsBuying(false);
    }
  };

  const handleToggleWishlist = async () => {
    if (isLiking || !book) return;

    if (!userService.isLoggedIn()) {
      handleSafeClose();
      navigate('/login');
      return;
    }

    setIsLiking(true);
    try {
      if (liked) {
        await userService.removeFavorite(String(book.id)); 
        setLiked(false);
      } else {
        await userService.addFavorite(String(book.id)); 
        setLiked(true);
      }
    } catch (error: any) {
      alert("Không thể cập nhật danh sách yêu thích.");
    } finally {
      setIsLiking(false);
    }
  };

  // ─── LOGIC CAROUSEL VÒNG LẶP VÔ TẬN ──────────────────────────────────────────
  
  // Chỉ lấy đúng 5 cuốn sách đầu tiên của mảng để hiển thị
  const visibleBooks = recommendedBooksList.slice(0, 5);
  // Hiển thị nút bấm khi có nhiều hơn 1 cuốn sách
  const showArrows = recommendedBooksList.length > 1;

  const handleNext = () => {
    setDirection(1); // Cờ báo hiệu đang trượt tới để set hiệu ứng
    setRecommendedBooksList((prev) => {
      if (prev.length <= 1) return prev;
      // Bốc phần tử đầu tiên, nối vào cuối mảng
      const [first, ...rest] = prev;
      return [...rest, first];
    });
  };

  const handlePrev = () => {
    setDirection(-1); // Cờ báo hiệu đang trượt lùi
    setRecommendedBooksList((prev) => {
      if (prev.length <= 1) return prev;
      // Bốc phần tử cuối cùng, nhét lên đầu mảng
      const last = prev[prev.length - 1];
      const rest = prev.slice(0, -1);
      return [last, ...rest];
    });
  };

  // Animation cho việc thoát/nhập của thẻ sách
  const cardVariants = {
    initial: (dir: number) => ({
      opacity: 0,
      x: dir > 0 ? 50 : -50, // Nếu tới thì đi vào từ bên phải, lùi thì từ bên trái
      scale: 0.9,
    }),
    animate: {
      opacity: 1,
      x: 0,
      scale: 1,
      transition: { type: "spring" as const, stiffness: 300, damping: 30 }
    },
    exit: (dir: number) => ({
      opacity: 0,
      x: dir > 0 ? -50 : 50,
      scale: 0.9,
      transition: { duration: 0.2 }
    })
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
        <div className="fixed inset-0 z-50 flex items-end md:items-center md:justify-center">
          <motion.div
            initial={{ opacity: 0 }}
            animate={{ opacity: 1 }}
            exit={{ opacity: 0 }}
            transition={{ duration: 0.2 }}
            onClick={handleSafeClose}
            className="absolute inset-0 bg-black/60 backdrop-blur-sm"
          />

          <motion.div
            initial={{ opacity: 0, y: '100%' }}
            animate={{ opacity: 1, y: 0 }}
            exit={{ opacity: 0, y: '100%' }}
            transition={{ type: 'spring', stiffness: 300, damping: 30 }}
            className="relative z-10 w-full max-h-[92vh] flex flex-col bg-[#F8F7F4] dark:bg-[#0D0C14] rounded-t-3xl overflow-hidden shadow-2xl md:max-w-4xl md:rounded-3xl"
          >
            <div className="flex justify-center pt-3 pb-1 md:hidden shrink-0">
              <div className="w-10 h-1 bg-gray-300 dark:bg-gray-700 rounded-full" />
            </div>

            <button
              onClick={handleSafeClose}
              className="absolute top-4 right-4 z-20 w-9 h-9 rounded-2xl bg-gray-100 dark:bg-white/10 flex items-center justify-center text-gray-600 dark:text-gray-300 hover:bg-gray-200 dark:hover:bg-white/15 transition-colors"
            >
              <X className="w-4.5 h-4.5" />
            </button>

            <div className="overflow-y-auto flex-1">
              <div className="relative overflow-hidden">
                <div
                  className="absolute inset-0 opacity-30 pointer-events-none"
                  style={{ background: `radial-gradient(ellipse at 70% 50%, ${book.accentColor}50, transparent)` }}
                />
                <div className="absolute inset-0 bg-gradient-to-b from-transparent to-[#F8F7F4] dark:to-[#0D0C14] pointer-events-none" />

                <div className="relative p-6 sm:p-8 grid sm:grid-cols-[auto_1fr] gap-6 items-start">
                  <div className="relative w-36 sm:w-44 shrink-0 rounded-2xl overflow-hidden shadow-2xl self-start" style={{ aspectRatio: '2/3' }}>
                    <ImageWithFallback src={book.cover} alt={book.title} className="w-full h-full object-cover" />
                    <div className="absolute inset-0 bg-gradient-to-t from-black/20 to-transparent pointer-events-none" />
                  </div>

                  <div className="space-y-3 pt-1">
                    <div className="flex flex-wrap gap-2 items-center">
                      {book.badges && book.badges.length > 0 && book.badges.map((badgeName: string, idx: number) => (
                        <span 
                          key={idx} 
                          className={`px-2.5 py-0.5 rounded-full text-[10px] sm:text-xs font-bold uppercase tracking-wide shadow-sm ${
                            badgeColors[badgeName] || 'bg-gray-200 text-gray-800 dark:bg-gray-700 dark:text-gray-200' 
                          }`}
                        >
                          {badgeName}
                        </span>
                      ))}
                    </div>

                    <h2 className="text-gray-900 dark:text-white leading-tight font-bold font-serif text-2xl sm:text-3xl">
                      {book.title}
                    </h2>
                    <p className="text-gray-500 dark:text-gray-400 text-sm">
                      bởi <span className="text-gray-700 dark:text-gray-300 font-medium">{book.author}</span>
                    </p>

                    <div className="flex items-center gap-2">
                      <div className="flex items-center gap-0.5">
                        {[...Array(5)].map((_, i) => (
                          <Star key={i} className={`w-4 h-4 ${i < Math.floor(book.rating) ? 'fill-amber-400 text-amber-400' : 'text-gray-300 dark:text-gray-600'}`} />
                        ))}
                      </div>
                      <span className="text-gray-700 dark:text-gray-300 font-semibold text-sm">{book.rating}</span>
                    </div>

                    <div className="grid grid-cols-2 sm:grid-cols-4 gap-2 mt-2">
                      {metaItems.map(item => (
                        <div key={item.label} className="bg-white/60 dark:bg-white/5 rounded-xl px-3 py-2">
                          <p className="text-[10px] text-gray-400 mb-0.5">{item.label}</p>
                          <p className="text-xs font-semibold text-gray-800 dark:text-gray-200">{item.value}</p>
                        </div>
                      ))}
                    </div>
                  </div>
                </div>
              </div>

              <div className="px-6 sm:px-8 pb-8 space-y-8">
                
                {/* ── BẢNG ĐIỀU KHIỂN & CÁC NÚT BẤM ── */}
                <div className="flex flex-wrap items-center gap-4 py-4 border-y border-gray-200 dark:border-white/8">
                  <div className="flex items-baseline gap-2">
                    {isAlreadyPurchased ? (
                      <span className="text-emerald-600 dark:text-emerald-400 font-black text-2xl">Sách của bạn</span>
                    ) : (
                      <span className="text-gray-900 dark:text-white font-black text-2xl">{formatPrice(book.price)}</span>
                    )}
                  </div>
                  
                  <div className="flex gap-2 ml-auto flex-wrap justify-end">
                    <button
                      onClick={() => { 
                        if (book?.id) trackingService.logEvent(String(book.id), 'detail_view');
                        handleSafeClose(); 
                        navigate(`/book/${book.id}`); 
                      }}
                      className={`flex items-center gap-2 px-4 py-2.5 rounded-2xl text-sm font-semibold transition-all ${
                        isAlreadyPurchased 
                          ? 'bg-indigo-100 text-indigo-700 hover:bg-indigo-200 dark:bg-indigo-500/20 dark:text-indigo-300 dark:hover:bg-indigo-500/30' 
                          : 'bg-gray-100 dark:bg-white/10 border border-gray-200 dark:border-white/10 text-gray-700 dark:text-gray-200 hover:bg-gray-200'
                      }`}
                    >
                      {isAlreadyPurchased ? <><Play className="w-4 h-4 fill-current" /> Đọc ngay</> : <><ExternalLink className="w-4 h-4" /> Xem chi tiết</>}
                    </button>

                    {!isAlreadyPurchased && (
                      <button
                        onClick={handleToggleWishlist}
                        disabled={isLiking}
                        className={`flex items-center gap-2 px-4 py-2.5 rounded-2xl text-sm font-semibold border transition-all ${
                          liked 
                            ? 'bg-rose-50 border-rose-200 text-rose-600 dark:bg-rose-500/10 dark:border-rose-500/20 dark:text-rose-400' 
                            : 'bg-white border-gray-200 text-gray-700 hover:bg-gray-50 dark:bg-transparent dark:border-white/10 dark:text-gray-300 dark:hover:bg-white/5'
                        }`}
                      >
                        {isLiking ? <span className="w-4 h-4 border-2 border-current border-t-transparent rounded-full animate-spin" /> : <Heart className={`w-4 h-4 ${liked ? 'fill-current' : ''}`} />}
                        {liked ? 'Đã thích' : 'Yêu thích'}
                      </button>
                    )}

                    <button
                      onClick={handleBuy}
                      disabled={isBuying || isAlreadyPurchased}
                      className={`flex items-center gap-2 px-5 py-2.5 rounded-2xl text-sm font-bold shadow-lg transition-all ${
                        isAlreadyPurchased 
                          ? 'bg-emerald-500 text-white cursor-default shadow-none' 
                          : isBuying 
                            ? 'bg-indigo-400 text-white cursor-wait' 
                            : 'bg-gradient-to-r from-indigo-600 to-violet-600 text-white hover:opacity-90'
                      }`}
                    >
                      {isBuying ? <span className="w-4 h-4 border-2 border-white/30 border-t-white rounded-full animate-spin" /> : isAlreadyPurchased ? <>✓ Đã mua</> : <><ShoppingCart className="w-4 h-4" /> Mua ngay</>}
                    </button>
                  </div>
                </div>

                {/* ── GIỚI THIỆU & REVIEWS ── */}
                <div className="space-y-8">
                  <div>
                    <h3 className="text-gray-900 dark:text-white font-semibold mb-3 flex items-center gap-2">
                      <BookOpen className="w-4.5 h-4.5 text-indigo-500" /> Giới thiệu sách
                    </h3>
                    <p className="text-gray-600 dark:text-gray-300 leading-relaxed text-sm whitespace-pre-line">
                      {book.longDescription}
                    </p>
                  </div>

                  <div>
                    <h3 className="text-gray-900 dark:text-white font-semibold mb-4 flex items-center gap-2">
                      <MessageCircle className="w-4.5 h-4.5 text-indigo-500" /> Nhận xét từ độc giả
                    </h3>
                    {loadingReviews ? (
                      <div className="flex justify-center py-6">
                        <span className="w-6 h-6 border-2 border-indigo-500 border-t-transparent rounded-full animate-spin" />
                      </div>
                    ) : bookReviews.length > 0 ? (
                      <div className="space-y-4">
                        {bookReviews.map((review) => (
                          <div key={review.id} className="bg-white/60 dark:bg-white/5 p-4 rounded-2xl border border-gray-100 dark:border-white/5">
                            <div className="flex justify-between items-start mb-2">
                              <span className="text-xs font-semibold text-gray-500 dark:text-gray-400">
                                {(review as any).full_name || (review as any).fullName || 'Độc giả'}
                              </span>
                              <span className="text-[10px] text-gray-400">
                                {new Date(review.time).toLocaleDateString('vi-VN')}
                              </span>
                            </div>
                            <p className="text-sm text-gray-800 dark:text-gray-200">"{review.review}"</p>
                          </div>
                        ))}
                      </div>
                    ) : (
                      <div className="text-center py-6 bg-white/40 dark:bg-white/5 rounded-2xl border border-dashed border-gray-200 dark:border-white/10 text-gray-500 text-sm">
                        Chưa có đánh giá nào. Hãy là người đầu tiên nhận xét cuốn sách này!
                      </div>
                    )}
                  </div>
                </div>

                {/* ── ĐỀ XUẤT TỪ HỆ THỐNG (VÒNG LẶP VÔ TẬN) ── */}
                <div>
                  <div className="flex items-center justify-between mb-4">
                    <h3 className="text-gray-900 dark:text-white font-semibold">Bạn cũng có thể thích</h3>
                    
                    {/* BỘ NÚT ĐIỀU HƯỚNG */}
                    {!loadingRecommendations && showArrows && (
                      <div className="flex items-center gap-2">
                        <button 
                          onClick={handlePrev}
                          className="w-8 h-8 flex items-center justify-center rounded-full bg-white dark:bg-white/10 border border-gray-200 dark:border-white/10 text-gray-600 dark:text-gray-300 hover:bg-gray-100 dark:hover:bg-white/20 transition-all shadow-sm"
                        >
                          <ChevronLeft className="w-4 h-4" />
                        </button>
                        <button 
                          onClick={handleNext}
                          className="w-8 h-8 flex items-center justify-center rounded-full bg-white dark:bg-white/10 border border-gray-200 dark:border-white/10 text-gray-600 dark:text-gray-300 hover:bg-gray-100 dark:hover:bg-white/20 transition-all shadow-sm"
                        >
                          <ChevronRight className="w-4 h-4" />
                        </button>
                      </div>
                    )}
                  </div>

                  {loadingRecommendations ? (
                     <div className="flex justify-center py-6">
                       <span className="w-6 h-6 border-2 border-indigo-500 border-t-transparent rounded-full animate-spin" />
                     </div>
                  ) : recommendedBooksList.length > 0 ? (
                    // Vùng chứa overflow-hidden để cắt gọn mép trượt
                    <div className="relative overflow-hidden -mx-2 px-2 py-4">
                      {/* ĐÃ SỬA: Chuyển sang thẻ Flex, bỏ tính năng scroll Native */}
                      <div className="flex gap-4 sm:gap-5 relative">
                        <AnimatePresence mode="popLayout" custom={direction}>
                          {visibleBooks.map((rb) => (
                            <motion.div
                              key={rb.id}
                              custom={direction}
                              layout // Phép màu làm thẻ tự động trượt mượt mà
                              variants={cardVariants}
                              initial="initial"
                              animate="animate"
                              exit="exit"
                              // ĐÃ SỬA: Dùng Tailwind toán học để thẻ có kích thước hoàn hảo (Responsive: 3 thẻ mobile, 4 thẻ tablet, 5 thẻ Desktop)
                              // Ép chiều rộng con bên trong (!w-full) để đè lên kích thước gốc cố định của BookCard
                              className="w-[calc((100%-32px)/3)] sm:w-[calc((100%-60px)/4)] md:w-[calc((100%-80px)/5)] shrink-0 [&>div]:!w-full [&>div]:!max-w-none"
                            >
                              <BookCard book={rb} onOpen={onOpenBook} size="sm" />
                            </motion.div>
                          ))}
                        </AnimatePresence>
                      </div>
                    </div>
                  ) : (
                    <div className="text-center py-6 bg-white/40 dark:bg-white/5 rounded-2xl border border-dashed border-gray-200 dark:border-white/10 text-gray-500 text-sm">
                      Chưa có sách đề xuất phù hợp.
                    </div>
                  )}
                </div>

              </div>
            </div>
          </motion.div>
        </div>
      )}
    </AnimatePresence>
  );
}