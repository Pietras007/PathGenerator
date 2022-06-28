using Geometric2.RasterizationClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL4;
using Geometric2.MatrixHelpers;
using Geometric2.Helpers;
using System.Drawing;
using Geometric2.Global;

namespace Geometric2.ModelGeneration
{
    public class IntersectionLines : Element
    {
        public bool DrawPolyline { get; set; }
        public List<Vector3> linePoints { get; set; }
        public int linesVBO, linesVAO, linesEBO;
        private float[] polylinePoints;
        uint[] polylineIndices;

        Camera _camera;
        int width, height;

        public IntersectionLines(List<Vector3> _drillPoints)
        {
            linePoints = new List<Vector3>();
            foreach (Vector3 drillPoint in _drillPoints)
            {
                this.linePoints.Add(drillPoint);
            }
        }

        public override void CreateGlElement(Shader _shader, ShaderGeometry _patchGeometryShader, TeselationShader _teselationShader, GlobalData globalData = null)
        {
            CreateLines();
            linesVAO = GL.GenVertexArray();
            linesVBO = GL.GenBuffer();
            linesEBO = GL.GenBuffer();
            var a_Position_Location = _shader.GetAttribLocation("a_Position");
            GL.BindVertexArray(linesVAO);
            GL.BindBuffer(BufferTarget.ArrayBuffer, linesVBO);
            GL.BufferData(BufferTarget.ArrayBuffer, polylinePoints.Length * sizeof(float), polylinePoints, BufferUsageHint.DynamicDraw);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, linesEBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, polylineIndices.Length * sizeof(uint), polylineIndices, BufferUsageHint.DynamicDraw);
            GL.VertexAttribPointer(a_Position_Location, 3, VertexAttribPointerType.Float, true, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(a_Position_Location);
        }

        public override void RenderGlElement(Shader _shader, Vector3 rotationCentre, ShaderGeometry _patchGeometryShader, TeselationShader _teselationShader)
        {
            //TempRotationQuaternion = Quaternion.FromEulerAngles((float)(2 * Math.PI * ElementRotationX / 360), (float)(2 * Math.PI * ElementRotationY / 360), (float)(2 * Math.PI * ElementRotationZ / 360));

            Matrix4 model = ModelMatrix.CreateModelMatrix(ElementScale * TempElementScale, RotationQuaternion, CenterPosition + Translation + TemporaryTranslation, rotationCentre, TempRotationQuaternion);
            FillBezierC0PolylineGeometry();
            if (DrawPolyline)
            {
                _shader.SetMatrix4("model", model);
                GL.BindVertexArray(linesVAO);
                _shader.SetVector3("fragmentColor", ColorHelper.ColorToVector(Color.Red));
                GL.DrawElements(PrimitiveType.Lines, 2 * polylinePoints.Length, DrawElementsType.UnsignedInt, 0);
                GL.BindVertexArray(0);
            }

            _shader.Use();
        }


        public void CreateLines()
        {
            if (linePoints.Count > 0)
            {
                polylinePoints = new float[3 * linePoints.Count];
                polylineIndices = new uint[2 * (linePoints.Count - 1)];
                int indiceIdx = 0;
                for (int i = 0; i < linePoints.Count; i++)
                {
                    polylinePoints[3 * i] = linePoints[i].X;
                    polylinePoints[3 * i + 1] = linePoints[i].Y;
                    polylinePoints[3 * i + 2] = linePoints[i].Z;
                    if (i < linePoints.Count - 1)
                    {
                        polylineIndices[indiceIdx] = (uint)i;
                        indiceIdx++;
                        polylineIndices[indiceIdx] = (uint)i + 1;
                        indiceIdx++;
                    }
                }
            }
        }

        private void FillBezierC0PolylineGeometry()
        {
            this.CreateLines();
            GL.BindVertexArray(linesVAO);
            GL.BindBuffer(BufferTarget.ArrayBuffer, linesVBO);
            GL.BufferData(BufferTarget.ArrayBuffer, polylinePoints.Length * sizeof(float), polylinePoints, BufferUsageHint.DynamicDraw);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, linesEBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, polylineIndices.Length * sizeof(uint), polylineIndices, BufferUsageHint.DynamicDraw);
        }
    }
}
