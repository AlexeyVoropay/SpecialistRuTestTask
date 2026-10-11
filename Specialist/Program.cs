using Specialist.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Основной сервис — под интерфейсом
builder.Services.AddScoped<IEmailService, EmailService>();

// Резервный — конкретный тип, т.к. он специфичен
builder.Services.AddScoped<IFallbackEmailService, FallbackEmailService>();

// Сервис расчёта скидок
builder.Services.AddScoped<IDiscountService, DiscountService>();

var app = builder.Build();

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