using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using JetAdminSystem.Data;
using JetAdminSystem.Models;

namespace JetAdminSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly JetAdminDbContext _context;

        public AuthController(JetAdminDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// API Đăng nhập và cấp Token
        /// POST: api/Auth/login
        /// </summary>
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            // 1. Kiểm tra tài khoản trong DB (Username & Password)
            var account = await _context.Accounts
                .FirstOrDefaultAsync(a => a.Username == loginDto.Username && a.Password == loginDto.Password);

            if (account == null)
            {
                return Unauthorized(new { success = false, message = "Sai tên đăng nhập hoặc mật khẩu!" });
            }

            // 2. Thiết lập thông tin người dùng vào Claims
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, account.Username),
                new Claim(ClaimTypes.Role, account.Role),
                new Claim(ClaimTypes.NameIdentifier, account.AccountId.ToString())
            };

            // 3. Tạo Key bí mật (Lưu ý: Chuỗi này phải giống hệt chuỗi khai báo ở Program.cs)
            var keyStr = "asp.net_jetadmin_apibackend2026_web";
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyStr));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // 4. Khởi tạo Token
            var token = new JwtSecurityToken(
                issuer: "JetAdmin",
                audience: "JetAdminUsers",
                claims: claims,
                expires: DateTime.Now.AddDays(1), // Token có giá trị trong 24h
                signingCredentials: creds
            );

            // 5. Trả về Token và thông tin cơ bản cho Frontend
            return Ok(new
            {
                success = true,
                token = new JwtSecurityTokenHandler().WriteToken(token),
                username = account.Username,
                role = account.Role
            });
        }

        /// <summary>
        /// (Bổ sung) API lấy thông tin người dùng hiện tại dựa vào Token
        /// GET: api/Auth/me
        /// </summary>
        [HttpGet("me")]
        [Authorize]
        public IActionResult GetMe()
        {
            var username = User.Identity?.Name;
            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            return Ok(new { username, role });
        }
    }

    // Class nhận dữ liệu Body từ Request
    public class LoginDto
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}