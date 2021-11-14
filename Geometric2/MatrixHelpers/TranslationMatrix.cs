using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geometric2.MatrixHelpers
{
    public static class TranslationMatrix
    {
        public static Matrix4 CreateTranslationMatrix(Vector3 translation)
        {
            return Matrix4.Transpose(new Matrix4
            {
                M11 = 1,
                M22 = 1,
                M33 = 1,
                M44 = 1,
                M14 = translation.X,
                M24 = translation.Y,
                M34 = translation.Z,
            });
        }
    }
}
