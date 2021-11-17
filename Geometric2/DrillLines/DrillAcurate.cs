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
    public static class DrillAcurate
    {

        public static void DrillAndSave(List<ModelGeneration.BezierPatchC0> patchC0)
        {
            List<string> pointsall = new List<string>();
            List<List<Patch>> patchesAll = new List<List<Patch>>();

            float[,] topLayer = new float[300, 300];
            foreach (var _patchC0 in patchC0)
            {
                List<Patch> patches = new List<Patch>();
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

                patchesAll.Add(patches);
            }

            List<Vector3> allPoints = new List<Vector3>();

            //Top
            List<Patch> topPatches = patchesAll[0];
            List<Patch> top = new List<Patch>();
            int idx = 0;
            foreach (var pp in topPatches)
            {
                if (idx % 4 == 0 || idx % 4 == 0)
                {
                    top.Add(pp);
                }
                idx++;
            }
            //Patch topPatch = topPatches[0];
            //foreach(var topPatch in top)
            for (float u = 0.0f; u <= 1.0f; u += 0.01f)
            {
                foreach (var topPatch in top)
                {
                    for (float v = 0.0f; v <= 1.0f; v += 0.01f)
                    {
                        allPoints.Add(topPatch.P(u, v));
                    }
                }
                allPoints.Add(allPoints.LastOrDefault() + new Vector3(0, 100, 0));
            }



            int numer = 0;
            foreach (var pp in allPoints)
            {
                var ppp = pp + new Vector3(0, 15.0f, 0);
                pointsall.Add("N3G" + numer.ToString() + "X" + ppp.X.ToString() + "Y" + (-ppp.Z).ToString() + "Z" + ppp.Y.ToString());
                numer++;
            }

            using (StreamWriter file = new StreamWriter("C://Users//User//Documents//New folder//t5.k08", append: false))
            {
                foreach (var line in pointsall)
                {
                    file.WriteLine(line);
                }
            }
        }
    }
}
