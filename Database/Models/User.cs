using System;
using System.Collections.Generic;

namespace Database.Models;

public partial class User
{
    public int Id { get; set; }

    [Required, MaxLength(100)] 
    public string Name { get; set; } = null!;

    [Required, EmailAddress, MaxLength(150)]
    public string Email { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}