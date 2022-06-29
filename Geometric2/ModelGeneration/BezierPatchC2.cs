using Geometric2.DrillLines;
using Geometric2.Global;
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
    public class BezierPatchC2 : Element, ISurface
    {
        public List<Vector3>[,] patches;
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


        public int bezierPatchC2Number;
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
        Texture texture = null;
        TextureUnit textureUnit;
        int textureId;

        private bool isTube;

        public bool wrapsU { get; set; }
        public bool wrapsV { get; set; }
        GlobalData globalData = null;

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
            this.FillPatches();
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

        public override void CreateGlElement(Shader _shader, ShaderGeometry _patchGeometryShader, TeselationShader _teselationShader, GlobalData globalData)
        {
            this.globalData = globalData;
            RegenerateBezierPatchC2();
            FillPatches();
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

            //var a_Position_Loc_gregoryShader = _teselationShader.GetAttribLocation("a_Position");
            bezierPatchC2VAO = GL.GenVertexArray();
            bezierPatchC2VBO = GL.GenBuffer();
            bezierPatchC2EBO = GL.GenBuffer();
            GL.BindVertexArray(bezierPatchC2VAO);
            GL.BindBuffer(BufferTarget.ArrayBuffer, bezierPatchC2VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, bezierPatchC2Points.Length * sizeof(float), bezierPatchC2Points, BufferUsageHint.DynamicDraw);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, bezierPatchC2EBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, bezierPatchC2Indices.Length * sizeof(uint), bezierPatchC2Indices, BufferUsageHint.DynamicDraw);
            //GL.VertexAttribPointer(a_Position_Loc_gregoryShader, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            //GL.EnableVertexAttribArray(0);
            var a_Position_Loc_gregoryShader = _teselationShader.GetAttribLocation("a_Position");
            GL.VertexAttribPointer(a_Position_Loc_gregoryShader, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);
            GL.EnableVertexAttribArray(a_Position_Loc_gregoryShader);

            var aTexCoords_gregoryShader = _teselationShader.GetAttribLocation("aTexCoords");
            GL.VertexAttribPointer(aTexCoords_gregoryShader, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));
            GL.EnableVertexAttribArray(aTexCoords_gregoryShader);
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

            _teselationShader.SetInt("showTrimmed", 0);
            var tex = texture;
            if (tex != null && globalData != null)
            {
                tex.Use(textureUnit);
                _teselationShader.SetInt("heightMap", textureId);
                if (textureId == 1)
                {
                    var showTrim = globalData.showTrim1 == true ? 1 : 0;
                    var trimmedPart = globalData.surface1_1 == true ? 1 : 0;
                    _teselationShader.SetInt("showTrimmed", showTrim);
                    _teselationShader.SetInt("trimmedPart", trimmedPart);

                }
                else if (textureId == 2)
                {
                    var showTrim = globalData.showTrim2 == true ? 1 : 0;
                    var trimmedPart = globalData.surface2_1 == true ? 1 : 0;
                    _teselationShader.SetInt("showTrimmed", showTrim);
                    _teselationShader.SetInt("trimmedPart", trimmedPart);
                }
            }

            GL.BindVertexArray(bezierPatchC2VAO);
            GL.PatchParameter(PatchParameterInt.PatchVertices, 16);
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
            GL.DrawElements(BeginMode.Patches, bezierPatchC2Indices.Length, DrawElementsType.UnsignedInt, 0);
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);

            _shader.Use();
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

        public void FillPatches()
        {
            patches = new List<Vector3>[splitA, splitB];
            UpdatePatches();
        }

        public void UpdatePatches()
        {
            uint width = (uint)splitB + 3;
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
                            uint width_pos = j + l;
                            uint height_pos = i + k;
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

            List<(Point, uint, float, float)> positions = new List<(Point, uint, float, float)>();

            uint width = (uint)splitB + 3;
            uint height = (uint)splitA + 3;
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

                            if (!positions.Contains((bezierPoints[(int)pos], pos, (float)(height_pos-1) / (float)(height - 3), (float)(width_pos-1) / (float)(width - 3))))
                            {
                                positions.Add((bezierPoints[(int)pos], pos, (float)(height_pos - 1) / (float)(height - 3), (float)(width_pos - 1) / (float)(width - 3)));
                            }
                        }
                    }
                }
            }

            var orderedPositions = positions.OrderBy(x => x.Item2).ToList();
            foreach (var b in orderedPositions)
            {
                vertices.Add(b.Item1.Position().X);
                vertices.Add(b.Item1.Position().Y);
                vertices.Add(b.Item1.Position().Z);
                vertices.Add(b.Item3);
                vertices.Add(b.Item4);
            }

            bezierPatchC2Points = vertices.ToArray();
            bezierPatchC2Indices = indices.ToArray();
        }

        public Vector3 P(float u, float v)
        {
            u = Clamp(u, 0, 1);
            v = Clamp(v, 0, 1);

            float PatchU_ = u * splitA;
            float PatchV_ = v * splitB;

            int patchA = 0;
            int patchB = 0;

            while (PatchU_ > 1.0f)
            {
                patchA++;
                PatchU_ -= 1.0f;
            }

            while (PatchV_ > 1.0f)
            {
                patchB++;
                PatchV_ -= 1.0f;
            }

            float PatchU = PatchU_;
            float PatchV = PatchV_;

            List<Vector3> points = patches[patchA, patchB];
            return CP(PatchU, PatchV, points);
        }

        public Vector3 CP(float u, float v, List<Vector3> points)
        {
            Vector3 deboor1 = HelpFunctions.DeBoor(v, points[0], points[1], points[2], points[3]);
            Vector3 deboor2 = HelpFunctions.DeBoor(v, points[4], points[5], points[6], points[7]);
            Vector3 deboor3 = HelpFunctions.DeBoor(v, points[8], points[9], points[10], points[11]);
            Vector3 deboor4 = HelpFunctions.DeBoor(v, points[12], points[13], points[14], points[15]);
            var res =  HelpFunctions.DeBoor(u, deboor1, deboor2, deboor3, deboor4);
            return res;
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

        public void SetTexture(Texture texture, TextureUnit textureUnit, int textureId)
        {
            this.texture = texture;
            this.textureUnit = textureUnit;
            this.textureId = textureId;
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

        private static int Clamp(int val, int min, int max)
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
                return -1;
            }
        }
    }
}
