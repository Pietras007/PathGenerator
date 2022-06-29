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

        public static List<Vector3> compressPaths(List<Vector3> paths)
        {
            List<Vector3> compressed = new List<Vector3>();
            compressed.Add(paths.First());
            for (int i = 1; i < paths.Count - 1; i++)
            {
                Vector3 first = compressed.ElementAt(compressed.Count - 1);
                Vector3 middle = paths.ElementAt(i);
                Vector3 last = paths.ElementAt(i + 1);
                //if (!(middle - first).Normalized().Equals((last - middle).Normalized()))
                if (((middle - first).Normalized() - (last - middle).Normalized()).Length > 0.001f)
                {
                    compressed.Add(new Vector3(middle));
                }
            }

            compressed.Add(paths.ElementAt(paths.Count - 1));
            return compressed;
        }

        public static bool IsInPolygon(this Vector2 point, IEnumerable<Vector2> _polygon)
        {
            Vector2 p = point;
            var polygon = _polygon.ToArray();
            double minX = polygon[0].X;
            double maxX = polygon[0].X;
            double minY = polygon[0].Y;
            double maxY = polygon[0].Y;
            for (int i = 1; i < polygon.Length; i++)
            {
                Vector2 q = polygon[i];
                minX = Math.Min(q.X, minX);
                maxX = Math.Max(q.X, maxX);
                minY = Math.Min(q.Y, minY);
                maxY = Math.Max(q.Y, maxY);
            }
            if (p.X < minX || p.X > maxX || p.Y < minY || p.Y > maxY)
            {
                return false;
            }

            bool inside = false;
            for (int i = 0, j = polygon.Length - 1; i < polygon.Length; j = i++)
            {
                if ((polygon[i].Y > p.Y) != (polygon[j].Y > p.Y) &&
                     p.X < (polygon[j].X - polygon[i].X) * (p.Y - polygon[i].Y) / (polygon[j].Y - polygon[i].Y) + polygon[i].X)
                {
                    inside = !inside;
                }
            }
            return inside;
        }

        //public static bool IsInPolygon(this Vector2 point, IEnumerable<Vector2> polygon)
        //{
        //    bool result = false;
        //    var a = polygon.Last();
        //    foreach (var b in polygon)
        //    {
        //        if ((b.X == point.X) && (b.Y == point.Y))
        //            return true;

        //        if ((b.Y == a.Y) && (point.Y == a.Y) && (a.X <= point.X) && (point.X <= b.X))
        //            return true;

        //        if ((b.Y < point.Y) && (a.Y > point.Y) || (a.Y < point.Y) && (b.Y > point.Y))
        //        {
        //            if (b.X + (point.Y - b.Y) / (a.Y - b.Y) * (a.X - b.X) < point.X)
        //                result = !result;
        //        }
        //        a = b;
        //    }
        //    return result;
        //}

        public static bool checkIfHeightOk(float x, float y, float R, float[,] topLayer, float height)
        {

            for (float _x = -R; _x <= R; _x += 1)
            {
                for (float _y = -R; _y <= R; _y += 1)
                {
                    if (_x + x >= 0 && _y + y >= 0 && _x + x < 300 && _y + y < 300)
                    {
                        if (_x * _x + _y * _y <= R * R)
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

        public static Vector3 DeBoor(float t, Vector3 B0_, Vector3 B1_, Vector3 B2_, Vector3 B3_)
        {
            float T0 = -1.0f;
            float T1 = 0.0f;
            float T2 = 1.0f;
            float T3 = 2.0f;
            float T4 = 3.0f;
            float Tm1 = -2.0f;

            float A1 = T2 - t;
            float A2 = T3 - t;
            float A3 = T4 - t;
            float B1 = t - T1;
            float B2 = t - T0;
            float B3 = t - Tm1;

            float N1 = 1;
            float N2 = 0;
            float N3 = 0;
            float N4 = 0;

            float saved = 0.0f;
            float term = 0.0f;

            term = N1 / (A1 + B1);
            N1 = saved + A1 * term;
            saved = B1 * term;

            N2 = saved;
            saved = 0.0f;

            term = N1 / (A1 + B2);
            N1 = saved + A1 * term;
            saved = B2 * term;

            term = N2 / (A2 + B1);
            N2 = saved + A2 * term;
            saved = B1 * term;

            N3 = saved;
            saved = 0.0f;

            term = N1 / (A1 + B3);
            N1 = saved + A1 * term;
            saved = B3 * term;

            term = N2 / (A2 + B2);
            N2 = saved + A2 * term;
            saved = B2 * term;

            term = N3 / (A3 + B1);
            N3 = saved + A3 * term;
            saved = B1 * term;

            N4 = saved;

            return N1 * B0_ + N2 * B1_ + N3 * B2_ + N4 * B3_;
        }
    }
}
