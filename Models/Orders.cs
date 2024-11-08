using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Movie2024.Models;

public partial class Orders
{
    public int OrderID { get; set; }

    public int UserID { get; set; }

    public int ShowtimeID { get; set; }

    public decimal TotalAmount { get; set; }

    public DateTime CreatedAt { get; set; }

    public ICollection<OrderSeats> OrderSeats { get; set; }

    public Showtimes Showtimes { get; set; }

   

}
