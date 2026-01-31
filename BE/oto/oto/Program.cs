using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;  // Đảm bảo đã cài đặt Newtonsoft.Json trong dự án
using oto.MyModels;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Thêm DbContext với SQL Server
string strcnn = builder.Configuration.GetConnectionString("cnn");
builder.Services.AddDbContext<OtoContext>(options => options.UseSqlServer(strcnn));

// Cấu hình các dịch vụ MVC và Newtonsoft.Json
builder.Services.AddControllers().AddJsonOptions(x =>
{
    // Giúp JSON trả về đơn giản, không bị lồng Metadata ($id, $ref)
    x.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    // Giúp giữ nguyên tên OrderId (không bị biến thành orderId)
    x.JsonSerializerOptions.PropertyNamingPolicy = null;
});

// Cấu hình Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Cấu hình CORS
app.UseCors(builder => builder.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors("AllowAll");
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
