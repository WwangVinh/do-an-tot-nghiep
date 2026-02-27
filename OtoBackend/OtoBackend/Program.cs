using Microsoft.EntityFrameworkCore;
using OtoBackend.Interfaces;
using OtoBackend.Models; // Khai báo thư mục Models
using OtoBackend.Repositories;


var builder = WebApplication.CreateBuilder(args);

// --- THÊM KẾT NỐI DATABASE (DEPENDENCY INJECTION) VÀO ĐÂY ---
builder.Services.AddDbContext<OtoContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
// -----------------------------------------------------------

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<ICarRepository, CarRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseCors("AllowAll"); // Bật CORS Ploicy Error lên

app.UseAuthorization();

app.MapControllers();

app.Run();