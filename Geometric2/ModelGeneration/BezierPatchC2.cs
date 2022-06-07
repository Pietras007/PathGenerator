using Geometric2.Helpers;
using Geometric2.MatrixHelpers;
using Geometric2.RasterizationClasses;
using OpenTK;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Geometric2.ModelGeneration
{
    public class BezierPatchC2 : Element
    {
        public bool DrawPolyline { get; set; }
        public int SegmentsU { get; set; }
        public int SegmentsV { get; set; }
        List<float> vertices = new List<float>();
        List<uint> indices = new List<uint>();

        private float[] bezierPatchC2Points;
        uint[] bezierPatchC2Indices;
        private float[] polylinePoints;
        uint[] polylineIndices;
        int[] pointNumber = new int[1];


        int bezierPatchC2Number;
        public List<Point> bezierPoints { get; set; }
        public int bezierPatchC2PolylineVBO, bezierPatchC2PolylineVAO, bezierPatchC2polylineEBO;
        public int bezierPatchC2VBO, bezierPatchC2VAO, bezierPatchC2EBO;
        Camera _camera;
        int width, height;
        float _width, _length;
        public int splitA, splitB;
        float x0, y0;
        private float r;
        private object bezierPatchC0Points;

        private bool isTube;

        public BezierPatchC2(int[] pointNumber, int bezierC2Number, Camera _camera, int width, int height, float[] values, bool isTube = false)
        {
            bezierPoints = new List<Point>();
            this.pointNumber = pointNumber;
            this._camera = _camera;
            this._width = values[0];
            this._length = values[1];
            this.splitA = (int)values[2];
            this.splitB = (int)values[3];
            this.r = values.Length > 4 ? values[4] : 0.0f;
            this.SegmentsU = 1;
            this.SegmentsV = 1;
            x0 = 0.0f;
            y0 = 0.0f;

            this.width = width;
            this.height = height;
            this.bezierPatchC2Number = bezierC2Number;
            this.isTube = isTube;
            FullName = "BezierPatchC2 " + bezierPatchC2Number;
            GenerateBezierPoints();
        }

        public BezierPatchC2(int bezierC2Number, Camera _camera, int width, int height, bool isTube = false)
        {
            bezierPoints = new List<Point>();
            this._camera = _camera;
            this.SegmentsU = 1;
            this.SegmentsV = 1;
            x0 = 0.0f;
            y0 = 0.0f;

            this.width = width;
            this.height = height;
            this.isTube = isTube;
            this.bezierPatchC2Number = bezierC2Number;
        }

        public override string ToString()
        {
            return FullName + " " + ElementName;
        }

        public override void CreateGlElement(Shader _shader, ShaderGeometry _patchGeometryShader, TeselationShader _teselationShader)
        {

            RegenerateBezierPatchC2();
            bezierPatchC2PolylineVAO = GL.GenVertexArray();
            bezierPatchC2PolylineVBO = GL.GenBuffer();
            bezierPatchC2polylineEBO = GL.GenBuffer();
            var a_Position_Location = _shader.GetAttribLocation("a_Position");
            GL.BindVertexArray(bezierPatchC2PolylineVAO);
            GL.BindBuffer(BufferTarget.ArrayBuffer, bezierPatchC2PolylineVBO);
            GL.BufferData(BufferTarget.ArrayBuffer, polylinePoints.Length * sizeof(float), polylinePoints, BufferUsageHint.DynamicDraw);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, bezierPatchC2polylineEBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, polylineIndices.Length * sizeof(uint), polylineIndices, BufferUsageHint.DynamicDraw);
            GL.VertexAttribPointer(a_Position_Location, 3, VertexAttribPointerType.Float, true, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(a_Position_Location);

            var a_Position_Loc_gregoryShader = _teselationShader.GetAttribLocation("a_Position");
            bezierPatchC2VAO = GL.GenVertexArray();
            bezierPatchC2VBO = GL.GenBuffer();
            bezierPatchC2EBO = GL.GenBuffer();
            GL.BindVertexArray(bezierPatchC2VAO);
            GL.BindBuffer(BufferTarget.ArrayBuffer, bezierPatchC2VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, bezierPatchC2Points.Length * sizeof(float), bezierPatchC2Points, BufferUsageHint.DynamicDraw);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, bezierPatchC2EBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, bezierPatchC2Indices.Length * sizeof(uint), bezierPatchC2Indices, BufferUsageHint.DynamicDraw);
            GL.VertexAttribPointer(a_Position_Loc_gregoryShader, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);



            //var a_geometry_Position_Location = _patchGeometryShader.GetAttribLocation("a_Position");
            ////var a_geometry_Position_Location = _geometryShader.GetAttribLocation("a_Position");
            ////var a_geometry_Position_Location = _geometryShader.GetAttribLocation("a_Position");
            //bezierPatchC2VAO = GL.GenVertexArray();
            //bezierPatchC2VBO = GL.GenBuffer();
            //bezierPatchC2EBO = GL.GenBuffer();
            //GL.BindVertexArray(bezierPatchC2VAO);
            //GL.BindBuffer(BufferTarget.ArrayBuffer, bezierPatchC2VBO);
            //GL.BufferData(BufferTarget.ArrayBuffer, bezierPatchC2Points.Length * sizeof(float), bezierPatchC2Points, BufferUsageHint.DynamicDraw);
            //GL.BindBuffer(BufferTarget.ElementArrayBuffer, bezierPatchC2EBO);
            //GL.BufferData(BufferTarget.ElementArrayBuffer, bezierPatchC2Indices.Length * sizeof(uint), bezierPatchC2Indices, BufferUsageHint.DynamicDraw);

            //GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 15 * sizeof(float), 0);
            //GL.EnableVertexAttribArray(0);

            //GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 15 * sizeof(float), (3 * sizeof(float)));
            //GL.EnableVertexAttribArray(1);

            //GL.VertexAttribPointer(2, 3, VertexAttribPointerType.Float, false, 15 * sizeof(float), (6 * sizeof(float)));
            //GL.EnableVertexAttribArray(2);

            //GL.VertexAttribPointer(3, 3, VertexAttribPointerType.Float, false, 15 * sizeof(float), (9 * sizeof(float)));
            //GL.EnableVertexAttribArray(3);

            //GL.VertexAttribPointer(4, 3, VertexAttribPointerType.Float, false, 15 * sizeof(float), (12 * sizeof(float)));
            //GL.EnableVertexAttribArray(4);
        }

        public override void RenderGlElement(Shader _shader, Vector3 rotationCentre, ShaderGeometry _patchGeometryShader, TeselationShader _teselationShader)
        {
            RegenerateBezierPatchC2();

            TempRotationQuaternion = Quaternion.FromEulerAngles((float)(2 * Math.PI * ElementRotationX / 360), (float)(2 * Math.PI * ElementRotationY / 360), (float)(2 * Math.PI * ElementRotationZ / 360));
            Matrix4 model = ModelMatrix.CreateModelMatrix(ElementScale * TempElementScale, RotationQuaternion, CenterPosition + Translation + TemporaryTranslation, rotationCentre, TempRotationQuaternion);
            Matrix4 model2 = ModelMatrix.CreateModelMatrix(ElementScale * TempElementScale, RotationQuaternion, new Vector3(0, 0, 0), rotationCentre, TempRotationQuaternion);

            if (DrawPolyline)
            {
                _shader.SetMatrix4("model", model);
                GL.BindVertexArray(bezierPatchC2PolylineVAO);
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

            _teselationShader.Use();
            _teselationShader.SetMatrix4("model", model);
            _teselationShader.SetFloat("SegmentsU", SegmentsU);
            _teselationShader.SetFloat("SegmentsV", SegmentsV);
            if (IsSelected)
            {
                _teselationShader.SetVector3("fragmentColor", ColorHelper.ColorToVector(Color.Orange));
            }
            else
            {
                _teselationShader.SetVector3("fragmentColor", ColorHelper.ColorToVector(Color.Black));
            }

            GL.BindVertexArray(bezierPatchC2VAO);
            GL.PatchParameter(PatchParameterInt.PatchVertices, 16);
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
            GL.DrawElements(BeginMode.Patches, bezierPatchC2Indices.Length, DrawElementsType.UnsignedInt, 0);
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);

            _shader.Use();



            //_patchGeometryShader.Use();
            //_patchGeometryShader.SetMatrix4("model", model2);
            //if (IsSelected)
            //{
            //    _patchGeometryShader.SetVector3("fragmentColor", ColorHelper.ColorToVector(Color.Orange));
            //}
            //else
            //{
            //    _patchGeometryShader.SetVector3("fragmentColor", ColorHelper.ColorToVector(Color.Black));
            //}
            ////_patchGeometryShader.SetMatrix4("view", _camera.GetViewMatrix());
            ////_patchGeometryShader.SetMatrix4("projection", _camera.GetProjectionMatrix());
            ////_patchGeometryShader.SetVector3("fragmentColor", ColorHelper.ColorToVector(Color.White));
            ////_patchGeometryShader.SetMatrix4("persp", _camera.GetProjectionMatrix());
            ////_patchGeometryShader.SetInt("width", width);
            ////_patchGeometryShader.SetInt("height", height);
            //GL.BindVertexArray(bezierPatchC2VAO);
            //GL.DrawElements(PrimitiveType.LinesAdjacency, bezierPatchC2Indices.Length, DrawElementsType.UnsignedInt, 0);
            //GL.BindVertexArray(0);

            //_shader.Use();
        }


        public void RegenerateBezierPatchC2()
        {
            FillPolylineGeometry();
            FillBezierPatchGeometry();
            FillBezierPatchC2PolylineGeometry();
            FillBezierPatchC2Geometry();
        }

        private void FillBezierPatchC2PolylineGeometry()
        {
            GL.BindVertexArray(bezierPatchC2PolylineVAO);
            GL.BindBuffer(BufferTarget.ArrayBuffer, bezierPatchC2PolylineVBO);
            GL.BufferData(BufferTarget.ArrayBuffer, polylinePoints.Length * sizeof(float), polylinePoints, BufferUsageHint.DynamicDraw);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, bezierPatchC2polylineEBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, polylineIndices.Length * sizeof(uint), polylineIndices, BufferUsageHint.DynamicDraw);
        }

        private void FillBezierPatchC2Geometry()
        {
            GL.BindVertexArray(bezierPatchC2VAO);
            GL.BindBuffer(BufferTarget.ArrayBuffer, bezierPatchC2VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, bezierPatchC2Points.Length * sizeof(float), bezierPatchC2Points, BufferUsageHint.DynamicDraw);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, bezierPatchC2EBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, bezierPatchC2Indices.Length * sizeof(uint), bezierPatchC2Indices, BufferUsageHint.DynamicDraw);
        }

        private void GenerateBezierPoints()
        {
            if (isTube)
            {
                if (splitB < 3) splitB = 3;
            }

            bezierPoints.Clear();
            if (isTube)
            {
                float z = 0.0f;
                float zDiff = _length / (splitA);
                float angleDiff = 2 * (float)Math.PI / (splitB);
                int k = 0;
                for (int i = -1; i <= splitA + 1; i++)
                {
                    float angle = 0.0f;
                    for (int j = 0; j < splitB; j++)
                    {
                        Point point = new Point(new Vector3(r * (float)Math.Sin(angle), r * (float)Math.Cos(angle), z), pointNumber[0], _camera);
                        pointNumber[0]++;
                        point.FullName += "_patchTubeC2_" + bezierPatchC2Number;
                        bezierPoints.Add(point);
                        angle += angleDiff;
                        k++;
                    }
                    bezierPoints.Add(bezierPoints[k - splitB]);
                    k++;
                    bezierPoints.Add(bezierPoints[k - splitB]);
                    k++;
                    bezierPoints.Add(bezierPoints[k - splitB]);
                    k++;
                    z += zDiff;
                }
            }
            else
            {

                float xdiff = _width / splitA;
                float ydiff = _length / splitB;
                x0 = -xdiff;
                int k = 0;
                for (int i = -1; i <= splitA + 1; i++)
                {
                    y0 = -ydiff;
                    for (int j = -1; j <= splitB + 1; j++)
                    {
                        Point point = new Point(new Vector3(x0, y0, 0.0f), pointNumber[0], _camera);
                        pointNumber[0]++;
                        point.FullName += "_patchC2_" + bezierPatchC2Number;
                        bezierPoints.Add(point);
                        y0 += ydiff;
                        k++;
                    }
                    x0 += xdiff;
                }
            }
        }

        private void FillPolylineGeometry()
        {
            List<Point> pointLines = new List<Point>();
            int k = 0;
            for (int i = -1; i <= splitA + 1; i++)
            {
                if (isTube)
                {
                    for (int j = 0; j < splitB; j++)
                    {
                        if (j != 0)
                        {
                            pointLines.Add(bezierPoints[k - 1]);
                            pointLines.Add(bezierPoints[k]);

                        }
                        if (i != -1)
                        {
                            pointLines.Add(bezierPoints[k]);
                            pointLines.Add(bezierPoints[k - (splitB + 3)]);
                        }
                        k++;
                    }

                    pointLines.Add(bezierPoints[k - 1]);
                    pointLines.Add(bezierPoints[k]);
                    k++;
                    pointLines.Add(bezierPoints[k - 1]);
                    pointLines.Add(bezierPoints[k]);
                    k++;
                    pointLines.Add(bezierPoints[k - 1]);
                    pointLines.Add(bezierPoints[k]);
                    k++;
                }
                else
                {
                    for (int j = -1; j <= splitB + 1; j++)
                    {
                        if (j != -1)
                        {
                            pointLines.Add(bezierPoints[k - 1]);
                            pointLines.Add(bezierPoints[k]);
                        }
                        if (i != -1)
                        {
                            pointLines.Add(bezierPoints[k - (splitB + 3)]);
                            pointLines.Add(bezierPoints[k]);
                        }
                        k++;
                    }
                }
            }

            polylinePoints = new float[3 * pointLines.Count];
            polylineIndices = new uint[pointLines.Count];
            int indiceIdx = 0;
            for (int i = 0; i < pointLines.Count; i++)
            {
                polylinePoints[3 * i] = pointLines[i].CenterPosition.X + pointLines[i].TemporaryTranslation.X + pointLines[i].Translation.X;
                polylinePoints[3 * i + 1] = pointLines[i].CenterPosition.Y + pointLines[i].TemporaryTranslation.Y + pointLines[i].Translation.Y;
                polylinePoints[3 * i + 2] = pointLines[i].CenterPosition.Z + pointLines[i].TemporaryTranslation.Z + pointLines[i].Translation.Z;


                if (i <= pointLines.Count - 1)
                {
                    polylineIndices[indiceIdx] = (uint)i;
                    indiceIdx++;
                }
            }
        }

        private void FillBezierPatchGeometry()
        {
            vertices.Clear();
            indices.Clear();
            foreach (var b in bezierPoints)
            {
                vertices.Add(b.Position().X);
                vertices.Add(b.Position().Y);
                vertices.Add(b.Position().Z);
            }

            uint width = (uint)splitB + 3;
            for (uint i = 0; i < splitA; i++)
            {
                for (uint j = 0; j < splitB; j++)
                {
                    for (uint k = 0; k < 4; k++)
                    {
                        for (uint l = 0; l < 4; l++)
                        {
                            uint width_pos = j + l;
                            uint height_pos = i + k;
                            uint pos = height_pos * width + width_pos;
                            indices.Add(pos);
                        }
                    }
                }
            }
            //int idx = 0;
            //for (int i = 0; i < splitA; i++)
            //{
            //    for (int j = 0; j < splitB; j++)
            //    {
            //        for (int k = 0; k < SegmentsU; k++)
            //        {
            //            for (int l = 0; l < SegmentsV; l += 120)
            //            {
            //                float begin = (float)l / SegmentsV;
            //                float end = (float)(l + 120) / SegmentsV;
            //                int parts = ((SegmentsV - l) < 120) ? (SegmentsV - l) : 120;
            //                if (end > 1.0f) end = 1.0f;

            //                AddPatch(i, j, (float)k / SegmentsU, (float)(k + 1) / SegmentsU, begin, end, parts, idx);
            //                idx += 4;
            //            }
            //        }
            //    }
            //}

            bezierPatchC2Points = vertices.ToArray();
            bezierPatchC2Indices = indices.ToArray();
        }

        //void AddPatch(int i, int j, float t1, float t2, float begin, float end, int parts, int idx)
        //{
        //    int w = splitB + 3;
        //    int start = i * w + j;
        //    int indexStart = idx;


        //    for (int k = 0; k < 4; k++)
        //    {
        //        var pos = bezierPoints[start].Position();
        //        vertices.Add(pos.X);
        //        vertices.Add(pos.Y);
        //        vertices.Add(pos.Z);
        //        if (i == 0 && j == 0 && t1 == 0)
        //        {
        //            vertices.Add(0.12f);
        //        }
        //        else
        //        {
        //            vertices.Add(1.0f);
        //        }
        //        vertices.Add(0.0f);
        //        vertices.Add(0.0f);
        //        pos = bezierPoints[start + 1].Position();
        //        vertices.Add(pos.X);
        //        vertices.Add(pos.Y);
        //        vertices.Add(pos.Z);
        //        pos = bezierPoints[start + 2].Position();
        //        vertices.Add(pos.X);
        //        vertices.Add(pos.Y);
        //        vertices.Add(pos.Z);
        //        pos = bezierPoints[start + 3].Position();
        //        vertices.Add(pos.X);
        //        vertices.Add(pos.Y);
        //        vertices.Add(pos.Z);
        //        indices.Add((uint)idx);
        //        idx++;
        //        start += w;
        //    }

        //    vertices[15 * (indexStart + 1) + 3] = t1;
        //    vertices[15 * (indexStart + 1) + 4] = t2;
        //    vertices[15 * (indexStart + 2) + 3] = begin;
        //    vertices[15 * (indexStart + 2) + 4] = end;
        //    vertices[15 * (indexStart + 2) + 5] = parts;
        //}
    }
}
