using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace CinemaTicketSystem.Models
{
    public class Movie
    {
        public int Id { get; set; }

        [Required, StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        [Range(0, 500)]
        [Precision(10, 2)]
        public decimal Price { get; set; }

        public bool Status { get; set; } = true;

        [Display(Name = "Release Date")]
        public DateTime DateTime { get; set; }

        [Display(Name = "Main Image")]
        public string MainImg { get; set; } = string.Empty;
  
        // العلاقات
        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;

        public int CinemaId { get; set; }
        public Cinema Cinema { get; set; } = null!;

        // الربط مع الممثلين
        public List<MovieActor> MovieActors { get; set; } = new();
        public List<MovieSubImage>? MovieSubImages { get; set; } = new();


    }

}
