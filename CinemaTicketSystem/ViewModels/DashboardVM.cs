namespace CinemaTicketSystem.ViewModels
{
    public class DashboardVM
    {
        public int TotalMovies { get; set; }
        public int TotalActors { get; set; }
        public int TotalCategories { get; set; }
        public int TotalCinemas { get; set; }
        public decimal HighestPrice { get; set; }
        public decimal AveragePrice { get; set; }


        public int TotalOrders { get; set; }             
        public Dictionary<string, double> MarketShare { get; set; } = new();  
    }
}

