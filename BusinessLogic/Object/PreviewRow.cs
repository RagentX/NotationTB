using NotationTB.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace NotationTB.BusinessLogic.Object
{
    internal class PreviewRow
    {
        public int ClassificationId { get; private set; }
        /// <summary>
        /// Наименование детали
        /// </summary>
        public String Name { get; private set; }
        /// <summary>
        /// Обозначение по чертежу
        /// </summary>
        public String PlanName { get; private set; }
        /// <summary>
        /// Марка стали
        /// </summary>
        public String MaterialStamp { get; private set; }
        /// <summary>
        /// Стандарт или ТУ
        /// </summary>
        public String MaterialStandart { get; private set; }
        /// <summary>
        /// ТУ на полуфабрикат
        /// </summary>
        public String ProductStandart { get; private set; }

        public Dictionary<int, bool> Values { get; set; } = new();

        public PreviewRow(string name, string planName,int classificationId, int materialStampId, int materialStandartId, int productStandartId)
        {
            using (AppDbContext db = new AppDbContext())
            {
                var materialStamp = db.MaterialsStamps.Where(c => c.Id == materialStampId).First();
                var materialStandart = db.MaterialsStandards.Where(c => c.Id == materialStandartId).First();
                var productStandart = db.ProductsStandards.Where(c => c.Id == productStandartId).First();
                var combination = db.MaterialsAndProductsCombinations.Where(c => 
                    c.MatStandardId == materialStandartId && 
                    c.MaterialId == materialStampId && 
                    c.ProStandardId == productStandartId).First();
                var baseRules = db.BasesRulesOperations.Where(b =>
                    b.ProductTypeId == productStandart.Type.Id &&
                    b.MaterialTypeId == materialStandart.Id &&
                    b.DesignationId == classificationId);
                foreach (var baseRule in baseRules) Values[baseRule.OperationTypeId] = baseRule.Value;

                var exceptionRules = db.ExceptionRulesOperations.Where(e =>
                    e.CombinationId == combination.Id);
                foreach (var exceptionRule in exceptionRules)
                    Values[exceptionRule.OperationTypeId] = exceptionRule.Value;
            }

        }
    }
}
