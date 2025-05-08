using System;
using System.Collections.Generic;

namespace ServerApp.Models;

public partial class User
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string Role { get; set; } = null!;

    public string? ImageUrl { get; set; }

    public virtual ICollection<UserAprrove> UserAprroves { get; set; } = new List<UserAprrove>();

    public virtual ICollection<UserData> UserData { get; set; } = new List<UserData>();
    public ICollection<TeacherData> SubjectsTaught { get; set; } = new List<TeacherData>();

}
