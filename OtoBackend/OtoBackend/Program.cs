using Microsoft.EntityFrameworkCore;
using OtoBackend.Interfaces;
using OtoBackend.Models; // Khai báo thư mục Models
using OtoBackend.Repositories;


var builder = WebApplication.CreateBuilder(args);

// --- THÊM KẾT NỐI DATABASE (DEPENDENCY INJECTION) VÀO ĐÂY ---
builder.Services.AddDbContext<OtoContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
// -----------------------------------------------------------

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
});
// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<ICarRepository, CarRepository>();
builder.Services.AddScoped<ICarImageRepository, CarImageRepository>();

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