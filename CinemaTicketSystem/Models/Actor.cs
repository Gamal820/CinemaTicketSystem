
using System.ComponentModel.DataAnnotations;

namespace CinemaTicketSystem.Models
{
    public class Actor
    {
        public int Id { get; set; }  

        [Required, StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Actor Image")]
        public string Img { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Bio { get; set; }   
 
        public List<MovieActor>? MovieActors { get; set; } = new();
    }

}
