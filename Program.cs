using Microsoft.EntityFrameworkCore;
using JetAdminSystem.Data;
using System.Text.Json.Serialization;
using JetAdminSystem.Models;
using JetAdminSystem.Services;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

// --- 1. XỬ LÝ DATETIME CHO POSTGRESQL (RENDERS) ---
// Tránh lỗi múi giờ khi deploy lên PostgreSQL trên Render
System.AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var builder = WebApplication.CreateBuilder(args);

// --- 2. CẤU HÌNH PORT CHO RENDER ---
if (!builder.Environment.IsDevelopment())
{
    var port = Environment.GetEnvironmentVariable("PORT") ?? "5000";
    builder.WebHost.UseUrls($"http://0.0.0.0:{port}");
}

// --- 3. CẤU HÌNH DATABASE ĐA MÔI TRƯỜNG ---
builder.Services.AddDbContext<JetAdminDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

    if (builder.Environment.IsDevelopment())
    {
        // Local: Dùng SQL Server (Đọc từ appsettings.Development.json)
        options.UseSqlServer(connectionString);
        Console.WriteLine("--> [DATABASE] Running in Development: Using SQL Server");
    }
    else
    {
        // Production: Dùng PostgreSQL (Đọc từ appsettings.json trên Render)
        options.UseNpgsql(connectionString);
        Console.WriteLine("--> [DATABASE] Running in Production: Using PostgreSQL");
    }
});

// --- 4. CẤU HÌNH JSON ---
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Ngăn lỗi vòng lặp dữ liệu (Circular Reference)
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        // Giữ nguyên định dạng PascalCase của Model (tùy chọn của Phú)
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
    });

// --- 5. JWT AUTHENTICATION ---
var secretKey = "asp.net_jetadmin_apibackend2026";
var key = Encoding.ASCII.GetBytes(secretKey);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false,
        RoleClaimType = System.Security.Claims.ClaimTypes.Role
    };
});

// --- 6. DỊCH VỤ BỔ TRỢ (CLOUDINARY & CORS) ---
builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection("CloudinarySettings"));
builder.Services.AddScoped<ICloudinaryService, CloudinaryService>();

builder.Services.AddCors(options => {
    options.AddPolicy("AllowReactApp", policy =>
        policy.WithOrigins("http://localhost:5173", "https://private-jet-management-izkf.onrender.com")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials());
});

builder.Services.AddEndpointsApiExplorer();

// --- 7. CẤU HÌNH SWAGGER ---
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "JetAdminSystem API", Version = "v1" });

    // Tích hợp ô nhập Token vào Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Nhập Token theo cú pháp: Bearer {your_token}",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement {
        {
            new OpenApiSecurityScheme {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            }, new string[] {}
        }
    });
});

var app = builder.Build();

// --- 8. MIDDLEWARE PIPELINE ---

// Luôn bật Swagger để Phú dễ test API
app.UseSwagger();
app.UseSwaggerUI(c => {
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "JetAdminSystem v1");

    // Đã bỏ RoutePrefix = string.Empty để quay về dùng /swagger mặc định
});

app.UseStaticFiles();
app.UseHttpsRedirection();
app.UseRouting();

// Thứ tự quan trọng: Cors -> Auth -> Authorization
app.UseCors("AllowReactApp");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();