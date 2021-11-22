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
    public static class DrillAcurate
    {

        public static void DrillAndSave(List<ModelGeneration.BezierPatchC0> patchC0)
        {
            float R = 4.0f;
            List<string> pointsall = new List<string>();
            //List<List<Patch>> patchesAll = new List<List<Patch>>();

            //float[,] topLayer = new float[300, 300];
            //foreach (var _patchC0 in patchC0)
            //{
            //    List<Patch> patches = new List<Patch>();
            //    int startIndex = 0;
            //    int prevPatchnumber = patches.Count;
            //    int patchNumber = 0 + prevPatchnumber;
            //    for (int i = 0; i < _patchC0.splitA * _patchC0.splitB; i++)
            //    {
            //        patches.Add(new Patch());
            //    }

            //    for (int j = 0; j < _patchC0.splitA; j++)
            //    {
            //        for (int k = 0; k < 4; k++)
            //        {
            //            patchNumber = _patchC0.splitB * j + prevPatchnumber;
            //            for (int i = 0; i < _patchC0.splitB; i++)
            //            {
            //                patches[patchNumber].points.Add(_patchC0.bezierPoints[startIndex].Position()); startIndex++;
            //                patches[patchNumber].points.Add(_patchC0.bezierPoints[startIndex].Position()); startIndex++;
            //                patches[patchNumber].points.Add(_patchC0.bezierPoints[startIndex].Position()); startIndex++;
            //                patches[patchNumber].points.Add(_patchC0.bezierPoints[startIndex].Position());

            //                patchNumber++;
            //            }
            //            startIndex++;
            //        }
            //        startIndex -= (_patchC0.splitB * 3 + 1);
            //    }

            //    patchesAll.Add(patches);
            //}

            List<Vector3> allPoints = new List<Vector3>();
            var top = patchC0[1];
            var topTube = patchC0[0];

            var deepTube = patchC0[2];

            BestPatch topPatchLeft = new BestPatch(top, 0, false);
            BestPatch topPatchRight = new BestPatch(top, 3, true);

            BestPatch deepTubeLeft = new BestPatch(deepTube, 0, false);
            BestPatch deepTubeRight = new BestPatch(deepTube, 3, true);

            BestPatch tube = new BestPatch(topTube);

            //DEEEP!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            var DeepLeftTopRight = FindIntersectPoints(deepTubeLeft, topPatchRight);
            var DeepLeftTopLeft = FindIntersectPoints(deepTubeLeft, topPatchLeft);

            var DeepRightTopRight = FindIntersectPoints(deepTubeRight, topPatchRight);
            var DeepRightTopLeft = FindIntersectPoints(deepTubeRight, topPatchLeft);

            List<(float, float)> intersectUVDeepLeft_RightTop = new List<(float, float)>();
            List<(float, float)> intersectUVDeepLeft_LeftTop = new List<(float, float)>();
            List<(float, float)> intersectUVDeepRight_RightTop = new List<(float, float)>();
            List<(float, float)> intersectUVDeepRight_LeftTop = new List<(float, float)>();

            List<(float, float)> TOPintersectUVDeepLeft_RightTop = new List<(float, float)>();
            List<(float, float)> TOPintersectUVDeepLeft_LeftTop = new List<(float, float)>();
            List<(float, float)> TOPintersectUVDeepRight_RightTop = new List<(float, float)>();
            List<(float, float)> TOPintersectUVDeepRight_LeftTop = new List<(float, float)>();

            foreach (var inter in DeepLeftTopRight.Item2)
            {
                intersectUVDeepLeft_RightTop.Add((inter.pParam.X, inter.pParam.Y));
                TOPintersectUVDeepLeft_RightTop.Add((inter.qParam.X, inter.qParam.Y));
            }

            foreach (var inter in DeepLeftTopLeft.Item2)
            {
                intersectUVDeepLeft_LeftTop.Add((inter.pParam.X, inter.pParam.Y));
                TOPintersectUVDeepLeft_LeftTop.Add((inter.qParam.X, inter.qParam.Y));
            }

            foreach (var inter in DeepRightTopRight.Item2)
            {
                intersectUVDeepRight_RightTop.Add((inter.pParam.X, inter.pParam.Y));
                TOPintersectUVDeepRight_RightTop.Add((inter.qParam.X, inter.qParam.Y));
            }

            foreach (var inter in DeepRightTopLeft.Item2)
            {
                intersectUVDeepRight_LeftTop.Add((inter.pParam.X, inter.pParam.Y));
                TOPintersectUVDeepRight_LeftTop.Add((inter.qParam.X, inter.qParam.Y));
            }
            //!!!!!!!!!!!!!!!!!!!!!!!

            var LeftTopIntersect = FindIntersectPoints(tube, topPatchLeft);
            var RightTopIntersect = FindIntersectPoints(tube, topPatchRight);

            var topPoints = GetTopPoints(top);
            var intersectpoint1 = GetIntersectionPoint(topPoints, tube, R);
            topPoints.Reverse();
            var intersectpoint2 = GetIntersectionPoint(topPoints, tube, R);

            List<(float, float)> intersectUVTube = new List<(float, float)>();
            List<(float, float)> intersectUVLeft = new List<(float, float)>();
            List<(float, float)> intersectUVRight = new List<(float, float)>();

            intersectUVTube.Add((intersectpoint1.Item2.uSlave, intersectpoint1.Item2.vSlave));
            foreach (var inter in LeftTopIntersect.Item2)
            {
                intersectUVTube.Add((inter.pParam.X, inter.pParam.Y));
                intersectUVLeft.Add((inter.qParam.X, inter.qParam.Y));
            }
            intersectUVTube.Add((intersectpoint2.Item2.uSlave, intersectpoint2.Item2.vSlave));

            foreach (var inter in RightTopIntersect.Item2)
            {
                intersectUVTube.Add((inter.pParam.X, inter.pParam.Y));
                intersectUVRight.Add((inter.qParam.X, inter.qParam.Y));
            }
            intersectUVTube.Add((intersectpoint1.Item2.uSlave, intersectpoint1.Item2.vSlave));


            //LeftSide
            Vector2[] pointsLeftSide1 = new Vector2[intersectUVLeft.Count];
            Vector2[] pointsLeftSide2 = new Vector2[TOPintersectUVDeepLeft_LeftTop.Count + TOPintersectUVDeepRight_LeftTop.Count];
            //Vector2[] pointsLeftSide3 = new Vector2[TOPintersectUVDeepRight_LeftTop.Count];
            int idx = 0;
            foreach (var inter in intersectUVLeft)
            {
                pointsLeftSide1[idx] = new Vector2(inter.Item1, inter.Item2);
                idx++;
            }
            idx = 0;
            foreach (var inter in TOPintersectUVDeepLeft_LeftTop)
            {
                pointsLeftSide2[idx] = new Vector2(inter.Item1, inter.Item2);
                idx++;

            }
            foreach (var inter in TOPintersectUVDeepRight_LeftTop)
            {
                pointsLeftSide2[idx] = new Vector2(inter.Item1, inter.Item2);
                idx++;
            }

            Vector2[] pointsRightSide1 = new Vector2[intersectUVRight.Count];
            Vector2[] pointsRightSide2 = new Vector2[TOPintersectUVDeepLeft_RightTop.Count + TOPintersectUVDeepRight_RightTop.Count];
            //Vector2[] pointsRightSide3 = new Vector2[TOPintersectUVDeepRight_RightTop.Count];
            idx = 0;
            foreach (var inter in intersectUVRight)
            {
                pointsRightSide1[idx] = new Vector2(inter.Item1, inter.Item2);
                idx++;
            }
            idx = 0;
            foreach (var inter in TOPintersectUVDeepLeft_RightTop)
            {
                pointsRightSide2[idx] = new Vector2(inter.Item1, inter.Item2);
                idx++;
            }
            foreach (var inter in TOPintersectUVDeepRight_RightTop)
            {
                pointsRightSide2[idx] = new Vector2(inter.Item1, inter.Item2);
                idx++;
            }


            //bool go = false;
            //float ux = 0.00f;
            //allPoints.Add(new Vector3(0, 30, 0));
            //allPoints.Add(new Vector3(5, 30, 5));
            //allPoints.Add(new Vector3(5, 20, 5));
            for (float u = 0.00f; u <= 1.0f; u += 0.02f)
            {
                bool first1 = true;
                bool first2 = true;
                for (float v = 0.00f; v <= 1.0f; v += 0.005f)
                {
                    if (!HelpFunctions.IsInPolygon(new Vector2(u, v), pointsLeftSide1) && !HelpFunctions.IsInPolygon(new Vector2(u, v), pointsLeftSide2))
                    {
                        Vector3 resVector = topPatchLeft.P(u, v);
                        if (resVector.Y >= 0.0f)
                        {
                            if (first1)
                            {
                                first1 = false;
                                resVector += new Vector3(0, 30, 0);
                            }
                            allPoints.Add(resVector);
                        }
                    }
                    else
                    {
                        Vector3 resVector = topPatchLeft.P(u, v);
                        if (resVector.Y >= 0.0f)
                        {
                            allPoints.Add(resVector + new Vector3(0, 30, 0));
                        }
                    }
                }

                allPoints.Add(allPoints.Last() + new Vector3(0, 30, 0));

                for (float v = 0.00f; v <= 1.0f; v += 0.005f)
                {
                    if (!HelpFunctions.IsInPolygon(new Vector2(u, v), pointsRightSide1) && !HelpFunctions.IsInPolygon(new Vector2(u, v), pointsRightSide2))
                    {
                        Vector3 resVector = topPatchRight.P(u, v);
                        if (resVector.Y >= 0.0f)
                        {
                            if (first2)
                            {
                                first2 = false;
                                resVector += new Vector3(0, 30, 0);
                            }
                            allPoints.Add(resVector);
                        }
                    }
                    else
                    {
                        Vector3 resVector = topPatchRight.P(u, v);
                        if (resVector.Y >= 0.0f)
                        {
                            allPoints.Add(resVector + new Vector3(0, 30, 0));
                        }
                    }
                }

                allPoints.Add(allPoints.Last() + new Vector3(0, 30, 0));
            }

            //Toptop
            Vector2[] pointsPloygon = new Vector2[intersectUVTube.Count];
            idx = 0;
            foreach (var inter in intersectUVTube)
            {
                pointsPloygon[idx] = new Vector2(inter.Item1, inter.Item2);
                idx++;
            }

            bool go = false;
            float ux = 0.00f;
            allPoints.Add(new Vector3(0, 30, 0));
            for (float v = 0.00f; v <= 1.0f; v += 0.02f)
            {
                go = !go;
                for (float u = 0.00f; u <= 1.0f; u += 0.02f)
                {
                    if (!go)
                    {
                        ux -= 0.02f;
                    }

                    if (HelpFunctions.IsInPolygon(new Vector2(ux, v), pointsPloygon))
                    {
                        allPoints.Add(tube.P(ux, v));
                    }

                    if (go)
                    {
                        ux += 0.02f;
                    }
                }
            }
            allPoints.Add(allPoints.LastOrDefault() + new Vector3(0, 15, 0));
            allPoints.Add(new Vector3(0, 30, 0));
            //allPoints.Clear();
            foreach (var inter in intersectUVTube)
            {
                allPoints.Add(tube.P(inter.Item1, inter.Item2));
            }



            //DEEP Part Drill
            Vector2[] pointsLeftDeep = new Vector2[intersectUVDeepLeft_RightTop.Count + intersectUVDeepLeft_LeftTop.Count];
            Vector2[] pointsRightDeep = new Vector2[intersectUVDeepRight_RightTop.Count + intersectUVDeepRight_LeftTop.Count];
            idx = 0;
            foreach (var inter in intersectUVDeepLeft_RightTop)
            {
                //allPoints.Add(deepTubeLeft.P(inter.Item1, inter.Item2));
                pointsLeftDeep[idx] = new Vector2(inter.Item1, inter.Item2);
                idx++;
            }

            foreach (var inter in intersectUVDeepLeft_LeftTop)
            {
                //allPoints.Add(deepTubeLeft.P(inter.Item1, inter.Item2));
                pointsLeftDeep[idx] = new Vector2(inter.Item1, inter.Item2);
                idx++;
            }

            idx = 0;
            foreach (var inter in intersectUVDeepRight_RightTop)
            {
                //allPoints.Add(deepTubeLeft.P(inter.Item1, inter.Item2));
                pointsRightDeep[idx] = new Vector2(inter.Item1, inter.Item2);
                idx++;
            }

            foreach (var inter in intersectUVDeepRight_LeftTop)
            {
                //allPoints.Add(deepTubeLeft.P(inter.Item1, inter.Item2));
                pointsRightDeep[idx] = new Vector2(inter.Item1, inter.Item2);
                idx++;
            }

            //allPoints.Clear();
            allPoints.Add(allPoints.LastOrDefault() + new Vector3(0, 30, 0));
            allPoints.Add(new Vector3(-45, 40, 12.5f));
            for (float u = 0.00f; u <= 1.0f; u += 0.02f)
            {
                bool first1 = true;
                bool first2 = true;
                for (float v = 0.00f; v <= 1.0f; v += 0.005f)
                {
                    if (!HelpFunctions.IsInPolygon(new Vector2(u, v), pointsLeftDeep))
                    {
                        Vector3 resVector = deepTubeLeft.P(u, v);
                        if (resVector.Y > 0.0f)
                        {
                            if (first1)
                            {
                                first1 = false;
                                resVector += new Vector3(0, 30, 0);
                            }
                            allPoints.Add(resVector);
                        }
                    }
                    else
                    {
                        Vector3 resVector = deepTubeLeft.P(u, v);
                        if (resVector.Y > 0.0f)
                        {
                            allPoints.Add(resVector + new Vector3(0, 30, 0));
                        }
                    }
                }

                allPoints.Add(allPoints.Last() + new Vector3(0, 30, 0));

                for (float v = 0.00f; v <= 1.0f; v += 0.005f)
                {
                    if (!HelpFunctions.IsInPolygon(new Vector2(u, v), pointsRightDeep))
                    {
                        Vector3 resVector = deepTubeRight.P(u, v);
                        if (resVector.Y > 0.0f)
                        {
                            if (first2)
                            {
                                first2 = false;
                                resVector += new Vector3(0, 30, 0);
                            }
                            allPoints.Add(resVector);
                        }
                    }
                    else
                    {
                        Vector3 resVector = deepTubeRight.P(u, v);
                        if (resVector.Y > 0.0f)
                        {
                            allPoints.Add(resVector + new Vector3(0, 30, 0));
                        }
                    }
                }

                allPoints.Add(allPoints.Last() + new Vector3(0, 30, 0));

                if (allPoints.LastOrDefault().Y > 50f)
                {
                    break;
                }
            }
            allPoints.Add(new Vector3(0, 50, 0));

            //End
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

        private static List<Vector3> GetTopPoints(ModelGeneration.BezierPatchC0 patch)
        {
            List<Vector3> zeroPoints = new List<Vector3>();
            foreach (var p in patch.bezierPoints)
            {
                Vector3 pPos = p.Position();
                if (Math.Abs(pPos.X) < 0.1f && pPos.Y > -0.1f)
                {
                    pPos.X = 0;
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

            List<Vector3> Side = new List<Vector3>();
            for (int i = 0; i < notRepeatPoints.Count; i++)
            {
                Side.Add(notRepeatPoints[i]);
                if (i % 3 == 0 && i != 0 && i != notRepeatPoints.Count - 1)
                {
                    Side.Add(notRepeatPoints[i]);
                }
            }

            return Side;
        }

        private static (List<Vector3>, List<(Vector2 pParam, Vector2 qParam)>) FindIntersectPoints(BestPatch master, BestPatch slave)
        {
            int ile = 0;
            List<Vector3> alpP = new List<Vector3>();
            List<(Vector2 pParam, Vector2 qParam)> res = new List<(Vector2, Vector2)>();
            do
            {
                res = Intersection.Curve(master, slave, 0.05f);
                ile = 0;
                alpP.Clear();
                if (res != null)
                    foreach (var x in res)
                    {

                        if (x.pParam.X >= 0 && x.pParam.Y >= 0 && x.qParam.X >= 0 && x.qParam.Y >= 0)
                        {
                            ile++;
                            alpP.Add(master.P(x.pParam.X, x.pParam.Y));
                        }

                        //if (x.qParam.X > 0)
                        //{
                        //    //ile++;
                        //    var xxx = 5;

                        //    //alpP.Add(slave.P(x.qParam.X, x.qParam.Y));

                        //}
                    }
            }
            while (res == null || ile < res.Count - 2 || ile < 100);

            return (alpP, res);
        }

        public static ((float uMaster, int patchNo), (float uSlave, float vSlave)) GetIntersectionPoint(List<Vector3> MasterLine, BestPatch patch, float R)
        {
            float smallestDist = float.MaxValue;
            float i_Add = 0.005f;
            float j_add = 0.01f;
            bool goOnce = true;
            for (int patchNo = 0; patchNo < MasterLine.Count / 4; patchNo++)
            {
                for (float i = 0.00f; i <= 1; i += i_Add)
                {
                    int k = 4 * patchNo;
                    float X = HelpFunctions.DeKastilio(new float[] { MasterLine[k].X, MasterLine[k + 1].X, MasterLine[k + 2].X, MasterLine[k + 3].X }, i, 4);
                    float Y = HelpFunctions.DeKastilio(new float[] { MasterLine[k].Y, MasterLine[k + 1].Y, MasterLine[k + 2].Y, MasterLine[k + 3].Y }, i, 4);
                    float Z = HelpFunctions.DeKastilio(new float[] { MasterLine[k].Z, MasterLine[k + 1].Z, MasterLine[k + 2].Z, MasterLine[k + 3].Z }, i, 4);
                    Vector3 currentPoint = new Vector3(X, Y, Z);

                    //Vector3 prev = new Vector3(HelpFunctions.DeKastilio(new float[] { MasterLine[k].X, MasterLine[k + 1].X, MasterLine[k + 2].X, MasterLine[k + 3].X }, i - 0.001f, 4),
                    //    HelpFunctions.DeKastilio(new float[] { MasterLine[k].Y, MasterLine[k + 1].Y, MasterLine[k + 2].Y, MasterLine[k + 3].Y }, i - 0.001f, 4),
                    //    HelpFunctions.DeKastilio(new float[] { MasterLine[k].Z, MasterLine[k + 1].Z, MasterLine[k + 2].Z, MasterLine[k + 3].Z }, i - 0.001f, 4));
                    //Vector3 post = new Vector3(HelpFunctions.DeKastilio(new float[] { MasterLine[k].X, MasterLine[k + 1].X, MasterLine[k + 2].X, MasterLine[k + 3].X }, i + 0.001f, 4),
                    //   HelpFunctions.DeKastilio(new float[] { MasterLine[k].Y, MasterLine[k + 1].Y, MasterLine[k + 2].Y, MasterLine[k + 3].Y }, i + 0.001f, 4),
                    //   HelpFunctions.DeKastilio(new float[] { MasterLine[k].Z, MasterLine[k + 1].Z, MasterLine[k + 2].Z, MasterLine[k + 3].Z }, i + 0.001f, 4));


                    //Vector3 tangent = (prev - post).Normalized();
                    //Vector3 bitangent = new Vector3(0, 0, 1);
                    //Vector3 normal =new  Vector3(0,1,0);

                    //currentPoint += (normal * R);

                    List<Vector3> ppp = new List<Vector3>();
                    //for (float v = 0.00f; v <= 1.00f; v += 0.01f)
                    float v = 0.5f;
                    {
                        for (float u = 0.00f; u <= 1.00f; u += 0.05f)
                        {

                            Vector3 currentPointSlave = patch.P(u, v);
                            ppp.Add(currentPointSlave);
                            float distance = (currentPointSlave - currentPoint).Length;
                            if (distance < smallestDist)
                            {
                                smallestDist = distance;
                            }

                            if (distance < 0.1 && goOnce)
                            {
                                return ((i, patchNo), (u, v));
                                //goOnce = false;
                                //i_Add /= 10;
                                //j_add /= 10;
                            }

                            //if (distance < 0.01)
                            //{
                            //    return ((i, patchNo), (u,v));
                            //}
                        }
                    }

                }
            }

            return ((0, 0), (0, 0));
        }
    }
}
