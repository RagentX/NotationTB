using System;
using System.Collections.Generic;

namespace NotationTB.Models;

/// <summary>
/// Дополнительные правила
/// </summary>
public partial class OptionalRule
{
    public int Id { get; set; }

    public int Name { get; set; }

    public bool ForAll { get; set; }

    public int OperationTypeId { get; set; }

    public int? DesignationId { get; set; }

    public int? MaterialTypeId { get; set; }
    public override string ToString() => $"{Name}";

    public virtual ClassificationDesignation? Designation { get; set; }

    public virtual MaterialsType? MaterialType { get; set; }

    public virtual OperationsType OperationType { get; set; } = null!;
}
