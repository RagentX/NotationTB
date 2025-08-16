using System;
using System.Collections.Generic;
using NotationTB.Data;

namespace NotationTB.Models;

/// <summary>
/// Стандарты изделий и полуфабрикатов
/// </summary>
public partial class ProductsStandard
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int TypeId { get; set; }

    public override string ToString() => $"{Name}:{Type.Name}";

    public virtual ICollection<MaterialsAndProductsCombination> MaterialsAndProductsCombinations { get; set; } = new List<MaterialsAndProductsCombination>();

    public virtual ProductsType Type 
    {
        get
        {
            using (var db = new AppDbContext())
            {
                return db.ProductsTypes.Where(p => p.Id == TypeId).FirstOrDefault();
            }
        }
        set
        {
            Type = value;
        }
    }
}
