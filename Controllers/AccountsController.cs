using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using JetAdminSystem.Data;
using JetAdminSystem.Models;

namespace JetAdminSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly JetAdminDbContext _context;

        public AccountsController(JetAdminDbContext context)
        {
            _context = context;
        }

        // 1. LẤY DANH SÁCH TẤT CẢ TÀI KHOẢN
        // GET: api/Accounts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Account>>> GetAccounts()
        {
            return await _context.Accounts.ToListAsync();
        }

        // 2. LẤY CHI TIẾT 1 TÀI KHOẢN
        // GET: api/Accounts/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Account>> GetAccount(int id)
        {
            var account = await _context.Accounts.FindAsync(id);

            if (account == null)
            {
                return NotFound(new { success = false, message = "Không tìm thấy tài khoản." });
            }

            return account;
        }

        // 3. CẬP NHẬT TÀI KHOẢN
        // PUT: api/Accounts/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAccount(int id, Account account)
        {
            if (id != account.AccountId)
            {
                return BadRequest(new { success = false, message = "ID tài khoản không khớp." });
            }

            // Nếu người dùng không nhập mật khẩu mới, giữ nguyên mật khẩu cũ
            if (string.IsNullOrEmpty(account.Password))
            {
                _context.Entry(account).Property(x => x.Password).IsModified = false;
            }

            _context.Entry(account).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AccountExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(new { success = true, message = "Cập nhật thành công." });
        }

        // 4. TẠO TÀI KHOẢN MỚI
        // POST: api/Accounts
        [HttpPost]
        public async Task<ActionResult<Account>> PostAccount(Account account)
        {
            // Kiểm tra username trùng lặp
            if (await _context.Accounts.AnyAsync(a => a.Username == account.Username))
            {
                return BadRequest(new { success = false, message = "Tên đăng nhập đã tồn tại." });
            }

            _context.Accounts.Add(account);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetAccount", new { id = account.AccountId }, account);
        }

        // 5. XÓA TÀI KHOẢN (PHÂN QUYỀN ADMIN)
        // DELETE: api/Accounts/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteAccount(int id)
        {
            try
            {
                var account = await _context.Accounts.FindAsync(id);

                if (account == null)
                {
                    return NotFound(new { success = false, message = $"Không tìm thấy ID {id}." });
                }

                // Ngăn Admin tự xóa chính mình
                var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (currentUserId == id.ToString())
                {
                    return BadRequest(new { success = false, message = "Bạn không thể tự xóa chính mình!" });
                }

                // Ngăn xóa Admin cuối cùng
                if (account.Role == "Admin")
                {
                    var adminCount = await _context.Accounts.CountAsync(a => a.Role == "Admin");
                    if (adminCount <= 1)
                    {
                        return BadRequest(new { success = false, message = "Phải có ít nhất một tài khoản Admin." });
                    }
                }

                _context.Accounts.Remove(account);
                await _context.SaveChangesAsync();

                return Ok(new { success = true, message = "Xóa tài khoản thành công." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi hệ thống.", detail = ex.Message });
            }
        }

        private bool AccountExists(int id)
        {
            return _context.Accounts.Any(e => e.AccountId == id);
        }
    }
}