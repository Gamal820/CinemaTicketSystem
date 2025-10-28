﻿using System.ComponentModel.DataAnnotations;

namespace CinemaTicketSystem.ViewModels
{
    public class ForgetPasswordVM
    {
        public int Id { get; set; }
        [Required]
        public string UserNameOREmail { get; set; } = string.Empty;
    }
}
