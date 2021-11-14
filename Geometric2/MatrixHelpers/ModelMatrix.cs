using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geometric2.MatrixHelpers
{
    public static class ModelMatrix
    {
        public static Matrix4 CreateModelMatrix(float scale, float rotationX, float rotationY, float rotationZ, Vector3 translation)
        {
            //Matrix4 modelMatrix = Matrix4.Identity;
            //modelMatrix = ScaleMatrix.CreateScaleMatrix(scale) * modelMatrix;
            //modelMatrix = RotationMatrix.CreateRotationMatrix_X(rotationX) * modelMatrix;
            //modelMatrix = RotationMatrix.CreateRotationMatrix_Y(rotationY) * modelMatrix;
            //modelMatrix = RotationMatrix.CreateRotationMatrix_Z(rotationZ) * modelMatrix;
            //modelMatrix = TranslationMatrix.CreateTranslationMatrix(translation) * modelMatrix;
            //return modelMatrix;
            return ScaleMatrix.CreateScaleMatrix(scale) * RotationMatrix.CreateRotationMatrix_X(rotationX) * RotationMatrix.CreateRotationMatrix_Y(rotationY) * RotationMatrix.CreateRotationMatrix_Z(rotationZ) * TranslationMatrix.CreateTranslationMatrix(translation);
        }

        public static Matrix4 CreateModelMatrix(float scale, Quaternion rotation, Vector3 translation, Vector3 rotationPoint, Quaternion tempRotation)
        {
            //Matrix4 modelMatrix = Matrix4.Identity;
            //modelMatrix = ScaleMatrix.CreateScaleMatrix(scale) * modelMatrix;
            //modelMatrix = RotationMatrix.CreateRotationMatrix_X(rotationX) * modelMatrix;
            //modelMatrix = RotationMatrix.CreateRotationMatrix_Y(rotationY) * modelMatrix;
            //modelMatrix = RotationMatrix.CreateRotationMatrix_Z(rotationZ) * modelMatrix;
            //modelMatrix = TranslationMatrix.CreateTranslationMatrix(translation) * modelMatrix;
            //return modelMatrix;
            return ScaleMatrix.CreateScaleMatrix(scale) * Matrix4.CreateFromQuaternion(rotation) * TranslationMatrix.CreateTranslationMatrix(translation) * TranslationMatrix.CreateTranslationMatrix(-rotationPoint) * Matrix4.CreateFromQuaternion(tempRotation) * Matrix4.CreateTranslation(rotationPoint);
        }
    }
}
