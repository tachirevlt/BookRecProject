import { useState, useEffect } from 'react';
import { motion } from 'motion/react';
import { useNavigate, useOutletContext } from 'react-router';
import {
  ArrowLeft, Calendar, Filter, Download, Check, BookOpen, TrendingUp, DollarSign, Package
} from 'lucide-react';
import { useBooks } from '../hooks/useBooks';
import { ImageWithFallback } from '../components/figma/ImageWithFallback';
import type { Book } from '../data/books';
import type { OutletContextType } from '../components/RootLayout';
import { userService } from '../../services/userService';
import { mapToBook } from '../../services/bookService';

type PurchaseStatus = 'completed' | 'pending' | 'failed';
type FilterType = 'all' | 'completed' | 'pending';

interface Purchase {
  id: number;
  bookId: string | number;
  book?: Book;
  date: string;
  amount: number;
  status: PurchaseStatus;
  paymentMethod: string;
  transactionId: string;
}

export function PurchaseHistory() {
  const navigate = useNavigate();
  const { onOpenBook } = useOutletContext<OutletContextType>();
  const { books } = useBooks();
  const [filter, setFilter] = useState<FilterType>('all');
  const [purchases, setPurchases] = useState<Purchase[]>([]);

  useEffect(() => {
    const fetchPurchases = async () => {
      try {
        const userId = userService.getCurrentUserId();
        if (!userId) {
          navigate('/login');
          return;
        }
        const apiUser = await userService.getUserById(userId);
        if (apiUser.purchasedBooks) {
          const apiPurchases = apiUser.purchasedBooks.map((bEntity, i) => {
            const b = mapToBook(bEntity);
            return {
              id: i + 1,
              bookId: b.id,
              book: b,
              date: new Date().toISOString(),
              amount: b.price || 50000,
              status: 'completed' as PurchaseStatus,
              paymentMethod: 'Ví hệ thống',
              transactionId: `TXN-${Math.floor(Math.random() * 100000)}`
            };
          });
          setPurchases(apiPurchases);
        }
      } catch (e) {
        console.error(e);
      }
    };
    fetchPurchases();
  }, []);

  const filteredPurchases = purchases.filter(p => {
    if (filter === 'all') return true;
    return p.status === filter;
  });

  const totalSpent = purchases.reduce((sum, p) => sum + p.amount, 0);
  const totalBooks = purchases.length;
  const genreStats = purchases.reduce((acc, p) => {
    const book = p.book || books.find(b => b.id === p.bookId);
    if (book) {
      const firstGenre = book.genres?.[0] || 'Unknown';
      acc[firstGenre] = (acc[firstGenre] || 0) + 1;
    }
    return acc;
  }, {} as Record<string, number>);
  const topGenre = Object.entries(genreStats).sort((a, b) => b[1] - a[1])[0]?.[0] || 'N/A';

  const statusColors: Record<PurchaseStatus, { bg: string; text: string; label: string }> = {
    completed: { bg: 'bg-emerald-100 dark:bg-emerald-900/30', text: 'text-emerald-700 dark:text-emerald-400', label: 'Hoàn thành' },
    pending: { bg: 'bg-amber-100 dark:bg-amber-900/30', text: 'text-amber-700 dark:text-amber-400', label: 'Đang xử lý' },
    failed: { bg: 'bg-rose-100 dark:bg-rose-900/30', text: 'text-rose-700 dark:text-rose-400', label: 'Thất bại' },
  };

  return (
    <div className="min-h-screen bg-[#F8F7F4] dark:bg-[#0D0C14] pt-16">

      {/* Header */}
      <div className="bg-white/80 dark:bg-[#16152B]/80 backdrop-blur-xl border-b border-gray-100 dark:border-white/8">
        <div className="max-w-[1400px] mx-auto px-4 sm:px-6 lg:px-8 py-6">
          <button onClick={() => navigate(-1)}
            className="flex items-center gap-2 text-gray-600 dark:text-gray-400 hover:text-gray-900 dark:hover:text-white transition-colors mb-4 group">
            <ArrowLeft className="w-4 h-4 group-hover:-translate-x-1 transition-transform" />
            <span className="text-sm font-medium">Quay lại</span>
          </button>
          <div className="flex items-center justify-between">
            <div>
              <h1 className="text-gray-900 dark:text-white font-bold" style={{ fontSize: '1.8rem' }}>
                Lịch sử mua hàng
              </h1>
              <p className="text-gray-500 dark:text-gray-400 text-sm mt-1">
                Quản lý và xem lại các giao dịch của bạn
              </p>
            </div>
            <button className="hidden sm:flex items-center gap-2 px-4 py-2.5 bg-white dark:bg-[#16152B] text-gray-700 dark:text-gray-300 border border-gray-200 dark:border-white/12 rounded-2xl font-medium text-sm hover:bg-gray-50 dark:hover:bg-white/8 transition-all">
              <Download className="w-4 h-4" /> Xuất hóa đơn
            </button>
          </div>
        </div>
      </div>

      <div className="max-w-[1400px] mx-auto px-4 sm:px-6 lg:px-8 py-8">

        {/* Stats Cards */}
        <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4 mb-8">
          {[
            { icon: Package, label: 'Tổng sách đã mua', value: totalBooks, unit: 'cuốn', bg: 'from-indigo-500 to-violet-600', shadow: 'shadow-indigo-200 dark:shadow-indigo-900/30' },
            { icon: DollarSign, label: 'Tổng chi tiêu', value: `${(totalSpent / 1000).toFixed(0)}k`, unit: 'VNĐ', bg: 'from-emerald-500 to-teal-600', shadow: 'shadow-emerald-200 dark:shadow-emerald-900/30' },
            { icon: TrendingUp, label: 'Thể loại yêu thích', value: topGenre, unit: '', bg: 'from-rose-500 to-pink-600', shadow: 'shadow-rose-200 dark:shadow-rose-900/30' },
            { icon: Calendar, label: 'Lần mua gần nhất', value: '2', unit: 'ngày trước', bg: 'from-amber-500 to-orange-600', shadow: 'shadow-amber-200 dark:shadow-amber-900/30' },
          ].map((stat, i) => {
            const Icon = stat.icon;
            return (
              <motion.div
                key={stat.label}
                initial={{ opacity: 0, y: 20 }}
                animate={{ opacity: 1, y: 0 }}
                transition={{ duration: 0.4, delay: i * 0.08 }}
                className="bg-white/90 dark:bg-[#16152B]/90 backdrop-blur-xl rounded-3xl border border-gray-100/80 dark:border-white/8 p-5 shadow-sm">
                <div className={`w-12 h-12 rounded-2xl bg-gradient-to-br ${stat.bg} flex items-center justify-center mb-4 shadow-lg ${stat.shadow}`}>
                  <Icon className="w-6 h-6 text-white" />
                </div>
                <div className="text-gray-900 dark:text-white font-bold mb-1" style={{ fontSize: '1.5rem' }}>
                  {stat.value}
                  {stat.unit && <span className="text-gray-400 text-sm font-medium ml-1">{stat.unit}</span>}
                </div>
                <p className="text-gray-500 dark:text-gray-400 text-xs">{stat.label}</p>
              </motion.div>
            );
          })}
        </div>

        {/* Filters */}
        <motion.div
          initial={{ opacity: 0, y: 20 }}
          animate={{ opacity: 1, y: 0 }}
          transition={{ duration: 0.4, delay: 0.3 }}
          className="flex items-center justify-between mb-6">

          <div className="flex items-center gap-2">
            <Filter className="w-4 h-4 text-gray-400" />
            <div className="flex gap-2">
              {[
                { id: 'all' as const, label: 'Tất cả', count: purchases.length },
                { id: 'completed' as const, label: 'Hoàn thành', count: purchases.filter(p => p.status === 'completed').length },
                { id: 'pending' as const, label: 'Đang xử lý', count: 0 },
              ].map(f => (
                <button
                  key={f.id}
                  onClick={() => setFilter(f.id)}
                  className={`flex items-center gap-1.5 px-3.5 py-2 rounded-xl text-xs font-semibold transition-all ${filter === f.id
                    ? 'bg-gradient-to-r from-indigo-600 to-violet-600 text-white shadow-md shadow-indigo-200/60 dark:shadow-indigo-900/20'
                    : 'bg-white dark:bg-[#16152B] text-gray-600 dark:text-gray-400 border border-gray-100 dark:border-white/8 hover:border-indigo-200 dark:hover:border-indigo-700/50'
                  }`}>
                  {f.label}
                  <span className={`px-1.5 py-0.5 rounded-full text-[9px] font-bold ${filter === f.id ? 'bg-white/30' : 'bg-gray-100 dark:bg-white/10'}`}>
                    {f.count}
                  </span>
                </button>
              ))}
            </div>
          </div>

        </motion.div>

        {/* Purchase List */}
        <motion.div
          initial={{ opacity: 0, y: 20 }}
          animate={{ opacity: 1, y: 0 }}
          transition={{ duration: 0.4, delay: 0.4 }}
          className="space-y-4">

          {filteredPurchases.map((purchase, i) => {
            const book = purchase.book || books.find(b => b.id === purchase.bookId);
            if (!book) return null;

            const statusStyle = statusColors[purchase.status];
            const purchaseDate = new Date(purchase.date).toLocaleDateString('vi-VN', { day: '2-digit', month: '2-digit', year: 'numeric' });

            return (
              <motion.div
                key={purchase.id}
                initial={{ opacity: 0, y: 10 }}
                animate={{ opacity: 1, y: 0 }}
                transition={{ delay: i * 0.05 }}
                className="bg-white/90 dark:bg-[#16152B]/90 backdrop-blur-xl rounded-3xl border border-gray-100/80 dark:border-white/8 p-5 shadow-sm hover:shadow-md transition-all group">

                <div className="flex flex-col sm:flex-row gap-5">

                  {/* Book Cover */}
                  <div
                    onClick={() => {
                      navigate(`/book/${book.id}`);
                      onOpenBook(book);
                    }}
                    className="shrink-0 w-20 sm:w-24 cursor-pointer">
                    <div className="relative rounded-2xl overflow-hidden shadow-md group-hover:shadow-lg transition-shadow" style={{ aspectRatio: '2/3' }}>
                      <ImageWithFallback src={book.cover} alt={book.title}
                        className="w-full h-full object-cover group-hover:scale-105 transition-transform duration-400" />
                      <motion.div
                        className="absolute inset-0 rounded-2xl pointer-events-none"
                        initial={false}
                        whileHover={{ boxShadow: `0 0 18px 2px ${book.accentColor}50` }}
                      />
                    </div>
                  </div>

                  {/* Details */}
                  <div className="flex-1 min-w-0">
                    <div className="flex items-start justify-between gap-3 mb-3">
                      <div className="flex-1 min-w-0">
                        <h3
                          onClick={() => {
                            navigate(`/book/${book.id}`);
                            onOpenBook(book);
                          }}
                          className="text-gray-900 dark:text-white font-bold text-base mb-1 cursor-pointer hover:text-indigo-600 dark:hover:text-indigo-400 transition-colors">
                          {book.title}
                        </h3>
                        <p className="text-gray-500 dark:text-gray-400 text-sm mb-2">{book.author}</p>
                        <div className="flex flex-wrap items-center gap-2">
                          <span className={`inline-flex items-center gap-1.5 px-2.5 py-1 rounded-full text-xs font-semibold ${statusStyle.bg} ${statusStyle.text}`}>
                            {purchase.status === 'completed' && <Check className="w-3 h-3" />}
                            {statusStyle.label}
                          </span>
                          <span className="text-gray-400 text-xs">·</span>
                          <span className="text-gray-500 dark:text-gray-400 text-xs flex items-center gap-1">
                            <Calendar className="w-3 h-3" /> {purchaseDate}
                          </span>
                        </div>
                      </div>

                      {/* Price */}
                      <div className="text-right shrink-0">
                        <div className="text-indigo-600 dark:text-indigo-400 font-bold text-lg">
                          {purchase.amount.toLocaleString('vi-VN')}₫
                        </div>
                        {book.originalPrice && book.originalPrice > purchase.amount && (
                          <div className="text-gray-400 line-through text-xs">
                            {book.originalPrice.toLocaleString('vi-VN')}₫
                          </div>
                        )}
                      </div>
                    </div>

                    {/* Transaction Info */}
                    <div className="flex flex-wrap items-center gap-4 pt-3 border-t border-gray-100 dark:border-white/8">
                      <div className="flex items-center gap-2 text-xs">
                        <span className="text-gray-400">Phương thức:</span>
                        <span className="text-gray-700 dark:text-gray-300 font-medium">{purchase.paymentMethod}</span>
                      </div>
                      <div className="flex items-center gap-2 text-xs">
                        <span className="text-gray-400">Mã GD:</span>
                        <span className="text-gray-700 dark:text-gray-300 font-mono font-medium">{purchase.transactionId}</span>
                      </div>
                      <button
                        onClick={() => {
                          navigate(`/book/${book.id}`);
                          onOpenBook(book);
                        }}
                        className="ml-auto flex items-center gap-1.5 px-3 py-1.5 bg-gradient-to-r from-indigo-600 to-violet-600 text-white text-xs font-semibold rounded-xl hover:opacity-90 transition-all shadow-sm">
                        <BookOpen className="w-3 h-3" /> Đọc ngay
                      </button>
                    </div>
                  </div>

                </div>
              </motion.div>
            );
          })}

        </motion.div>

        {/* Empty State */}
        {filteredPurchases.length === 0 && (
          <motion.div
            initial={{ opacity: 0, scale: 0.95 }}
            animate={{ opacity: 1, scale: 1 }}
            transition={{ duration: 0.4 }}
            className="bg-white/90 dark:bg-[#16152B]/90 backdrop-blur-xl rounded-3xl border border-gray-100/80 dark:border-white/8 p-12 text-center shadow-sm">
            <div className="w-20 h-20 rounded-full bg-gradient-to-br from-gray-100 to-gray-200 dark:from-white/8 dark:to-white/5 flex items-center justify-center mx-auto mb-4">
              <Package className="w-10 h-10 text-gray-400" />
            </div>
            <h3 className="text-gray-900 dark:text-white font-bold text-lg mb-2">Chưa có giao dịch nào</h3>
            <p className="text-gray-500 dark:text-gray-400 text-sm mb-6">
              Bạn chưa mua sách nào với bộ lọc này
            </p>
            <button
              onClick={() => navigate('/')}
              className="px-6 py-3 bg-gradient-to-r from-indigo-600 to-violet-600 text-white rounded-2xl font-semibold hover:opacity-95 transition-all shadow-lg shadow-indigo-200/60 dark:shadow-indigo-900/30">
              Khám phá sách
            </button>
          </motion.div>
        )}

      </div>

    </div>
  );
}
