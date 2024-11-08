using Microsoft.AspNetCore.Mvc;

using Movie2024.Models;

namespace Movie2024.Models;

public class MoviesAndSeats
{
    public Movies Movies { get; set; }
    public List<Showtimes> Showtimes { get; set; }
    public List<Seats> Seats { get; set; }
    public Theaters Theaters { get; set; }
    public Users Users { get; set; }
    public Orders Orders { get; set; }
    public OrderSeats OrderSeats { get; set; }
    public ShowtimeSeats ShowtimeSeats { get; set; }


}