using System;
using System.Collections.Generic;

namespace NotationTB.Models;

/// <summary>
/// комбинации типа материала и стандартов материала и полуфобриката
/// </summary>
public partial class MaterialsAndProductsCombination
{
    public int Id { get; set; }

    public int MaterialId { get; set; }

    public int ProStandardId { get; set; }

    public int MatStandardId { get; set; }

    public virtual ICollection<ExceptionRulesOperation> ExceptionRulesOperations { get; set; } = new List<ExceptionRulesOperation>();

    public virtual MaterialsStandard MatStandard { get; set; } = null!;

    public virtual MaterialsType Material { get; set; } = null!;

    public virtual ProductsStandard ProStandard { get; set; } = null!;
}
