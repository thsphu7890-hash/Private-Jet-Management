using JetAdminSystem.Data;
using JetAdminSystem.Models;
using JetAdminSystem.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JetAdminSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AircraftController(
        JetAdminDbContext db,
        ICloudinaryService cloudinaryService) : ControllerBase
    {
        // 1. LẤY DANH SÁCH TOÀN BỘ CHUYÊN CƠ
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Aircraft>>> GetAircrafts()
        {
            return await db.Aircrafts
                .Include(a => a.Category)
                .OrderByDescending(a => a.AircraftId)
                .ToListAsync();
        }

        // 2. LẤY CHI TIẾT MỘT CHUYÊN CƠ THEO ID
        [HttpGet("{id}")]
        public async Task<ActionResult<Aircraft>> GetAircraft(int id)
        {
            var aircraft = await db.Aircrafts
                .Include(a => a.Category)
                .FirstOrDefaultAsync(a => a.AircraftId == id);

            if (aircraft == null) return NotFound("Không tìm thấy máy bay.");
            return aircraft;
        }

        // 3. THÊM MỚI CHUYÊN CƠ (Hỗ trợ nhiều ảnh nội thất)
        [HttpPost]
        public async Task<ActionResult<Aircraft>> PostAircraft([FromForm] Aircraft aircraft, [FromForm] List<IFormFile>? InteriorFiles)
        {
            // Xử lý 1 ảnh Ngoại thất
            if (aircraft.ExteriorFile != null && aircraft.ExteriorFile.Length > 0)
            {
                var res = await cloudinaryService.UploadImageAsync(aircraft.ExteriorFile, "jet_exteriors");
                if (res?.SecureUrl != null) aircraft.ExteriorImageUrl = res.SecureUrl.AbsoluteUri;
            }

            // Xử lý NHIỀU ảnh Nội thất (InteriorFiles)
            if (InteriorFiles != null && InteriorFiles.Count > 0)
            {
                var imageUrls = new List<string>();
                foreach (var file in InteriorFiles)
                {
                    var res = await cloudinaryService.UploadImageAsync(file, "jet_interiors");
                    if (res?.SecureUrl != null) imageUrls.Add(res.SecureUrl.AbsoluteUri);
                }
                // Nối các URL lại bằng dấu phẩy
                aircraft.InteriorImageUrl = string.Join(",", imageUrls);
            }

            db.Aircrafts.Add(aircraft);
            await db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAircraft), new { id = aircraft.AircraftId }, aircraft);
        }

        // 4. CẬP NHẬT THÔNG TIN (Hỗ trợ status 'InFlight' và nhiều ảnh)
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAircraft(int id, [FromForm] Aircraft data, [FromForm] List<IFormFile>? InteriorFiles)
        {
            if (data == null) return BadRequest("Dữ liệu gửi lên bị trống.");

            var existing = await db.Aircrafts.FindAsync(id);
            if (existing == null) return NotFound("Không tìm thấy chuyên cơ trong hệ thống.");

            // Cập nhật thông tin cơ bản
            existing.AircraftName = data.AircraftName;
            existing.TailNumber = data.TailNumber;
            existing.Model = data.Model;
            existing.CategoryId = data.CategoryId;
            existing.Capacity = data.Capacity;
            existing.PricePerHour = data.PricePerHour;

            // Status: Cho phép nhận 'Ready', 'Maintenance', 'InFlight'
            existing.Status = data.Status;

            // Cập nhật ảnh Ngoại thất nếu có file mới
            if (data.ExteriorFile != null && data.ExteriorFile.Length > 0)
            {
                var res = await cloudinaryService.UploadImageAsync(data.ExteriorFile, "jet_exteriors");
                if (res?.SecureUrl != null) existing.ExteriorImageUrl = res.SecureUrl.AbsoluteUri;
            }

            // Cập nhật NHIỀU ảnh Nội thất mới (ghi đè hoặc bổ sung tùy logic của ông)
            if (InteriorFiles != null && InteriorFiles.Count > 0)
            {
                var imageUrls = new List<string>();
                foreach (var file in InteriorFiles)
                {
                    var res = await cloudinaryService.UploadImageAsync(file, "jet_interiors");
                    if (res?.SecureUrl != null) imageUrls.Add(res.SecureUrl.AbsoluteUri);
                }
                // Ở đây tui đang để là GHI ĐÈ chuỗi cũ bằng danh sách ảnh mới
                existing.InteriorImageUrl = string.Join(",", imageUrls);
            }

            try
            {
                await db.SaveChangesAsync();
                return Ok(new { message = "Cập nhật thành công!", data = existing });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi lưu Database: {ex.Message}");
            }
        }

        // 5. XÓA CHUYÊN CƠ
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAircraft(int id)
        {
            var aircraft = await db.Aircrafts.FindAsync(id);
            if (aircraft == null) return NotFound();

            db.Aircrafts.Remove(aircraft);
            await db.SaveChangesAsync();

            return NoContent();
        }
    }
}