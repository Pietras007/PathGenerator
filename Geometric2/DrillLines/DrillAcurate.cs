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
            var top = patchC0[2];
            var topTube = patchC0[1];


            //BestPatch topPatch;// = new BestPatch(patchC0[2]);
            BestPatch topPatchLeft = new BestPatch(top, 0, false);
            BestPatch topPatchRight = new BestPatch(top, 3, true);

            //allPoints.Add(new Vector3(0, 30, 0));
            //allPoints.Add(new Vector3(0, 30, -70));
            //for (float u = 0.005f; u <= 1.0f; u += 0.01f)
            //{

            //    for (float v = 0.005f; v <= 1.0f; v += 0.01f)
            //    {
            //        allPoints.Add(topPatchRight.P(u, v));
            //    }
            //    allPoints.Add(new Vector3(0, 30, 70));
            //    allPoints.Add(new Vector3(0, 30, -70));
            //}


            BestPatch tube = new BestPatch(topTube);

            var LeftTopIntersect = FindIntersectPoints(tube, topPatchLeft);
            var RightTopIntersect = FindIntersectPoints(tube, topPatchRight);

            var topPoints = GetTopPoints(top);
            var intersectpoint1 = GetIntersectionPoint(topPoints, tube, R);
            topPoints.Reverse();
            var intersectpoint2 = GetIntersectionPoint(topPoints, tube, R);
            //allPoints.Add(tube.P(intersectpoint1.Item2.uSlave, intersectpoint1.Item2.vSlave));
            //allPoints.Add(tube.P(intersectpoint2.Item2.uSlave, intersectpoint2.Item2.vSlave));

            List<(float, float)> intersectUVTube = new List<(float, float)>();

            intersectUVTube.Add((intersectpoint1.Item2.uSlave, intersectpoint1.Item2.vSlave));
            foreach (var inter in LeftTopIntersect.Item2)
            {
                intersectUVTube.Add((inter.pParam.X, inter.pParam.Y));
            }
            intersectUVTube.Add((intersectpoint2.Item2.uSlave, intersectpoint2.Item2.vSlave));

            foreach (var inter in RightTopIntersect.Item2)
            {
                intersectUVTube.Add((inter.pParam.X, inter.pParam.Y));
            }
            intersectUVTube.Add((intersectpoint1.Item2.uSlave, intersectpoint1.Item2.vSlave));

            foreach (var inter in intersectUVTube)
            {
                allPoints.Add(tube.P(inter.Item1, inter.Item2));
            }

            ////topPoints.Reverse();
            ////var intersectpoint2 = GetIntersectionPoint(topPoints, tube, R);
            ////LeftTopIntersect.Add(tube.P(intersectpoint2.Item2.uSlave, intersectpoint2.Item2.vSlave));

            //foreach (var inter in RightTopIntersect)
            //{
            //    allPoints.Add(inter);
            //}

            //LeftTopIntersect.Add(tube.P(intersectpoint1.Item2.uSlave, intersectpoint1.Item2.vSlave));

            //allPoints.Add(new Vector3(0, 30, 0));
            //allPoints.Add(new Vector3(0, 30, -70));
            //for (float u = 0.005f; u <= 1.0f; u += 0.01f)
            //{

            //    for (float v = 0.005f; v <= 1.0f; v += 0.01f)
            //    {
            //        allPoints.Add(topPatch.P(u, v));
            //    }
            //    allPoints.Add(new Vector3(0, 30, 70));
            //    allPoints.Add(new Vector3(0, 30, -70));
            //}

            ////BestPatch tube2 = new BestPatch(patchC0[1]);
            ////BestPatch tube1 = new BestPatch(patchC0[3]);

            ////BestPatch holePart = new BestPatch(patchC0[1]);

            //for (float u = 0.005f; u <= 1.0f; u += 0.01f)
            //{

            //    for (float v = 0.005f; v <= 1.0f; v += 0.01f)
            //    {
            //        allPoints.Add(tube.P(u, v));
            //    }
            //    allPoints.Add(new Vector3(0, -100, 0));
            //}

            //for (float u = 0.005f; u <= 1.0f; u += 0.01f)
            //{

            //    for (float v = 0.005f; v <= 1.0f; v += 0.01f)
            //    {
            //        allPoints.Add(tube2.P(u, v));
            //    }
            //    allPoints.Add(new Vector3(0, -100, 0));
            //}

            //for (float u = 0.005f; u <= 1.0f; u += 0.01f)
            //{

            //    for (float v = 0.005f; v <= 1.0f; v += 0.01f)
            //    {
            //        allPoints.Add(tube1.P(u, v));
            //    }
            //    allPoints.Add(new Vector3(0, -100, 0));
            //}

            //for (float u = 0.005f; u <= 1.0f; u += 0.01f)
            //{

            //    for (float v = 0.005f; v <= 1.0f; v += 0.01f)
            //    {
            //        allPoints.Add(tube.P(u, v));
            //    }
            //    allPoints.Add(new Vector3(0, -100, 0));
            //}

            //for (float u = 0.005f; u <= 1.0f; u += 0.01f)
            //{

            //    for (float v = 0.005f; v <= 1.0f; v += 0.01f)
            //    {
            //        allPoints.Add(topPatch.P(u, v));
            //    }
            //    allPoints.Add(new Vector3(0, -100, 0));
            //}





            ////Top
            //List<Patch> topPatches = patchesAll[0];
            //List<Patch> topLeft = new List<Patch>();
            //List<Patch> topRight = new List<Patch>();
            //int idx = 0;
            //foreach (var pp in topPatches)
            //{
            //    if (idx % 4 == 0)
            //    {
            //        topLeft.Add(pp);
            //    }
            //    idx++;
            //}

            //foreach (var pp in topPatches)
            //{
            //    if (idx % 4 == 3)
            //    {
            //        topRight.Add(pp);
            //    }
            //    idx++;
            //}

            ////FirstUp
            //List<Patch> up1Patches = patchesAll[3];
            //List<Patch> up1 = new List<Patch>();
            //idx = 0;
            //foreach (var pp in up1Patches)
            //{
            //    if (idx % 4 == 0 || idx % 4 == 1)
            //    {
            //        up1.Add(pp);
            //    }
            //    idx++;
            //}

            //allPoints.Add(new Vector3(0, 30, 0));
            //allPoints.Add(new Vector3(0, 30, -70));
            //bool under = false;
            ////float dist = 
            //for (float u = 0.002f; u <= 1.0f; u += 0.04f)
            //{
            //    foreach (var topPatch in topLeft)
            //    {
            //        for (float v = 0.002f; v <= 1.0f; v += 0.04f)
            //        {
            //            var point = topPatch.P(u, v);
            //            var inter = Intersection.Point(topPatch, up1[0], point);
            //            if (inter != null && (topPatch.P(inter.Value.pParam.X, inter.Value.pParam.Y) - point).Length < 0.1f)
            //            {
            //                under = !under;
            //            }

            //            if (under)
            //            {
            //                point += new Vector3(0, 20, 0);
            //            }
            //            allPoints.Add(point);
            //        }
            //        //allPoints.Add(allPoints.LastOrDefault() + new Vector3(0, 100, 0));
            //    }
            //    allPoints.Add(new Vector3(0,30,70));
            //    allPoints.Add(new Vector3(0, 30, -70));

            //foreach (var topPatch in topRight)
            //{
            //    for (float v = 0.002f; v <= 1.0f; v += 0.025f)
            //    {
            //        allPoints.Add(topPatch.P(u, v));
            //    }
            //    //allPoints.Add(allPoints.LastOrDefault() + new Vector3(0, 100, 0));
            //}
            //allPoints.Add(new Vector3(0, 30, 70));
            //allPoints.Add(new Vector3(0, 30, -70));
            //}

            //foreach (var pp in topPatches)
            //{
            //    if (idx % 4 == 3)
            //    {
            //        top.Add(pp);
            //    }
            //    idx++;
            //}

            //Patch topPatch = topPatches[0];
            //foreach (var _topPatch in top)
            //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            //for (float u = 0.0f; u <= 1.0f; u += 0.01f)
            //{
            //    foreach (var topPatch in top)
            //    {
            //        for (float v = 0.0f; v <= 1.0f; v += 0.01f)
            //        {
            //            allPoints.Add(topPatch.P(u, v));
            //        }
            //        allPoints.Add(allPoints.LastOrDefault() + new Vector3(0, 100, 0));
            //    }
            //    allPoints.Add(allPoints.LastOrDefault() + new Vector3(0, -100, 0));
            //}

            //for (float u = 0.0f; u <= 1.0f; u += 0.01f)
            //{
            //    foreach (var topPatch in up1)
            //    {
            //        for (float v = 0.0f; v <= 1.0f; v += 0.01f)
            //        {
            //            allPoints.Add(topPatch.P(u, v));
            //        }
            //        //allPoints.Add(allPoints.LastOrDefault() + new Vector3(0, 100, 0));
            //    }
            //    allPoints.Add(allPoints.LastOrDefault() + new Vector3(0, -100, 0));
            //}
            //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

            //for (float u = 0.0f; u <= 1.0f; u += 0.01f)
            //{
            //    for (float v = 0.0f; v <= 1.0f; v += 0.01f)
            //    {
            //        allPoints.Add(top[3].P(u, v));
            //    }
            //    allPoints.Add(allPoints.LastOrDefault() + new Vector3(0, -100, 0));
            //    //allPoints.Add(allPoints.LastOrDefault() + new Vector3(0, 100, 0));
            //}

            //Patch patch = up1[3];
            //Patch first = up1[0];
            //foreach (var _first in up1Patches)
            //    for (float u = 0.005f; u <= 1.0f; u += 0.01f)
            //    {
            //        foreach (var first in up1Patches)
            //        {
            //            for (float v = 0.005f; v <= 1.0f; v += 0.01f)
            //            {
            //                allPoints.Add(first.P(u,v));
            //            }
            //            allPoints.Add(allPoints.LastOrDefault() + new Vector3(0, 100, 0));
            //        }
            //        //allPoints.Add(allPoints.LastOrDefault() + new Vector3(0, 100, 0));
            //    }


            //for (float u = 0.005f; u <= 1.0f; u += 0.01f)
            //{
            //    for (float v = 0.005f; v <= 1.0f; v += 0.01f)
            //    {
            //        allPoints.Add(top[3].P(u, v));
            //    }
            //    allPoints.Add(allPoints.LastOrDefault() + new Vector3(0, -100, 0));
            //}

            //for (float u = 0.00f; u <= 1.0f; u += 0.01f)
            //{
            //    for (float v = 0.00f; v <= 1.0f; v += 0.01f)
            //    {
            //        allPoints.Add(up1[0].P(u, v));
            //    }
            //    allPoints.Add(allPoints.LastOrDefault() + new Vector3(0, -100, 0));
            //}

            //for (float u = 0.00f; u <= 1.0f; u += 0.01f)
            //{
            //    for (float v = 0.00f; v <= 1.0f; v += 0.01f)
            //    {
            //        allPoints.Add(up1[1].P(u, v));
            //    }
            //    allPoints.Add(allPoints.LastOrDefault() + new Vector3(0, -100, 0));
            //}

            //allPoints.AddRange(FindIntersectPoints(top[4], up1[0]));
            ////allPoints.AddRange(FindIntersectPoints(top[3], up1[0]));
            //allPoints.AddRange(FindIntersectPoints(top[3], up1[1]));
            //allPoints.AddRange(FindIntersectPoints(top[3], up1[1]));
            //int ile = 0;
            //List<Vector3> alpP = new List<Vector3>();
            //List<(Vector2 pParam, Vector2 qParam) > res = new List<(Vector2, Vector2)>();
            //do
            //{
            //    res = Intersection.Curve(top[3], up1[0], 0.01f);
            //    ile = 0;
            //    alpP.Clear();
            //    if(res != null)
            //    foreach (var x in res)
            //    {

            //            if(x.pParam.X > 0)
            //            {
            //                ile++;
            //                //alpP.Add(top[3].P(x.pParam.X, x.pParam.Y));
            //            }

            //            if (x.qParam.X > 0)
            //        {
            //            var xxx = 5;

            //                alpP.Add(up1[0].P(x.qParam.X, x.qParam.Y));

            //        }
            //    }
            //}
            //while (ile < ((float)res.Count - 0.1*res.Count));

            //allPoints.AddRange(alpP);

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
                res = Intersection.Curve(master, slave, 0.02f);
                ile = 0;
                alpP.Clear();
                if (res != null)
                    foreach (var x in res)
                    {

                        if (x.pParam.X > 0)
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
            while (res == null || ile < res.Count);

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
