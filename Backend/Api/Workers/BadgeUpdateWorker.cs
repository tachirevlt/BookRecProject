using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Application.Services; 
using Core.Interfaces;
using Core.Models;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Api.Workers 
{
    public class BadgeUpdateWorker : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<BadgeUpdateWorker> _logger;

        public BadgeUpdateWorker(IServiceScopeFactory scopeFactory, ILogger<BadgeUpdateWorker> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Badge Update Worker khởi động.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var bookRepository = scope.ServiceProvider.GetRequiredService<IBookRepository>();
                    var badgeService = scope.ServiceProvider.GetRequiredService<IBadgeService>();

                    // 1. Lấy sách
                    var bookEntities = await bookRepository.GetAllBooksAsync(stoppingToken);
                    var bookDtos = bookEntities.Select(BookDTO.FromEntity).Where(dto => dto != null).ToList();

                    // 2. Chạy thuật toán gán nhãn
                    badgeService.AssignBadges(bookDtos!);

                    // 3. CHỈ cập nhật duy nhất trường badges, tuyệt đối không chạm vào trường khác
                    foreach (var entity in bookEntities)
                    {
                        var dto = bookDtos.FirstOrDefault(d => d!.book_id == entity.book_id);
                        if (dto != null)
                        {
                            entity.badges = dto.badges;
                        }
                    }

                    // 4. Lưu thay đổi
                    await bookRepository.SaveChangesAsync(stoppingToken);
                    _logger.LogInformation($"Cập nhật nhãn thành công cho {bookEntities.Count} sách.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Lỗi tại BadgeUpdateWorker.");
                }

                await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
            }
        }
    }
}