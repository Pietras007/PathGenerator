using System;
using System.Collections.Generic;
using System.Drawing;
using Geometric2.Global;
using Geometric2.Helpers;
using Geometric2.MatrixHelpers;
using Geometric2.RasterizationClasses;
using OpenTK;
using OpenTK.Graphics.OpenGL4;

namespace Geometric2.ModelGeneration
{
    public class BezierC2 : Element
    {
        public bool DrawBSplineLine { get; set; }
        public bool DrawBernsteinLine { get; set; }
        public bool UseBernsteinBasis { get; set; }

        public List<Point> deBoorePoints { get; set; }
        public List<Point> bernsteinPoints { get; set; }
        public int bezierC2BernsteinLineVBO, bezierC2BernsteinLineVAO, bezierC2BernsteinLineEBO;
        private float[] bernsteinLinePoints;
        uint[] bernsteinLineIndices;

        public int bezierC2BSplineLineVBO, bezierC2BSplineLineVAO, bezierC2BSplineLineEBO;
        private float[] bSplineLinePoints;
        uint[] bSplineLineIndices;

        public int bezierC2VBO, bezierC2VAO, bezierC2EBO;
        private float[] bezierC2Points;
        uint[] bezierC2Indices;


        public int bezierC2Number;

        int renderNum;
        Camera _camera;
        int width, height;

        public BezierC2(int bezierC2Number, Camera _camera, int width, int height)
        {
            deBoorePoints = new List<Point>();
            bernsteinPoints = new List<Point>();
            this._camera = _camera;
            this.width = width;
            this.height = height;
            this.bezierC2Number = bezierC2Number;
            FullName = "BezierC2 " + bezierC2Number;
        }

        public override string ToString()
        {
            return FullName + " " + ElementName;
        }


        public override void CreateGlElement(Shader _shader, ShaderGeometry _geometryShader, TeselationShader _gregoryShader = null, GlobalData globalData = null)
        {
            var a_Position_Location = _shader.GetAttribLocation("a_Position");

            RegenerateBezierC2();
            bezierC2BernsteinLineVAO = GL.GenVertexArray();
            bezierC2BernsteinLineVBO = GL.GenBuffer();
            bezierC2BernsteinLineEBO = GL.GenBuffer();
            GL.BindVertexArray(bezierC2BernsteinLineVAO);
            GL.BindBuffer(BufferTarget.ArrayBuffer, bezierC2BernsteinLineVBO);
            GL.BufferData(BufferTarget.ArrayBuffer, bernsteinLinePoints.Length * sizeof(float), bernsteinLinePoints, BufferUsageHint.DynamicDraw);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, bezierC2BernsteinLineEBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, bernsteinLineIndices.Length * sizeof(uint), bernsteinLineIndices, BufferUsageHint.DynamicDraw);
            GL.VertexAttribPointer(a_Position_Location, 3, VertexAttribPointerType.Float, true, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(a_Position_Location);


            bezierC2BSplineLineVAO = GL.GenVertexArray();
            bezierC2BSplineLineVBO = GL.GenBuffer();
            bezierC2BSplineLineEBO = GL.GenBuffer();
            GL.BindVertexArray(bezierC2BSplineLineVAO);
            GL.BindBuffer(BufferTarget.ArrayBuffer, bezierC2BSplineLineVBO);
            GL.BufferData(BufferTarget.ArrayBuffer, bSplineLinePoints.Length * sizeof(float), bSplineLinePoints, BufferUsageHint.DynamicDraw);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, bezierC2BSplineLineEBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, bSplineLineIndices.Length * sizeof(uint), bSplineLineIndices, BufferUsageHint.DynamicDraw);
            GL.VertexAttribPointer(a_Position_Location, 3, VertexAttribPointerType.Float, true, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(a_Position_Location);


            var a_geometry_Position_Location = _geometryShader.GetAttribLocation("a_Position");
            bezierC2VAO = GL.GenVertexArray();
            bezierC2VBO = GL.GenBuffer();
            bezierC2EBO = GL.GenBuffer();
            GL.BindVertexArray(bezierC2VAO);
            GL.BindBuffer(BufferTarget.ArrayBuffer, bezierC2VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, bezierC2Points.Length * sizeof(float), bezierC2Points, BufferUsageHint.DynamicDraw);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, bezierC2EBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, bezierC2Indices.Length * sizeof(uint), bezierC2Indices, BufferUsageHint.DynamicDraw);
            GL.VertexAttribPointer(a_geometry_Position_Location, 3, VertexAttribPointerType.Float, true, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(a_geometry_Position_Location);
        }

