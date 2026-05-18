using MediatR;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Core.Entities;
using Core.Interfaces;

namespace Application.Commands
{
    public class PurchaseBookCommand : IRequest<UserBookEntity>
    {
        public Guid user_id { get; set; }
        public Guid book_id { get; set; }
    }

    public class PurchaseBookCommandHandler : IRequestHandler<PurchaseBookCommand, UserBookEntity>
    {
        private readonly IUserBookRepository _userBookRepository;
        private readonly IBookRepository _bookRepository;
        private readonly IUserRepository _userRepository;
        private readonly ITrackingRepository _trackingRepository;
        private readonly IWishlistRepository _wishlistRepository; // Thêm Wishlist Repository

        public PurchaseBookCommandHandler(
            IUserBookRepository userBookRepository,
            IBookRepository bookRepository,
            IUserRepository userRepository,
            ITrackingRepository trackingRepository,
            IWishlistRepository wishlistRepository) // Tiêm (Inject) vào constructor
        {
            _userBookRepository = userBookRepository;
            _bookRepository = bookRepository;
            _userRepository = userRepository;
            _trackingRepository = trackingRepository;
            _wishlistRepository = wishlistRepository;
        }

        public async Task<UserBookEntity> Handle(PurchaseBookCommand request, CancellationToken cancellationToken)
        {
            var book = await _bookRepository.GetBookByIdAsync(request.book_id, cancellationToken);
            if (book == null) throw new KeyNotFoundException($"Không tìm thấy sách với ID: {request.book_id}");

            var user = await _userRepository.GetUserByIdAsync(request.user_id, cancellationToken);
            if (user == null) throw new KeyNotFoundException($"Không tìm thấy user với ID: {request.user_id}");

            // 1. Kiểm tra đã mua chưa
            var alreadyPurchased = await _userBookRepository.HasPurchasedAsync(request.user_id, request.book_id, cancellationToken);
            if (alreadyPurchased) throw new InvalidOperationException("Bạn đã mua cuốn sách này rồi.");

            // 2. Kiểm tra số dư
            if (user.current_balance < book.price)
                throw new InvalidOperationException("Số dư không đủ để mua cuốn sách này.");

            // 3. Trừ tiền
            user.current_balance -= book.price;
            await _userRepository.SaveChangesAsync(cancellationToken);

            // 4. Thêm vào UserBooks (Sở hữu sách)
            var userBook = new UserBookEntity
            {
                user_id = request.user_id,
                book_id = request.book_id,
                purchase_price = book.price,
                purchase_date = DateTime.UtcNow,
                current_chapter = 0
            };
            var result = await _userBookRepository.AddUserBookAsync(userBook, cancellationToken);

            // 5. THÊM TRACKING EVENT VÀO LOG
            var trackingEvent = new TrackingEventEntity
            {
                user_id = request.user_id,
                book_id = request.book_id,
                event_type = "purchase"
            };
            await _trackingRepository.AddTrackingEventAsync(trackingEvent);

            // 6. Tăng bộ đếm purchases_7d và purchases_30d của sách
            await _trackingRepository.IncrementBookStatsAsync(request.book_id, "purchase");

            // 7. XÓA KHỎI WISHLIST (Nếu tồn tại)
            await _wishlistRepository.RemoveFromWishlistAsync(request.user_id, request.book_id, cancellationToken);

            return result;
        }
    }
}