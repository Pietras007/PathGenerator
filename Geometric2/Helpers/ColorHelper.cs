using OpenTK;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geometric2.Helpers
{
    public static class ColorHelper
    {
        public static Vector3 ColorToVector(Color color)
        {
            return new Vector3(color.R, color.G, color.B).Normalized();
        }
    }
}
