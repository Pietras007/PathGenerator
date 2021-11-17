using Geometric2.Intersect;
using OpenTK;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geometric2.DrillLines
{
    public static class AllPatches
    {
        public static void DrillAndSaveAll(List<ModelGeneration.BezierPatchC0> patchC0)
        {
            List<Patch> patches = new List<Patch>();
            List<string> pointsall = new List<string>();
            foreach (var _patchC0 in patchC0)
            {
                int startIndex = 0;
                int prevPatchnumber = patches.Count;
                int patchNumber = 0 + prevPatchnumber;
                for (int i = 0; i < _patchC0.splitA * _patchC0.splitB; i++)
                {
                    patches.Add(new Patch());
                }

                for (int j = 0; j < _patchC0.splitA; j++)
                {
                    for (int k = 0; k < 4; k++)
                    {
                        patchNumber = _patchC0.splitB * j + prevPatchnumber;
                        for (int i = 0; i < _patchC0.splitB; i++)
                        {
                            patches[patchNumber].points.Add(_patchC0.bezierPoints[startIndex].Position()); startIndex++;
                            patches[patchNumber].points.Add(_patchC0.bezierPoints[startIndex].Position()); startIndex++;
                            patches[patchNumber].points.Add(_patchC0.bezierPoints[startIndex].Position()); startIndex++;
                            patches[patchNumber].points.Add(_patchC0.bezierPoints[startIndex].Position());

                            patchNumber++;
                        }
                        startIndex++;
                    }
                    startIndex -= (_patchC0.splitB * 3 + 1);
                }

                foreach (var ppppp in patches)
                {
                    Vector3[] four1 = new Vector3[101];
                    Vector3[] four2 = new Vector3[101];
                    Vector3[] four3 = new Vector3[101];
                    Vector3[] four4 = new Vector3[101];

                    Vector3[] results = new Vector3[101 * 101];

                    int idx = 0;
                    for (float i = 0.00f; i <= 1; i += 0.01f)
                    {
                        four1[idx].X = HelpFunctions.DeKastilio(new float[] { ppppp.points[0].X, ppppp.points[1].X, ppppp.points[2].X, ppppp.points[3].X }, i, 4);
                        four1[idx].Y = HelpFunctions.DeKastilio(new float[] { ppppp.points[0].Y, ppppp.points[1].Y, ppppp.points[2].Y, ppppp.points[3].Y }, i, 4);
                        four1[idx].Z = HelpFunctions.DeKastilio(new float[] { ppppp.points[0].Z, ppppp.points[1].Z, ppppp.points[2].Z, ppppp.points[3].Z }, i, 4);

                        four2[idx].X = HelpFunctions.DeKastilio(new float[] { ppppp.points[4 + 0].X, ppppp.points[4 + 1].X, ppppp.points[4 + 2].X, ppppp.points[4 + 3].X }, i, 4);
                        four2[idx].Y = HelpFunctions.DeKastilio(new float[] { ppppp.points[4 + 0].Y, ppppp.points[4 + 1].Y, ppppp.points[4 + 2].Y, ppppp.points[4 + 3].Y }, i, 4);
                        four2[idx].Z = HelpFunctions.DeKastilio(new float[] { ppppp.points[4 + 0].Z, ppppp.points[4 + 1].Z, ppppp.points[4 + 2].Z, ppppp.points[4 + 3].Z }, i, 4);

                        four3[idx].X = HelpFunctions.DeKastilio(new float[] { ppppp.points[8 + 0].X, ppppp.points[8 + 1].X, ppppp.points[8 + 2].X, ppppp.points[8 + 3].X }, i, 4);
                        four3[idx].Y = HelpFunctions.DeKastilio(new float[] { ppppp.points[8 + 0].Y, ppppp.points[8 + 1].Y, ppppp.points[8 + 2].Y, ppppp.points[8 + 3].Y }, i, 4);
                        four3[idx].Z = HelpFunctions.DeKastilio(new float[] { ppppp.points[8 + 0].Z, ppppp.points[8 + 1].Z, ppppp.points[8 + 2].Z, ppppp.points[8 + 3].Z }, i, 4);

                        four4[idx].X = HelpFunctions.DeKastilio(new float[] { ppppp.points[12 + 0].X, ppppp.points[12 + 1].X, ppppp.points[12 + 2].X, ppppp.points[12 + 3].X }, i, 4);
                        four4[idx].Y = HelpFunctions.DeKastilio(new float[] { ppppp.points[12 + 0].Y, ppppp.points[12 + 1].Y, ppppp.points[12 + 2].Y, ppppp.points[12 + 3].Y }, i, 4);
                        four4[idx].Z = HelpFunctions.DeKastilio(new float[] { ppppp.points[12 + 0].Z, ppppp.points[12 + 1].Z, ppppp.points[12 + 2].Z, ppppp.points[12 + 3].Z }, i, 4);

                        idx++;
                    }

                    idx = 0;
                    for (int k = 0; k < 101; k++)
                    {
                        for (float i = 0.00f; i <= 1; i += 0.01f)
                        {
                            float X = HelpFunctions.DeKastilio(new float[] { four1[k].X, four2[k].X, four3[k].X, four4[k].X }, i, 4);
                            float Y = HelpFunctions.DeKastilio(new float[] { four1[k].Y, four2[k].Y, four3[k].Y, four4[k].Y }, i, 4);
                            float Z = HelpFunctions.DeKastilio(new float[] { four1[k].Z, four2[k].Z, four3[k].Z, four4[k].Z }, i, 4);
                            results[idx].X = X;
                            results[idx].Y = Y;
                            results[idx].Z = Z;
                            idx++;

                           
                        }
                    }

                    foreach (var x in results)
                    {
                        var pppPos = x;
                        pointsall.Add("N3G01X" + (pppPos.X).ToString() + "Y" + (-pppPos.Z).ToString() + "Z" + (pppPos.Y + 15.0f).ToString());
                    }
                }
            }

            using (StreamWriter file = new StreamWriter("C://Users//User//Documents//New folder//t0.k01", append: false))
            {
                foreach (var line in pointsall)
                {
                    file.WriteLine(line);
                }
            }
        }
    }
}