        public override void RenderGlElement(Shader _shader, Vector3 rotationCentre, ShaderGeometry _geometryShader, TeselationShader _gregoryShader = null)
        {
            renderNum++;
            if (renderNum == 20)
            {
                renderNum = 0;
                RegenerateBezierC2();
            }


            TempRotationQuaternion = Quaternion.FromEulerAngles((float)(2 * Math.PI * ElementRotationX / 360), (float)(2 * Math.PI * ElementRotationY / 360), (float)(2 * Math.PI * ElementRotationZ / 360));
            Matrix4 model = ModelMatrix.CreateModelMatrix(ElementScale * TempElementScale, RotationQuaternion, CenterPosition + Translation + TemporaryTranslation, rotationCentre, TempRotationQuaternion);

            if (DrawBernsteinLine)
            {
                _shader.SetMatrix4("model", model);
                GL.BindVertexArray(bezierC2BernsteinLineVAO);
                _shader.SetVector3("fragmentColor", ColorHelper.ColorToVector(Color.Cyan));
                GL.DrawElements(PrimitiveType.Lines, 2 * bernsteinLinePoints.Length, DrawElementsType.UnsignedInt, 0);
                GL.BindVertexArray(0);
            }

            if (DrawBSplineLine)
            {
                _shader.SetMatrix4("model", model);
                GL.BindVertexArray(bezierC2BSplineLineVAO);
                _shader.SetVector3("fragmentColor", ColorHelper.ColorToVector(Color.Purple));
                GL.DrawElements(PrimitiveType.Lines, 2 * bSplineLinePoints.Length, DrawElementsType.UnsignedInt, 0);
                GL.BindVertexArray(0);
            }

            _geometryShader.Use();
            _geometryShader.SetMatrix4("model", model);
            _geometryShader.SetInt("width", width);
            _geometryShader.SetInt("height", height);
            GL.BindVertexArray(bezierC2VAO);
            if (IsSelected)
            {
                // _geometryShader.SetVector3("fragmentColor", ColorHelper.ColorToVector(Color.Orange));
            }
            else
            {
                // _geometryShader.SetVector3("fragmentColor", ColorHelper.ColorToVector(Color.Black));
            }

            GL.DrawElements(PrimitiveType.LinesAdjacency, bezierC2Indices.Length, DrawElementsType.UnsignedInt, 0);
            GL.BindVertexArray(0);

            _shader.Use();
        }

        public void MoveDeBoorePoints(int indexOfBernsteinPoint, int bernstainCase, int deBooreToMove)
        {
            Vector3 resultPosition = new Vector3(0, 0, 0);
            Vector3 mid, temporaryVector;

            switch (bernstainCase)
            {
                case 0:
                    mid = 0.5f * (deBoorePoints[deBooreToMove - 1].Position() + deBoorePoints[deBooreToMove + 1].Position());
                    temporaryVector = bernsteinPoints[indexOfBernsteinPoint].Position() - mid;
                    resultPosition = mid + 3.0f / 2.0f * temporaryVector;
                    break;

                case 1:
                    temporaryVector = bernsteinPoints[indexOfBernsteinPoint].Position() - deBoorePoints[deBooreToMove - 1].Position();
                    resultPosition = deBoorePoints[deBooreToMove - 1].Position() + 3.0f * temporaryVector;
                    break;

                case 2:
                    temporaryVector = bernsteinPoints[indexOfBernsteinPoint].Position() - deBoorePoints[deBooreToMove - 1].Position();
                    resultPosition = deBoorePoints[deBooreToMove - 1].Position() + 3.0f / 2.0f * temporaryVector;
                    break;

                case 3:
                    mid = 0.5f * (deBoorePoints[deBooreToMove - 1].Position() + deBoorePoints[deBooreToMove + 1].Position());
                    temporaryVector = bernsteinPoints[indexOfBernsteinPoint].Position() - mid;
                    resultPosition = mid + 3.0f / 2.0f * temporaryVector;
                    break;
            }

            deBoorePoints[deBooreToMove].CenterPosition = resultPosition;
            deBoorePoints[deBooreToMove].TemporaryTranslation = new Vector3(0, 0, 0);
            deBoorePoints[deBooreToMove].Translation = new Vector3(0, 0, 0);
        }



