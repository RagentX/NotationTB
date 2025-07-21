using System;
using System.Collections.Generic;

namespace NotationTB.Models;

/// <summary>
/// Правила для формирования ТУ
/// </summary>
public partial class ExceptionRulesOperation
{
    public int Id { get; set; }

    public int CombinationId { get; set; }

    public int DesignationId { get; set; }

    public int OperationTypeId { get; set; }

    public bool Value { get; set; }

    public virtual MaterialsAndProductsCombination Combination { get; set; } = null!;

    public virtual ClassificationDesignation Designation { get; set; } = null!;

    public virtual OperationsType OperationType { get; set; } = null!;
}
