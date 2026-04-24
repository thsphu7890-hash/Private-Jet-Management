using Microsoft.EntityFrameworkCore;
using JetAdminSystem.Data;
using System.Text.Json.Serialization;
using JetAdminSystem.Models;
using JetAdminSystem.Services;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

// --- DÒNG NÀY PHẢI ĐẶT SAU USING VÀ TRƯỚC BUILDER ĐỂ FIX LỖI DATETIME POSTGRES ---
System.AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var builder = WebApplication.CreateBuilder(args);

// --- 0. CẤU HÌNH PORT CHO RENDER (GIÚP APP KHÔNG BỊ CRASH) ---
var port = Environment.GetEnvironmentVariable("PORT") ?? "5000";
builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

// --- 1. CẤU HÌNH DATABASE ĐA MÔI TRƯỜNG (SQL SERVER <-> POSTGRESQL) ---
builder.Services.AddDbContext<JetAdminDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

    if (builder.Environment.IsDevelopment())
    {
        // Chạy Local dùng SQL Server
        options.UseSqlServer(connectionString);
    }
    else
    {
        // Chạy trên Render dùng PostgreSQL
        options.UseNpgsql(connectionString);
    }
});

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });

// --- 2. CẤU HÌNH AUTHENTICATION (JWT) ---
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

// --- 3. DỊCH VỤ CLOUDINARY & CORS ---
builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection("CloudinarySettings"));
builder.Services.AddScoped<ICloudinaryService, CloudinaryService>();

builder.Services.AddCors(options => {
    options.AddPolicy("AllowReactApp", policy =>
        policy.WithOrigins(
                "http://localhost:5173",
                "https://private-jet-management-izkf.onrender.com"
              )
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials());
});

builder.Services.AddEndpointsApiExplorer();

// --- 4. CẤU HÌNH SWAGGER ---
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "JetAdminSystem API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Nhập Token: Bearer {your_token}",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            new string[] {}
        }
    });
});

var app = builder.Build();

// --- 5. MIDDLEWARE PIPELINE ---
app.UseSwagger();
app.UseSwaggerUI(c => {
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "JetAdminSystem v1");
    c.RoutePrefix = string.Empty;
});

app.UseHttpsRedirection();
app.UseCors("AllowReactApp");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();