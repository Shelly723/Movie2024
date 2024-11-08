using System;
using System.Collections.Generic;

namespace Movie2024.Models;

public partial class Seats
{
    public int SeatID { get; set; }

    public int TheaterID { get; set; }

    public string SeatNumber { get; set; } = null!;

    public string IsAvailable { get; set; } = null!;

    // 對應的影廳
    public Theaters Theater { get; set; } = null!;
    // 定義一對多的關聯
    public ICollection<ShowtimeSeats> ShowtimeSeats { get; set; }

    public ICollection<OrderSeats> OrderSeats { get; set; }
}
