using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geometric2.MatrixHelpers
{
    public static class RotationMatrix
    {
        public static Matrix4 CreateRotationMatrix_X(float rotationX)
        {
            return new Matrix4
            {
                M11 = 1,
                M22 = (float)Math.Cos(rotationX),
                M23 = -(float)Math.Sin(rotationX),
                M32 = (float)Math.Sin(rotationX),
                M33 = (float)Math.Cos(rotationX),
                M44 = 1,
            };
        }

        public static Matrix4 CreateRotationMatrix_Y(float rotationY)
        {
            return new Matrix4
            {
                M11 = (float)Math.Cos(rotationY),
                M13 = -(float)Math.Sin(rotationY),
                M22 = 1,
                M31 = (float)Math.Sin(rotationY),
                M33 = (float)Math.Cos(rotationY),
                M44 = 1,
            };
        }

        public static Matrix4 CreateRotationMatrix_Z(float rotationZ)
        {
            return new Matrix4
            {
                M11 = (float)Math.Cos(rotationZ),
                M12 = -(float)Math.Sin(rotationZ),
                M21 = (float)Math.Sin(rotationZ),
                M22 = (float)Math.Cos(rotationZ),
                M33 = 1,
                M44 = 1,
            };
        }
    }
}
