using Geometric2.Intersect;
using Intersect;
using OpenTK;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geometric2.DrillLines
{
    public static class DrillSign
    {

        public static void DrillAndSave()
        {
            float R = 0.5f;
            List<string> pointsall = new List<string>();
            List<Vector3> allPoints = new List<Vector3>();
            Vector3 startPoint = new Vector3(65f, 2.0f, -60f);
            allPoints.Add(new Vector3(0, 50, 0));
            allPoints.Add(startPoint + new Vector3(0,50,0));
            allPoints.Add(startPoint);

            //P
            allPoints.AddRange(PointsFromNumbers(new int[] { 5, 1, 2, 4, 3 }, startPoint));
            startPoint = startPoint.NextPoint();
            allPoints.AddRange(ToNextPoint(startPoint, allPoints));

            //I
            allPoints.AddRange(PointsFromNumbers(new int[] {  5,1}, startPoint));
            startPoint = startPoint.NextPoint(true);
            allPoints.AddRange(ToNextPoint(startPoint, allPoints));

            //O
            allPoints.AddRange(PointsFromNumbers(new int[] { 1,5,6,2,1 }, startPoint));
            startPoint = startPoint.NextPoint();
            allPoints.AddRange(ToNextPoint(startPoint, allPoints));

            //T
            allPoints.AddRange(PointsFromNumbers(new int[] { 9,7,1,2 }, startPoint));
            startPoint = startPoint.NextPoint();
            allPoints.AddRange(ToNextPoint(startPoint, allPoints));

            //R
            allPoints.AddRange(PointsFromNumbers(new int[] { 5,1,2,4,3,8,6 }, startPoint));
            startPoint = startPoint.NextPoint();
            allPoints.AddRange(ToNextPoint(startPoint, allPoints));

            startPoint = startPoint.NextPoint();
            allPoints.AddRange(ToNextPoint(startPoint, allPoints));


            //S
            allPoints.AddRange(PointsFromNumbers(new int[] { 2,1,3,4,6,5 }, startPoint));
            startPoint = startPoint.NextPoint();
            allPoints.AddRange(ToNextPoint(startPoint, allPoints));

            //A
            allPoints.AddRange(PointsFromNumbers(new int[] { 5, 1, 2, 6,4,3 }, startPoint));
            startPoint = startPoint.NextPoint();
            allPoints.AddRange(ToNextPoint(startPoint, allPoints));

            //M
            allPoints.AddRange(PointsFromNumbers(new int[] { 5, 1, 8,2,6 }, startPoint));
            startPoint = startPoint.NextPoint();
            allPoints.AddRange(ToNextPoint(startPoint, allPoints));

            //B
            allPoints.AddRange(PointsFromNumbers(new int[] { 5, 1, 2, 4, 8,4,6,5 }, startPoint));
            startPoint = startPoint.NextPoint();
            allPoints.AddRange(ToNextPoint(startPoint, allPoints));

            //O
            allPoints.AddRange(PointsFromNumbers(new int[] { 5, 1, 2, 6,5 }, startPoint));
            startPoint = startPoint.NextPoint();
            allPoints.AddRange(ToNextPoint(startPoint, allPoints));

            //R
            allPoints.AddRange(PointsFromNumbers(new int[] { 5, 1, 2, 4, 3, 8, 6 }, startPoint));
            startPoint = startPoint.NextPoint();
            allPoints.AddRange(ToNextPoint(startPoint, allPoints));

            //S
            allPoints.AddRange(PointsFromNumbers(new int[] { 2, 1, 3, 4, 6, 5 }, startPoint));
            startPoint = startPoint.NextPoint();
            allPoints.AddRange(ToNextPoint(startPoint, allPoints));

            //K
            allPoints.AddRange(PointsFromNumbers(new int[] { 5, 1,3,2,3,6 }, startPoint));
            startPoint = startPoint.NextPoint();
            allPoints.AddRange(ToNextPoint(startPoint, allPoints));

            //I
            allPoints.AddRange(PointsFromNumbers(new int[] { 5,1 }, startPoint));
            startPoint = startPoint.NextPoint(true);
            allPoints.AddRange(ToNextPoint(startPoint, allPoints));

            startPoint = startPoint.NextPoint();
            allPoints.AddRange(ToNextPoint(startPoint, allPoints));

            //2
            allPoints.AddRange(PointsFromNumbers(new int[] {1,2,4,3,5,6 }, startPoint));
            startPoint = startPoint.NextPoint();
            allPoints.AddRange(ToNextPoint(startPoint, allPoints));

            //0
            allPoints.AddRange(PointsFromNumbers(new int[] { 5, 1,2,6,5 }, startPoint));
            startPoint = startPoint.NextPoint();
            allPoints.AddRange(ToNextPoint(startPoint, allPoints));

            //2
            allPoints.AddRange(PointsFromNumbers(new int[] { 1, 2, 4, 3, 5, 6 }, startPoint));
            startPoint = startPoint.NextPoint();
            allPoints.AddRange(ToNextPoint(startPoint, allPoints));

            //1
            allPoints.AddRange(PointsFromNumbers(new int[] { 3,2,6 }, startPoint));
            startPoint = startPoint.NextPoint();
            allPoints.AddRange(ToNextPoint(startPoint, allPoints));

            //R
            allPoints.AddRange(PointsFromNumbers(new int[] { 5, 1,2,4,3,8,6 }, startPoint));
            startPoint = startPoint.NextPoint();
            allPoints.AddRange(ToNextPoint(startPoint, allPoints));

            //R
            allPoints.AddRange(PointsFromNumbers(new int[] { 5 }, startPoint));
            startPoint = startPoint.NextPoint();
            allPoints.AddRange(ToNextPoint(startPoint, allPoints));


            allPoints.Add(new Vector3(0, 50, 0));

            //End
            int numer = 0;
            foreach (var pp in allPoints)
            {
                var ppp = pp + new Vector3(0, 15.0f, 0);
                pointsall.Add("N" + numer.ToString() + "G01X" + ppp.X.ToString("F3") + "Y" + (ppp.Z).ToString("F3") + "Z" + ppp.Y.ToString("F3"));
                numer++;
            }

            using (StreamWriter file = new StreamWriter("C://Users//User//Documents//New folder//t4.k01", append: false))
            {
                foreach (var line in pointsall)
                {
                    file.WriteLine(line);
                }
            }
        }

        private static List<Vector3> ToNextPoint(Vector3 nextpoint, List<Vector3> allPoints)
        {
            List<Vector3> pointsToGo = new List<Vector3>();
            pointsToGo.Add(allPoints.Last() + new Vector3(0, 2, 0));
            pointsToGo.Add(nextpoint);

            return pointsToGo;
        }

        private static Vector3 NextPoint(this Vector3 vector, bool half = false)
        {
            float move = 6.0f;
            if (half) move = 3.0f;

            return vector + new Vector3(0, 0, move);

        }

        private static List<Vector3> PointsFromNumbers(int[] indices, Vector3 startPoint)
        {
            float deep = -1.0f;
            List<Vector3> vectors = new List<Vector3>();
            for(int i = 0; i < indices.Length; i++)
            {
                switch (indices[i])
                {
                    case 1:
                        vectors.Add(new Vector3(startPoint.X, deep, startPoint.Z));
                        break;
                    case 2:
                        vectors.Add(new Vector3(startPoint.X, deep, startPoint.Z + 3.0f));
                        break;
                    case 3:
                        vectors.Add(new Vector3(startPoint.X + 3.0f, deep, startPoint.Z));
                        break;
                    case 4:
                        vectors.Add(new Vector3(startPoint.X + 3.0f, deep, startPoint.Z + 3.0f));
                        break;
                    case 5:
                        vectors.Add(new Vector3(startPoint.X + 6.0f, deep, startPoint.Z));
                        break;
                    case 6:
                        vectors.Add(new Vector3(startPoint.X + 6.0f, deep, startPoint.Z + 3.0f));
                        break;


                    case 7:
                        vectors.Add(new Vector3(startPoint.X, deep, startPoint.Z + 1.5f));
                        break;
                    case 8:
                        vectors.Add(new Vector3(startPoint.X + 3.0f, deep, startPoint.Z + 1.5f));
                        break;
                    case 9:
                        vectors.Add(new Vector3(startPoint.X + 6.0f, deep, startPoint.Z + 1.5f));
                        break;
                }
            }

            vectors.Insert(0, vectors.First() + new Vector3(0, 2, 0));
            return vectors;
        }

    }
}
