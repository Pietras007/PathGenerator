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
        public List<Point> bezierPoints { get; set; }
        public int splitA, splitB;

        private List<float> vertices = new List<float>();
        private List<uint> indices = new List<uint>();
        private int pointNumber = 0;

        private int bezierPatchC0PolylineVBO, bezierPatchC0PolylineVAO, bezierPatchC0polylineEBO;
        private int bezierPatchC0VBO, bezierPatchC0VAO, bezierPatchC0EBO;
        private float[] polylinePoints;
        private uint[] polylineIndices;

        private float[] bezierPatchC0Points;
        private uint[] bezierPatchC0Indices;
        public int bezierPatchC0Number;

        private Camera _camera;
        private int width, height;
        private float _width, _length;
        private float x0, y0;
        private float r;

        private bool isTube;

        public BezierPatchC0(int bezierC0Number, Camera _camera, int width, int height, float[] values, bool isTube = false)
        {
            bezierPoints = new List<Point>();
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
            this.bezierPatchC0Number = bezierC0Number;
            this.isTube = isTube;
            FullName = "BezierPatchC0 " + bezierPatchC0Number;
            GenerateBezierPoints();
        }

        public BezierPatchC0(int bezierC0Number, Camera _camera, int width, int height, bool isTube = false)
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
            this.isTube = isTube;
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

            var a_Position_Loc_gregoryShader = _teselationShader.GetAttribLocation("a_Position");
            bezierPatchC0VAO = GL.GenVertexArray();
            bezierPatchC0VBO = GL.GenBuffer();
            bezierPatchC0EBO = GL.GenBuffer();
            GL.BindVertexArray(bezierPatchC0VAO);
            GL.BindBuffer(BufferTarget.ArrayBuffer, bezierPatchC0VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, bezierPatchC0Points.Length * sizeof(float), bezierPatchC0Points, BufferUsageHint.DynamicDraw);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, bezierPatchC0EBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, bezierPatchC0Indices.Length * sizeof(uint), bezierPatchC0Indices, BufferUsageHint.DynamicDraw);
            GL.VertexAttribPointer(a_Position_Loc_gregoryShader, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);
        }

        public override void RenderGlElement(Shader _shader, Vector3 rotationCentre, ShaderGeometry _patchGeometryShader, TeselationShader _teselationShader)
        {
            RegenerateBezierPatchC0();

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

            GL.BindVertexArray(bezierPatchC0VAO);
            GL.PatchParameter(PatchParameterInt.PatchVertices, 16);
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
            GL.DrawElements(BeginMode.Patches, bezierPatchC0Indices.Length, DrawElementsType.UnsignedInt, 0);
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
            GL.BindVertexArray(bezierPatchC0VAO);
            GL.BindBuffer(BufferTarget.ArrayBuffer, bezierPatchC0VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, bezierPatchC0Points.Length * sizeof(float), bezierPatchC0Points, BufferUsageHint.DynamicDraw);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, bezierPatchC0EBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, bezierPatchC0Indices.Length * sizeof(uint), bezierPatchC0Indices, BufferUsageHint.DynamicDraw);
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
                float zDiff = _length / (3 * splitA);
                float angleDiff = 2 * (float)Math.PI / (3 * splitB);
                int k = 0;
                for (int i = 0; i < 3 * splitA + 1; i++)
                {
                    float angle = 0.0f;
                    for (int j = 0; j < 3 * splitB; j++)
                    {
                        Point point = new Point(new Vector3(r * (float)Math.Sin(angle), r * (float)Math.Cos(angle), z), pointNumber, _camera);
                        pointNumber++;
                        point.FullName += "_patchTubeC0_" + bezierPatchC0Number;
                        bezierPoints.Add(point);
                        angle += angleDiff;
                        k++;
                    }
                    bezierPoints.Add(bezierPoints[k - 3 * splitB]);
                    k++;
                    z += zDiff;
                }
            }
            else
            {
                float xdiff = _width / (3 * splitA);
                float ydiff = _length / (3 * splitB);
                int k = 0;
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
        }

        private void FillPolylineGeometry()
        {
            List<Point> pointLines = new List<Point>();
            int k = 0;
            for (int i = 0; i < 3 * splitA + 1; i++)
            {
                if (isTube)
                {
                    for (int j = 0; j < 3 * splitB; j++)
                    {
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
                else
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

            uint width = (uint)splitB * 3 + 1;
            for (uint i = 0; i < splitA; i++)
            {
                for (uint j = 0; j < splitB; j++)
                {
                    for (uint k = 0; k < 4; k++)
                    {
                        for (uint l = 0; l < 4; l++)
                        {
                            uint width_pos = 3 * j + l;
                            uint height_pos = 3 * i + k;
                            uint pos = height_pos * width + width_pos;
                            indices.Add(pos);
                        }
                    }
                }
            }

            bezierPatchC0Points = vertices.ToArray();
            bezierPatchC0Indices = indices.ToArray();
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
    }
}
