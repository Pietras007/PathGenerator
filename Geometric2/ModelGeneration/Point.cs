using System;
using System.Drawing;
using Geometric2.Helpers;
using Geometric2.MatrixHelpers;
using Geometric2.RasterizationClasses;
using OpenTK;
using OpenTK.Graphics.OpenGL4;


namespace Geometric2.ModelGeneration
{
    public class Point : Element
    {
        int pointVBO, pointVAO, pointEBO;
        public int pointNumber;
        public Camera _camera;

        float[] points = new float[]
        {
             0.0f, 0.0f, 0.0f
        };

        uint[] pointsIndices = new uint[] {
            0
        };

        public Point()
        {
        }

        public Point(Vector3 position, int pointNumber, Camera _camera)
        {
            CenterPosition = position;
            this.pointNumber = pointNumber;
            this._camera = _camera;
            this.FullName = "Point " + pointNumber;
        }

        public override string ToString()
        {
            return FullName + " " + ElementName;
        }

        public override void CreateGlElement(Shader _shader, ShaderGeometry _geometryShader = null, TeselationShader _gregoryShader = null)
        {
            FillTorusGeometry();
            var a_Position_Location = _shader.GetAttribLocation("a_Position");
            pointVAO = GL.GenVertexArray();
            pointVBO = GL.GenBuffer();
            pointEBO = GL.GenBuffer();
            GL.BindVertexArray(pointVAO);
            GL.BindBuffer(BufferTarget.ArrayBuffer, pointVBO);
            GL.BufferData(BufferTarget.ArrayBuffer, points.Length * sizeof(float), points, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, pointEBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, pointsIndices.Length * sizeof(uint), pointsIndices, BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(a_Position_Location, 3, VertexAttribPointerType.Float, true, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(a_Position_Location);

            CreateCenterOfElement(_shader);
        }

        public override void RenderGlElement(Shader _shader, Vector3 rotationCentre, ShaderGeometry _geometryShader = null, TeselationShader _gregoryShader = null)
        {
            ElementScale = _camera.CameraDist;
            //Matrix4 modelMatrix = ModelMatrix.CreateModelMatrix(ElementScale * TempElementScale, (float)(2 * Math.PI * ElementRotationX / 360), (float)(2 * Math.PI * ElementRotationY / 360), (float)(2 * Math.PI * ElementRotationZ / 360), CenterPosition + Translation + TemporaryTranslation);
            TempRotationQuaternion = Quaternion.FromEulerAngles((float)(2 * Math.PI * ElementRotationX / 360), (float)(2 * Math.PI * ElementRotationY / 360), (float)(2 * Math.PI * ElementRotationZ / 360));
            Matrix4 model = ModelMatrix.CreateModelMatrix(ElementScale * TempElementScale, RotationQuaternion, CenterPosition + Translation + TemporaryTranslation, rotationCentre, TempRotationQuaternion);
            _shader.SetMatrix4("model", model);
            GL.BindVertexArray(pointVAO);
            if (IsSelected)
            {
                _shader.SetVector3("fragmentColor", ColorHelper.ColorToVector(Color.Orange));
            }
            else
            {
                _shader.SetVector3("fragmentColor", ColorHelper.ColorToVector(Color.Black));
            }

            //GL.DrawElements(PrimitiveType.Points, 1, DrawElementsType.UnsignedInt, 0 * sizeof(int));
            GL.DrawElements(PrimitiveType.Lines, 2 * points.Length, DrawElementsType.UnsignedInt, 0);
            GL.BindVertexArray(0);
            ElementScale = 1.0f;
            model = ModelMatrix.CreateModelMatrix(ElementScale * TempElementScale, RotationQuaternion, CenterPosition + Translation + TemporaryTranslation, rotationCentre, TempRotationQuaternion);
            _shader.SetMatrix4("model", model);
            RenderCenterOfElement(_shader);
        }

        public Vector3 Position()
        {
            return this.CenterPosition + this.TemporaryTranslation + this.Translation;
        }

        public void CombinePosition()
        {
            this.CenterPosition = this.CenterPosition + this.TemporaryTranslation + this.Translation;
            this.TemporaryTranslation = new Vector3(0, 0, 0);
            this.Translation = new Vector3(0, 0, 0);
        }

        private (float[], uint[]) GenerateTorus(double majorRadius, double minorRadius, int majorSegments, int minorSegments)
        {
            float[] resultVertices = new float[3 * majorSegments * minorSegments];
            uint[] indices = new uint[4 * majorSegments * minorSegments];
            for (int maj = 0; maj < majorSegments; maj++)
            {
                double majorAngle = 2 * Math.PI * maj / majorSegments;
                for (int min = 0; min < minorSegments; min++)
                {
                    double minorAngle = 2 * Math.PI * min / minorSegments;

                    resultVertices[3 * maj * minorSegments + 3 * min] = (float)((majorRadius + minorRadius * Math.Cos(minorAngle)) * Math.Cos(majorAngle));
                    resultVertices[3 * maj * minorSegments + 3 * min + 1] = (float)((majorRadius + minorRadius * Math.Cos(minorAngle)) * Math.Sin(majorAngle));
                    resultVertices[3 * maj * minorSegments + 3 * min + 2] = (float)(minorRadius * Math.Sin(minorAngle));

                    if (min != minorSegments - 1)
                    {
                        indices[4 * maj * minorSegments + 4 * min] = (uint)(maj * minorSegments + min);
                        indices[4 * maj * minorSegments + 4 * min + 1] = (uint)(maj * minorSegments + min + 1);
                        indices[4 * maj * minorSegments + 4 * min + 2] = (uint)(maj * minorSegments + min);
                        var _maj = maj != majorSegments - 1 ? maj + 1 : 0;
                        indices[4 * maj * minorSegments + 4 * min + 3] = (uint)(_maj * minorSegments + min);
                    }
                    else
                    {
                        indices[4 * maj * minorSegments + 4 * min] = (uint)(maj * minorSegments + min);
                        indices[4 * maj * minorSegments + 4 * min + 1] = (uint)(maj * minorSegments);
                        indices[4 * maj * minorSegments + 4 * min + 2] = (uint)(maj * minorSegments + min);
                        var _maj = maj != majorSegments - 1 ? maj + 1 : 0;
                        indices[4 * maj * minorSegments + 4 * min + 3] = (uint)(_maj * minorSegments + min);
                    }
                }
            }

            return (resultVertices, indices);
        }

        private void FillTorusGeometry()
        {
            var x = GenerateTorus(0.0006, 0.0012, 20, 20);
            points = x.Item1;
            pointsIndices = x.Item2;
        }
    }
}
