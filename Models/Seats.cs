using System;
using System.Collections.Generic;

namespace Movie2024.Models;

public partial class Seats
{
    public int SeatID { get; set; }

    public int TheaterID { get; set; }

    public string SeatNumber { get; set; } = null!;

    public string IsAvailable { get; set; } = null!;
}
