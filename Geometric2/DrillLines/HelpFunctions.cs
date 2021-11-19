using OpenTK;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geometric2.DrillLines
{
    public static class HelpFunctions
    {
        /// <summary>
        /// Determines if the given point is inside the polygon
        /// </summary>
        /// <param name="polygon">the vertices of polygon</param>
        /// <param name="testPoint">the given point</param>
        /// <returns>true if the point is inside the polygon; otherwise, false</returns>
        public static bool IsPointInPolygon4(Vector2[] polygon, Vector2 testPoint)
        {
            bool result = false;
            int j = polygon.Count() - 1;
            for (int i = 0; i < polygon.Count(); i++)
            {
                if (polygon[i].Y < testPoint.Y && polygon[j].Y >= testPoint.Y || polygon[j].Y < testPoint.Y && polygon[i].Y >= testPoint.Y)
                {
                    if (polygon[i].X + (testPoint.Y - polygon[i].Y) / (polygon[j].Y - polygon[i].Y) * (polygon[j].X - polygon[i].X) < testPoint.X)
                    {
                        result = !result;
                    }
                }
                j = i;
            }
            return result;
        }

        public static bool IsInPolygon(this Vector2 point, IEnumerable<Vector2> polygon)
        {
            bool result = false;
            var a = polygon.Last();
            foreach (var b in polygon)
            {
                if ((b.X == point.X) && (b.Y == point.Y))
                    return true;

                if ((b.Y == a.Y) && (point.Y == a.Y) && (a.X <= point.X) && (point.X <= b.X))
                    return true;

                if ((b.Y < point.Y) && (a.Y >= point.Y) || (a.Y < point.Y) && (b.Y >= point.Y))
                {
                    if (b.X + (point.Y - b.Y) / (a.Y - b.Y) * (a.X - b.X) <= point.X)
                        result = !result;
                }
                a = b;
            }
            return result;
        }

        public static bool checkIfHeightOk(float x, float y, float R, float[,] topLayer, float height)
        {

            for (float _x = -R; _x <= R; _x += 1)
            {
                for (float _y = -R; _y <= R; _y += 1)
                {
                    if (_x + x >= 0 && _y + y >= 0 && _x + x < 300 && _y + y < 300)
                    {
                        if (_x*_x + _y*_y <= R*R)
                        {
                            float xa = x;
                            float ya = y;
                            float za = height;

                            float xb = x + _x;
                            float yb = y + _y;

                            float val = R * R - (xb - xa) * (xb - xa) - (yb - ya) * (yb - ya);
                            float zb = za + (R - (float)Math.Sqrt(val)) / 2.0f;

                            //if (topLayer[(int)(_x + x), (int)(_y + y)] != 0)
                            //{
                            //    var xxx = 5;
                            //}

                            if (topLayer[(int)(_x + x), (int)(_y + y)] > zb)
                            {
                                return false;
                            }
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
