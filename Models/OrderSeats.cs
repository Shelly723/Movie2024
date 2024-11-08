using System;
using System.Collections.Generic;

namespace Movie2024.Models;

public partial class OrderSeats
{
    public int OrderSeatID { get; set; }

    public int OrderID { get; set; }

    public int SeatID { get; set; }

    public Orders Orders { get; set; }

    public Seats Seats { get; set; }
}
