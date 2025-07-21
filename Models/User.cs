using System;
using System.Collections.Generic;

namespace NotationTB.Models;

/// <summary>
/// Пользователи
/// </summary>
public partial class User
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Surname { get; set; } = null!;

    public string Login { get; set; } = null!;

    public string Password { get; set; } = null!;
}
