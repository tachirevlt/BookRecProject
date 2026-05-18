using MediatR;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Core.Entities;
using Core.Interfaces;

namespace Application.Commands
{
    public class AddToWishlistCommand : IRequest<UserWishlistEntity>
    {
        public Guid user_id { get; set; }
        public Guid book_id { get; set; }
        public string? collection_name { get; set; }
    }

    public class AddToWishlistCommandHandler : IRequestHandler<AddToWishlistCommand, UserWishlistEntity>
    {
        private readonly IWishlistRepository _wishlistRepository;
        private readonly IBookRepository _bookRepository;
        private readonly ITrackingRepository _trackingRepository;
        private readonly IUserBookRepository _userBookRepository; // Khai báo thêm

        public AddToWishlistCommandHandler(
            IWishlistRepository wishlistRepository,
            IBookRepository bookRepository,
            ITrackingRepository trackingRepository,
            IUserBookRepository userBookRepository) // Tiêm (Inject) vào constructor
        {
            _wishlistRepository = wishlistRepository;
            _bookRepository = bookRepository;
            _trackingRepository = trackingRepository;
            _userBookRepository = userBookRepository; // Gán giá trị
        }

        public async Task<UserWishlistEntity> Handle(AddToWishlistCommand request, CancellationToken cancellationToken)
        {
            var book = await _bookRepository.GetBookByIdAsync(request.book_id, cancellationToken);
            if (book == null) throw new KeyNotFoundException($"Không tìm thấy sách với ID: {request.book_id}");

            // 1. Kiểm tra xem người dùng đã sở hữu sách này chưa
            var alreadyPurchased = await _userBookRepository.HasPurchasedAsync(request.user_id, request.book_id, cancellationToken);
            if (alreadyPurchased) throw new InvalidOperationException("Bạn không thể thêm sách đã sở hữu vào danh sách yêu thích.");

            // 2. Kiểm tra xem sách đã có trong Wishlist chưa
            var alreadyExists = await _wishlistRepository.IsInWishlistAsync(request.user_id, request.book_id, cancellationToken);
            if (alreadyExists) throw new InvalidOperationException("Sách này đã có trong danh sách yêu thích.");

            // 3. Lưu sách vào danh sách Wishlist của người dùng
            var item = new UserWishlistEntity
            {
                user_id = request.user_id,
                book_id = request.book_id,
                price_at_addition = book.price,
                collection_name = request.collection_name,
                added_at = DateTime.UtcNow
            };

            var result = await _wishlistRepository.AddToWishlistAsync(item, cancellationToken);

            // 4. GHI NHẬN LỊCH SỬ TRACKING EVENT 
            var trackingEvent = new TrackingEventEntity
            {
                user_id = request.user_id,
                book_id = request.book_id,
                event_type = "add_wishlist"
            };
            await _trackingRepository.AddTrackingEventAsync(trackingEvent);

            // 5. Tăng bộ đếm favorite_7d và favorite_30d của sách
            await _trackingRepository.IncrementBookStatsAsync(request.book_id, "add_wishlist");

            return result;
        }
    }
}