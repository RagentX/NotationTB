using NotationTB.Data;

namespace NotationTB.BusinessLogic.Object;

public class NotationRule
{
    private int combinationId = -1;
    private readonly int materialTypeId;

    public List<OperationRow> OperationRows = new();
    private readonly int productTypeId;

    public NotationRule(int materialTypeId, int productTypeId)
    {
        OperationRows.Clear();
        this.materialTypeId = materialTypeId;
        this.productTypeId = productTypeId;
        using (var db = new AppDbContext())
        {
            var i = 0;
            var classifications = db.ClassificationDesignations.OrderBy(c => c.Id).ToList();
            foreach (var classification in classifications)
            {
                OperationRows.Add(new OperationRow(classification.Id));
                var baseRules = db.BasesRulesOperations.Where(b =>
                    b.ProductTypeId == productTypeId &&
                    b.MaterialTypeId == materialTypeId &&
                    b.DesignationId == classification.Id);
                foreach (var baseRule in baseRules) OperationRows[i].Values[baseRule.OperationTypeId] = baseRule.Value;

                i++;
            }
        }
    }

    public NotationRule(int combinationId)
    {
        this.combinationId = combinationId;
        OperationRows.Clear();
        using (var db = new AppDbContext())
        {
            var combination = db.MaterialsAndProductsCombinations.Where(c => c.Id == combinationId).First();

            var materialStampId = combination.MaterialId;
            var productStandardId = combination.ProStandardId;

            materialTypeId = db.MaterialsStamps.Where(m => m.Id == materialStampId).Select(m => m.TypeId).First();
            productTypeId = db.ProductsStandards.Where(p => p.Id == productStandardId).Select(p => p.TypeId).First();
            var i = 0;
            var classifications = db.ClassificationDesignations;


            foreach (var classification in classifications)
            {
                OperationRows.Add(new OperationRow(classification.Id));
                var baseRules = db.BasesRulesOperations.Where(b =>
                    b.ProductTypeId == productTypeId &&
                    b.MaterialTypeId == materialTypeId &&
                    b.DesignationId == classification.Id);
                foreach (var baseRule in baseRules) OperationRows[i].Values[baseRule.OperationTypeId] = baseRule.Value;

                var exceptionRules = db.ExceptionRulesOperations.Where(e =>
                    e.CombinationId == combinationId);
                foreach (var exceptionRule in exceptionRules)
                    OperationRows[i].Values[exceptionRule.OperationTypeId] = exceptionRule.Value;
            }
        }
    }

    public void Save(int combinationId)
    {
        this.combinationId = combinationId;
        using (var db = new AppDbContext())
        {
            var i = 0;
            var classifications = db.ClassificationDesignations.OrderBy(c => c.Id).ToList();
            foreach (var classification in classifications)
            {
                var baseRules = db.BasesRulesOperations.Where(b =>
                    b.ProductTypeId == productTypeId &&
                    b.MaterialTypeId == materialTypeId &&
                    b.DesignationId == classification.Id);
                foreach (var baseRule in baseRules)
                    if (OperationRows[i].Values[baseRule.OperationTypeId] == baseRule.Value &&
                        db.ExceptionRulesOperations.Where(e =>
                            e.CombinationId == combinationId &&
                            e.DesignationId == classification.Id &&
                            e.OperationTypeId == baseRule.OperationTypeId).Any() == false)
                    {
                        OperationRows[i].Values.Remove(baseRule.OperationTypeId);
                    }

                i++;
            }

            foreach (var OperationRow in OperationRows)
            {
                foreach (var values in OperationRow.Values)
                {
                    ExceptionRulesOperationBL.AddOrReplase( combinationId, OperationRow.ClassificationId, values.Key, values.Value);
                }
            }
        }
    }

    public void Save()
    {
        if (combinationId == -1)
            throw new ArgumentException("Не заполнен код комбинации, используйте сохранение с кобом комбинации",
                nameof(combinationId));
        Save(combinationId);
    }
}