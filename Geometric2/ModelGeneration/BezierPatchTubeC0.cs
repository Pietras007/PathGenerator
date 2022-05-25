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
        public List<Point> bezierPoints { get; set; }
        public int splitA, splitB;

        private List<float> vertices = new List<float>();
        private List<uint> indices = new List<uint>();
        private int pointNumber = 0;

        public int bezierPatchC0PolylineVBO, bezierPatchC0PolylineVAO, bezierPatchC0polylineEBO;
        public int bezierPatchC0VBO, bezierPatchC0VAO, bezierPatchC0EBO;
        private float[] polylinePoints;
        uint[] polylineIndices;

        private float[] bezierPatchC0Points;
        uint[] bezierPatchC0Indices;
        int bezierPatchTubeC0Number;

        private Camera _camera;
        private int width, height;
        private float _width, _length;
        private float r;
        private float x, y;

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
            bezierPoints.Clear();
            if (splitB < 3) splitB = 3;
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
            int k = 0;
            for (int i = 0; i < 3 * splitA + 1; i++)
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
    }
}
