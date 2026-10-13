// start of EnrolmentsController
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Data;
using RaceDay.Api.Dtos;
using RaceDay.Api.Models;
using RaceDay.Api.Security;

namespace RaceDay.Api.Controllers
{
    [ApiController]
    public class EnrolmentsController : ApiControllerBase
    {
        private readonly RaceDayDbContext _db;

        public EnrolmentsController(RaceDayDbContext db)
        {
            _db = db;
        }

        // POST /api/events/{eventId}/enrolments — Participant only
        [HttpPost("api/events/{eventId}/enrolments")]
        [SessionAuthorize("Participant")]
        public async Task<IActionResult> Enrol(int eventId, CreateEnrolmentRequest request)
        {
            var ev = await _db.Events.FirstOrDefaultAsync(e => e.EventId == eventId);
            if (ev == null)
            {
                return NotFound("Event not found.");
            }

            // the chosen category must belong to this specific event
            bool categoryValid = await _db.Categories
                .AnyAsync(c => c.CategoryId == request.CategoryId && c.EventId == eventId);

            if (!categoryValid)
            {
                return BadRequest("Invalid category for this event.");
            }

            bool alreadyEnrolled = await _db.Enrolments
                .AnyAsync(e => e.ParticipantId == CurrentUserId && e.EventId == eventId);

            if (alreadyEnrolled)
            {
                return Conflict("Already enrolled in this event.");
            }

            var enrolment = new Enrolment
            {
                ParticipantId = CurrentUserId,
                EventId = eventId,
                CategoryId = request.CategoryId
            };

            _db.Enrolments.Add(enrolment);
            await _db.SaveChangesAsync();

            var response = await BuildEnrolmentResponse(enrolment.EnrolmentId);
            return StatusCode(201, response);
        }

        // GET /api/users/me/enrolments — Participant only
        [HttpGet("api/users/me/enrolments")]
        [SessionAuthorize("Participant")]
        public async Task<IActionResult> GetMyEnrolments()
        {
            var enrolments = await _db.Enrolments
                .Include(e => e.Event)
                .Include(e => e.Category)
                .Where(e => e.ParticipantId == CurrentUserId)
                .Select(e => new EnrolmentResponse
                {
                    EnrolmentId = e.EnrolmentId,
                    ParticipantId = e.ParticipantId,
                    EventId = e.EventId,
                    EventName = e.Event.Name,
                    CategoryId = e.CategoryId,
                    CategoryName = e.Category.CategoryName,
                    EnrolmentDate = e.EnrolmentDate,
                    EnrolmentStatus = e.EnrolmentStatus
                })
                .ToListAsync();

            return Ok(enrolments);
        }

        // GET /api/events/{eventId}/enrolments — Organiser, owner only
        [HttpGet("api/events/{eventId}/enrolments")]
        [SessionAuthorize("Organiser")]
        public async Task<IActionResult> GetForEvent(int eventId)
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

            var enrolments = await _db.Enrolments
                .Include(e => e.Event)
                .Include(e => e.Category)
                .Where(e => e.EventId == eventId)
                .Select(e => new EnrolmentResponse
                {
                    EnrolmentId = e.EnrolmentId,
                    ParticipantId = e.ParticipantId,
                    EventId = e.EventId,
                    EventName = e.Event.Name,
                    CategoryId = e.CategoryId,
                    CategoryName = e.Category.CategoryName,
                    EnrolmentDate = e.EnrolmentDate,
                    EnrolmentStatus = e.EnrolmentStatus
                })
                .ToListAsync();

            return Ok(enrolments);
        }

        // shared mapping helper used after Enrol(), to avoid repeating the Include/Select pattern
        private async Task<EnrolmentResponse> BuildEnrolmentResponse(int enrolmentId)
        {
            return await _db.Enrolments
                .Include(e => e.Event)
                .Include(e => e.Category)
                .Where(e => e.EnrolmentId == enrolmentId)
                .Select(e => new EnrolmentResponse
                {
                    EnrolmentId = e.EnrolmentId,
                    ParticipantId = e.ParticipantId,
                    EventId = e.EventId,
                    EventName = e.Event.Name,
                    CategoryId = e.CategoryId,
                    CategoryName = e.Category.CategoryName,
                    EnrolmentDate = e.EnrolmentDate,
                    EnrolmentStatus = e.EnrolmentStatus
                })
                .FirstAsync();
        }
    }
}
// end of EnrolmentsController