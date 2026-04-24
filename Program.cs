using Microsoft.EntityFrameworkCore;
using JetAdminSystem.Data;
using System.Text.Json.Serialization;
using JetAdminSystem.Models;
using JetAdminSystem.Services;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// --- 1. CẤU HÌNH DATABASE & CONTROLLERS ---
builder.Services.AddDbContext<JetAdminDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });

// --- 2. CẤU HÌNH AUTHENTICATION (QUAN TRỌNG NHẤT) ---
// Chuỗi Key này phải khớp 100% với chuỗi ở AuthController
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
        RoleClaimType = System.Security.Claims.ClaimTypes.Role // Giúp [Authorize(Roles="Admin")] hoạt động
    };
});

// --- 3. CÁC DỊCH VỤ KHÁC ---
builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection("CloudinarySettings"));
builder.Services.AddScoped<ICloudinaryService, CloudinaryService>();

builder.Services.AddCors(options => {
    options.AddPolicy("AllowReactApp", policy =>
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials());
});

builder.Services.AddEndpointsApiExplorer();

// --- 4. CẤU HÌNH SWAGGER (Bổ sung nút Authorize để test Token) ---
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "JetAdminSystem API", Version = "v1" });

    // Thêm cấu hình để có nút "Authorize" (ổ khóa) trên Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Nhập Token theo cú pháp: Bearer {your_token}",
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

    c.MapType<IFormFile>(() => new OpenApiSchema { Type = "string", Format = "binary" });
    c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
});

var app = builder.Build();

// --- 5. MIDDLEWARE (THỨ TỰ RẤT QUAN TRỌNG) ---
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "JetAdminSystem v1"));
}

app.UseHttpsRedirection();
app.UseCors("AllowReactApp");

// LƯU Ý: Authentication phải đứng TRƯỚC Authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();