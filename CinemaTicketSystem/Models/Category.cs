using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace CinemaTicketSystem.Models
{
    public class Category
    {
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Category Name is required")]
        [StringLength(100)]
        public string Name { get; set; }=string.Empty;
       
        [ValidateNever]
        public ICollection<Movie> Movies { get; set; }
    }
}
