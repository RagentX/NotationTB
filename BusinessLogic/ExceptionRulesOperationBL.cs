using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using NotationTB.Data;
using NotationTB.Models;

namespace NotationTB.BusinessLogic
{
    internal  class ExceptionRulesOperationBL
    {
        public static void  AddOrReplase(int combinationID, int designationId, int operationTypeID, bool value)
        {
            //todo: сделать автоматическую првоерку, если условие соответствует стандртному правилу, удалять строку исключения
            using (AppDbContext db = new AppDbContext())
            {
                if (db.ExceptionRulesOperations
                    .Where(e => e.CombinationId == combinationID && e.DesignationId == designationId &&
                                e.OperationTypeId == operationTypeID).Select(e => e.Id).Any())
                {
                    int id = db.ExceptionRulesOperations
                        .Where(e => e.CombinationId == combinationID && e.DesignationId == designationId &&
                                    e.OperationTypeId == operationTypeID).Select(e => e.Id).First();
                    var rule = db.ExceptionRulesOperations
                        .Where(e => e.CombinationId == combinationID && e.DesignationId == designationId &&
                                    e.OperationTypeId == operationTypeID).First();
                    rule.Value = value;
                    db.ExceptionRulesOperations.Update(rule);
                }
                else
                {
                    var rule = new ExceptionRulesOperation();
                    rule.CombinationId = combinationID;
                    rule.DesignationId = designationId;
                    rule.OperationTypeId = operationTypeID;
                    rule.Value = value;
                    db.ExceptionRulesOperations.Add(rule);
                }
                db.SaveChanges();
            }
        }

    }
}
