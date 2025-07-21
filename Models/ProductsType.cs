using System;
using System.Collections.Generic;

namespace NotationTB.Models;

/// <summary>
/// Вид полуфабриката или изделия
/// </summary>
public partial class ProductsType
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<BasesRulesOperation> BasesRulesOperations { get; set; } = new List<BasesRulesOperation>();

    public virtual ICollection<ProductsStandard> ProductsStandards { get; set; } = new List<ProductsStandard>();
}
