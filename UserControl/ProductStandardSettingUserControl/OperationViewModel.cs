using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotationTB.UserControl.ProductStandardSettingUserControl
{
    class OperationViewModel
    {
        public string Classification { get; set; }
        public Dictionary<int, string> OperationValues { get; set; } = new();
    }
}
