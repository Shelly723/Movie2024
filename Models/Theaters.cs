using System;
using System.Collections.Generic;

namespace Movie2024.Models;

public partial class Theaters
{
    public int TheaterID { get; set; }

    public string TheaterName { get; set; } = null!;

    public string Location { get; set; } = null!;

    public int SeatingCapacity { get; set; }
}
