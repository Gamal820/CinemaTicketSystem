using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace CinemaTicketSystem.ViewModels.CinemaVMs
{
    public class CreateCinemaVM
    {
        [Required(ErrorMessage = "Cinema Name is required")]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Cinema Image")]
        public IFormFile? Img { get; set; }
    }
}
