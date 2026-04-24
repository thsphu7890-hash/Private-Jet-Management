using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using JetAdminSystem.Data;
using JetAdminSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JetAdminSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingsController : ControllerBase
    {
        private readonly JetAdminDbContext _context;

        public BookingsController(JetAdminDbContext context)
        {
            _context = context;
        }

        // 1. GET: api/Bookings
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Booking>>> GetBookings()
        {
            // Include đầy đủ để hiển thị ở danh sách Admin
            return await _context.Bookings
                .Include(b => b.Passenger)
                .Include(b => b.Broker)
                .Include(b => b.Billing)
                .Include(b => b.Schedule)
                    .ThenInclude(s => s.DepartureAirport)
                .Include(b => b.Schedule)
                    .ThenInclude(s => s.ArrivalAirport)
                .OrderByDescending(b => b.BookingId)
                .AsNoTracking()
                .ToListAsync();
        }

        // 2. GET: api/Bookings/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Booking>> GetBooking(int id)
        {
            var booking = await _context.Bookings
                .Include(b => b.Passenger)
                .Include(b => b.Broker)
                .Include(b => b.Billing)
                .Include(b => b.Schedule)
                    .ThenInclude(s => s.Aircraft)
                .Include(b => b.Schedule)
                    .ThenInclude(s => s.DepartureAirport)
                .Include(b => b.Schedule)
                    .ThenInclude(s => s.ArrivalAirport)
                .FirstOrDefaultAsync(b => b.BookingId == id);

            if (booking == null) return NotFound(new { message = "Không tìm thấy đơn đặt chỗ!" });

            return booking;
        }

        // 3. POST: api/Bookings - Tạo đơn kèm hóa đơn tự động (Hệ thống nội bộ)
        [HttpPost]
        public async Task<ActionResult<Booking>> PostBooking(Booking booking)
        {
            // Bắt đầu Transaction đảm bảo không bị rác dữ liệu nếu một trong hai bảng lỗi
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // 1. Kiểm tra lịch trình và lấy thông tin máy bay
                var schedule = await _context.FlightSchedules
                    .Include(s => s.Aircraft)
                    .FirstOrDefaultAsync(s => s.ScheduleId == booking.ScheduleId);

                if (schedule == null)
                    return BadRequest(new { message = "Lịch trình không tồn tại." });

                // 2. Tính tiền dựa trên số giờ bay (Business Logic máy bay tư nhân)
                var duration = schedule.ArrivalTime - schedule.DepartureTime;
                decimal totalHours = (decimal)duration.TotalHours;

                if (totalHours > 0 && schedule.Aircraft != null)
                {
                    // Lưu giá trị vào Booking (Tiền gốc)
                    booking.TotalAmount = totalHours * schedule.Aircraft.PricePerHour;
                }

                booking.BookingDate = DateTime.Now;
                booking.Status = "Pending"; // Mặc định đơn mới là chờ thanh toán

                _context.Bookings.Add(booking);
                await _context.SaveChangesAsync(); // Lưu để sinh BookingId cho Billing

                // 3. Tính toán hoa hồng môi giới (Broker)
                decimal commission = 0;
                if (booking.BrokerId.HasValue)
                {
                    var broker = await _context.Brokers.FindAsync(booking.BrokerId);
                    if (broker != null)
                    {
                        commission = booking.TotalAmount * (broker.CommissionRate / 100);
                    }
                }

                // 4. Tự động tạo Billing khớp với Model GrandTotal mới
                var billing = new Billing
                {
                    BookingId = booking.BookingId,
                    SubTotal = booking.TotalAmount,           // Tiền gốc
                    TaxAmount = booking.TotalAmount * 0.1m,    // Thuế VAT 10%
                    GrandTotal = booking.TotalAmount * 1.1m,   // Tổng cộng sau thuế
                    BrokerCommission = commission,             // Chiết khấu cho môi giới
                    PaymentMethod = "Bank Transfer",           // Mặc định
                    PaymentStatus = "Unpaid",                  // Chờ đối soát
                    CreatedDate = DateTime.Now
                };

                _context.Billings.Add(billing);
                await _context.SaveChangesAsync();

                // Lưu tất cả thay đổi
                await transaction.CommitAsync();

                return CreatedAtAction(nameof(GetBooking), new { id = booking.BookingId }, booking);
            }
            catch (Exception ex)
            {
                // Hoàn tác nếu có lỗi
                await transaction.RollbackAsync();
                return StatusCode(500, new { message = "Lỗi tạo đơn: " + (ex.InnerException?.Message ?? ex.Message) });
            }
        }

        // 4. PUT: api/Bookings/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBooking(int id, Booking booking)
        {
            if (id != booking.BookingId) return BadRequest();

            _context.Entry(booking).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Bookings.Any(e => e.BookingId == id)) return NotFound();
                throw;
            }

            return NoContent();
        }

        // 5. DELETE: api/Bookings/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBooking(int id)
        {
            var booking = await _context.Bookings
                .Include(b => b.Billing)
                .FirstOrDefaultAsync(b => b.BookingId == id);

            if (booking == null) return NotFound();

            // Xóa cả đơn và hóa đơn liên quan
            if (booking.Billing != null)
            {
                _context.Billings.Remove(booking.Billing);
            }

            _context.Bookings.Remove(booking);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Đã xóa đơn đặt chỗ và dữ liệu thanh toán liên quan." });
        }
    }
}