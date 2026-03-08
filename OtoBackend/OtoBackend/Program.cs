using LogicBusiness.Interfaces;
using LogicBusiness.Interfaces.Admin;
using LogicBusiness.Interfaces.Customer;
using LogicBusiness.Interfaces.Repositories;
using LogicBusiness.Repositories;
using LogicBusiness.Services.Admin;
using LogicBusiness.Services.Customer;
using LogicBusiness.Services.Repositories;
using Microsoft.EntityFrameworkCore;
using SqlServer.DBContext;
using SqlServer.Repositories;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder.AllowAnyOrigin()  // Cho phép mọi nguồn (bao gồm cả link figma lạ kia)
                   .AllowAnyMethod()  // Cho phép GET, POST, PUT, DELETE...
                   .AllowAnyHeader(); // Cho phép mọi Header
        });
});

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
builder.Services.AddScoped<ICarSpecificationRepository, CarSpecificationRepository>();
builder.Services.AddScoped<ICarFeatureRepository, CarFeatureRepository>();
builder.Services.AddScoped<IFeatureRepository, FeatureRepository>();



builder.Services.AddScoped<ICarService, CarService>();
builder.Services.AddScoped<ICarAdminService, CarAdminService>();
builder.Services.AddScoped<IFeatureService, FeatureService>();
builder.Services.AddScoped<ICarSpecificationService, CarSpecificationService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
var app = builder.Build();



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");

app.UseHttpsRedirection();

app.UseStaticFiles();

//app.UseCors("AllowAll"); // Bật CORS Ploicy Error lên

app.UseAuthorization();

app.MapControllers();

app.Run();