using CinemaTicketSystem.Models;
using CinemaTicketSystem.DataAccess;
using System.Collections.Generic;

namespace CinemaTicketSystem.Repositories
{

    public class MovieSubImageRepository : Repository<MovieSubImage>
    {
        public MovieSubImageRepository(ApplicationDBContext context) : base(context)
        {
        }
    }
}