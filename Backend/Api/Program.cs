using Application;
using Infrastructure;
using Core;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCore(builder.Configuration); 
builder.Services.AddApplicationDI();

var connectionString = builder.Configuration.GetSection("ConnectionStringOptions:DefaultConnection").Value
    ?? throw new InvalidOperationException("Configuration 'ConnectionStringOptions:DefaultConnection' is missing.");

builder.Services.AddInfrastructureDI(connectionString);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();




