using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using JetAdminSystem.Data;
using JetAdminSystem.Models;

namespace JetAdminSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BrokersController : ControllerBase
    {
        private readonly JetAdminDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public BrokersController(JetAdminDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        // 1. GET: api/Brokers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Broker>>> GetBrokers()
        {
            return await _context.Brokers.ToListAsync();
        }

        // 2. GET: api/Brokers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Broker>> GetBroker(int id)
        {
            var broker = await _context.Brokers.FindAsync(id);
            if (broker == null) return NotFound();
            return Ok(broker);
        }

        // 3. POST: api/Brokers (Thêm mới từ Form + Upload ảnh)
        [HttpPost]
        public async Task<ActionResult<Broker>> PostBroker([FromForm] Broker broker, IFormFile? imageFile)
        {
            try
            {
                if (imageFile != null && imageFile.Length > 0)
                {
                    broker.AvatarUrl = await SaveImageToFolder(imageFile);
                }

                _context.Brokers.Add(broker);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetBroker), new { id = broker.BrokerId }, broker);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi khi lưu dữ liệu: {ex.Message}");
            }
        }

        // 4. PUT: api/Brokers/5 (Cập nhật)
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBroker(int id, [FromForm] Broker broker, IFormFile? imageFile)
        {
            if (id != broker.BrokerId) return BadRequest(new { message = "ID không khớp." });

            try
            {
                if (imageFile != null && imageFile.Length > 0)
                {
                    broker.AvatarUrl = await SaveImageToFolder(imageFile);
                }

                _context.Entry(broker).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BrokerExists(id)) return NotFound();
                else throw;
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi khi cập nhật: {ex.Message}");
            }

            return NoContent();
        }

        // 5. DELETE: api/Brokers/5 (Fix gạch đỏ và kiểm tra ràng buộc)
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBroker(int id)
        {
            var broker = await _context.Brokers.FindAsync(id);
            if (broker == null) return NotFound();

            // Kiểm tra ràng buộc với bảng Bookings
            var hasBookings = await _context.Bookings.AnyAsync(b => b.BrokerId == id);
            if (hasBookings)
            {
                return BadRequest(new { message = "Không thể xóa vì đối tác đang có đơn hàng." });
            }

            _context.Brokers.Remove(broker);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Xóa đối tác thành công." });
        }

        // --- HÀM HỖ TRỢ LƯU FILE VÀO FOLDER ---
        private async Task<string> SaveImageToFolder(IFormFile file)
        {
            // Đảm bảo WebRootPath không null (đặc biệt khi chạy trên một số môi trường dev)
            string rootPath = _environment.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            string uploadsFolder = Path.Combine(rootPath, "uploads");

            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            // Tạo tên file duy nhất tránh trùng lặp trong folder
            string uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            return uniqueFileName;
        }

        private bool BrokerExists(int id) => _context.Brokers.Any(e => e.BrokerId == id);
    }
}