        public void RegenerateBezierC2()
        {
            int indexer = 0;
            if (!UseBernsteinBasis)
            {
                bernsteinPoints.Clear();
                for (int i = 3; i < deBoorePoints.Count; i++)
                {
                    Vector3 pos0 = deBoorePoints[i - 3].CenterPosition + deBoorePoints[i - 3].TemporaryTranslation + deBoorePoints[i - 3].Translation;
                    Vector3 pos1 = deBoorePoints[i - 2].CenterPosition + deBoorePoints[i - 2].TemporaryTranslation + deBoorePoints[i - 2].Translation;
                    Vector3 pos2 = deBoorePoints[i - 1].CenterPosition + deBoorePoints[i - 1].TemporaryTranslation + deBoorePoints[i - 1].Translation;
                    Vector3 pos3 = deBoorePoints[i].CenterPosition + deBoorePoints[i].TemporaryTranslation + deBoorePoints[i].Translation;

                    float[] xs = GenerateBernstainFromDeBoore(pos0.X, pos1.X, pos2.X, pos3.X);
                    float[] ys = GenerateBernstainFromDeBoore(pos0.Y, pos1.Y, pos2.Y, pos3.Y);
                    float[] zs = GenerateBernstainFromDeBoore(pos0.Z, pos1.Z, pos2.Z, pos3.Z);

                    if (bernsteinPoints.Count == 0)
                    {
                        bernsteinPoints.Add(new Point(new Vector3(xs[0], ys[0], zs[0]), -1, _camera));
                    }

                    bernsteinPoints.Add(new Point(new Vector3(xs[1], ys[1], zs[1]), -1, _camera));
                    bernsteinPoints.Add(new Point(new Vector3(xs[2], ys[2], zs[2]), -1, _camera));
                    bernsteinPoints.Add(new Point(new Vector3(xs[3], ys[3], zs[3]), -1, _camera));
                }
            }
            else
            {
                for (int i = 3; i < deBoorePoints.Count; i++)
                {
                    Vector3 pos0 = deBoorePoints[i - 3].CenterPosition + deBoorePoints[i - 3].TemporaryTranslation + deBoorePoints[i - 3].Translation;
                    Vector3 pos1 = deBoorePoints[i - 2].CenterPosition + deBoorePoints[i - 2].TemporaryTranslation + deBoorePoints[i - 2].Translation;
                    Vector3 pos2 = deBoorePoints[i - 1].CenterPosition + deBoorePoints[i - 1].TemporaryTranslation + deBoorePoints[i - 1].Translation;
                    Vector3 pos3 = deBoorePoints[i].CenterPosition + deBoorePoints[i].TemporaryTranslation + deBoorePoints[i].Translation;

                    float[] xs = GenerateBernstainFromDeBoore(pos0.X, pos1.X, pos2.X, pos3.X);
                    float[] ys = GenerateBernstainFromDeBoore(pos0.Y, pos1.Y, pos2.Y, pos3.Y);
                    float[] zs = GenerateBernstainFromDeBoore(pos0.Z, pos1.Z, pos2.Z, pos3.Z);

                    if (indexer == 0)
                    {
                        bernsteinPoints[indexer].CombinePosition();
                        bernsteinPoints[indexer].CenterPosition = new Vector3(xs[0], ys[0], zs[0]);
                        indexer++;
                    }
                    bernsteinPoints[indexer].CombinePosition();
                    bernsteinPoints[indexer].CenterPosition = new Vector3(xs[1], ys[1], zs[1]);
                    indexer++;

                    bernsteinPoints[indexer].CombinePosition();
                    bernsteinPoints[indexer].CenterPosition = new Vector3(xs[2], ys[2], zs[2]);
                    indexer++;

                    bernsteinPoints[indexer].CombinePosition();
                    bernsteinPoints[indexer].CenterPosition = new Vector3(xs[3], ys[3], zs[3]);
                    indexer++;
                }
            }

            FillBSplineGeometry();
            FillBernsteinGeometry();
            FillBezierGeometry();


            FilBSplinePolylineGeometry();
            FillBernsteinPolylineGeometry();
            FillBezierC2Geometry();
        }

