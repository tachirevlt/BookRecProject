using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration; // Bạn có thể cần using này
using Core.Interfaces;
using Core.Options;
using Infrastructure.Persistence;
using Infrastructure.Repositories;


namespace Infrastructure
{
    public static class DependencyInjection
    {
        // THAY ĐỔI 1: Thêm tham số "string connectionString"
        public static IServiceCollection AddInfrastructureDI(this IServiceCollection services, string connectionString)
        {
            // THAY ĐỔI 2: Dùng trực tiếp "connectionString"
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(connectionString);
            });
            


            services.AddScoped<IBookRepository, BookRepository>();

            return services;
        }
    }
}