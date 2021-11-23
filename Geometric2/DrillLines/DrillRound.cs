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
            List<Vector3> resultPoints = new List<Vector3>();

            float R = 5.0f;
            var resBig = GetRound(patchC0[1], true);
            List<Vector3> rightBig = resBig.rightSide;
            List<Vector3> leftBig = resBig.leftSide;


            var resSmall = GetRound(patchC0[2], false);
            List<Vector3> rightSmall = resSmall.rightSide;
            List<Vector3> leftSmall = resSmall.leftSide;


            var firstIntersect = FindIntersection.GetIntersectionPoint(rightBig, rightSmall, R);
            var secondIntersect = FindIntersection.GetIntersectionPoint(rightBig, leftSmall, R);
            var thirdIntersect = FindIntersection.GetIntersectionPoint(leftBig, leftSmall, R);
            var fourthIntersect = FindIntersection.GetIntersectionPoint(leftBig, rightSmall, R);

            List<Vector3> roundPoints = new List<Vector3>();
            roundPoints.Add(rightBig.FirstOrDefault() + new Vector3(0, 0, -R));
            roundPoints.AddRange(GeneratePointsFromTo(0.00f, 0, firstIntersect.Item1.uMaster, firstIntersect.Item1.patchNo, rightBig, R));
            roundPoints.AddRange(GeneratePointsFromTo(firstIntersect.Item2.uSlave, firstIntersect.Item2.patchNo, 1.00f, rightSmall.Count / 4, rightSmall, R));
            roundPoints.Add(leftSmall.FirstOrDefault() + new Vector3(R, 0, 0));
            roundPoints.AddRange(GeneratePointsFromTo(0.00f, 0, secondIntersect.Item2.uSlave, secondIntersect.Item2.patchNo, leftSmall, R));
            roundPoints.AddRange(GeneratePointsFromTo(secondIntersect.Item1.uMaster, secondIntersect.Item1.patchNo, 1.00f, rightBig.Count / 4, rightBig, R));

            roundPoints.Add(leftBig.FirstOrDefault() + new Vector3(0, 0, R));
            roundPoints.AddRange(GeneratePointsFromTo(0.00f, 0, thirdIntersect.Item1.uMaster, thirdIntersect.Item1.patchNo, leftBig, R));
            roundPoints.AddRange(GeneratePointsFromTo(thirdIntersect.Item2.uSlave, thirdIntersect.Item2.patchNo, 1.00f, leftSmall.Count / 4, leftSmall, R));
            roundPoints.Add(rightSmall.FirstOrDefault() + new Vector3(-R, 0, 0));
            roundPoints.AddRange(GeneratePointsFromTo(0.00f, 0, fourthIntersect.Item2.uSlave, fourthIntersect.Item2.patchNo, rightSmall, R));
            roundPoints.AddRange(GeneratePointsFromTo(fourthIntersect.Item1.uMaster, fourthIntersect.Item1.patchNo, 1.00f, leftBig.Count / 4, leftBig, R));

            roundPoints.Add(rightBig.FirstOrDefault() + new Vector3(0, 0, -R));

            resultPoints.Add(new Vector3(0, 30, 0));
            resultPoints.Add(new Vector3(-90, 30, -90));
            resultPoints.Add(new Vector3(-90, 0, -90));

            List<Vector2> roundPoints2 = new List<Vector2>();
            foreach (var p in roundPoints)
            {
                roundPoints2.Add(new Vector2(p.X, p.Z));
            }

            bool go = false;
            for (float i = -75.0f; i <= 80.0f; i += (2 * R - 0.5f))
            {
                go = !go;
                bool first = true;
                for (float j = -75.0f; j < 0.00f; j += (0.5f * R - 0.1f))
                {
                    Vector3 currPoint = new Vector3(j, 0, i);
                    Vector2 currPoint2 = new Vector2(j, i);
                    if (!go)
                    {
                        currPoint = new Vector3(-75.0f - j, 0, i);
                        currPoint2 = new Vector2(-75.0f - j, i);
                    }

                    if (!HelpFunctions.IsInPolygon(currPoint2, roundPoints2) && !IsIn(currPoint, roundPoints, R))
                    {

                        if (first)
                        {
                            first = false;
                            var point = currPoint;
                            point -= new Vector3(0, 0, (2 * R - 0.5f));

                            var point2 = resultPoints.LastOrDefault();
                            point2 += new Vector3(0, 0, (2 * R - 0.5f));

                            if (point.X < point2.X)
                            {
                                resultPoints.Add(point);
                            }
                            else
                            {
                                resultPoints.Add(point2);
                            }
                        }
                        resultPoints.Add(currPoint);
                    }
                }
            }
            resultPoints.Add(resultPoints.LastOrDefault() + new Vector3(0, 30, 0));

            resultPoints.Add(new Vector3(0, 30, 0));
            resultPoints.Add(new Vector3(90, 30, -90));
            resultPoints.Add(new Vector3(90, 0, -90));
            //for (float i = -75.0f; i <= 75.0f; i += (R - 0.1f))
            //{
            //    Vector3 firstVec = new Vector3();
            //    bool first = true;
            //    for (float j = 75.0f; j > 0.00f; j -= (0.5f * R - 0.1f))
            //    {
            //        Vector3 currPoint = new Vector3(j, 0, i);
            //        if (first)
            //        {
            //            first = false;
            //            firstVec = currPoint;
            //        }

            //        foreach (var p in roundPoints)
            //        {
            //            if ((currPoint - p).Length < 0.3f * R)
            //            {
            //                j = 0;
            //            }
            //        }

            //        if (j != 0)
            //        {
            //            resultPoints.Add(currPoint);
            //        }
            //    }

            //    resultPoints.Add(firstVec);
            //}
            go = false;
            for (float i = -75.0f; i <= 80.0f; i += (2 * R - 0.5f))
            {
                go = !go;
                bool first = true;
                for (float j = 75.0f; j > 0.00f; j -= (0.5f * R - 0.1f))
                {
                    Vector3 currPoint = new Vector3(j, 0, i);
                    Vector2 currPoint2 = new Vector2(j, i);
                    if (!go)
                    {
                        currPoint = new Vector3(75.0f - j, 0, i);
                        currPoint2 = new Vector2(75.0f - j, i);
                    }

                    if (!HelpFunctions.IsInPolygon(currPoint2, roundPoints2) && !IsIn(currPoint, roundPoints, R))
                    {

                        if (first)
                        {
                            first = false;
                            var point = currPoint;
                            point -= new Vector3(0, 0, (2 * R - 0.5f));

                            var point2 = resultPoints.LastOrDefault();
                            point2 += new Vector3(0, 0, (2 * R - 0.5f));

                            if (point.X > point2.X)
                            {
                                resultPoints.Add(point);
                            }
                            else
                            {
                                resultPoints.Add(point2);
                            }
                        }
                        resultPoints.Add(currPoint);
                    }
                }
            }

            resultPoints.Add(resultPoints.LastOrDefault() + new Vector3(0, 30, 0));

            resultPoints.Add(new Vector3(0, 30, 0));
            resultPoints.Add(new Vector3(0, 30, -90));
            resultPoints.Add(new Vector3(0, 0, -90));
            resultPoints.AddRange(roundPoints);
            resultPoints.Add(new Vector3(roundPoints.LastOrDefault().X, 30, roundPoints.LastOrDefault().Z));
            resultPoints.Add(new Vector3(0, 30, 0));

            int numer = 0;
            List<string> pointsall = new List<string>();
            foreach (var pp in resultPoints)
            {
                var ppp = pp + new Vector3(0, 15.0f, 0);
                pointsall.Add("N3G" + numer.ToString() + "X" + ppp.X.ToString() + "Y" + (-ppp.Z).ToString() + "Z" + ppp.Y.ToString());
                numer++;
            }

            using (StreamWriter file = new StreamWriter("C://Users//User//Documents//New folder//t3.f10", append: false))
            {
                foreach (var line in pointsall)
                {
                    file.WriteLine(line);
                }
            }
        }

        private static bool IsIn(Vector3 currPoint, List<Vector3> roundPoints, float R)
        {
            foreach (var p in roundPoints)
            {
                if ((currPoint - p).Length < 0.3f * R)
                {
                    return true;
                }
            }

            return false;
        }

        private static List<Vector3> GeneratePointsFromTo(float uStart, int patchStart, float uEnd, int patchEnd, List<Vector3> points, float R)
        {
            List<Vector3> resultPoints = new List<Vector3>();
            List<Vector3> processingPoints = points;
            bool start = true;
            float startU = uStart;
            for (int patchNo = patchStart; patchNo < processingPoints.Count / 4; patchNo++)
            {
                int k = 4 * patchNo;
                if (start)
                {
                    start = false;
                }
                else
                {
                    startU = 0.00f;
                }

                for (float i = startU; (i <= 1 && patchNo < patchEnd) || (i <= uEnd && patchNo == patchEnd); i += 0.01f)
                {
                    float X = HelpFunctions.DeKastilio(new float[] { processingPoints[k].X, processingPoints[k + 1].X, processingPoints[k + 2].X, processingPoints[k + 3].X }, i, 4);
                    float Y = HelpFunctions.DeKastilio(new float[] { processingPoints[k].Y, processingPoints[k + 1].Y, processingPoints[k + 2].Y, processingPoints[k + 3].Y }, i, 4);
                    float Z = HelpFunctions.DeKastilio(new float[] { processingPoints[k].Z, processingPoints[k + 1].Z, processingPoints[k + 2].Z, processingPoints[k + 3].Z }, i, 4);
                    Vector3 currentPoint = new Vector3(X, Y, Z);

                    Vector3 prev = new Vector3(HelpFunctions.DeKastilio(new float[] { processingPoints[k].X, processingPoints[k + 1].X, processingPoints[k + 2].X, processingPoints[k + 3].X }, i - 0.001f, 4),
                        HelpFunctions.DeKastilio(new float[] { processingPoints[k].Y, processingPoints[k + 1].Y, processingPoints[k + 2].Y, processingPoints[k + 3].Y }, i - 0.001f, 4),
                        HelpFunctions.DeKastilio(new float[] { processingPoints[k].Z, processingPoints[k + 1].Z, processingPoints[k + 2].Z, processingPoints[k + 3].Z }, i - 0.001f, 4));
                    Vector3 post = new Vector3(HelpFunctions.DeKastilio(new float[] { processingPoints[k].X, processingPoints[k + 1].X, processingPoints[k + 2].X, processingPoints[k + 3].X }, i + 0.001f, 4),
                       HelpFunctions.DeKastilio(new float[] { processingPoints[k].Y, processingPoints[k + 1].Y, processingPoints[k + 2].Y, processingPoints[k + 3].Y }, i + 0.001f, 4),
                       HelpFunctions.DeKastilio(new float[] { processingPoints[k].Z, processingPoints[k + 1].Z, processingPoints[k + 2].Z, processingPoints[k + 3].Z }, i + 0.001f, 4));


                    Vector3 tangent = (prev - post).Normalized();
                    Vector3 bitangent = new Vector3(0, 1, 0);
                    Vector3 normal = Vector3.Cross(tangent, bitangent);

                    currentPoint += (normal * R);
                    resultPoints.Add(currentPoint);
                }
            }

            return resultPoints;
        }

        private static (List<Vector3> rightSide, List<Vector3> leftSide) GetRound(ModelGeneration.BezierPatchC0 patch, bool isBigger)
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

            List<Vector3> _rightSide;
            List<Vector3> _leftSide;
            if (isBigger)
            {
                _rightSide = notRepeatPoints.Where(x => x.X >= 0).ToList();
                _leftSide = notRepeatPoints.Where(x => x.X <= 0).ToList();
            }
            else
            {
                float maxX = notRepeatPoints.Max(x => x.X);
                Vector3 dividePoint = notRepeatPoints.Where(x => x.X == maxX).FirstOrDefault();
                _rightSide = notRepeatPoints.Where(x => x.Z <= dividePoint.Z).ToList();
                _leftSide = notRepeatPoints.Where(x => x.Z >= dividePoint.Z).ToList();
            }

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

    }
}
