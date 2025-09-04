using System.ComponentModel.DataAnnotations;
using Helpdesk.Api.Models;

namespace Helpdesk.Api.DTOs
{
    public class TicketCreateDto
    {
        [Required, MaxLength(120)]
        public string Title { get; set; } = default!;

        [Required, MaxLength(4000)]
        public string Description { get; set; } = default!;

        [Required]
        public TicketPriority Priority { get; set; } = TicketPriority.Medium;

        [MaxLength(120)]
        public string? Reporter { get; set; }

        public DateTime? DueDateUtc { get; set; }
    }

    public class TicketUpdateDto
    {
        [MaxLength(120)]
        public string? Title { get; set; }

        [MaxLength(4000)]
        public string? Description { get; set; }

        public TicketPriority? Priority { get; set; }

        public TicketStatus? Status { get; set; }

        [MaxLength(120)]
        public string? AssignedTo { get; set; }

        public DateTime? DueDateUtc { get; set; }
    }

    public class TicketListItemDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = default!;
        public TicketPriority Priority { get; set; }
        public TicketStatus Status { get; set; }
        public string? AssignedTo { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public int CommentCount { get; set; }
    }

    public class TicketDetailDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = default!;
        public string Description { get; set; } = default!;
        public TicketPriority Priority { get; set; }
        public TicketStatus Status { get; set; }
        public string? Reporter { get; set; }
        public string? AssignedTo { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public DateTime UpdatedAtUtc { get; set; }
        public DateTime? DueDateUtc { get; set; }
        public DateTime? ClosedAtUtc { get; set; }
        public List<CommentDto> Comments { get; set; } = new();
    }

    public class CommentCreateDto
    {
        [Required, MaxLength(120)]
        public string Author { get; set; } = default!;

        [Required, MaxLength(2000)]
        public string Body { get; set; } = default!;
    }

    public class CommentDto
    {
        public int Id { get; set; }
        public string Author { get; set; } = default!;
        public string Body { get; set; } = default!;
        public DateTime CreatedAtUtc { get; set; }
    }
}
