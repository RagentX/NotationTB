using System.Windows.Automation;
using NotationTB.Data;
using NotationTB.Models;

namespace NotationTB.BusinessLogic.Object;

internal class NotationRule
{
    private int designationId;
    private int materialsStampId;
    private int materialsStandardId;
    private int productsStandardId;
    private readonly Dictionary<int, bool> rules;

    private NotationRule(int materialsStampId, int materialsStandardId, int productsStandardId, int designationId,
        Dictionary<int, bool> rules)
    {
        this.materialsStampId = materialsStampId;
        this.materialsStandardId = materialsStandardId;
        this.productsStandardId = productsStandardId;
        this.designationId = designationId;
        this.rules = rules;
    }
    /// <summary>
    /// Создание списка правил для материала с описаными критериями 
    /// </summary>
    /// <param name="materialsStampId"></param>
    /// <param name="materialsStandardId"></param>
    /// <param name="productsStandardId"></param>
    /// <param name="designationId"></param>
    private NotationRule(int materialsStampId, int materialsStandardId, int productsStandardId, int designationId)
    {
        using (var db = new AppDbContext())
        {
            //var rulesOperations = db.ExceptionRulesOperations.Where(r =>
            //(
            //    r.MatStandardId == materialsStampId &&
            //    r.ProStandardId == materialsStandardId &&
            //    r.ProStandardId == productsStandardId &&
            //    r.DesignationId == designationId
            //)).ToList();
            //foreach (var rule in rulesOperations)
            //{
            //    rules = new Dictionary<int, bool>();
            //    rules.Add(rule.OperationTypeId.Value,rule.Value);
            //}
        }
    }
    /// <summary>
    /// Добавления правила
    /// </summary>
    /// <param name="id">Код типа операции из БД</param>
    /// <param name="value">Значение</param>
    /// <exception cref="Exception"></exception>
    public void AddRule(int id, bool value)
    {
        using (var db = new AppDbContext())
        {
            if (db.OperationsTypes.Where(o => o.Id == id).Any())
                rules.Add(id, value);
            else
                throw new Exception("Такого кода типа операции нет в БД");
        }
    }
}