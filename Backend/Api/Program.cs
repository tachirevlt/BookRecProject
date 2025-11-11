using Application;
using Infrastructure;
using Core;
using System.Text; 
using Microsoft.AspNetCore.Authentication.JwtBearer; 
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy =>
                      {
                          // Thay thế "*" bằng địa chỉ Frontend của bạn khi lên Production
                          policy.WithOrigins("*")
                                .AllowAnyHeader()
                                .AllowAnyMethod();
                      });
});

builder.Services.AddCore(builder.Configuration); 
builder.Services.AddApplicationDI();

var connectionString = builder.Configuration.GetSection("ConnectionStringOptions:DefaultConnection").Value
    ?? throw new InvalidOperationException("Configuration 'ConnectionStringOptions:DefaultConnection' is missing.");

builder.Services.AddInfrastructureDI(connectionString);

var jwtKeyString = builder.Configuration["JwtSettings:Key"] 
    ?? throw new InvalidOperationException("Không tìm thấy cấu hình 'JwtSettings:Key' trong appsettings.json.");

var keyBytes = Encoding.UTF8.GetBytes(jwtKeyString);
if (keyBytes.Length < 32)
{
    throw new InvalidOperationException("Key bí mật (JwtSettings:Key) phải có ít nhất 32 ký tự (256-bit).");
}
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    // Định nghĩa "Bearer"
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Nhập 'Bearer' [dấu cách] và sau đó là token của bạn.\n\nVí dụ: 'Bearer 12345abcdef'"
    });

    // Yêu cầu token cho các endpoint
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    // DÒNG ĐÚNG LÀ ĐÂY:
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});


builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
        ValidAudience = builder.Configuration["JwtSettings:Audience"],
        
        // Dùng biến "keyBytes" đã được kiểm tra an toàn
        IssuerSigningKey = new SymmetricSecurityKey(keyBytes) 
    };
});

// Thêm dịch vụ Phân quyền (để dùng [Authorize])
builder.Services.AddAuthorization();

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors(MyAllowSpecificOrigins);
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();




