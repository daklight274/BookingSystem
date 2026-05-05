using BookingSystem.BookingService.Consumers;
using BookingSystem.BookingService.Data;
using MassTransit;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
// ── Database ───────────────────────────────────────
builder.Services.AddDbContext<BookingDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration
        .GetConnectionString("BookingDb")));

// ── MassTransit + RabbitMQ ─────────────────────────
// Booking Service chỉ PUBLISH events, không consume ở tuần 2
// (Consume PaymentCompleted sẽ thêm ở tuần 3)
builder.Services.AddMassTransit(x =>
{
    // Consumers tuần 3 — thêm mới
    x.AddConsumer<PaymentCompletedConsumer>();
    x.AddConsumer<PaymentFailedConsumer>();
    x.UsingRabbitMq((ctx, cfg) =>
    {
        cfg.Host(builder.Configuration["RabbitMQ:Host"], "/", h =>
        {
            h.Username(builder.Configuration["RabbitMQ:Username"]!);
            h.Password(builder.Configuration["RabbitMQ:Password"]!);
        });

        cfg.ConfigureEndpoints(ctx);
    });
});
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
    c.SwaggerDoc("v1", new() { Title = "Booking Service API", Version = "v1" }));

var app = builder.Build();
// ── Auto migrate ───────────────────────────────────
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<BookingDbContext>();
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
