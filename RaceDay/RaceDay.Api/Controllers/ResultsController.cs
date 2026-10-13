// start of ResultsController
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Data;
using RaceDay.Api.Dtos;
using RaceDay.Api.Models;
using RaceDay.Api.Security;

namespace RaceDay.Api.Controllers
{
    [ApiController]
    public class ResultsController : ApiControllerBase
    {
        private readonly RaceDayDbContext _db;

        public ResultsController(RaceDayDbContext db)
        {
            _db = db;
        }

        // POST /api/enrolments/{enrolmentId}/result — Organiser, owner of the event only
        [HttpPost("api/enrolments/{enrolmentId}/result")]
        [SessionAuthorize("Organiser")]
        public async Task<IActionResult> CaptureResult(int enrolmentId, CreateResultRequest request)
        {
            var enrolment = await _db.Enrolments
                .Include(e => e.Event)
                .Include(e => e.Result)
                .FirstOrDefaultAsync(e => e.EnrolmentId == enrolmentId);

            if (enrolment == null)
            {
                return NotFound("Enrolment not found.");
            }

            if (!IsOwnerOf(enrolment.Event))
            {
                return StatusCode(403);
            }

            if (enrolment.Result != null)
            {
                return Conflict("A result has already been captured for this enrolment.");
            }

            var result = new Result
            {
                EnrolmentId = enrolmentId,
                FinishTime = request.FinishTime,
                FinishingPosition = request.FinishingPosition
            };

            _db.Results.Add(result);
            await _db.SaveChangesAsync();

            var response = new ResultResponse
            {
                ResultId = result.ResultId,
                EnrolmentId = result.EnrolmentId,
                EventName = enrolment.Event.Name,
                CategoryName = (await _db.Categories.FindAsync(enrolment.CategoryId))!.CategoryName,
                FinishTime = result.FinishTime,
                FinishingPosition = result.FinishingPosition
            };

            return StatusCode(201, response);
        }

        // GET /api/users/me/results — Participant only
        [HttpGet("api/users/me/results")]
        [SessionAuthorize("Participant")]
        public async Task<IActionResult> GetMyResults()
        {
            var results = await _db.Results
                .Include(r => r.Enrolment).ThenInclude(e => e.Event)
                .Include(r => r.Enrolment).ThenInclude(e => e.Category)
                .Where(r => r.Enrolment.ParticipantId == CurrentUserId)
                .Select(r => new ResultResponse
                {
                    ResultId = r.ResultId,
                    EnrolmentId = r.EnrolmentId,
                    EventName = r.Enrolment.Event.Name,
                    CategoryName = r.Enrolment.Category.CategoryName,
                    FinishTime = r.FinishTime,
                    FinishingPosition = r.FinishingPosition
                })
                .ToListAsync();

            return Ok(results);
        }
    }
}
// end of ResultsController