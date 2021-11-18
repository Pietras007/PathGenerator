using Geometric2.DrillLines;
using Intersect;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geometric2.Intersect
{
    public class BestPatch : ISurface
    {
        private Patch[,] patchesAll;
        private int splitA { get; set; }
        private int splitB { get; set; }

        private float R = 4.0f;

        public BestPatch(ModelGeneration.BezierPatchC0 _patchC0)
        {
            splitA = _patchC0.splitB;
            splitB = _patchC0.splitA;
            patchesAll = new Patch[splitA, splitB];


            List<Patch> patches = new List<Patch>();
            int startIndex = 0;

            for (int j = 0; j < splitA; j++)
            {
                for (int i = 0; i < splitB; i++)
                {
                    patchesAll[j, i] = new Patch();
                }
            }
            int patchNumber = 0;
            for (int j = 0; j < _patchC0.splitA; j++)
            {
                for (int k = 0; k < 4; k++)
                {
                    patchNumber = _patchC0.splitB * j;
                    for (int i = 0; i < _patchC0.splitB; i++)
                    {
                        int d = patchNumber % splitA;
                        int e = patchNumber / splitA;
                        patchesAll[d, e].points.Add(_patchC0.bezierPoints[startIndex].Position()); startIndex++;
                        patchesAll[d, e].points.Add(_patchC0.bezierPoints[startIndex].Position()); startIndex++;
                        patchesAll[d, e].points.Add(_patchC0.bezierPoints[startIndex].Position()); startIndex++;
                        patchesAll[d, e].points.Add(_patchC0.bezierPoints[startIndex].Position());
                        patchNumber++;
                    }
                    startIndex++;
                }
                startIndex -= (_patchC0.splitB * 3 + 1);
            }


            //for (int j = 0; j < _patchC0.splitA; j++)
            //{
            //    for (int k = 0; k < 4; k++)
            //    {
            //        patchNumber = _patchC0.splitB * j + prevPatchnumber;
            //        for (int i = 0; i < _patchC0.splitB; i++)
            //        {
            //            patches[patchNumber].points.Add(_patchC0.bezierPoints[startIndex].Position()); startIndex++;
            //            patches[patchNumber].points.Add(_patchC0.bezierPoints[startIndex].Position()); startIndex++;
            //            patches[patchNumber].points.Add(_patchC0.bezierPoints[startIndex].Position()); startIndex++;
            //            patches[patchNumber].points.Add(_patchC0.bezierPoints[startIndex].Position());

            //            patchNumber++;
            //        }
            //        startIndex++;
            //    }
            //    startIndex -= (_patchC0.splitB * 3 + 1);
            //}
        }


        public Vector3 P(float u, float v)
        {
            Vector3 currentPoint = CP(u, v);
            Vector3 tangent = T(u, v);
            Vector3 bitangent = B(u, v);
            Vector3 normal = Vector3.Cross(tangent, bitangent).Normalized();

            //currentPoint += (-normal * R);
            //currentPoint -= new Vector3(0, R, 0);

            return currentPoint;
        }

        public Vector3 CP(float u, float v)
        {
            if(u < 0 || u > 1 || v < 0 || v > 1)
            {
                var x = 5;
            }

            u = Clamp(u, 0, 1);
            v = Clamp(v, 0, 1);
            float valA = (u * splitA);
            float valB = (v * splitB);

            var patchA = (int)Math.Floor(valA);
            if (patchA == splitA) patchA--;

            var patchB = (int)Math.Floor(valB);
            if (patchB == splitB) patchB--;

            float patchU = valA - patchA;
            float patchV = valB - patchB;

            return patchesAll[patchA, patchB].P(patchU, patchV);
        }

        public Vector3 B(float u, float v)
        {
            if (v + 1e-4f < 1)
                return (CP(u, v + 1e-4f) - CP(u, v)) / (1e-4f);
            else
                return (CP(u, v) - CP(u, v - 1e-4f)) / (1e-4f);
        }

        public Vector3 N(float u, float v)
        {
            Vector3 tangent = T(u, v);
            Vector3 bitangent = B(u, v);
            Vector3 normal = Vector3.Cross(tangent, bitangent);
            return normal.Normalized();
        }

        public Vector3 T(float u, float v)
        {
            if (u + 1e-4f < 1)
                return (CP(u + 1e-4f, v) - (CP(u, v))) / (1e-4f);
            else
                return (CP(u, v) - (CP(u - 1e-4f, v))) / (1e-4f);
        }

        public bool WrapsU()
        {
            return true;
        }

        public bool WrapsV()
        {
            return false;
        }

        private static float Clamp(float val, float min, float max)
        {
            if (val < min)
            {
                return min;
            }
            else if (val > max)
            {
                return max;
            }
            else if(val >= min && val <= max)
            {
                return val;
            }
            else
            {
                return 0.5f;
            }
        }
    }
}
