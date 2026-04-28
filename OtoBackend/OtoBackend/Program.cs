using LogicBusiness.Repositories;
using LogicBusiness.Services.Admin;
using LogicBusiness.Services.Customer;
using LogicBusiness.Services.Shared;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SqlServer.DBContext;
using SqlServer.Repositories;
using System.Text;
using Microsoft.AspNetCore.SignalR;
using LogicBusiness.Interfaces.Admin;
using LogicBusiness.Interfaces.Customer;
using LogicBusiness.Interfaces.Repositories;
using LogicBusiness.Interfaces.Shared;


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
var jwtSettings = builder.Configuration.GetSection("Jwt");
var secretKey = Encoding.UTF8.GetBytes(jwtSettings["Key"]);

builder.Services.AddAuthentication(options =>
{
    // Bắt buộc phải có 3 dòng này để .NET biết mặc định sẽ dùng JWT Bearer
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false; // Set false nếu chạy localhost (HTTP)
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(secretKey)
    };
});

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "OtoBackend API", Version = "v1" });

    // 1. Định nghĩa Security Scheme (Thêm nút Authorize)
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header sử dụng scheme Bearer. \r\n\r\n Nhập 'Bearer' [khoảng trắng] và sau đó dán Token của bạn vào.\r\n\r\nVí dụ: 'Bearer 12345abcdef'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    // 2. Yêu cầu Swagger gắn Token này vào mỗi request
    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header,
            },
            new List<string>()
        }
    });
});

builder.Services.AddCors(options => {
    options.AddDefaultPolicy(policy => {
        policy.WithOrigins("http://localhost:7033") // URL React của ní
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials(); // Bắt buộc cho SignalR
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
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ICarPricingVersionRepository, CarPricingVersionRepository>();
builder.Services.AddScoped<ICarImageRepository, CarImageRepository>();
builder.Services.AddScoped<ICarSpecificationRepository, CarSpecificationRepository>();
builder.Services.AddScoped<ICarFeatureRepository, CarFeatureRepository>();
builder.Services.AddScoped<IFeatureRepository, FeatureRepository>();
builder.Services.AddScoped<ICarInventoryRepository, CarInventoryRepository>();
builder.Services.AddScoped<IShowroomRepository, ShowroomRepository>();
builder.Services.AddScoped<IBookingRepository, BookingRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
builder.Services.AddScoped<IConsignmentRepository,ConsignmentRepository>();
builder.Services.AddScoped<ICarWishlistRepository,CarWishlistRepository>();
builder.Services.AddScoped<IBannerRepository, BannerRepository>();
builder.Services.AddScoped<IDashboardRepository, DashboardRepository>();
builder.Services.AddScoped<IPromotionRepository, PromotionRepository>();
builder.Services.AddScoped<ISystemSettingRepository, SystemSettingRepository>();
builder.Services.AddScoped<IReviewRepository, ReviewRepository>();
builder.Services.AddScoped<IArticleRepository, ArticleRepository>();



builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ICarService, CarService>();
builder.Services.AddScoped<IPricingService, PricingService>();
builder.Services.AddScoped<ICarAdminService, CarAdminService>();
builder.Services.AddScoped<IPricingAdminService, PricingAdminService>();
builder.Services.AddScoped<IFeatureService, FeatureService>();
builder.Services.AddScoped<ICarSpecificationService, CarSpecificationService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IShowroomService, ShowroomService>();
builder.Services.AddScoped<ICarInventoryService, CarInventoryService>();
builder.Services.AddScoped<IBookingService, BookingService>();
builder.Services.AddScoped<IBookingAdminService, BookingAdminService>();
builder.Services.AddScoped<IOrderAdminService, OrderAdminService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IConsignmentService, ConsignmentService>();
builder.Services.AddScoped<ICarWishlistService, CarWishlistService>();
builder.Services.AddScoped<IChatRepository, ChatRepository>();
builder.Services.AddScoped<IChatService, ChatService>();
builder.Services.AddScoped<IBannerAdminService, BannerAdminService>();
builder.Services.AddScoped<IDashboardAdminService, DashboardAdminService>();
builder.Services.AddScoped<IPromotionAdminService, PromotionAdminService>();
builder.Services.AddScoped<ISystemSettingAdminService, SystemSettingAdminService>();
builder.Services.AddScoped<IReviewService, ReviewService>();
builder.Services.AddScoped<IArticleService, ArticleService>();

builder.Services.AddHttpClient<IAiAdvisorService, AiAdvisorService>();

builder.Services.AddSignalR();


var app = builder.Build();

// DB init strategy:
// - Development: EnsureCreated() to bootstrap schema from the current model
//   (repo doesn't include a full baseline migration).
// - Production: Migrate() to apply incremental schema updates.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<OtoContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("DbMigrations");

    if (app.Environment.IsDevelopment())
    {
        db.Database.EnsureCreated();
    }
    else
    {
        const int maxAttempts = 10;
        for (var attempt = 1; attempt <= maxAttempts; attempt++)
        {
            try
            {
                db.Database.Migrate();
                break;
            }
            catch (Exception ex) when (attempt < maxAttempts)
            {
                logger.LogWarning(ex, "Database migration attempt {Attempt}/{MaxAttempts} failed. Retrying...", attempt, maxAttempts);
                Thread.Sleep(TimeSpan.FromSeconds(3));
            }
        }
    }
}



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseRouting();

app.UseCors("AllowAll");

app.UseHttpsRedirection();

app.UseStaticFiles();

app.MapHub<LogicBusiness.Hubs.ChatHub >("/chathub");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();