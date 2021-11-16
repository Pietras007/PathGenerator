using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geometric2.DrillLines
{
    public static class FindIntersection
    {
        public static ((float uMaster, int patchNo), (float uSlave, int patchNo)) GetIntersectionPoint(List<Vector3> MasterLine, List<Vector3> slaveLine, float R)
        {
            float smallestDist = float.MaxValue;
            float i_Add = 0.005f;
            float j_add = 0.005f;
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

                    Vector3 prev = new Vector3(HelpFunctions.DeKastilio(new float[] { MasterLine[k].X, MasterLine[k + 1].X, MasterLine[k + 2].X, MasterLine[k + 3].X }, i - 0.001f, 4),
                        HelpFunctions.DeKastilio(new float[] { MasterLine[k].Y, MasterLine[k + 1].Y, MasterLine[k + 2].Y, MasterLine[k + 3].Y }, i - 0.001f, 4),
                        HelpFunctions.DeKastilio(new float[] { MasterLine[k].Z, MasterLine[k + 1].Z, MasterLine[k + 2].Z, MasterLine[k + 3].Z }, i - 0.001f, 4));
                    Vector3 post = new Vector3(HelpFunctions.DeKastilio(new float[] { MasterLine[k].X, MasterLine[k + 1].X, MasterLine[k + 2].X, MasterLine[k + 3].X }, i + 0.001f, 4),
                       HelpFunctions.DeKastilio(new float[] { MasterLine[k].Y, MasterLine[k + 1].Y, MasterLine[k + 2].Y, MasterLine[k + 3].Y }, i + 0.001f, 4),
                       HelpFunctions.DeKastilio(new float[] { MasterLine[k].Z, MasterLine[k + 1].Z, MasterLine[k + 2].Z, MasterLine[k + 3].Z }, i + 0.001f, 4));


                    Vector3 tangent = (prev - post).Normalized();
                    Vector3 bitangent = new Vector3(0, 1, 0);
                    Vector3 normal = Vector3.Cross(tangent, bitangent);

                    currentPoint += (normal * R);


                    List<Vector3> processingPoints = slaveLine;
                    for (int patchNoSlave = 0; patchNoSlave < processingPoints.Count / 4; patchNoSlave++)
                    {
                        for (float j = 0.00f; j <= 1; j += j_add)
                        {
                            int kSlave = 4 * patchNoSlave;
                            float XSlave = HelpFunctions.DeKastilio(new float[] { processingPoints[kSlave].X, processingPoints[kSlave + 1].X, processingPoints[kSlave + 2].X, processingPoints[kSlave + 3].X }, j, 4);
                            float YSlave = HelpFunctions.DeKastilio(new float[] { processingPoints[kSlave].Y, processingPoints[kSlave + 1].Y, processingPoints[kSlave + 2].Y, processingPoints[kSlave + 3].Y }, j, 4);
                            float ZSlave = HelpFunctions.DeKastilio(new float[] { processingPoints[kSlave].Z, processingPoints[kSlave + 1].Z, processingPoints[kSlave + 2].Z, processingPoints[kSlave + 3].Z }, j, 4);
                            Vector3 currentPointSlave = new Vector3(XSlave, YSlave, ZSlave);

                            Vector3 prevSlave = new Vector3(HelpFunctions.DeKastilio(new float[] { processingPoints[kSlave].X, processingPoints[kSlave + 1].X, processingPoints[kSlave + 2].X, processingPoints[kSlave + 3].X }, j - 0.001f, 4),
                                HelpFunctions.DeKastilio(new float[] { processingPoints[kSlave].Y, processingPoints[kSlave + 1].Y, processingPoints[kSlave + 2].Y, processingPoints[kSlave + 3].Y }, j - 0.001f, 4),
                                HelpFunctions.DeKastilio(new float[] { processingPoints[kSlave].Z, processingPoints[kSlave + 1].Z, processingPoints[kSlave + 2].Z, processingPoints[kSlave + 3].Z }, j - 0.001f, 4));
                            Vector3 postSlave = new Vector3(HelpFunctions.DeKastilio(new float[] { processingPoints[kSlave].X, processingPoints[kSlave + 1].X, processingPoints[kSlave + 2].X, processingPoints[kSlave + 3].X }, j + 0.001f, 4),
                               HelpFunctions.DeKastilio(new float[] { processingPoints[kSlave].Y, processingPoints[kSlave + 1].Y, processingPoints[kSlave + 2].Y, processingPoints[kSlave + 3].Y }, j + 0.001f, 4),
                               HelpFunctions.DeKastilio(new float[] { processingPoints[kSlave].Z, processingPoints[kSlave + 1].Z, processingPoints[kSlave + 2].Z, processingPoints[kSlave + 3].Z }, j + 0.001f, 4));


                            Vector3 tangentSlave = (prevSlave - postSlave).Normalized();
                            Vector3 bitangentSlave = new Vector3(0, 1, 0);
                            Vector3 normalSlave = Vector3.Cross(tangentSlave, bitangentSlave);

                            currentPointSlave += (normalSlave * R);

                            float distance = (currentPointSlave - currentPoint).Length;
                            if(distance < smallestDist)
                            {
                                smallestDist = distance;
                            }

                            if(distance < 0.1 && goOnce)
                            {
                                goOnce = false;
                                i_Add /= 10;
                                j_add /= 10;
                            }

                            if (distance < 0.01)
                            {
                                return ((i, patchNo), (j, patchNoSlave));
                            }
                        }
                    }

                }
            }

            return ((0,0), (0,0));
        }
    }
}
