using System;
using System.Collections.Generic;

namespace NotationTB.Models;

/// <summary>
/// Стандарты материалов
/// </summary>
public partial class MaterialsStandard
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public override string ToString() => $"{Id}:{Name}";

    public virtual ICollection<MaterialsAndProductsCombination> MaterialsAndProductsCombinations { get; set; } = new List<MaterialsAndProductsCombination>();
}
