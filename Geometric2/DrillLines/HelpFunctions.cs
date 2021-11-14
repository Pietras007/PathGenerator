using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geometric2.DrillLines
{
    public static class HelpFunctions
    {
        public static bool checkIfHeightOk(float x, float y, float z, float R, float[,] topLayer, float height)
        {

            for (float _x = -R; _x <= R; _x += 1)
            {
                for (float _y = -R; _y <= R; _y += 1)
                {
                    if (_x + x >= 0 && _y + y >= 0 && _x + x < 600 && _y + y < 600)
                    {

                        float xa = x;
                        float ya = y;
                        float za = height + R;

                        float xb = x + _x;
                        float yb = y + _y;

                        float val = R * R - (xb - xa) * (xb - xa) - (yb - ya) * (yb - ya);
                        float zb = -(float)Math.Sqrt(val) + za;

                        if (topLayer[(int)(_x + x), (int)(_y + y)] > zb)
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        public static float DeKastilio(float[] vert, float t, int degree)
        {
            for (int i = 0; i < degree; i++)
            {
                for (int j = 0; j < degree - i - 1; j++)
                {
                    vert[j] = (1 - t) * vert[j] + t * vert[j + 1];
                }
            }

            return vert[0];
        }
    }
}
