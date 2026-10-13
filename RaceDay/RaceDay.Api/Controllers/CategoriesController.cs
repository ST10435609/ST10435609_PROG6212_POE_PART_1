// start of CategoriesController
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Data;
using RaceDay.Api.Dtos;
using RaceDay.Api.Models;
using RaceDay.Api.Security;

namespace RaceDay.Api.Controllers
{
    [ApiController]
    public class CategoriesController : ApiControllerBase
    {
        private readonly RaceDayDbContext _db;

        public CategoriesController(RaceDayDbContext db)
        {
            _db = db;
        }

        // GET /api/events/{eventId}/categories — public, any
        [HttpGet("api/events/{eventId}/categories")]
        public async Task<IActionResult> GetForEvent(int eventId)
        {
            bool eventExists = await _db.Events.AnyAsync(e => e.EventId == eventId);
            if (!eventExists)
            {
                return NotFound("Event not found.");
            }

            var categories = await _db.Categories
                .Where(c => c.EventId == eventId)
                .Select(c => new CategoryResponse
                {
                    CategoryId = c.CategoryId,
                    EventId = c.EventId,
                    CategoryName = c.CategoryName
                })
                .ToListAsync();

            return Ok(categories);
        }

        // POST /api/events/{eventId}/categories — Organiser, owner only
        [HttpPost("api/events/{eventId}/categories")]
        [SessionAuthorize("Organiser")]
        public async Task<IActionResult> Create(int eventId, CategoryRequest request)
        {
            var ev = await _db.Events.FirstOrDefaultAsync(e => e.EventId == eventId);
            if (ev == null)
            {
                return NotFound("Event not found.");
            }

            if (!IsOwnerOf(ev))
            {
                return StatusCode(403);
            }

            var category = new Category
            {
                EventId = eventId,
                CategoryName = request.CategoryName
            };

            _db.Categories.Add(category);
            await _db.SaveChangesAsync();

            var response = new CategoryResponse
            {
                CategoryId = category.CategoryId,
                EventId = category.EventId,
                CategoryName = category.CategoryName
            };

            return StatusCode(201, response);
        }

        // PUT /api/categories/{id} — Organiser, owner only
        [HttpPut("api/categories/{id}")]
        [SessionAuthorize("Organiser")]
        public async Task<IActionResult> Update(int id, CategoryRequest request)
        {
            var category = await _db.Categories
                .Include(c => c.Event)
                .FirstOrDefaultAsync(c => c.CategoryId == id);

            if (category == null)
            {
                return NotFound("Category not found.");
            }

            if (!IsOwnerOf(category.Event))
            {
                return StatusCode(403);
            }

            category.CategoryName = request.CategoryName;
            await _db.SaveChangesAsync();

            var response = new CategoryResponse
            {
                CategoryId = category.CategoryId,
                EventId = category.EventId,
                CategoryName = category.CategoryName
            };

            return Ok(response);
        }

        // DELETE /api/categories/{id} — Organiser, owner only
        [HttpDelete("api/categories/{id}")]
        [SessionAuthorize("Organiser")]
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _db.Categories
                .Include(c => c.Event)
                .FirstOrDefaultAsync(c => c.CategoryId == id);

            if (category == null)
            {
                return NotFound("Category not found.");
            }

            if (!IsOwnerOf(category.Event))
            {
                return StatusCode(403);
            }

            _db.Categories.Remove(category);
            await _db.SaveChangesAsync();

            return NoContent();
        }
    }
}
// end of CategoriesController