
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Data;
using RaceDay.Api.Dtos;
using RaceDay.Api.Models;
using RaceDay.Api.Security;

namespace RaceDay.Api.Controllers
{
    [ApiController]
    [Route("api/events")]
    public class EventsController : ApiControllerBase
    {
        private readonly RaceDayDbContext _db;
        private readonly RaceDay.Api.Services.BlobStorageService _blob_storage_service;

        public EventsController(
            RaceDayDbContext db,
            RaceDay.Api.Services.BlobStorageService blob_storage_service)
        {
            _db = db;
            _blob_storage_service = blob_storage_service;
        }

        // GET /api/events
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var events = await _db.Events
                .Include(e => e.EventType)
                .Select(e => new EventResponse
                {
                    EventId = e.EventId,
                    OrganiserId = e.OrganiserId,
                    Name = e.Name,
                    Description = e.Description,
                    EventDate = e.EventDate,
                    Location = e.Location,
                    Distance = e.Distance,
                    EventType = e.EventType.EventTypeName,
                    BannerImageUrl = e.BannerImageUrl
                })
                .ToListAsync();

            return Ok(events);
        }

        // GET /api/events/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var ev = await _db.Events
                .Include(e => e.EventType)
                .FirstOrDefaultAsync(e => e.EventId == id);

            if (ev == null)
            {
                return NotFound("Event not found.");
            }

            var response = new EventResponse
            {
                EventId = ev.EventId,
                OrganiserId = ev.OrganiserId,
                Name = ev.Name,
                Description = ev.Description,
                EventDate = ev.EventDate,
                Location = ev.Location,
                Distance = ev.Distance,
                EventType = ev.EventType.EventTypeName,
                BannerImageUrl = ev.BannerImageUrl
            };

            return Ok(response);
        }

        // POST /api/events/{id}/banner — Organiser, owner only
        [HttpPost("{id}/banner")]
        [SessionAuthorize("Organiser")]
        public async Task<IActionResult> UploadBanner(int id, IFormFile file)
        {
            var ev = await _db.Events.FirstOrDefaultAsync(e => e.EventId == id);
            if (ev == null)
            {
                return NotFound("Event not found.");
            }

            if (!IsOwnerOf(ev))
            {
                return StatusCode(403);
            }

            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            if (file.Length > 5 * 1024 * 1024)
            {
                return BadRequest("File too large. Maximum size is 5MB.");
            }

            var allowed_types = new[] { "image/jpeg", "image/png", "image/webp" };
            if (!allowed_types.Contains(file.ContentType))
            {
                return BadRequest("Only JPEG, PNG or WEBP images are allowed.");
            }

            var url = await _blob_storage_service.UploadAsync("event-banners", file);
            ev.BannerImageUrl = url;
            await _db.SaveChangesAsync();

            return Ok(new { bannerImageUrl = url });
        }


        // POST /api/events
        [HttpPost]
        [SessionAuthorize("Organiser")]
        public async Task<IActionResult> Create(CreateEventRequest request)
        {
            var eventType = await _db.EventTypes
                .FirstOrDefaultAsync(t =>
                    t.EventTypeName == request.EventType);

            if (eventType == null)
            {
                return BadRequest("Invalid event type.");
            }

            var ev = new Event
            {
                OrganiserId = CurrentUserId,
                EventTypeId = eventType.EventTypeId,
                Name = request.Name,
                Description = request.Description,
                EventDate = request.EventDate,
                Location = request.Location,
                Distance = request.Distance,
                BannerImageUrl = request.BannerImageUrl
            };

            _db.Events.Add(ev);

            await _db.SaveChangesAsync();

            var response = new EventResponse
            {
                EventId = ev.EventId,
                OrganiserId = ev.OrganiserId,
                Name = ev.Name,
                Description = ev.Description,
                EventDate = ev.EventDate,
                Location = ev.Location,
                Distance = ev.Distance,
                EventType = eventType.EventTypeName,
                BannerImageUrl = ev.BannerImageUrl
            };

            return StatusCode(201, response);
        }

        // PUT /api/events/{id}
        [HttpPut("{id}")]
        [SessionAuthorize("Organiser")]
        public async Task<IActionResult> Update(
            int id,
            UpdateEventRequest request)
        {
            var ev = await _db.Events
                .Include(e => e.EventType)
                .FirstOrDefaultAsync(e => e.EventId == id);

            if (ev == null)
            {
                return NotFound("Event not found.");
            }

            if (!IsOwnerOf(ev))
            {
                return StatusCode(403);
            }

            var eventType = await _db.EventTypes
                .FirstOrDefaultAsync(t =>
                    t.EventTypeName == request.EventType);

            if (eventType == null)
            {
                return BadRequest("Invalid event type.");
            }

            ev.EventTypeId = eventType.EventTypeId;
            ev.Name = request.Name;
            ev.Description = request.Description;
            ev.EventDate = request.EventDate;
            ev.Location = request.Location;
            ev.Distance = request.Distance;
            ev.BannerImageUrl = request.BannerImageUrl;

            await _db.SaveChangesAsync();

            var response = new EventResponse
            {
                EventId = ev.EventId,
                OrganiserId = ev.OrganiserId,
                Name = ev.Name,
                Description = ev.Description,
                EventDate = ev.EventDate,
                Location = ev.Location,
                Distance = ev.Distance,
                EventType = eventType.EventTypeName,
                BannerImageUrl = ev.BannerImageUrl
            };

            return Ok(response);
        }

        // DELETE /api/events/{id} — Organiser, owner only
        [HttpDelete("{id}")]
        [SessionAuthorize("Organiser")]
        public async Task<IActionResult> Delete(int id)
        {
            var ev = await _db.Events.FirstOrDefaultAsync(e => e.EventId == id);

            if (ev == null)
            {
                return NotFound("Event not found.");
            }

            if (!IsOwnerOf(ev))
            {
                return StatusCode(403);
            }

            _db.Events.Remove(ev);

            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                // Restrict foreign keys block this delete while categories or
                // enrolments still reference the event
                return Conflict("Cannot delete this event because it still has categories or enrolments linked to it.");
            }

            return NoContent();
        }
    }
}
