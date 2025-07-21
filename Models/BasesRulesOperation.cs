using System;
using System.Collections.Generic;

namespace NotationTB.Models;

/// <summary>
/// основная логика
/// </summary>
public partial class BasesRulesOperation
{
    public int Id { get; set; }

    public int ProductTypeId { get; set; }

    public int MaterialTypeId { get; set; }

    public int OperationTypeId { get; set; }

    public int DesignationId { get; set; }

    public bool Value { get; set; }

    public virtual ClassificationDesignation Designation { get; set; } = null!;

    public virtual MaterialsType MaterialType { get; set; } = null!;

    public virtual OperationsType OperationType { get; set; } = null!;

    public virtual ProductsType ProductType { get; set; } = null!;
}
