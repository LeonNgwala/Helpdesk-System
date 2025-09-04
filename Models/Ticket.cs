using System.ComponentModel.DataAnnotations;

namespace Helpdesk.Api.Models
{
    public class Ticket
    {
        public int Id { get; set; }                         // PK

        [Required, MaxLength(120)]
        public string Title { get; set; } = default!;

        [Required, MaxLength(4000)]
        public string Description { get; set; } = default!;

        [Required]
        public TicketPriority Priority { get; set; } = TicketPriority.Medium;

        [Required]
        public TicketStatus Status { get; set; } = TicketStatus.Open;

        [MaxLength(120)]
        public string? Reporter { get; set; }               // who raised it

        [MaxLength(120)]
        public string? AssignedTo { get; set; }             // assignee (email/name for now)

        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
        public DateTime? DueDateUtc { get; set; }
        public DateTime? ClosedAtUtc { get; set; }

        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    }
}
