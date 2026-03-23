using Microsoft.EntityFrameworkCore;
using OnMuhasebeApp.Application.Interfaces;
using OnMuhasebeApp.Application.Services;
using OnMuhasebeApp.Infrastructure.Context;

var builder = WebApplication.CreateBuilder(args);

// 1. CORS AYARI: Tarayıcı engelini kökten çözer
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy => policy.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());
});

// 2. VERİTABANI: Masaüstündeki gerçek veritabanına bağlanır
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(@"Data Source=C:\Users\ahmet\OneDrive\Masaüstü\OnMuhasebeApp\OnMuhasebeApp.Presentation\bin\Debug\net8.0-windows\OnMuhasebeApp.db"));

builder.Services.AddScoped<IAppDbContext>(provider => provider.GetService<AppDbContext>());
builder.Services.AddScoped<IFaturaService, FaturaManager>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// CORS'u aktif et (Build'den hemen sonra olmalı)
app.UseCors("AllowAll");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// HTTPS yönlendirmesini test aşamasında kapatıyoruz (Çakışma olmasın diye)
// app.UseHttpsRedirection(); 

app.UseAuthorization();
app.MapControllers();
app.Run();