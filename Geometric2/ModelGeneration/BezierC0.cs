using System;
using System.Collections.Generic;
using System.Drawing;
using Geometric2.Helpers;
using Geometric2.MatrixHelpers;
using Geometric2.RasterizationClasses;
using OpenTK;
using OpenTK.Graphics.OpenGL4;

namespace Geometric2.ModelGeneration
{
    public class BezierC0 : Element
    {
        public bool DrawPolyline { get; set; }
        public List<Point> bezierPoints { get; set; }
        public int bezierCPolyline0VBO, bezierC0PolylineVAO, bezierC0polylineEBO;
        public int bezierC0VBO, bezierC0VAO, bezierC0EBO;
        private float[] polylinePoints;
        uint[] polylineIndices;

        private float[] bezierC0Points;
        uint[] bezierC0Indices;

        public int torusMajorDividions = 6;
        public int torusMinorDividions = 6;
        public int bezierC0Number;

        int renderNum;
        Camera _camera;
        int width, height;

        public BezierC0(int bezierC0Number, Camera _camera, int width, int height)
        {
            bezierPoints = new List<Point>();
            this._camera = _camera;
            this.width = width;
            this.height = height;
            this.bezierC0Number = bezierC0Number;
            FullName = "BezierC0 " + bezierC0Number;
        }

        public override string ToString()
        {
            return FullName + " " + ElementName;
        }

        public override void CreateGlElement(Shader _shader, ShaderGeometry _geometryShader, TeselationShader _gregoryShader = null)
        {
            RegenerateBezierC0();
            bezierC0PolylineVAO = GL.GenVertexArray();
            bezierCPolyline0VBO = GL.GenBuffer();
            bezierC0polylineEBO = GL.GenBuffer();
            var a_Position_Location = _shader.GetAttribLocation("a_Position");
            GL.BindVertexArray(bezierC0PolylineVAO);
            GL.BindBuffer(BufferTarget.ArrayBuffer, bezierCPolyline0VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, polylinePoints.Length * sizeof(float), polylinePoints, BufferUsageHint.DynamicDraw);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, bezierC0polylineEBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, polylineIndices.Length * sizeof(uint), polylineIndices, BufferUsageHint.DynamicDraw);
            GL.VertexAttribPointer(a_Position_Location, 3, VertexAttribPointerType.Float, true, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(a_Position_Location);


            var a_geometry_Position_Location = _geometryShader.GetAttribLocation("a_Position");
            bezierC0VAO = GL.GenVertexArray();
            bezierC0VBO = GL.GenBuffer();
            bezierC0EBO = GL.GenBuffer();
            GL.BindVertexArray(bezierC0VAO);
            GL.BindBuffer(BufferTarget.ArrayBuffer, bezierC0VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, bezierC0Points.Length * sizeof(float), bezierC0Points, BufferUsageHint.DynamicDraw);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, bezierC0EBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, bezierC0Indices.Length * sizeof(uint), bezierC0Indices, BufferUsageHint.DynamicDraw);
            GL.VertexAttribPointer(a_geometry_Position_Location, 3, VertexAttribPointerType.Float, true, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(a_geometry_Position_Location);
        }

        public override void RenderGlElement(Shader _shader, Vector3 rotationCentre, ShaderGeometry _geometryShader, TeselationShader _gregoryShader = null)
        {
            renderNum++;
            if (renderNum == 20)
            {
                renderNum = 0;
                RegenerateBezierC0();
            }


            TempRotationQuaternion = Quaternion.FromEulerAngles((float)(2 * Math.PI * ElementRotationX / 360), (float)(2 * Math.PI * ElementRotationY / 360), (float)(2 * Math.PI * ElementRotationZ / 360));
            Matrix4 model = ModelMatrix.CreateModelMatrix(ElementScale * TempElementScale, RotationQuaternion, CenterPosition + Translation + TemporaryTranslation, rotationCentre, TempRotationQuaternion);

            if (DrawPolyline)
            {
                _shader.SetMatrix4("model", model);
                GL.BindVertexArray(bezierC0PolylineVAO);
                _shader.SetVector3("fragmentColor", ColorHelper.ColorToVector(Color.Cyan));
                GL.DrawElements(PrimitiveType.Lines, 2 * polylinePoints.Length, DrawElementsType.UnsignedInt, 0);
                GL.BindVertexArray(0);
            }

            _geometryShader.Use();
            _geometryShader.SetMatrix4("model", model);
            _geometryShader.SetInt("width", width);
            _geometryShader.SetInt("height", height);
            GL.BindVertexArray(bezierC0VAO);
            if (IsSelected)
            {
                // _geometryShader.SetVector3("fragmentColor", ColorHelper.ColorToVector(Color.Orange));
            }
            else
            {
                // _geometryShader.SetVector3("fragmentColor", ColorHelper.ColorToVector(Color.Black));
            }

            GL.DrawElements(PrimitiveType.LinesAdjacency, bezierC0Indices.Length, DrawElementsType.UnsignedInt, 0);
            GL.BindVertexArray(0);

            _shader.Use();
        }


