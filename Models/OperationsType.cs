using System;
using System.Collections.Generic;

namespace NotationTB.Models;

/// <summary>
/// Типы операций
/// </summary>
public partial class OperationsType
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public override string ToString() => $"{Id}:{Name}";

    public virtual ICollection<BasesRulesOperation> BasesRulesOperations { get; set; } = new List<BasesRulesOperation>();

    public virtual ICollection<ExceptionRulesOperation> ExceptionRulesOperations { get; set; } = new List<ExceptionRulesOperation>();

    public virtual ICollection<OptionalRule> OptionalRules { get; set; } = new List<OptionalRule>();
}
