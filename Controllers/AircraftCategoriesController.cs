using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using JetAdminSystem.Data;
using JetAdminSystem.Models;

namespace JetAdminSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AircraftCategoriesController : ControllerBase
    {
        private readonly JetAdminDbContext _context;

        public AircraftCategoriesController(JetAdminDbContext context)
        {
            _context = context;
        }

        // GET: api/AircraftCategories
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AircraftCategory>>> GetAircraftCategories()
        {
            return await _context.AircraftCategories.ToListAsync();
        }

        // GET: api/AircraftCategories/5
        [HttpGet("{id}")]
        public async Task<ActionResult<AircraftCategory>> GetAircraftCategory(int id)
        {
            var aircraftCategory = await _context.AircraftCategories.FindAsync(id);

            if (aircraftCategory == null)
            {
                return NotFound();
            }

            return aircraftCategory;
        }

        // PUT: api/AircraftCategories/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAircraftCategory(int id, AircraftCategory aircraftCategory)
        {
            if (id != aircraftCategory.CategoryId)
            {
                return BadRequest();
            }

            _context.Entry(aircraftCategory).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AircraftCategoryExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/AircraftCategories
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<AircraftCategory>> PostAircraftCategory(AircraftCategory aircraftCategory)
        {
            _context.AircraftCategories.Add(aircraftCategory);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetAircraftCategory", new { id = aircraftCategory.CategoryId }, aircraftCategory);
        }

        // DELETE: api/AircraftCategories/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAircraftCategory(int id)
        {
            var aircraftCategory = await _context.AircraftCategories.FindAsync(id);
            if (aircraftCategory == null)
            {
                return NotFound();
            }

            _context.AircraftCategories.Remove(aircraftCategory);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool AircraftCategoryExists(int id)
        {
            return _context.AircraftCategories.Any(e => e.CategoryId == id);
        }
    }
}
