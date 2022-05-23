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
    public class BezierPatchC0 : Element
    {
        public bool DrawPolyline { get; set; }
        public float PlaneWidth { get; set; }
        public float PlaneHeight { get; set; }
        public int SegmentsU { get; set; }
        public int SegmentsV { get; set; }

        List<float> vertices = new List<float>();
        List<uint> indices = new List<uint>();
        int pointNumber = 0;


        public List<Point> bezierPoints { get; set; }
        public int bezierPatchC0PolylineVBO, bezierPatchC0PolylineVAO, bezierPatchC0polylineEBO;
        public int bezierPatchC0VBO, bezierPatchC0VAO, bezierPatchC0EBO;
        private float[] polylinePoints;
        uint[] polylineIndices;

        private float[] bezierPatchC0Points;
        uint[] bezierPatchC0Indices;

        int bezierPatchC0Number;

        public int GregoryVBO, GregoryVAO, GregoryEBO;

        int renderNum;
        Camera _camera;
        int width, height;
        float _width, _length;
        public int splitA, splitB;
        float x0, y0;

        private List<float> GregoryPointsList = new List<float>();
        private List<uint> GregoryIndicesList = new List<uint>();

        private float[] GregoryPoints = new float[3 * 16];
        uint[] GregoryIndices = new uint[16];

        public BezierPatchC0(int bezierC0Number, Camera _camera, int width, int height, float[] values)
        {
            bezierPoints = new List<Point>();
            this._camera = _camera;
            this._width = values[0];
            this._length = values[1];
            this.splitA = (int)values[2];
            this.splitB = (int)values[3];
            this.SegmentsU = 1;
            this.SegmentsV = 1;
            x0 = 0.0f;
            y0 = 0.0f;


            this.width = width;
            this.height = height;
            this.bezierPatchC0Number = bezierC0Number;
            FullName = "BezierPatchC0 " + bezierPatchC0Number;
            GenerateBezierPoints();
        }

        public BezierPatchC0(int bezierC0Number, Camera _camera, int width, int height)
        {
            bezierPoints = new List<Point>();
            this._camera = _camera;
            this.SegmentsU = 1;
            this.SegmentsV = 1;
            x0 = 0.0f;
            y0 = 0.0f;


            this.width = width;
            this.height = height;
            this.bezierPatchC0Number = bezierC0Number;
        }

        public override string ToString()
        {
            return FullName + " " + ElementName;
        }

        public override void CreateGlElement(Shader _shader, ShaderGeometry _patchGeometryShader, TeselationShader _teselationShader)
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


            //var a_geometry_Position_Location = _patchGeometryShader.GetAttribLocation("a_Position");
            //bezierPatchC0VAO = GL.GenVertexArray();
            //bezierPatchC0VBO = GL.GenBuffer();
            //bezierPatchC0EBO = GL.GenBuffer();
            //GL.BindVertexArray(bezierPatchC0VAO);
            //GL.BindBuffer(BufferTarget.ArrayBuffer, bezierPatchC0VBO);
            //GL.BufferData(BufferTarget.ArrayBuffer, bezierPatchC0Points.Length * sizeof(float), bezierPatchC0Points, BufferUsageHint.DynamicDraw);
            //GL.BindBuffer(BufferTarget.ElementArrayBuffer, bezierPatchC0EBO);
            //GL.BufferData(BufferTarget.ElementArrayBuffer, bezierPatchC0Indices.Length * sizeof(uint), bezierPatchC0Indices, BufferUsageHint.DynamicDraw);

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

            var a_Position_Loc_gregoryShader = _teselationShader.GetAttribLocation("a_Position");
            GregoryVAO = GL.GenVertexArray();
            GregoryVBO = GL.GenBuffer();
            GregoryEBO = GL.GenBuffer();
            GL.BindVertexArray(GregoryVAO);
            GL.BindBuffer(BufferTarget.ArrayBuffer, GregoryVBO);
            GL.BufferData(BufferTarget.ArrayBuffer, GregoryPoints.Length * sizeof(float), GregoryPoints, BufferUsageHint.DynamicDraw);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, GregoryEBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, GregoryIndices.Length * sizeof(uint), GregoryIndices, BufferUsageHint.DynamicDraw);
            GL.VertexAttribPointer(a_Position_Loc_gregoryShader, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);
        }

        public override void RenderGlElement(Shader _shader, Vector3 rotationCentre, ShaderGeometry _patchGeometryShader, TeselationShader _teselationShader)
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
            /*
            _patchGeometryShader.Use();
            _patchGeometryShader.SetMatrix4("model", model2);
            if (IsSelected)
            {
                _patchGeometryShader.SetVector3("fragmentColor", ColorHelper.ColorToVector(Color.Orange));
            }
            else
            {
                _patchGeometryShader.SetVector3("fragmentColor", ColorHelper.ColorToVector(Color.Black));
            }
            //_patchGeometryShader.SetMatrix4("view", _camera.GetViewMatrix());
            //_patchGeometryShader.SetMatrix4("projection", _camera.GetProjectionMatrix());
            //_patchGeometryShader.SetVector3("fragmentColor", ColorHelper.ColorToVector(Color.White));
            //_patchGeometryShader.SetMatrix4("persp", _camera.GetProjectionMatrix());
            //_patchGeometryShader.SetInt("width", width);
            //_patchGeometryShader.SetInt("height", height);
            GL.BindVertexArray(bezierPatchC0VAO);
            GL.DrawElements(PrimitiveType.LinesAdjacency, bezierPatchC0Indices.Length, DrawElementsType.UnsignedInt, 0);
            GL.BindVertexArray(0);

            _shader.Use();*/

            _teselationShader.Use();
            _teselationShader.SetMatrix4("model", model);
            _teselationShader.SetFloat("splitA", SegmentsU);
            _teselationShader.SetFloat("splitB", SegmentsV);
            if (IsSelected)
            {
                _teselationShader.SetVector3("fragmentColor", ColorHelper.ColorToVector(Color.Orange));
            }
            else
            {
                _teselationShader.SetVector3("fragmentColor", ColorHelper.ColorToVector(Color.Black));
            }

            GL.BindVertexArray(GregoryVAO);
            GL.PatchParameter(PatchParameterInt.PatchVertices, 16);
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
            GL.DrawElements(BeginMode.Patches, GregoryIndices.Length, DrawElementsType.UnsignedInt, 0);
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);

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
            GL.BindVertexArray(GregoryVAO);
            GL.BindBuffer(BufferTarget.ArrayBuffer, GregoryVBO);
            GL.BufferData(BufferTarget.ArrayBuffer, GregoryPoints.Length * sizeof(float), GregoryPoints, BufferUsageHint.DynamicDraw);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, GregoryEBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, GregoryIndices.Length * sizeof(uint), GregoryIndices, BufferUsageHint.DynamicDraw);


            //    GL.BindVertexArray(bezierPatchC0VAO);
            //    GL.BindBuffer(BufferTarget.ArrayBuffer, bezierPatchC0VBO);
            //    GL.BufferData(BufferTarget.ArrayBuffer, bezierPatchC0Points.Length * sizeof(float), bezierPatchC0Points, BufferUsageHint.DynamicDraw);
            //    GL.BindBuffer(BufferTarget.ElementArrayBuffer, bezierPatchC0EBO);
            //    GL.BufferData(BufferTarget.ElementArrayBuffer, bezierPatchC0Indices.Length * sizeof(uint), bezierPatchC0Indices, BufferUsageHint.DynamicDraw);
        }

        private void GenerateBezierPoints()
        {
            bezierPoints.Clear();
            float xdiff = _width / (3 * splitA);
            float ydiff = _length / (3 * splitB);
            int k = 0;
            uint idx = 0;
            for (int i = 0; i < 3 * splitA + 1; i++)
            {
                y0 = 0.0f;
                for (int j = 0; j < 3 * splitB + 1; j++)
                {
                    Point point = new Point(new Vector3(x0, y0, 0.0f), pointNumber, _camera);
                    pointNumber++;
                    point.FullName += "_patchC0_" + bezierPatchC0Number;
                    bezierPoints.Add(point);
                    y0 += ydiff;
                    k++;
                }

                x0 += xdiff;
            }
        }

        private void FillPolylineGeometry()
        {
            List<Point> pointLines = new List<Point>();
            int k = 0;
            for (int i = 0; i < 3 * splitA + 1; i++)
            {
                for (int j = 0; j < 3 * splitB + 1; j++)
                {
                    if (j != 0)
                    {
                        pointLines.Add(bezierPoints[k - 1]);
                        pointLines.Add(bezierPoints[k]);
                    }
                    if (i != 0)
                    {
                        pointLines.Add(bezierPoints[k - (3 * splitB + 1)]);
                        pointLines.Add(bezierPoints[k]);
                    }
                    k++;
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
                    //polylineIndices[indiceIdx] = (uint)i + 1;
                    //indiceIdx++;
                }
            }
        }

        private void FillBezierPatchGeometry()
        {
            uint idxxx = 0;
            for (int i = 0; i < splitA; i++)
            {
                for (int j = 0; j < splitB; j++)
                {
                    for (int k = 0; k < 4; k++)
                    {
                        for (int l = 0; l < 4; l++)
                        //for (int l = 0; l < 4; l++)
                        //{
                        //    for (int k = 0; k < 4; k++)
                        {
                            int width_size = splitA * 3 + 1;
                            int width_pos = l + i * 3;
                            int height_amount = width_size * 3 * j;
                            GregoryPointsList.Add(bezierPoints[height_amount + k * width_size + width_pos].Position().X);
                            GregoryPointsList.Add(bezierPoints[height_amount + k * width_size + width_pos].Position().Y);
                            GregoryPointsList.Add(bezierPoints[height_amount + k * width_size + width_pos].Position().Z);
                            GregoryIndicesList.Add(idxxx); idxxx++;
                        }
                    }
                }
            }

            //foreach (var point in bezierPoints)
            //{
            //    GregoryPointsList.Add(point.Position().X);
            //    GregoryPointsList.Add(point.Position().Y);
            //    GregoryPointsList.Add(point.Position().Z);
            //    GregoryIndicesList.Add(idxxx); idxxx++;


                GregoryPoints = GregoryPointsList.ToArray();
                GregoryIndices = GregoryIndicesList.ToArray();
            //}


            //vertices.Clear();
            //indices.Clear();
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

            //bezierPatchC0Points = vertices.ToArray();
            //bezierPatchC0Indices = indices.ToArray();
        }

        public List<SinglePatch> GetAllPatches()
        {
            List<SinglePatch> res = new List<SinglePatch>();// w razie czego tu szukac bledu
            int stride = 3 * splitB + 1;
            for (int i = 0; i < splitA; ++i)
            {
                for (int j = 0; j < splitB; ++j)
                {
                    List<List<Point>> patch = new List<List<Point>>();
                    for (int x = 0; x < 4; ++x)
                    {
                        List<Point> line = new List<Point>();
                        int start = (3 * i + x) * stride + j * 3;
                        for (int y = 0; y < 4; ++y)
                        {
                            line.Add(bezierPoints[start + y]);
                        }
                        patch.Add(line);
                    }
                    SinglePatch sp;
                    sp.bezier = this;
                    sp.patch = patch;
                    res.Add(sp);
                }
            }
            return res;
        }

        //void AddPatch(int i, int j, float t1, float t2, float begin, float end, int parts, int idx)
        //{
        //    int w = 3 * splitB + 1;
        //    int beginindex = 3 * i * w + 3 * j;
        //    int indexstart = idx;

        //    for (int k = 0; k < 4; k++)
        //    {
        //        var pos = bezierPoints[beginindex].Position();
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
        //        pos = bezierPoints[beginindex + 1].Position();
        //        vertices.Add(pos.X);
        //        vertices.Add(pos.Y);
        //        vertices.Add(pos.Z);
        //        pos = bezierPoints[beginindex + 2].Position();
        //        vertices.Add(pos.X);
        //        vertices.Add(pos.Y);
        //        vertices.Add(pos.Z);
        //        pos = bezierPoints[beginindex + 3].Position();
        //        vertices.Add(pos.X);
        //        vertices.Add(pos.Y);
        //        vertices.Add(pos.Z);
        //        indices.Add((uint)idx);
        //        idx++;
        //        beginindex += w;
        //    }
        //    vertices[15 * (indexstart + 1) + 3] = t1;
        //    vertices[15 * (indexstart + 1) + 4] = t2;
        //    vertices[15 * (indexstart + 2) + 3] = begin;
        //    vertices[15 * (indexstart + 2) + 4] = end;
        //    vertices[15 * (indexstart + 2) + 5] = parts;
        //}

    }
}
