using System;
using System.Collections.Generic;

namespace Movie2024.Models;

public partial class Users
{
    public int UserID { get; set; }

    public string UserName { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string? Email { get; set; }

    public string? PhoneNumber { get; set; }

    public DateTime CreatedAt { get; set; }

    public string? isStaff { get; set; }
}
