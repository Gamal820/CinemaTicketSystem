namespace CinemaTicketSystem.ViewModels
{
    public class ManageRoleVM
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string? CurrentRole { get; set; }
        public string? NewRole { get; set; }
        public List<string> AllRoles { get; set; } = new();
    }
}
