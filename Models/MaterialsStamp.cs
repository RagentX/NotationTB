using System;
using System.Collections.Generic;

namespace NotationTB.Models;

/// <summary>
/// Марки материала
/// </summary>
public partial class MaterialsStamp
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int TypeId { get; set; }

    public override string ToString() => $"{Name}";

    public virtual MaterialsType Type { get; set; } = null!;
}
