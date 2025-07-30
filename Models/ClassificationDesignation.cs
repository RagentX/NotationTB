using System;
using System.Collections.Generic;

namespace NotationTB.Models;

/// <summary>
/// Класификационные обоначения
/// </summary>
public partial class ClassificationDesignation
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;
    public override string ToString() => $"{Name}";

    public virtual ICollection<BasesRulesOperation> BasesRulesOperations { get; set; } = new List<BasesRulesOperation>();

    public virtual ICollection<ExceptionRulesOperation> ExceptionRulesOperations { get; set; } = new List<ExceptionRulesOperation>();

    public virtual ICollection<OptionalRule> OptionalRules { get; set; } = new List<OptionalRule>();
}
