using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLTrainApp.Model.Logic
{
    public class BaseCoordinate
    {
        public Double Right { get; set; }
        public Double Left { get; set; }
        public Double Bottom { get; set; }
        public Double Top { get; set; }

        public Double Height { get; set; }
        public Double Width { get; set; }
    }
}
