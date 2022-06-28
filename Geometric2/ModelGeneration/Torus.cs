using System;
using System.Drawing;
using Geometric2.Global;
using Geometric2.Helpers;
using Geometric2.MatrixHelpers;
using Geometric2.RasterizationClasses;
using Intersect;
using OpenTK;
using OpenTK.Graphics.OpenGL4;

namespace Geometric2.ModelGeneration
{
    public class Torus : Element//, ISurface
    {
        public int torusVBO, torusVAO, torusEBO;
        private float[] torusPoints;
        uint[] indices;

        public float torus_R = 2.2f;
        public float torus_r = 1.4f;
        public int torusMajorDividions = 6;
        public int torusMinorDividions = 6;
        public int torusNumber;

        public Torus()
        {
        }

        public Torus(float x, float y, float z)
        {
            //torus_X_trans = x;
            //torus_Y_trans = y;
            //torus_Z_trans = z;
            CenterPosition = new Vector3(x, y, z);
        }

        public Torus(Vector3 center, int torusNumber)
        {
            CenterPosition = center;
            this.torusNumber = torusNumber;
            FullName = "Torus " + torusNumber;
        }

        public override string ToString()
        {
            return FullName + " " + ElementName;
        }

        public override void CreateGlElement(Shader _shader, ShaderGeometry _geometryShader = null, TeselationShader _gregoryShader = null, GlobalData globalData = null)
        {
            FillTorusGeometry();
            torusVAO = GL.GenVertexArray();
            torusVBO = GL.GenBuffer();
            torusEBO = GL.GenBuffer();
            var a_Position_Location = _shader.GetAttribLocation("a_Position");
            GL.BindVertexArray(torusVAO);
            GL.BindBuffer(BufferTarget.ArrayBuffer, torusVBO);
            GL.BufferData(BufferTarget.ArrayBuffer, torusPoints.Length * sizeof(float), torusPoints, BufferUsageHint.DynamicDraw);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, torusEBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.DynamicDraw);
            GL.VertexAttribPointer(a_Position_Location, 3, VertexAttribPointerType.Float, true, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(a_Position_Location);

            CreateCenterOfElement(_shader);
        }

        public override void RenderGlElement(Shader _shader, Vector3 rotationCentre, ShaderGeometry _geometryShader = null, TeselationShader _gregoryShader = null)
        {
            //Matrix4 model = ModelMatrix.CreateModelMatrix(ElementScale * TempElementScale, (float)(2 * Math.PI * ElementRotationX / 360), (float)(2 * Math.PI * ElementRotationY / 360), (float)(2 * Math.PI * ElementRotationZ / 360), CenterPosition + Translation + TemporaryTranslation);
            TempRotationQuaternion = Quaternion.FromEulerAngles((float)(2 * Math.PI * ElementRotationX / 360), (float)(2 * Math.PI * ElementRotationY / 360), (float)(2 * Math.PI * ElementRotationZ / 360));
            Matrix4 model = ModelMatrix.CreateModelMatrix(ElementScale * TempElementScale, RotationQuaternion, CenterPosition + Translation + TemporaryTranslation, rotationCentre, TempRotationQuaternion);
            _shader.SetMatrix4("model", model);
            GL.BindVertexArray(torusVAO);
            if (IsSelected)
            {
                _shader.SetVector3("fragmentColor", ColorHelper.ColorToVector(Color.Orange));
            }
            else
            {
                _shader.SetVector3("fragmentColor", ColorHelper.ColorToVector(Color.Black));
            }

            GL.DrawElements(PrimitiveType.Lines, 2 * torusPoints.Length, DrawElementsType.UnsignedInt, 0);
            GL.BindVertexArray(0);

            RenderCenterOfElement(_shader);
        }

        public void RegenerateTorusVertices()
        {
            FillTorusGeometry();
            GL.BindVertexArray(torusVAO);
            GL.BindBuffer(BufferTarget.ArrayBuffer, torusVBO);
            GL.BufferData(BufferTarget.ArrayBuffer, torusPoints.Length * sizeof(float), torusPoints, BufferUsageHint.DynamicDraw);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, torusEBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.DynamicDraw);
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
                    resultVertices[3 * maj * minorSegments + 3 * min + 1] = (float)(minorRadius * Math.Sin(minorAngle)); //(float)((majorRadius + minorRadius * Math.Cos(minorAngle)) * Math.Sin(majorAngle));
                    resultVertices[3 * maj * minorSegments + 3 * min + 2] = (float)((majorRadius + minorRadius * Math.Cos(minorAngle)) * Math.Sin(majorAngle)); //(float)(minorRadius * Math.Sin(minorAngle));

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
            var x = GenerateTorus(torus_R, torus_r, torusMajorDividions, torusMinorDividions);
            torusPoints = x.Item1;
            indices = x.Item2;
        }
        public Vector3 Position()
        {
            return this.CenterPosition + this.TemporaryTranslation + this.Translation;
        }

        public Vector3 P(float u, float v)
        {
            throw new NotImplementedException();
        }

        public Vector3 T(float u, float v)
        {
            throw new NotImplementedException();
        }

        public Vector3 B(float u, float v)
        {
            throw new NotImplementedException();
        }

        public Vector3 N(float u, float v)
        {
            throw new NotImplementedException();
        }

        public bool WrapsU()
        {
            return true;
        }

        public bool WrapsV()
        {
            return true;
        }
    }
}
