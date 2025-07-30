using System;
using System.Collections.Generic;

namespace NotationTB.Models;

/// <summary>
/// Стандарты изделий и полуфабрикатов
/// </summary>
public partial class ProductsStandard
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int TypeId { get; set; }

    public override string ToString() => $"{Name}";

    public virtual ICollection<MaterialsAndProductsCombination> MaterialsAndProductsCombinations { get; set; } = new List<MaterialsAndProductsCombination>();

    public virtual ProductsType Type { get; set; } = null!;
}
