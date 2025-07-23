using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NotationTB.Data;

namespace NotationTB.BusinessLogic.Object
{
    public class OperationRow
    {
        public int ClassificationId { get; private set; }

        public String Classification { get; private set; }
        public Dictionary<int, bool> Values { get; set; } = new();

        public OperationRow(int classificationId)
        {
            ClassificationId = classificationId;
            using (AppDbContext db = new AppDbContext())
            {
                Classification = db.ClassificationDesignations.Where(c => c.Id == ClassificationId)
                    .Select(c => c.Name).First();
            }
            
        }
    }
}
