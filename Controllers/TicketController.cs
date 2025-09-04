using Helpdesk.Api.Data;
using Helpdesk.Api.DTOs;
using Helpdesk.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Helpdesk.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TicketsController : ControllerBase
    {
        private readonly AppDbContext _db;

        public TicketsController(AppDbContext db) => _db = db;

        // GET: api/tickets
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TicketListItemDto>>> GetAll()
        {
            var items = await _db.Tickets
                .Select(t => new TicketListItemDto
                {
                    Id = t.Id,
                    Title = t.Title,
                    Priority = t.Priority,
                    Status = t.Status,
                    AssignedTo = t.AssignedTo,
                    CreatedAtUtc = t.CreatedAtUtc,
                    CommentCount = t.Comments.Count
                })
                .OrderByDescending(t => t.CreatedAtUtc)
                .ToListAsync();

            return Ok(items);
        }

        // GET: api/tickets/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<TicketDetailDto>> GetById(int id)
        {
            var t = await _db.Tickets
                .Include(x => x.Comments)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (t == null) return NotFound();

            var dto = new TicketDetailDto
            {
                Id = t.Id,
                Title = t.Title,
                Description = t.Description,
                Priority = t.Priority,
                Status = t.Status,
                Reporter = t.Reporter,
                AssignedTo = t.AssignedTo,
                CreatedAtUtc = t.CreatedAtUtc,
                UpdatedAtUtc = t.UpdatedAtUtc,
                DueDateUtc = t.DueDateUtc,
                ClosedAtUtc = t.ClosedAtUtc,
                Comments = t.Comments
                    .OrderByDescending(c => c.CreatedAtUtc)
                    .Select(c => new CommentDto
                    {
                        Id = c.Id,
                        Author = c.Author,
                        Body = c.Body,
                        CreatedAtUtc = c.CreatedAtUtc
                    })
                    .ToList()
            };

            return Ok(dto);
        }

        // POST: api/tickets
        [HttpPost]
        public async Task<ActionResult<TicketDetailDto>> Create([FromBody] TicketCreateDto input)
        {
            // ModelState is auto-validated by [ApiController]
            var entity = new Ticket
            {
                Title = input.Title,
                Description = input.Description,
                Priority = input.Priority,
                Reporter = input.Reporter,
                DueDateUtc = input.DueDateUtc
            };

            _db.Tickets.Add(entity);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = entity.Id }, new TicketDetailDto
            {
                Id = entity.Id,
                Title = entity.Title,
                Description = entity.Description,
                Priority = entity.Priority,
                Status = entity.Status,
                Reporter = entity.Reporter,
                AssignedTo = entity.AssignedTo,
                CreatedAtUtc = entity.CreatedAtUtc,
                UpdatedAtUtc = entity.UpdatedAtUtc,
                DueDateUtc = entity.DueDateUtc,
                ClosedAtUtc = entity.ClosedAtUtc,
                Comments = new List<CommentDto>()
            });
        }

        // PUT: api/tickets/5  (partial-style update)
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] TicketUpdateDto input)
        {
            var t = await _db.Tickets.FindAsync(id);
            if (t == null) return NotFound();

            if (input.Title is not null) t.Title = input.Title;
            if (input.Description is not null) t.Description = input.Description;
            if (input.Priority.HasValue) t.Priority = input.Priority.Value;
            if (input.Status.HasValue) t.Status = input.Status.Value;
            if (input.AssignedTo is not null) t.AssignedTo = input.AssignedTo;
            if (input.DueDateUtc.HasValue) t.DueDateUtc = input.DueDateUtc.Value;

            await _db.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/tickets/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var t = await _db.Tickets.FindAsync(id);
            if (t == null) return NotFound();

            _db.Tickets.Remove(t);
            await _db.SaveChangesAsync();
            return NoContent();
        }

        // POST: api/tickets/5/comments
        [HttpPost("{id:int}/comments")]
        public async Task<ActionResult<CommentDto>> AddComment(int id, [FromBody] CommentCreateDto input)
        {
            var t = await _db.Tickets.FindAsync(id);
            if (t == null) return NotFound();

            var c = new Comment
            {
                TicketId = id,
                Author = input.Author,
                Body = input.Body
            };

            _db.Comments.Add(c);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id }, new CommentDto
            {
                Id = c.Id, Author = c.Author, Body = c.Body, CreatedAtUtc = c.CreatedAtUtc
            });
        }

        // POST: api/tickets/5/transition?status=InProgress
        [HttpPost("{id:int}/transition")]
        public async Task<IActionResult> Transition(int id, [FromQuery] TicketStatus status)
        {
            var t = await _db.Tickets.FindAsync(id);
            if (t == null) return NotFound();

            // naive rules: Open -> InProgress -> Resolved -> Closed
            bool ok = t.Status switch
            {
                TicketStatus.Open when status is TicketStatus.InProgress => true,
                TicketStatus.InProgress when status is TicketStatus.Resolved => true,
                TicketStatus.Resolved when status is TicketStatus.Closed => true,
                _ => false
            };

            if (!ok) return BadRequest("Invalid status transition.");

            t.Status = status;
            await _db.SaveChangesAsync();
            return NoContent();
        }

        // POST: api/tickets/5/assign?assignee=it.support@company.com
        [HttpPost("{id:int}/assign")]
        public async Task<IActionResult> Assign(int id, [FromQuery] string assignee)
        {
            if (string.IsNullOrWhiteSpace(assignee)) return BadRequest("Assignee required.");
            var t = await _db.Tickets.FindAsync(id);
            if (t == null) return NotFound();

            t.AssignedTo = assignee.Trim();
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}
