using Intersect;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geometric2.Global
{
    public class GlobalData
    {
        public bool showTrim1 = false;
        public bool showTrim2 = false;
        public bool surface1_1 = false;
        public bool surface1_2 = false;
        public bool surface2_1 = false;
        public bool surface2_2 = false;
        public Vector3 selectedPoint = new Vector3(0, 0, 0);
        public bool UseSelectedPoint { get; set; }
    }
}
