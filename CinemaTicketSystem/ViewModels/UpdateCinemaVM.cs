using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace CinemaTicketSystem.ViewModels.CinemaVMs
{
    public class UpdateCinemaVM
    {
        public int CinemaId { get; set; }

        [Required(ErrorMessage = "Cinema Name is required")]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Current Image")]
        public string? Img { get; set; }

        [Display(Name = "New Image")]
        public IFormFile? NewImg { get; set; }
    }
}
