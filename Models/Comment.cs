using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Helpdesk.Api.Models
{
    public class Comment
    {
        public int Id { get; set; }

        [Required, ForeignKey(nameof(Ticket))]
        public int TicketId { get; set; }

        public Ticket Ticket { get; set; } = default!;

        [Required, MaxLength(120)]
        public string Author { get; set; } = default!;

        [Required, MaxLength(2000)]
        public string Body { get; set; } = default!;

        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    }
}
