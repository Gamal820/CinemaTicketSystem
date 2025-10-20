using System.ComponentModel.DataAnnotations;

namespace CinemaTicketSystem.Models
{
    public class MovieSubImage
    {
        public int Id { get; set; }

        [Required]
        public int MovieId { get; set; }

        [Required]
        public string? Img { get; set; } = string.Empty;

        public Movie Movie { get; set; } = null!;
    }
}
