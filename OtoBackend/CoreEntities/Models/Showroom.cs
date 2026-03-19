using System;
using System.Collections.Generic;

namespace CoreEntities.Models;

public partial class Showroom
{
    public int ShowroomId { get; set; }

    public string Name { get; set; } = null!;

    public string Address { get; set; } = null!;

    public string? Hotline { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    public virtual ICollection<User> Users { get; set; } = new List<User>(); // Danh sách nhân viên thuộc showroom
    public virtual ICollection<CarInventory> CarInventories { get; set; } = new List<CarInventory>(); // Xe trong kho
    //public virtual ICollection<Booking> Bookings { get; set; } // Lịch hẹn tại showroom này
}
