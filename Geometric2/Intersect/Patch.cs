using Geometric2.DrillLines;
using Geometric2.RasterizationClasses;
using Intersect;
using OpenTK;
using System;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL4;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geometric2.Intersect
{
    public class Patch : ISurface
    {
        public List<Vector3> points = new List<Vector3>();
        private float R = 4.0f;
        public Vector3 P(float u, float v)
        {
            //Vector3 four1 = new Vector3();
            //Vector3 four2 = new Vector3();
            //Vector3 four3 = new Vector3();
            //Vector3 four4 = new Vector3();


            //four1.X = HelpFunctions.DeKastilio(new float[] { points[0].X, points[1].X, points[2].X, points[3].X }, u, 4);
            //four1.Y = HelpFunctions.DeKastilio(new float[] { points[0].Y, points[1].Y, points[2].Y, points[3].Y }, u, 4);
            //four1.Z = HelpFunctions.DeKastilio(new float[] { points[0].Z, points[1].Z, points[2].Z, points[3].Z }, u, 4);

            //four2.X = HelpFunctions.DeKastilio(new float[] { points[4 + 0].X, points[4 + 1].X, points[4 + 2].X, points[4 + 3].X }, u, 4);
            //four2.Y = HelpFunctions.DeKastilio(new float[] { points[4 + 0].Y, points[4 + 1].Y, points[4 + 2].Y, points[4 + 3].Y }, u, 4);
            //four2.Z = HelpFunctions.DeKastilio(new float[] { points[4 + 0].Z, points[4 + 1].Z, points[4 + 2].Z, points[4 + 3].Z }, u, 4);

            //four3.X = HelpFunctions.DeKastilio(new float[] { points[8 + 0].X, points[8 + 1].X, points[8 + 2].X, points[8 + 3].X }, u, 4);
            //four3.Y = HelpFunctions.DeKastilio(new float[] { points[8 + 0].Y, points[8 + 1].Y, points[8 + 2].Y, points[8 + 3].Y }, u, 4);
            //four3.Z = HelpFunctions.DeKastilio(new float[] { points[8 + 0].Z, points[8 + 1].Z, points[8 + 2].Z, points[8 + 3].Z }, u, 4);

            //four4.X = HelpFunctions.DeKastilio(new float[] { points[12 + 0].X, points[12 + 1].X, points[12 + 2].X, points[12 + 3].X }, u, 4);
            //four4.Y = HelpFunctions.DeKastilio(new float[] { points[12 + 0].Y, points[12 + 1].Y, points[12 + 2].Y, points[12 + 3].Y }, u, 4);
            //four4.Z = HelpFunctions.DeKastilio(new float[] { points[12 + 0].Z, points[12 + 1].Z, points[12 + 2].Z, points[12 + 3].Z }, u, 4);

            Vector3 currentPoint = CP(u, v);// new Vector3(HelpFunctions.DeKastilio(new float[] { four1.X, four2.X, four3.X, four4.X }, v, 4),
                                            //HelpFunctions.DeKastilio(new float[] { four1.Y, four2.Y, four3.Y, four4.Y }, v, 4),
                                            //HelpFunctions.DeKastilio(new float[] { four1.Z, four2.Z, four3.Z, four4.Z }, v, 4));

            //Vector3 prev = new Vector3(HelpFunctions.DeKastilio(new float[] { four1.X, four2.X, four3.X, four4.X }, v - 0.001f, 4),
            //    HelpFunctions.DeKastilio(new float[] { four1.Y, four2.Y, four3.Y, four4.Y }, v - 0.001f, 4),
            //    HelpFunctions.DeKastilio(new float[] { four1.Z, four2.Z, four3.Z, four4.Z }, v - 0.001f, 4));

            //Vector3 post = new Vector3(HelpFunctions.DeKastilio(new float[] { four1.X, four2.X, four3.X, four4.X }, v + 0.001f, 4),
            //    HelpFunctions.DeKastilio(new float[] { four1.Y, four2.Y, four3.Y, four4.Y }, v + 0.001f, 4),
            //    HelpFunctions.DeKastilio(new float[] { four1.Z, four2.Z, four3.Z, four4.Z }, v + 0.001f, 4));

            //Vector3 tangent = T(u, v);// (prev - post);
            //Vector3 bitangent = B(u, v);
            //Vector3 normal = Vector3.Cross(tangent, bitangent).Normalized();

            //currentPoint += (-normal * R);
            //currentPoint -= new Vector3(0, R, 0);

            return currentPoint;
        }

        public Vector3 CP(float u, float v)
        {
            Vector3 four1 = new Vector3();
            Vector3 four2 = new Vector3();
            Vector3 four3 = new Vector3();
            Vector3 four4 = new Vector3();


            four1.X = HelpFunctions.DeKastilio(new float[] { points[0].X, points[1].X, points[2].X, points[3].X }, u, 4);
            four1.Y = HelpFunctions.DeKastilio(new float[] { points[0].Y, points[1].Y, points[2].Y, points[3].Y }, u, 4);
            four1.Z = HelpFunctions.DeKastilio(new float[] { points[0].Z, points[1].Z, points[2].Z, points[3].Z }, u, 4);

            four2.X = HelpFunctions.DeKastilio(new float[] { points[4 + 0].X, points[4 + 1].X, points[4 + 2].X, points[4 + 3].X }, u, 4);
            four2.Y = HelpFunctions.DeKastilio(new float[] { points[4 + 0].Y, points[4 + 1].Y, points[4 + 2].Y, points[4 + 3].Y }, u, 4);
            four2.Z = HelpFunctions.DeKastilio(new float[] { points[4 + 0].Z, points[4 + 1].Z, points[4 + 2].Z, points[4 + 3].Z }, u, 4);

            four3.X = HelpFunctions.DeKastilio(new float[] { points[8 + 0].X, points[8 + 1].X, points[8 + 2].X, points[8 + 3].X }, u, 4);
            four3.Y = HelpFunctions.DeKastilio(new float[] { points[8 + 0].Y, points[8 + 1].Y, points[8 + 2].Y, points[8 + 3].Y }, u, 4);
            four3.Z = HelpFunctions.DeKastilio(new float[] { points[8 + 0].Z, points[8 + 1].Z, points[8 + 2].Z, points[8 + 3].Z }, u, 4);

            four4.X = HelpFunctions.DeKastilio(new float[] { points[12 + 0].X, points[12 + 1].X, points[12 + 2].X, points[12 + 3].X }, u, 4);
            four4.Y = HelpFunctions.DeKastilio(new float[] { points[12 + 0].Y, points[12 + 1].Y, points[12 + 2].Y, points[12 + 3].Y }, u, 4);
            four4.Z = HelpFunctions.DeKastilio(new float[] { points[12 + 0].Z, points[12 + 1].Z, points[12 + 2].Z, points[12 + 3].Z }, u, 4);

            Vector3 currentPoint = new Vector3(HelpFunctions.DeKastilio(new float[] { four1.X, four2.X, four3.X, four4.X }, v, 4),
                HelpFunctions.DeKastilio(new float[] { four1.Y, four2.Y, four3.Y, four4.Y }, v, 4),
                HelpFunctions.DeKastilio(new float[] { four1.Z, four2.Z, four3.Z, four4.Z }, v, 4));

            return currentPoint;
        }

        public Vector3 B(float u, float v)
        {
            if (v + 1e-4f < 1)
                return (CP(u, v + 1e-4f) - CP(u, v)) / (1e-4f);
            else
                return (CP(u, v) - CP(u, v - 1e-4f)) / (1e-4f);


            //Vector3 four1Prev = new Vector3();
            //Vector3 four2Prev = new Vector3();
            //Vector3 four3Prev = new Vector3();
            //Vector3 four4Prev = new Vector3();

            //four1Prev.X = HelpFunctions.DeKastilio(new float[] { points[0].X, points[1].X, points[2].X, points[3].X }, u - 0.001f, 4);
            //four1Prev.Y = HelpFunctions.DeKastilio(new float[] { points[0].Y, points[1].Y, points[2].Y, points[3].Y }, u - 0.001f, 4);
            //four1Prev.Z = HelpFunctions.DeKastilio(new float[] { points[0].Z, points[1].Z, points[2].Z, points[3].Z }, u - 0.001f, 4);

            //four2Prev.X = HelpFunctions.DeKastilio(new float[] { points[4 + 0].X, points[4 + 1].X, points[4 + 2].X, points[4 + 3].X }, u - 0.001f, 4);
            //four2Prev.Y = HelpFunctions.DeKastilio(new float[] { points[4 + 0].Y, points[4 + 1].Y, points[4 + 2].Y, points[4 + 3].Y }, u - 0.001f, 4);
            //four2Prev.Z = HelpFunctions.DeKastilio(new float[] { points[4 + 0].Z, points[4 + 1].Z, points[4 + 2].Z, points[4 + 3].Z }, u - 0.001f, 4);

            //four3Prev.X = HelpFunctions.DeKastilio(new float[] { points[8 + 0].X, points[8 + 1].X, points[8 + 2].X, points[8 + 3].X }, u - 0.001f, 4);
            //four3Prev.Y = HelpFunctions.DeKastilio(new float[] { points[8 + 0].Y, points[8 + 1].Y, points[8 + 2].Y, points[8 + 3].Y }, u - 0.001f, 4);
            //four3Prev.Z = HelpFunctions.DeKastilio(new float[] { points[8 + 0].Z, points[8 + 1].Z, points[8 + 2].Z, points[8 + 3].Z }, u - 0.001f, 4);

            //four4Prev.X = HelpFunctions.DeKastilio(new float[] { points[12 + 0].X, points[12 + 1].X, points[12 + 2].X, points[12 + 3].X }, u - 0.001f, 4);
            //four4Prev.Y = HelpFunctions.DeKastilio(new float[] { points[12 + 0].Y, points[12 + 1].Y, points[12 + 2].Y, points[12 + 3].Y }, u - 0.001f, 4);
            //four4Prev.Z = HelpFunctions.DeKastilio(new float[] { points[12 + 0].Z, points[12 + 1].Z, points[12 + 2].Z, points[12 + 3].Z }, u - 0.001f, 4);

            //Vector3 four1Post = new Vector3();
            //Vector3 four2Post = new Vector3();
            //Vector3 four3Post = new Vector3();
            //Vector3 four4Post = new Vector3();

            //four1Post.X = HelpFunctions.DeKastilio(new float[] { points[0].X, points[1].X, points[2].X, points[3].X }, u + 0.001f, 4);
            //four1Post.Y = HelpFunctions.DeKastilio(new float[] { points[0].Y, points[1].Y, points[2].Y, points[3].Y }, u + 0.001f, 4);
            //four1Post.Z = HelpFunctions.DeKastilio(new float[] { points[0].Z, points[1].Z, points[2].Z, points[3].Z }, u + 0.001f, 4);

            //four2Post.X = HelpFunctions.DeKastilio(new float[] { points[4 + 0].X, points[4 + 1].X, points[4 + 2].X, points[4 + 3].X }, u + 0.001f, 4);
            //four2Post.Y = HelpFunctions.DeKastilio(new float[] { points[4 + 0].Y, points[4 + 1].Y, points[4 + 2].Y, points[4 + 3].Y }, u + 0.001f, 4);
            //four2Post.Z = HelpFunctions.DeKastilio(new float[] { points[4 + 0].Z, points[4 + 1].Z, points[4 + 2].Z, points[4 + 3].Z }, u + 0.001f, 4);

            //four3Post.X = HelpFunctions.DeKastilio(new float[] { points[8 + 0].X, points[8 + 1].X, points[8 + 2].X, points[8 + 3].X }, u + 0.001f, 4);
            //four3Post.Y = HelpFunctions.DeKastilio(new float[] { points[8 + 0].Y, points[8 + 1].Y, points[8 + 2].Y, points[8 + 3].Y }, u + 0.001f, 4);
            //four3Post.Z = HelpFunctions.DeKastilio(new float[] { points[8 + 0].Z, points[8 + 1].Z, points[8 + 2].Z, points[8 + 3].Z }, u + 0.001f, 4);

            //four4Post.X = HelpFunctions.DeKastilio(new float[] { points[12 + 0].X, points[12 + 1].X, points[12 + 2].X, points[12 + 3].X }, u + 0.001f, 4);
            //four4Post.Y = HelpFunctions.DeKastilio(new float[] { points[12 + 0].Y, points[12 + 1].Y, points[12 + 2].Y, points[12 + 3].Y }, u + 0.001f, 4);
            //four4Post.Z = HelpFunctions.DeKastilio(new float[] { points[12 + 0].Z, points[12 + 1].Z, points[12 + 2].Z, points[12 + 3].Z }, u + 0.001f, 4);


            //Vector3 prev = new Vector3(HelpFunctions.DeKastilio(new float[] { four1Prev.X, four2Prev.X, four3Prev.X, four4Prev.X }, v, 4),
            //    HelpFunctions.DeKastilio(new float[] { four1Prev.Y, four2Prev.Y, four3Prev.Y, four4Prev.Y }, v, 4),
            //    HelpFunctions.DeKastilio(new float[] { four1Prev.Z, four2Prev.Z, four3Prev.Z, four4Prev.Z }, v, 4));

            //Vector3 post = new Vector3(HelpFunctions.DeKastilio(new float[] { four1Post.X, four1Post.X, four1Post.X, four1Post.X }, v, 4),
            //    HelpFunctions.DeKastilio(new float[] { four1Post.Y, four1Post.Y, four1Post.Y, four1Post.Y }, v, 4),
            //    HelpFunctions.DeKastilio(new float[] { four1Post.Z, four1Post.Z, four1Post.Z, four1Post.Z }, v, 4));

            //Vector3 bitangent = (prev - post);
            //return bitangent;
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


            //Vector3 four1 = new Vector3();
            //Vector3 four2 = new Vector3();
            //Vector3 four3 = new Vector3();
            //Vector3 four4 = new Vector3();


            //four1.X = HelpFunctions.DeKastilio(new float[] { points[0].X, points[1].X, points[2].X, points[3].X }, u, 4);
            //four1.Y = HelpFunctions.DeKastilio(new float[] { points[0].Y, points[1].Y, points[2].Y, points[3].Y }, u, 4);
            //four1.Z = HelpFunctions.DeKastilio(new float[] { points[0].Z, points[1].Z, points[2].Z, points[3].Z }, u, 4);

            //four2.X = HelpFunctions.DeKastilio(new float[] { points[4 + 0].X, points[4 + 1].X, points[4 + 2].X, points[4 + 3].X }, u, 4);
            //four2.Y = HelpFunctions.DeKastilio(new float[] { points[4 + 0].Y, points[4 + 1].Y, points[4 + 2].Y, points[4 + 3].Y }, u, 4);
            //four2.Z = HelpFunctions.DeKastilio(new float[] { points[4 + 0].Z, points[4 + 1].Z, points[4 + 2].Z, points[4 + 3].Z }, u, 4);

            //four3.X = HelpFunctions.DeKastilio(new float[] { points[8 + 0].X, points[8 + 1].X, points[8 + 2].X, points[8 + 3].X }, u, 4);
            //four3.Y = HelpFunctions.DeKastilio(new float[] { points[8 + 0].Y, points[8 + 1].Y, points[8 + 2].Y, points[8 + 3].Y }, u, 4);
            //four3.Z = HelpFunctions.DeKastilio(new float[] { points[8 + 0].Z, points[8 + 1].Z, points[8 + 2].Z, points[8 + 3].Z }, u, 4);

            //four4.X = HelpFunctions.DeKastilio(new float[] { points[12 + 0].X, points[12 + 1].X, points[12 + 2].X, points[12 + 3].X }, u, 4);
            //four4.Y = HelpFunctions.DeKastilio(new float[] { points[12 + 0].Y, points[12 + 1].Y, points[12 + 2].Y, points[12 + 3].Y }, u, 4);
            //four4.Z = HelpFunctions.DeKastilio(new float[] { points[12 + 0].Z, points[12 + 1].Z, points[12 + 2].Z, points[12 + 3].Z }, u, 4);

            //Vector3 prev = new Vector3(HelpFunctions.DeKastilio(new float[] { four1.X, four2.X, four3.X, four4.X }, v-0.001f, 4),
            //    HelpFunctions.DeKastilio(new float[] { four1.Y, four2.Y, four3.Y, four4.Y }, v - 0.001f, 4),
            //    HelpFunctions.DeKastilio(new float[] { four1.Z, four2.Z, four3.Z, four4.Z }, v - 0.001f, 4));

            //Vector3 post = new Vector3(HelpFunctions.DeKastilio(new float[] { four1.X, four2.X, four3.X, four4.X }, v + 0.001f, 4),
            //    HelpFunctions.DeKastilio(new float[] { four1.Y, four2.Y, four3.Y, four4.Y }, v + 0.001f, 4),
            //    HelpFunctions.DeKastilio(new float[] { four1.Z, four2.Z, four3.Z, four4.Z }, v + 0.001f, 4));

            //Vector3 tangent = (prev - post).Normalized();
            //return tangent;
        }

        public bool WrapsU()
        {
            return true;
        }

        public bool WrapsV()
        {
            return false;
        }

        public void SetTexture(Texture texture, TextureUnit textureUnit, int textureId)
        {
            throw new NotImplementedException();
        }
    }
}
