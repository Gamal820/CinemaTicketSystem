using CinemaTicketSystem.Models;

namespace CinemaTicketSystem.ViewModels
{
    public class MovieVM
    {
         
        public Movie? Movie { get; set; }
        public IEnumerable<Category>? Categories { get; set; }
        public IEnumerable<Cinema>? Cinemas { get; set; }
        public IEnumerable<Actor>? Actors { get; set; }
        public List<int>? SelectedActors { get; set; }
        public List<string>? DeletedSubImages { get; set; }


        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public string CinemaName { get; set; } = string.Empty;
        public string MainImg { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public bool Status { get; set; }
    }
}

