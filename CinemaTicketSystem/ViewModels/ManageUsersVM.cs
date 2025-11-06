using CinemaTicketSystem.Models;

namespace CinemaTicketSystem.ViewModels
{
    public class ManageUsersVM
    {
        public List<ApplicationUser> Users { get; set; } = new ();
        public List<string> Roles { get; set; } = new();
        public string? SearchTerm { get; set; }
        public string? SelectedRole { get; set; }
    }
}
