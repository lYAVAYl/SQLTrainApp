using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace SQLTrainApp.Model.Logic
{
    public class LineVM : BaseCoordinate
    {
        public Double X1 { get; set; }
        public Double Y1 { get; set; }
        public Double X2 { get; set; }
        public Double Y2 { get; set; }

        public Brush StrokeColor { get; set; } = Brushes.Black;
    }
}
