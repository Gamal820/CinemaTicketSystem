using System;
using System.ComponentModel.DataAnnotations;

namespace CinemaTicketSystem.Models
{
    public class Promotion
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Promo Code")]
        public string Code { get; set; } = string.Empty;

        [Range(1, 100, ErrorMessage = "Discount must be between 1% and 100%.")]
        [Display(Name = "Discount (%)")]
        public double DiscountPercent { get; set; }

        [Display(Name = "Valid From")]
        public DateTime StartDate { get; set; } = DateTime.Now;

        [Display(Name = "Valid To")]
        public DateTime EndDate { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
