using OpenTK;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geometric2.DrillLines
{
    public static class DrillRound
    {
        public static void DrillAndSave(List<ModelGeneration.BezierPatchC0> patchC0)
        {
            var resBig = GetRound(patchC0[0]);
            List<Vector3> rightBig = resBig.rightSide;
            List<Vector3> leftBig = resBig.leftSide;


            var resSmall = GetRound(patchC0[4]);
            List<Vector3> rightSmall = resSmall.rightSide;
            List<Vector3> leftSmall = resSmall.leftSide;

            List<Vector3> resultPoints = new List<Vector3>();

            List<Vector3> processingPoints = rightBig;
            for (int patchNo = 0; patchNo < rightBig.Count / 4; patchNo++)
            {
                for (float i = 0.00f; i <= 1; i += 0.01f)
                {
                    int k = 4 * patchNo;
                    float X = HelpFunctions.DeKastilio(new float[] { processingPoints[k].X, processingPoints[k + 1].X, processingPoints[k + 2].X, processingPoints[k + 3].X }, i, 4);
                    float Y = HelpFunctions.DeKastilio(new float[] { processingPoints[k].Y, processingPoints[k + 1].Y, processingPoints[k + 2].Y, processingPoints[k + 3].Y }, i, 4);
                    float Z = HelpFunctions.DeKastilio(new float[] { processingPoints[k].Z, processingPoints[k + 1].Z, processingPoints[k + 2].Z, processingPoints[k + 3].Z }, i, 4);
                    resultPoints.Add(new Vector3(X, Y, Z));
                }
            }

            int numer = 0;
            List<string> pointsall = new List<string>();
            foreach (var ppp in resultPoints)
            {
                pointsall.Add("N3G" + numer.ToString() + "X" + ppp.X.ToString() + "Y" + ppp.Y.ToString() + "Z" + ppp.Z.ToString());
                numer++;
            }

            //using (StreamWriter file = new StreamWriter("C://Users//User//Documents//New folder//t1.k16", append: false))
            //{
            //    foreach (var line in pointsall)
            //    {
            //        file.WriteLine(line);
            //    }
            //}

            //foreach (var patch in patchC0)
            //{
            //    List<Vector3> zeroPoints = new List<Vector3>();
            //    foreach (var p in patch.bezierPoints)
            //    {
            //        Vector3 pPos = p.Position();
            //        if (Math.Abs(pPos.Y) < 0.00001f)
            //        {
            //            pPos.Y = 0;
            //            zeroPoints.Add(pPos);
            //        }
            //    }

            //    List<Vector3> notRepeatPoints = new List<Vector3>();
            //    foreach (var p in zeroPoints)
            //    {
            //        if (!notRepeatPoints.Contains(p))
            //        {
            //            notRepeatPoints.Add(p);
            //        }
            //    }

            //    List<Vector3> rightSide = notRepeatPoints.Where(x => x.X >= 0).ToList();
            //    List<Vector3> leftSide = notRepeatPoints.Where(x => x.X <= 0).ToList();
            //    leftSide.Reverse();

            //    int k = 0;
            //    List<Vector3> processingPoints = rightSide;
            //    for (float i = 0.00f; i <= 1; i += 0.01f)
            //    {
            //        float X = HelpFunctions.DeKastilio(new float[] { processingPoints[k].X, processingPoints[k + 1].X, processingPoints[k + 2].X, processingPoints[k + 3].X }, i, 4);
            //        float Y = HelpFunctions.DeKastilio(new float[] { processingPoints[k].Y, processingPoints[k + 1].Y, processingPoints[k + 2].Y, processingPoints[k + 3].Y }, i, 4);
            //        float Z = HelpFunctions.DeKastilio(new float[] { processingPoints[k].Z, processingPoints[k + 1].Z, processingPoints[k + 2].Z, processingPoints[k + 3].Z }, i, 4);

            //        k += 3;
            //    }
            //}
        }

        private static (List<Vector3> rightSide, List<Vector3> leftSide) GetRound(ModelGeneration.BezierPatchC0 patch)
        {
            List<Vector3> zeroPoints = new List<Vector3>();
            foreach (var p in patch.bezierPoints)
            {
                Vector3 pPos = p.Position();
                if (Math.Abs(pPos.Y) < 0.00001f)
                {
                    pPos.Y = 0;
                    zeroPoints.Add(pPos);
                }
            }

            List<Vector3> notRepeatPoints = new List<Vector3>();
            foreach (var p in zeroPoints)
            {
                if (!notRepeatPoints.Contains(p))
                {
                    notRepeatPoints.Add(p);
                }
            }

            List<Vector3> _rightSide = notRepeatPoints.Where(x => x.X >= 0).ToList();
            List<Vector3> _leftSide = notRepeatPoints.Where(x => x.X <= 0).ToList();
            _leftSide.Reverse();


            List<Vector3> rightSide = new List<Vector3>();
            List<Vector3> leftSide = new List<Vector3>();
            for (int i = 0; i < _rightSide.Count; i++)
            {
                rightSide.Add(_rightSide[i]);
                leftSide.Add(_leftSide[i]);
                if (i % 3 == 0 && i != 0 && i != _rightSide.Count - 1)
                {
                    rightSide.Add(_rightSide[i]);
                    leftSide.Add(_leftSide[i]);
                }
            }

            return (rightSide, leftSide);
        }

    }
}
