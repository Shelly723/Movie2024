using System;
using System.Collections.Generic;

namespace Movie2024.Models;

public partial class ShowtimeSeats
{
    public int ShowtimeSeatID { get; set; }

    public int ShowtimeID { get; set; }

    public int SeatID { get; set; } 

    public string IsAvailable { get; set; } = null!;

    public Showtimes Showtimes { get; set; }
    public Seats Seats { get; set; }
}
