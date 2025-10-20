using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace CinemaTicketSystem.Models
{
    public class Cinema
    {
        public int CinemaId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        public string? Img { get; set; }

        [ValidateNever]
        public ICollection<Movie> Movies { get; set; }
    }
}
