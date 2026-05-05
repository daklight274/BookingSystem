using BookingSystem.RoomService.Consumers;
using BookingSystem.RoomService.Data;
using BookingSystem.Shared.Contracts;
using MassTransit;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
// Database
builder.Services.AddDbContext<RoomDbContext>(opts => opts.UseSqlServer(builder.Configuration.GetConnectionString("RoomDb")));

// ── MassTransit + RabbitMQ (sẽ dùng từ tuần 2) ────
builder.Services.AddMassTransit(x =>
{
    // Đăng ký consumers
    x.AddConsumer<BookingCreatedConsumer>();
    x.AddConsumer<BookingCancelledConsumer>();
    x.AddConsumer<PaymentFailedConsumer>();

    x.UsingRabbitMq((ctx, cfg) =>
    {
        cfg.Host(builder.Configuration["RabbitMQ:Host"], "/", h =>
        {
            h.Username(builder.Configuration["RabbitMQ:Username"]!);
            h.Password(builder.Configuration["RabbitMQ:Password"]!);
        });

        // Tự tạo queue tên theo consumer class
        cfg.ConfigureEndpoints(ctx);
    });
});
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
// ── Auto migrate + seed data khi khởi động ─────────
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<RoomDbContext>();
    db.Database.Migrate();
    SeedData.Initialize(db);
}
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
