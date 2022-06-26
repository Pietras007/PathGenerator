using Geometric2.DrillLines;
using Geometric2.Helpers;
using Geometric2.MatrixHelpers;
using Geometric2.RasterizationClasses;
using Intersect;
using OpenTK;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Geometric2.ModelGeneration
{
    public class BezierPatchC0 : Element, ISurface
    {
        public List<Vector3>[,] patches;
        public bool DrawPolyline { get; set; }
        public float PlaneWidth { get; set; }
        public float PlaneHeight { get; set; }
        public int SegmentsU { get; set; }
        public int SegmentsV { get; set; }
        public List<Point> bezierPoints { get; set; }
        public int splitA, splitB;

        private List<float> vertices = new List<float>();
        private List<uint> indices = new List<uint>();
        private int[] pointNumber = new int[1];

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

        public bool isTube;

        public bool wrapsU { get; set; }
        public bool wrapsV { get; set; }

        public BezierPatchC0(int[] pointNumber, int bezierC0Number, Camera _camera, int width, int height, float[] values, bool isTube = false)
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
            this.bezierPatchC0Number = bezierC0Number;
            this.isTube = isTube;
            //this.wrapsV = isTube;
            FullName = "BezierPatchC0 " + bezierPatchC0Number;
            GenerateBezierPoints();
            this.FillPatches();
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
            //this.wrapsV = isTube;
        }

        public override string ToString()
        {
            return FullName + " " + ElementName;
        }

        public override void CreateGlElement(Shader _shader, ShaderGeometry _patchGeometryShader, TeselationShader _teselationShader)
        {
            RegenerateBezierPatchC0();
            FillPatches();
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
            UpdatePatches();
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
                        Point point = new Point(new Vector3(r * (float)Math.Sin(angle), r * (float)Math.Cos(angle), z), pointNumber[0], _camera);
                        pointNumber[0]++;
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
                        Point point = new Point(new Vector3(x0, y0, 0.0f), pointNumber[0], _camera);
                        pointNumber[0]++;
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

        public List<PatchC0> GetAllPatches()
        {
            List<PatchC0> res = new List<PatchC0>();
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
                    PatchC0 sp;
                    sp.bezier = this;
                    sp.patch = patch;
                    res.Add(sp);
                }
            }
            return res;
        }

        public Vector3 P(float u, float v)
        {
            u = Clamp(u, 0, 1);
            v = Clamp(v, 0, 1);
            float valA = (u * splitA);
            float valB = (v * splitB);
            var patchA = (int)Math.Floor(valA);
            if (patchA == splitA) patchA--;
            var patchB = (int)Math.Floor(valB);
            if (patchB == splitB) patchB--;
            float patchU = valA - patchA;
            float patchV = valB - patchB;
            List<Vector3> points = patches[patchA, patchB];
            return CP(patchU, patchV, points);
        }

        public void FillPatches()
        {
            patches = new List<Vector3>[splitA, splitB];
            UpdatePatches();
        }

        public void UpdatePatches()
        {
            uint width = (uint)splitB * 3 + 1;
            int patch_idx = 0;
            int point_patch_idx = 0;
            for (uint i = 0; i < splitA; i++)
            {
                for (uint j = 0; j < splitB; j++)
                {
                    Vector3[] _pointrefs = new Vector3[16];
                    point_patch_idx = 0;
                    for (uint k = 0; k < 4; k++)
                    {
                        for (uint l = 0; l < 4; l++)
                        {
                            uint width_pos = 3 * j + l;
                            uint height_pos = 3 * i + k;
                            uint pos = height_pos * width + width_pos;
                            _pointrefs[point_patch_idx] = bezierPoints[(int)pos].Position();
                            point_patch_idx++;
                        }
                    }

                    patches[i, j] = _pointrefs.ToList();
                    patch_idx++;
                }
            }
        }

        public Vector3 CP(float v, float u, List<Vector3> points)
        {
            Vector3 four1 = new Vector3();
            Vector3 four2 = new Vector3();
            Vector3 four3 = new Vector3();
            Vector3 four4 = new Vector3();


            four1.X = HelpFunctions.DeKastilio(new float[] { points[0].X, points[1].X, points[2].X, points[3].X }, u, 4);
            four1.Y = HelpFunctions.DeKastilio(new float[] { points[0].Y, points[1].Y, points[2].Y, points[3].Y }, u, 4);
            four1.Z = HelpFunctions.DeKastilio(new float[] { points[0].Z, points[1].Z, points[2].Z, points[3].Z }, u, 4);

            four2.X = HelpFunctions.DeKastilio(new float[] { points[4 + 0].X, points[4 + 1].X, points[4 + 2].X, points[4 + 3].X }, u, 4);
            four2.Y = HelpFunctions.DeKastilio(new float[] { points[4 + 0].Y, points[4 + 1].Y, points[4 + 2].Y, points[4 + 3].Y }, u, 4);
            four2.Z = HelpFunctions.DeKastilio(new float[] { points[4 + 0].Z, points[4 + 1].Z, points[4 + 2].Z, points[4 + 3].Z }, u, 4);

            four3.X = HelpFunctions.DeKastilio(new float[] { points[8 + 0].X, points[8 + 1].X, points[8 + 2].X, points[8 + 3].X }, u, 4);
            four3.Y = HelpFunctions.DeKastilio(new float[] { points[8 + 0].Y, points[8 + 1].Y, points[8 + 2].Y, points[8 + 3].Y }, u, 4);
            four3.Z = HelpFunctions.DeKastilio(new float[] { points[8 + 0].Z, points[8 + 1].Z, points[8 + 2].Z, points[8 + 3].Z }, u, 4);

            four4.X = HelpFunctions.DeKastilio(new float[] { points[12 + 0].X, points[12 + 1].X, points[12 + 2].X, points[12 + 3].X }, u, 4);
            four4.Y = HelpFunctions.DeKastilio(new float[] { points[12 + 0].Y, points[12 + 1].Y, points[12 + 2].Y, points[12 + 3].Y }, u, 4);
            four4.Z = HelpFunctions.DeKastilio(new float[] { points[12 + 0].Z, points[12 + 1].Z, points[12 + 2].Z, points[12 + 3].Z }, u, 4);

            Vector3 currentPoint = new Vector3(HelpFunctions.DeKastilio(new float[] { four1.X, four2.X, four3.X, four4.X }, v, 4),
                HelpFunctions.DeKastilio(new float[] { four1.Y, four2.Y, four3.Y, four4.Y }, v, 4),
                HelpFunctions.DeKastilio(new float[] { four1.Z, four2.Z, four3.Z, four4.Z }, v, 4));

            return currentPoint;
        }

        public Vector3 T(float u, float v)
        {
            if (u + 1e-4f < 1)
                return (P(u + 1e-4f, v) - (P(u, v))) / (1e-4f);
            else
                return (P(u, v) - (P(u - 1e-4f, v))) / (1e-4f);
        }

        public Vector3 B(float u, float v)
        {
            if (v + 1e-4f < 1)
                return (P(u, v + 1e-4f) - P(u, v)) / (1e-4f);
            else
                return (P(u, v) - P(u, v - 1e-4f)) / (1e-4f);
        }

        public Vector3 N(float u, float v)
        {
            Vector3 tangent = T(u, v);
            Vector3 bitangent = B(u, v);
            Vector3 normal = Vector3.Cross(tangent, bitangent);
            return normal.Normalized();
        }

        public bool WrapsU()
        {
            return wrapsU;
        }

        public bool WrapsV()
        {
            return wrapsV;
        }

        private static float Clamp(float val, float min, float max)
        {
            if (val < min)
            {
                return min;
            }
            else if (val > max)
            {
                return max;
            }
            else if (val >= min && val <= max)
            {
                return val;
            }
            else
            {
                return 0.5f;
            }
        }
    }
}