        private float[] GenerateBernstainFromDeBoore(float _p1, float _p2, float _p3, float _p4)
        {
            float[] result = new float[4];

            float p1 = _p1 + 2.0f / 3.0f * (_p2 - _p1);
            float p2 = _p2 + 1.0f / 3.0f * (_p3 - _p2);
            float p3 = _p2 + 2.0f / 3.0f * (_p3 - _p2);
            float p4 = _p3 + 1.0f / 3.0f * (_p4 - _p3);

            result[0] = (p1 + p2) / 2.0f;
            result[1] = p2;
            result[2] = p3;
            result[3] = (p3 + p4) / 2.0f;
            return result;
        }

        private void FilBSplinePolylineGeometry()
        {
            GL.BindVertexArray(bezierC2BSplineLineVAO);
            GL.BindBuffer(BufferTarget.ArrayBuffer, bezierC2BSplineLineVBO);
            GL.BufferData(BufferTarget.ArrayBuffer, bSplineLinePoints.Length * sizeof(float), bSplineLinePoints, BufferUsageHint.DynamicDraw);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, bezierC2BSplineLineEBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, bSplineLineIndices.Length * sizeof(uint), bSplineLineIndices, BufferUsageHint.DynamicDraw);
        }

        private void FillBernsteinPolylineGeometry()
        {
            GL.BindVertexArray(bezierC2BernsteinLineVAO);
            GL.BindBuffer(BufferTarget.ArrayBuffer, bezierC2BernsteinLineVBO);
            GL.BufferData(BufferTarget.ArrayBuffer, bernsteinLinePoints.Length * sizeof(float), bernsteinLinePoints, BufferUsageHint.DynamicDraw);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, bezierC2BernsteinLineEBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, bernsteinLineIndices.Length * sizeof(uint), bernsteinLineIndices, BufferUsageHint.DynamicDraw);
        }

        private void FillBezierC2Geometry()
        {
            GL.BindVertexArray(bezierC2VAO);
            GL.BindBuffer(BufferTarget.ArrayBuffer, bezierC2VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, bezierC2Points.Length * sizeof(float), bezierC2Points, BufferUsageHint.DynamicDraw);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, bezierC2EBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, bezierC2Indices.Length * sizeof(uint), bezierC2Indices, BufferUsageHint.DynamicDraw);
        }

        private void FillBSplineGeometry()
        {
            bSplineLinePoints = new float[3 * deBoorePoints.Count];
            if (deBoorePoints.Count == 0)
            {
                bSplineLineIndices = new uint[0];
            }
            else
            {
                bSplineLineIndices = new uint[2 * (deBoorePoints.Count - 1)];
            }

            int indiceIdx = 0;
            for (int i = 0; i < deBoorePoints.Count; i++)
            {
                bSplineLinePoints[3 * i] = deBoorePoints[i].CenterPosition.X + deBoorePoints[i].TemporaryTranslation.X + deBoorePoints[i].Translation.X;
                bSplineLinePoints[3 * i + 1] = deBoorePoints[i].CenterPosition.Y + deBoorePoints[i].TemporaryTranslation.Y + deBoorePoints[i].Translation.Y;
                bSplineLinePoints[3 * i + 2] = deBoorePoints[i].CenterPosition.Z + deBoorePoints[i].TemporaryTranslation.Z + deBoorePoints[i].Translation.Z;

                if (i < deBoorePoints.Count - 1)
                {
                    bSplineLineIndices[indiceIdx] = (uint)i;
                    indiceIdx++;
                    bSplineLineIndices[indiceIdx] = (uint)i + 1;
                    indiceIdx++;
                }
            }

        }

        private void FillBernsteinGeometry()
        {
            bernsteinLinePoints = new float[3 * bernsteinPoints.Count];
            if (bernsteinPoints.Count == 0)
            {
                bernsteinLineIndices = new uint[0];
            }
            else
            {
                bernsteinLineIndices = new uint[2 * (bernsteinPoints.Count - 1)];
            }

            int indiceIdx = 0;
            for (int i = 0; i < bernsteinPoints.Count; i++)
            {
                bernsteinLinePoints[3 * i] = bernsteinPoints[i].CenterPosition.X + bernsteinPoints[i].TemporaryTranslation.X + bernsteinPoints[i].Translation.X;
                bernsteinLinePoints[3 * i + 1] = bernsteinPoints[i].CenterPosition.Y + bernsteinPoints[i].TemporaryTranslation.Y + bernsteinPoints[i].Translation.Y;
                bernsteinLinePoints[3 * i + 2] = bernsteinPoints[i].CenterPosition.Z + bernsteinPoints[i].TemporaryTranslation.Z + bernsteinPoints[i].Translation.Z;

                if (i < bernsteinPoints.Count - 1)
                {
                    bernsteinLineIndices[indiceIdx] = (uint)i;
                    indiceIdx++;
                    bernsteinLineIndices[indiceIdx] = (uint)i + 1;
                    indiceIdx++;
                }
            }

        }

        private void FillBezierGeometry()
        {
            int bezierParts = (int)Math.Ceiling((double)(bernsteinPoints.Count - 1) / 3.0);
            if (bernsteinPoints.Count == 1)
            {
                bezierParts = 1;
            }

            bezierC2Points = new float[3 * bezierParts * 4];
            if (bernsteinPoints.Count == 0)
            {
                bezierC2Indices = new uint[0];
            }
            else
            {
                bezierC2Indices = new uint[4 * bezierParts];
            }

            for (int i = 0; i < bezierParts * 4; i++)
            {
                int _x = i;
                if (_x >= bernsteinPoints.Count)
                {
                    bezierC2Points[3 * _x] = float.PositiveInfinity;
                }
                else
                {
                    bezierC2Points[3 * _x] = bernsteinPoints[_x].CenterPosition.X + bernsteinPoints[_x].TemporaryTranslation.X + bernsteinPoints[_x].Translation.X;
                    bezierC2Points[3 * _x + 1] = bernsteinPoints[_x].CenterPosition.Y + bernsteinPoints[_x].TemporaryTranslation.Y + bernsteinPoints[_x].Translation.Y;
                    bezierC2Points[3 * _x + 2] = bernsteinPoints[_x].CenterPosition.Z + bernsteinPoints[_x].TemporaryTranslation.Z + bernsteinPoints[_x].Translation.Z;
                }
            }

            int indiceIdx = 0;
            int x = 0;
            for (int k = 0; k < bezierParts; k++)
            {

                bezierC2Indices[indiceIdx] = (uint)x;
                indiceIdx++;
                bezierC2Indices[indiceIdx] = (uint)x + 1;
                indiceIdx++;
                bezierC2Indices[indiceIdx] = (uint)x + 2;
                indiceIdx++;
                bezierC2Indices[indiceIdx] = (uint)x + 3;
                indiceIdx++;
                x += 3;
            }
        }
    }
}
