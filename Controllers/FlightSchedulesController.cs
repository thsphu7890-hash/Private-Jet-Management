using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using JetAdminSystem.Models;
using JetAdminSystem.Data;

namespace JetAdminSystem.Controllers;

[Route("api/[controller]")]
[ApiController]
// Sử dụng Primary Constructor với JetAdminDbContext cho khớp hoàn toàn với Program.cs
public class FlightSchedulesController(JetAdminDbContext context) : ControllerBase
{
    // 1. GET: api/FlightSchedules - Lấy danh sách lịch trình kèm thông tin sân bay
    [HttpGet]
    public async Task<ActionResult<IEnumerable<FlightSchedule>>> GetFlightSchedules()
    {
        try
        {
            // Chỉ định rõ Set<FlightSchedule> để tránh lỗi gạch đỏ ở DepartureTime
            var query = context.Set<FlightSchedule>()
                .Include(s => s.DepartureAirport)
                .Include(s => s.ArrivalAirport)
                .Include(s => s.Aircraft)
                .AsNoTracking();

            // Sắp xếp theo thời gian khởi hành mới nhất lên đầu
            return await query.OrderByDescending(s => s.DepartureTime).ToListAsync();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Lỗi server khi tải lịch trình: {ex.Message}");
        }
    }

    // 2. GET: api/FlightSchedules/5 - Lấy chi tiết một lịch trình
    [HttpGet("{id}")]
    public async Task<ActionResult<FlightSchedule>> GetFlightSchedule(int id)
    {
        var schedule = await context.Set<FlightSchedule>()
            .Include(s => s.DepartureAirport)
            .Include(s => s.ArrivalAirport)
            .Include(s => s.Aircraft)
            .FirstOrDefaultAsync(s => s.ScheduleId == id);

        if (schedule == null) return NotFound(new { message = "Không tìm thấy lịch trình!" });

        return Ok(schedule);
    }

    // 3. POST: api/FlightSchedules - Tạo lịch trình mới
    [HttpPost]
    public async Task<ActionResult<FlightSchedule>> PostFlightSchedule(FlightSchedule flightSchedule)
    {
        try
        {
            context.Set<FlightSchedule>().Add(flightSchedule);
            await context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetFlightSchedule), new { id = flightSchedule.ScheduleId }, flightSchedule);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = "Lỗi khi lưu lịch trình", detail = ex.Message });
        }
    }

    // 4. PUT: api/FlightSchedules/5 - Cập nhật lịch trình
    [HttpPut("{id}")]
    public async Task<IActionResult> PutFlightSchedule(int id, FlightSchedule flightSchedule)
    {
        if (id != flightSchedule.ScheduleId) return BadRequest();

        context.Entry(flightSchedule).State = EntityState.Modified;

        try
        {
            await context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!context.Set<FlightSchedule>().Any(e => e.ScheduleId == id)) return NotFound();
            throw;
        }

        return NoContent();
    }

    // 5. DELETE: api/FlightSchedules/5 - Xóa lịch trình
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteFlightSchedule(int id)
    {
        var schedule = await context.Set<FlightSchedule>().FindAsync(id);
        if (schedule == null) return NotFound();

        context.Set<FlightSchedule>().Remove(schedule);
        await context.SaveChangesAsync();

        return NoContent();
    }
}