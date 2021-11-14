using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geometric2.MatrixHelpers
{
    public static class ProjectionMatrix
    {
        public static Matrix4 CreateProjectionMatrix(float fov, float aspectRatio, float near, float far)
        {
            Matrix4 matrix = new Matrix4
            {
                M11 = (float)(1 / (Math.Tan(fov / 2) * aspectRatio)),
                M22 = (float)(1 / Math.Tan(fov / 2)),
                M33 = (float)((far + near) / (near - far)),
                M43 = -1,
                M34 = (float)(2 * far / (near - far)),
            };

            return Matrix4.Transpose(matrix);
        }

        public static Matrix4 CreateAnaglyphProjectionMatrix(float l, float r, float b, float t, float near, float far)
        {
            float n = near;
            float f = far;

            Matrix4 matrix = new Matrix4
            {
                M11 = (2 * n) / (r - l),
                M22 = (2 * n) / (t - b),
                M13 = (r + l) / (r - l),
                M23 = (t + b) / (t - b),
                M33 = -(f + n) / (f - n),
                M43 = -1,
                M34 = -(2 * f * n) / (f - n),
            };

            return Matrix4.Transpose(matrix);
        }
    }
}
