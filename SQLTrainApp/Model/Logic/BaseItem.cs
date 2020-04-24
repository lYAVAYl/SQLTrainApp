using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLTrainApp.Model.Logic
{
    public abstract class BaseItem
    {
        public int ItemID { get; set; }
        public string ItemText { get; set; }

        public BaseItem(int id, string text)
        {
            ItemID = id;
            ItemText = text;
        }
    }
}
