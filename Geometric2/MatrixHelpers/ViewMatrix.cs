using Geometric2.RasterizationClasses;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geometric2.MatrixHelpers
{
    public static class ViewMatrix
    {
        public static Matrix4 CreateViewMatrix(Camera camera, float eyeDist = 0)
        {
            //return Matrix4.Identity * TranslationMatrix.CreateTranslationMatrix(new Vector3(0,0,-5));
            return TranslationMatrix.CreateTranslationMatrix(-camera.CameraCenterPoint) * RotationMatrix.CreateRotationMatrix_Y(camera.RotationY) * RotationMatrix.CreateRotationMatrix_X(camera.RotationX) * TranslationMatrix.CreateTranslationMatrix(new Vector3(eyeDist, 0, -camera.CameraDist));
        }

        //public static Matrix4 CreateViewMatrix(Vector3 cameraPosition, Vector3 _front, Vector3 _up)
        //{
        //    Vector3 eye = cameraPosition;
        //    Vector3 center = cameraPosition + _front;
        //    Vector3 up = _up;

        //    Vector3 f = (center - eye).Normalized();
        //    Vector3 u = up.Normalized();
        //    Vector3 s = Vector3.Cross(f, u);
        //    u = Vector3.Cross(s, f);

        //    Matrix4 result = Matrix4.Identity;
        //    result.M11 = s.X;
        //    result.M21 = s.Y;
        //    result.M31 = s.Z;
        //    result.M12 = u.X;
        //    result.M22 = u.Y;
        //    result.M32 = u.Z;
        //    result.M13 = -f.X;
        //    result.M23 = -f.Y;
        //    result.M33 = -f.Z;
        //    result.M41 = -Vector3.Dot(s, eye);
        //    result.M42 = -Vector3.Dot(u, eye);
        //    result.M43 = Vector3.Dot(f, eye);
        //    return result;
        //}
    }
}
