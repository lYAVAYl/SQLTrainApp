using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLTrainApp.Model.Logic
{
    public class ComplaintItem
    {
        public int ItemID { get; set; }
        public int TaskID { get; set; }
        public string SenderLogin { get; set; }
        public string Comment { get; set; }
    }
}
