using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using JetAdminSystem.Data;
using JetAdminSystem.Models;
using JetAdminSystem.Services;

namespace JetAdminSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    // Sử dụng Primary Constructor cho gọn giống code mẫu của Phú
    public class PassengersController(JetAdminDbContext context, ICloudinaryService cloudinary) : ControllerBase
    {
        // 1. LẤY DANH SÁCH (Phân trang hoặc sắp xếp mới nhất lên đầu)
        [HttpGet]
        public async Task<ActionResult> Get()
        {
            var passengers = await context.Passengers
                .AsNoTracking()
                .OrderByDescending(p => p.PassengerId)
                .ToListAsync();
            return Ok(passengers);
        }

        // 2. CHI TIẾT (Kèm lịch sử Booking)
        [HttpGet("{id}")]
        public async Task<ActionResult> Get(int id)
        {
            var p = await context.Passengers
                .Include(x => x.Bookings)
                    .ThenInclude(b => b.Schedule)
                .FirstOrDefaultAsync(x => x.PassengerId == id);

            if (p == null) return NotFound(new { message = "Không tìm thấy hành khách." });
            return Ok(p);
        }

        // 3. THÊM MỚI
        [HttpPost]
        public async Task<ActionResult> Post([FromForm] Passenger p, IFormFile? avatarFile, IFormFile? passportFile)
        {
            try
            {
                // Upload Avatar nếu có
                if (avatarFile != null)
                {
                    var avatarResult = await cloudinary.UploadImageAsync(avatarFile, "Avatars");
                    p.ImageUrl = avatarResult.SecureUrl.AbsoluteUri;
                }

                // Upload Passport Scan nếu có
                if (passportFile != null)
                {
                    var passportResult = await cloudinary.UploadImageAsync(passportFile, "Passports");
                    p.PassportScanUrl = passportResult.SecureUrl.AbsoluteUri;
                }

                context.Passengers.Add(p);
                await context.SaveChangesAsync();

                return CreatedAtAction(nameof(Get), new { id = p.PassengerId }, p);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Lỗi khi thêm mới: " + ex.Message });
            }
        }

        // 4. CẬP NHẬT (Xử lý giữ ảnh cũ nếu không upload ảnh mới)
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromForm] Passenger p, IFormFile? avatarFile, IFormFile? passportFile)
        {
            // Kiểm tra khớp ID
            if (id != p.PassengerId) return BadRequest(new { message = "ID không khớp giữa URL và dữ liệu." });

            // Tìm bản ghi cũ trong DB (Dùng AsNoTracking để tránh xung đột khi Attach lại p)
            var old = await context.Passengers.AsNoTracking().FirstOrDefaultAsync(x => x.PassengerId == id);
            if (old == null) return NotFound(new { message = "Hành khách không tồn tại." });

            try
            {
                // Xử lý ảnh đại diện: Nếu có file mới thì upload, không thì giữ URL cũ
                if (avatarFile != null)
                    p.ImageUrl = (await cloudinary.UploadImageAsync(avatarFile, "Avatars")).SecureUrl.AbsoluteUri;
                else
                    p.ImageUrl = old.ImageUrl;

                // Xử lý ảnh passport: Tương tự như trên
                if (passportFile != null)
                    p.PassportScanUrl = (await cloudinary.UploadImageAsync(passportFile, "Passports")).SecureUrl.AbsoluteUri;
                else
                    p.PassportScanUrl = old.PassportScanUrl;

                // Đánh dấu Entity đã thay đổi
                context.Entry(p).State = EntityState.Modified;
                await context.SaveChangesAsync();

                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                return BadRequest(new { message = "Dữ liệu đã bị thay đổi bởi người khác, vui lòng thử lại." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Lỗi cập nhật: " + ex.Message });
            }
        }

        // 5. XÓA (Có kiểm tra ràng buộc Booking)
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var p = await context.Passengers
                .Include(x => x.Bookings)
                .FirstOrDefaultAsync(x => x.PassengerId == id);

            if (p == null) return NotFound(new { message = "Hành khách không tồn tại." });

            // Ràng buộc: Không cho xóa khách đã có lịch sử đặt chỗ (Booking)
            if (p.Bookings != null && p.Bookings.Any())
            {
                return BadRequest(new { message = "Không thể xóa hành khách đã có lịch sử đặt vé (Bookings)." });
            }

            context.Passengers.Remove(p);
            await context.SaveChangesAsync();

            return Ok(new { message = "Đã xóa hành khách thành công." });
        }
    }
}