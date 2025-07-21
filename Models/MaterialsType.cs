using System;
using System.Collections.Generic;

namespace NotationTB.Models;

/// <summary>
/// Типы материалов(структурный класс)
/// </summary>
public partial class MaterialsType
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public bool IsAustenit { get; set; }

    public virtual ICollection<BasesRulesOperation> BasesRulesOperations { get; set; } = new List<BasesRulesOperation>();

    public virtual ICollection<MaterialsAndProductsCombination> MaterialsAndProductsCombinations { get; set; } = new List<MaterialsAndProductsCombination>();

    public virtual ICollection<MaterialsStamp> MaterialsStamps { get; set; } = new List<MaterialsStamp>();

    public virtual ICollection<OptionalRule> OptionalRules { get; set; } = new List<OptionalRule>();
}
