using Microsoft.AspNetCore.Mvc;
using HelpdeskSystem.Models;
using HelpdeskSystem.Data;

namespace HelpdeskSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TicketsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TicketsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/tickets
        [HttpGet]
        public IActionResult GetTickets()
        {
            return Ok(_context.Tickets.ToList());
        }

        // GET: api/tickets/{id}
        [HttpGet("{id}")]
        public IActionResult GetTicket(int id)
        {
            var ticket = _context.Tickets.Find(id);
            if (ticket == null)
            {
                return NotFound(new { message = "Ticket not found" });
            }
            return Ok(ticket);
        }

        // POST: api/tickets
        [HttpPost]
        public IActionResult CreateTicket(Ticket ticket)
        {
            _context.Tickets.Add(ticket);
            _context.SaveChanges();
            return CreatedAtAction(nameof(GetTicket), new { id = ticket.Id }, ticket);
        }

        // PUT: api/tickets/{id}
        [HttpPut("{id}")]
        public IActionResult UpdateTicket(int id, Ticket updatedTicket)
        {
            var ticket = _context.Tickets.Find(id);
            if (ticket == null)
            {
                return NotFound(new { message = "Ticket not found" });
            }

            ticket.Title = updatedTicket.Title;
            ticket.Description = updatedTicket.Description;
            ticket.Status = updatedTicket.Status;

            _context.SaveChanges();

            return Ok(ticket);
        }

        // DELETE: api/tickets/{id}
        [HttpDelete("{id}")]
        public IActionResult DeleteTicket(int id)
        {
            var ticket = _context.Tickets.Find(id);
            if (ticket == null)
            {
                return NotFound(new { message = "Ticket not found" });
            }

            _context.Tickets.Remove(ticket);
            _context.SaveChanges();

            return NoContent();
        }
    }
}
