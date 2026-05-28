namespace Application.Services
{
    using Core.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class BadgeService : IBadgeService
    {
        public void AssignBadges(List<BookDTO> books)
        {
            // Bảo vệ vòng ngoài: Dữ liệu null hoặc rỗng thì thoát ngay
            if (books == null || !books.Any()) return;

            int currentYear = DateTime.UtcNow.Year;

            // =========================================================
            // 1. TÍNH NGƯỠNG BEST SELLER (Top 10% sách có lượt mua cao nhất)
            // =========================================================
            var bestSellerThreshold = 0;
            var purchasesList = books
                .Where(b => b.purchases_7d > 0)
                .Select(b => b.purchases_7d)
                .OrderByDescending(p => p)
                .ToList();

            if (purchasesList.Any())
            {
                int top10PercentIndex = (int)Math.Ceiling(purchasesList.Count * 0.1) - 1;
                if (top10PercentIndex >= 0)
                {
                    bestSellerThreshold = purchasesList[top10PercentIndex];
                }
            }

            // =========================================================
            // 2. XỬ LÝ GẮN NHÃN CHO TỪNG CUỐN SÁCH
            // =========================================================
            foreach (var book in books)
            {
                // Danh sách tạm lưu nhãn và độ ưu tiên (Priority: nhỏ hơn = ưu tiên hơn)
                var candidateBadges = new List<(string Name, int Priority)>();

                // --- CẤP 1: NHÃN THỦ CÔNG CỦA ADMIN ---
                if (!string.IsNullOrWhiteSpace(book.badge))
                {
                    candidateBadges.Add((book.badge, 1));
                }

                // --- CẤP 2: BEST SELLER ---
                if (book.purchases_7d >= bestSellerThreshold && book.purchases_7d > 0)
                {
                    candidateBadges.Add(("Best Seller", 2));
                }

                // --- CẤP 3: TRENDING (So sánh gia tốc) ---
                double score7d = (book.purchases_7d * 10) + (book.favorite_7d * 5) + (book.views_7d * 1);
                double score30d = (book.purchases_30d * 10) + (book.favorite_30d * 5) + (book.views_30d * 1);
                
                // Quy đổi 30 ngày ra trung bình 1 tuần
                double averageWeeklyPast = score30d > 0 ? (score30d / 4.0) : 1; 
                
                // Tăng trưởng gấp 1.5 lần và điểm nền > 50
                if (score7d > (averageWeeklyPast * 1.8) && score7d > 50)
                {
                    candidateBadges.Add(("Trending", 3));
                }

                // --- CẤP 4: NEW (Sách trong 10 năm trở lại) ---
                if (book.original_publication_year.HasValue)
                {
                    if ((currentYear - book.original_publication_year.Value) <= 10)
                    {
                        candidateBadges.Add(("New", 4));
                    }
                }

                // =========================================================
                // 3. XUẤT RA DỮ LIỆU CUỐI CÙNG
                // =========================================================
                book.badges = candidateBadges
                    .OrderBy(b => b.Priority)    // Xếp hạng ưu tiên
                    .Select(b => b.Name)         // Chỉ lấy tên nhãn
                    .Distinct()                  // Đề phòng admin lỡ nhập trùng tên
                    .Take(2)                     // Giới hạn UI tối đa 2 nhãn
                    .ToList();
            }
        }
    }
}