using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using JetAdminSystem.Data;
using JetAdminSystem.Models;
using JetAdminSystem.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JetAdminSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BillingsController : ControllerBase
    {
        // 1. GET: api/Billings - Danh sách hóa đơn
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Billing>>> GetBillings()
        {
            var context = HttpContext.RequestServices.GetRequiredService<JetAdminDbContext>();
            return await context.Billings
                .Include(b => b.Booking)
                    .ThenInclude(bk => bk.Passenger)
                .OrderByDescending(b => b.CreatedDate)
                .AsNoTracking()
                .ToListAsync();
        }

        // 2. POST: api/Billings - TẠO HÓA ĐƠN (Cho phép tạo nhiều cái cho 1 Booking)
        [HttpPost]
        public async Task<ActionResult<Billing>> PostBilling([FromForm] Billing billing, IFormFile? file)
        {
            var context = HttpContext.RequestServices.GetRequiredService<JetAdminDbContext>();
            var cloudinary = HttpContext.RequestServices.GetRequiredService<ICloudinaryService>();

            try
            {
                // ĐÃ XÓA đoạn check existingBilling để Phú có thể tạo nhiều hóa đơn tùy ý

                if (file != null)
                {
                    var result = await cloudinary.UploadImageAsync(file, "jet_admin_invoices");
                    if (result != null)
                    {
                        billing.InvoiceImageUrl = result.SecureUrl.ToString();
                    }
                }

                // Thiết lập thông tin cơ bản
                billing.CreatedDate = DateTime.Now;
                billing.PaymentStatus = billing.PaymentStatus ?? "Unpaid";

                // Tránh lỗi EF cố gắng insert lại Booking đã tồn tại
                // Chúng ta chỉ cần BookingId để định danh Foreign Key
                billing.Booking = null;

                context.Billings.Add(billing);
                await context.SaveChangesAsync();

                return Ok(billing);
            }
            catch (Exception ex)
            {
                // Trả về lỗi chi tiết từ SQL (Ví dụ: Sai kiểu dữ liệu, vượt quá độ dài ký tự...)
                var innerMsg = ex.InnerException?.Message ?? ex.Message;
                return StatusCode(500, new { message = "Lỗi hệ thống: " + innerMsg });
            }
        }

        // 3. POST: api/Billings/{id}/confirm - XÁC NHẬN THANH TOÁN
        [HttpPost("{id}/confirm")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> ConfirmPayment(int id, IFormFile? file, [FromForm] string? method)
        {
            var context = HttpContext.RequestServices.GetRequiredService<JetAdminDbContext>();
            var cloudinary = HttpContext.RequestServices.GetRequiredService<ICloudinaryService>();

            var billing = await context.Billings
                .Include(b => b.Booking)
                .FirstOrDefaultAsync(b => b.BillingId == id);

            if (billing == null) return NotFound(new { message = "Không tìm thấy hóa đơn" });

            if (file != null)
            {
                var result = await cloudinary.UploadImageAsync(file, "jet_admin_invoices");
                if (result != null)
                {
                    billing.InvoiceImageUrl = result.SecureUrl.ToString();
                }
            }

            billing.PaymentStatus = "Paid";
            billing.PaymentDate = DateTime.Now;
            billing.PaymentMethod = method ?? "Bank Transfer";

            // Khi một hóa đơn được xác nhận, cập nhật trạng thái Booking liên quan
            if (billing.Booking != null)
            {
                billing.Booking.Status = "Confirmed";
            }

            await context.SaveChangesAsync();
            return Ok(new { message = "Xác nhận thành công!", url = billing.InvoiceImageUrl });
        }

        // 4. DELETE: api/Billings/{id} - Xóa hóa đơn
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBilling(int id)
        {
            var context = HttpContext.RequestServices.GetRequiredService<JetAdminDbContext>();
            var billing = await context.Billings.FindAsync(id);
            if (billing == null) return NotFound();

            context.Billings.Remove(billing);
            await context.SaveChangesAsync();
            return Ok(new { message = "Đã xóa hóa đơn" });
        }
    }
}