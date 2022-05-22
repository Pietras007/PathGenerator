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
    public class BezierPatchTubeC0 : Element
    {
        public bool DrawPolyline { get; set; }
        public float PlaneWidth { get; set; }
        public float PlaneHeight { get; set; }
        public int SegmentsU { get; set; }
        public int SegmentsV { get; set; }

        List<float> vertices = new List<float>();
        List<uint> indices = new List<uint>();


        public List<Point> bezierPoints { get; set; }
        public int bezierPatchC0PolylineVBO, bezierPatchC0PolylineVAO, bezierPatchC0polylineEBO;
        public int bezierPatchC0VBO, bezierPatchC0VAO, bezierPatchC0EBO;
        private float[] polylinePoints;
        uint[] polylineIndices;

        private float[] bezierPatchC0Points;
        uint[] bezierPatchC0Indices;

        int bezierPatchTubeC0Number;

        Camera _camera;
        int width, height;
        float _width, _length;
        public int splitA, splitB;
        float r;
        float x, y;
        int pointNumber = 0;

        public BezierPatchTubeC0(int bezierC0Number, Camera _camera, int width, int height, float[] values)
        {
            bezierPoints = new List<Point>();
            this._camera = _camera;
            this._width = values[0];
            this._length = values[1];
            this.splitA = (int)values[2];
            this.splitB = (int)values[3];
            this.r = values[4];
            this.SegmentsU = 1;
            this.SegmentsV = 1;
            x = 0.0f;
            y = 0.0f;


            this.width = width;
            this.height = height;
            this.bezierPatchTubeC0Number = bezierC0Number;
            FullName = "BezierPatchTubeC0 " + bezierPatchTubeC0Number;
            GenerateBezierPoints();
        }

        public override string ToString()
        {
            return FullName + " " + ElementName;
        }

        public override void CreateGlElement(Shader _shader, ShaderGeometry _patchGeometryShader, GregoryShader _gregoryShader = null)
        {

            RegenerateBezierPatchC0();
            bezierPatchC0PolylineVAO = GL.GenVertexArray();
            bezierPatchC0PolylineVBO = GL.GenBuffer();
            bezierPatchC0polylineEBO = GL.GenBuffer();
            var a_Position_Location = _shader.GetAttribLocation("a_Position");
            GL.BindVertexArray(bezierPatchC0PolylineVAO);
            GL.BindBuffer(BufferTarget.ArrayBuffer, bezierPatchC0PolylineVBO);
            GL.BufferData(BufferTarget.ArrayBuffer, polylinePoints.Length * sizeof(float), polylinePoints, BufferUsageHint.DynamicDraw);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, bezierPatchC0polylineEBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, polylineIndices.Length * sizeof(uint), polylineIndices, BufferUsageHint.DynamicDraw);
            GL.VertexAttribPointer(a_Position_Location, 3, VertexAttribPointerType.Float, true, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(a_Position_Location);


            var a_geometry_Position_Location = _patchGeometryShader.GetAttribLocation("a_Position");
            //var a_geometry_Position_Location = _geometryShader.GetAttribLocation("a_Position");
            //var a_geometry_Position_Location = _geometryShader.GetAttribLocation("a_Position");
            bezierPatchC0VAO = GL.GenVertexArray();
            bezierPatchC0VBO = GL.GenBuffer();
            bezierPatchC0EBO = GL.GenBuffer();
            GL.BindVertexArray(bezierPatchC0VAO);
            GL.BindBuffer(BufferTarget.ArrayBuffer, bezierPatchC0VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, bezierPatchC0Points.Length * sizeof(float), bezierPatchC0Points, BufferUsageHint.DynamicDraw);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, bezierPatchC0EBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, bezierPatchC0Indices.Length * sizeof(uint), bezierPatchC0Indices, BufferUsageHint.DynamicDraw);

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 15 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 15 * sizeof(float), (3 * sizeof(float)));
            GL.EnableVertexAttribArray(1);

            GL.VertexAttribPointer(2, 3, VertexAttribPointerType.Float, false, 15 * sizeof(float), (6 * sizeof(float)));
            GL.EnableVertexAttribArray(2);

            GL.VertexAttribPointer(3, 3, VertexAttribPointerType.Float, false, 15 * sizeof(float), (9 * sizeof(float)));
            GL.EnableVertexAttribArray(3);

            GL.VertexAttribPointer(4, 3, VertexAttribPointerType.Float, false, 15 * sizeof(float), (12 * sizeof(float)));
            GL.EnableVertexAttribArray(4);
        }

        public override void RenderGlElement(Shader _shader, Vector3 rotationCentre, ShaderGeometry _patchGeometryShader, GregoryShader _gregoryShader = null)
        {
            //renderNum++;
            //if (renderNum == 20)
            //{
            //    renderNum = 0;
                RegenerateBezierPatchC0();
            //}


            TempRotationQuaternion = Quaternion.FromEulerAngles((float)(2 * Math.PI * ElementRotationX / 360), (float)(2 * Math.PI * ElementRotationY / 360), (float)(2 * Math.PI * ElementRotationZ / 360));
            Matrix4 model = ModelMatrix.CreateModelMatrix(ElementScale * TempElementScale, RotationQuaternion, CenterPosition + Translation + TemporaryTranslation, rotationCentre, TempRotationQuaternion);
            Matrix4 model2 = ModelMatrix.CreateModelMatrix(ElementScale * TempElementScale, RotationQuaternion, new Vector3(0, 0, 0), rotationCentre, TempRotationQuaternion);

            if (DrawPolyline)
            {
                _shader.SetMatrix4("model", model);
                GL.BindVertexArray(bezierPatchC0PolylineVAO);
                if (IsSelected)
                {
                    _shader.SetVector3("fragmentColor", ColorHelper.ColorToVector(Color.Orange));
                }
                else
                {
                    _shader.SetVector3("fragmentColor", ColorHelper.ColorToVector(Color.Red));
                }

                GL.DrawElements(PrimitiveType.Lines, 2 * polylinePoints.Length, DrawElementsType.UnsignedInt, 0);
                GL.BindVertexArray(0);
            }

            _patchGeometryShader.Use();
            _patchGeometryShader.SetMatrix4("model", model2);
            //_patchGeometryShader.SetMatrix4("model", model);
            //_patchGeometryShader.SetMatrix4("view", _camera.GetViewMatrix());
            //_patchGeometryShader.SetMatrix4("projection", _camera.GetProjectionMatrix());
            if (IsSelected)
            {
                _patchGeometryShader.SetVector3("fragmentColor", ColorHelper.ColorToVector(Color.Orange));
            }
            else
            {
                _patchGeometryShader.SetVector3("fragmentColor", ColorHelper.ColorToVector(Color.Black));
            }
            //_patchGeometryShader.SetMatrix4("persp", _camera.GetProjectionMatrix());
            //_patchGeometryShader.SetInt("width", width);
            //_patchGeometryShader.SetInt("height", height);
            GL.BindVertexArray(bezierPatchC0VAO);
            GL.DrawElements(PrimitiveType.LinesAdjacency, bezierPatchC0Indices.Length, DrawElementsType.UnsignedInt, 0);
            GL.BindVertexArray(0);

            _shader.Use();
        }


        public void RegenerateBezierPatchC0()
        {
            FillPolylineGeometry();
            FillBezierPatchGeometry();
            FillBezierPatchC0PolylineGeometry();
            FillBezierPatchC0Geometry();
        }

        private void FillBezierPatchC0PolylineGeometry()
        {
            GL.BindVertexArray(bezierPatchC0PolylineVAO);
            GL.BindBuffer(BufferTarget.ArrayBuffer, bezierPatchC0PolylineVBO);
            GL.BufferData(BufferTarget.ArrayBuffer, polylinePoints.Length * sizeof(float), polylinePoints, BufferUsageHint.DynamicDraw);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, bezierPatchC0polylineEBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, polylineIndices.Length * sizeof(uint), polylineIndices, BufferUsageHint.DynamicDraw);
        }

        private void FillBezierPatchC0Geometry()
        {
            GL.BindVertexArray(bezierPatchC0VAO);
            GL.BindBuffer(BufferTarget.ArrayBuffer, bezierPatchC0VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, bezierPatchC0Points.Length * sizeof(float), bezierPatchC0Points, BufferUsageHint.DynamicDraw);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, bezierPatchC0EBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, bezierPatchC0Indices.Length * sizeof(uint), bezierPatchC0Indices, BufferUsageHint.DynamicDraw);
        }

        private void GenerateBezierPoints()
        {
            bezierPoints.Clear();
            if (splitB < 3) splitB = 3;
            float z = 0.0f;
            float angle = 0.0f;
            float zDiff = _length / (3 * splitA);
            float angleDiff = 2 * (float)Math.PI / (3 * splitB);
            int k = 0;
            for (int i = 0; i < 3 * splitA + 1; i++)
            {
                angle = 0.0f;
                for (int j = 0; j < 3 * splitB; j++)
                {
                    Point point = new Point(new Vector3(r * (float)Math.Sin(angle), r * (float)Math.Cos(angle), z), pointNumber, _camera);
                    pointNumber++;
                    point.FullName += "_patchTubeC0_" + bezierPatchTubeC0Number;
                    bezierPoints.Add(point);
                    angle += angleDiff;
                    k++;
                }
                bezierPoints.Add(bezierPoints[k - 3 * splitB]);
                k++;
                z += zDiff;
            }
        }

        private void FillPolylineGeometry()
        {
            List<Point> pointLines = new List<Point>();
            if (splitB < 3) splitB = 3;
            float angleDiff = 2 * (float)Math.PI / (3 * splitB);
            int k = 0;
            for (int i = 0; i < 3 * splitA + 1; i++)
            {
                float angle = 0.0f;
                for (int j = 0; j < 3 * splitB; j++)
                {
                    angle += angleDiff;
                    if (j != 0)
                    {
                        pointLines.Add(bezierPoints[k - 1]);
                        pointLines.Add(bezierPoints[k]);
                        
                    }
                    if (i != 0)
                    {
                        pointLines.Add(bezierPoints[k]);
                        pointLines.Add(bezierPoints[k - (3 * splitB + 1)]);
                        
                    }
                    k++;
                }
                pointLines.Add(bezierPoints[k - 1]);
                pointLines.Add(bezierPoints[k]);

                k++;
            }

            polylinePoints = new float[3 * pointLines.Count];
            polylineIndices = new uint[pointLines.Count];
            int indiceIdx = 0;
            for (int i = 0; i < pointLines.Count ; i++)
            {

                if(pointLines[i].CenterPosition == new Vector3(0,0,0))
                {
                    var x = 7;
                }

                polylinePoints[3 * i] = pointLines[i].CenterPosition.X + pointLines[i].TemporaryTranslation.X + pointLines[i].Translation.X;
                polylinePoints[3 * i + 1] = pointLines[i].CenterPosition.Y + pointLines[i].TemporaryTranslation.Y + pointLines[i].Translation.Y;
                polylinePoints[3 * i + 2] = pointLines[i].CenterPosition.Z + pointLines[i].TemporaryTranslation.Z + pointLines[i].Translation.Z;


                if (i <= pointLines.Count - 1)
                {
                    polylineIndices[indiceIdx] = (uint)i;
                    indiceIdx++;
                    //polylineIndices[indiceIdx] = (uint)i + 1;
                    //indiceIdx++;
                }
            }
        }

        private void FillBezierPatchGeometry()
        {
            vertices.Clear();
            indices.Clear();
            int idx = 0;
            for (int i = 0; i < splitA; i++)
            {
                for (int j = 0; j < splitB; j++)
                {
                    for (int k = 0; k < SegmentsU; k++)
                    {
                        for (int l = 0; l < SegmentsV; l += 120)
                        {
                            float begin = (float)l / SegmentsV;
                            float end = (float)(l + 120) / SegmentsV;
                            int parts = ((SegmentsV - l) < 120) ? (SegmentsV - l) : 120;
                            if (end > 1.0f) end = 1.0f;

                            AddPatch(i, j, (float)k / SegmentsU, (float)(k + 1) / SegmentsU, begin, end, parts, idx);
                            idx += 4;
                        }
                    }
                }
            }

            bezierPatchC0Points = vertices.ToArray();
            bezierPatchC0Indices = indices.ToArray();
        }

        void AddPatch(int i, int j, float t1, float t2, float begin, float end, int parts, int idx)
        {
            int w = 3 * splitB + 1;
            int start = 3 * i * w + 3 * j;
            int indexStart = idx;


            for (int k = 0; k < 4; k++)
            {
                var pos = bezierPoints[start].Position();
                vertices.Add(pos.X);
                vertices.Add(pos.Y);
                vertices.Add(pos.Z);
                if (i == 0 && j == 0 && t1 == 0)
                {
                    vertices.Add(0.12f);
                }
                else
                {
                    vertices.Add(1.0f);
                }
                vertices.Add(0.0f);
                vertices.Add(0.0f);
                pos = bezierPoints[start + 1].Position();
                vertices.Add(pos.X);
                vertices.Add(pos.Y);
                vertices.Add(pos.Z);
                pos = bezierPoints[start + 2].Position();
                vertices.Add(pos.X);
                vertices.Add(pos.Y);
                vertices.Add(pos.Z);
                pos = bezierPoints[start + 3].Position();
                vertices.Add(pos.X);
                vertices.Add(pos.Y);
                vertices.Add(pos.Z);
                indices.Add((uint)idx);
                idx++;
                start += w;
            }

            vertices[15 * (indexStart + 1) + 3] = t1;
            vertices[15 * (indexStart + 1) + 4] = t2;
            vertices[15 * (indexStart + 2) + 3] = begin;
            vertices[15 * (indexStart + 2) + 4] = end;
            vertices[15 * (indexStart + 2) + 5] = parts;
        }

    }
}