        public void RegenerateBezierC0()
        {
            FillPolylineGeometry();
            FillBezierGeometry();
            FillBezierC0PolylineGeometry();
            FillBezierC0Geometry();
        }

        private void FillBezierC0PolylineGeometry()
        {
            GL.BindVertexArray(bezierC0PolylineVAO);
            GL.BindBuffer(BufferTarget.ArrayBuffer, bezierCPolyline0VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, polylinePoints.Length * sizeof(float), polylinePoints, BufferUsageHint.DynamicDraw);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, bezierC0polylineEBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, polylineIndices.Length * sizeof(uint), polylineIndices, BufferUsageHint.DynamicDraw);
        }

        private void FillBezierC0Geometry()
        {
            GL.BindVertexArray(bezierC0VAO);
            GL.BindBuffer(BufferTarget.ArrayBuffer, bezierC0VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, bezierC0Points.Length * sizeof(float), bezierC0Points, BufferUsageHint.DynamicDraw);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, bezierC0EBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, bezierC0Indices.Length * sizeof(uint), bezierC0Indices, BufferUsageHint.DynamicDraw);
        }

        private void FillPolylineGeometry()
        {
            polylinePoints = new float[3 * bezierPoints.Count];
            if (bezierPoints.Count == 0)
            {
                polylineIndices = new uint[0];
            }
            else
            {
                polylineIndices = new uint[2 * (bezierPoints.Count - 1)];
            }

            int indiceIdx = 0;
            for (int i = 0; i < bezierPoints.Count; i++)
            {
                polylinePoints[3 * i] = bezierPoints[i].CenterPosition.X + bezierPoints[i].TemporaryTranslation.X + bezierPoints[i].Translation.X;
                polylinePoints[3 * i + 1] = bezierPoints[i].CenterPosition.Y + bezierPoints[i].TemporaryTranslation.Y + bezierPoints[i].Translation.Y;
                polylinePoints[3 * i + 2] = bezierPoints[i].CenterPosition.Z + bezierPoints[i].TemporaryTranslation.Z + bezierPoints[i].Translation.Z;

                if (i < bezierPoints.Count - 1)
                {
                    polylineIndices[indiceIdx] = (uint)i;
                    indiceIdx++;
                    polylineIndices[indiceIdx] = (uint)i + 1;
                    indiceIdx++;
                }
            }

        }

        private void FillBezierGeometry()
        {
            int bezierParts = (int)Math.Ceiling((double)(bezierPoints.Count - 1) / 3.0);
            if (bezierPoints.Count == 1)
            {
                bezierParts = 1;
            }

            bezierC0Points = new float[3 * bezierParts * 4];
            if (bezierPoints.Count == 0)
            {
                bezierC0Indices = new uint[0];
            }
            else
            {
                bezierC0Indices = new uint[4 * bezierParts];
            }

            for (int i = 0; i < bezierParts * 4; i++)
            {
                int _x = i;
                if (_x >= bezierPoints.Count)
                {
                    bezierC0Points[3 * _x] = float.PositiveInfinity;
                }
                else
                {
                    bezierC0Points[3 * _x] = bezierPoints[_x].CenterPosition.X + bezierPoints[_x].TemporaryTranslation.X + bezierPoints[_x].Translation.X;
                    bezierC0Points[3 * _x + 1] = bezierPoints[_x].CenterPosition.Y + bezierPoints[_x].TemporaryTranslation.Y + bezierPoints[_x].Translation.Y;
                    bezierC0Points[3 * _x + 2] = bezierPoints[_x].CenterPosition.Z + bezierPoints[_x].TemporaryTranslation.Z + bezierPoints[_x].Translation.Z;
                }
            }

            int indiceIdx = 0;
            int x = 0;
            for (int k = 0; k < bezierParts; k++)
            {

                bezierC0Indices[indiceIdx] = (uint)x;
                indiceIdx++;
                bezierC0Indices[indiceIdx] = (uint)x + 1;
                indiceIdx++;
                bezierC0Indices[indiceIdx] = (uint)x + 2;
                indiceIdx++;
                bezierC0Indices[indiceIdx] = (uint)x + 3;
                indiceIdx++;
                x += 3;
            }
        }
    }
}
