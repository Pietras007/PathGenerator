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
    public class InterpolatedBezierC2 : Element
    {
        public bool DrawPolyline { get; set; }
        public List<Point> interpolatedBC2Points { get; set; }
        public List<Point> bernsteinListPoints { get; set; }

        public int interpolatedPolylineC2VBO, interpolatedPolylineC2VAO, interpolatedPolylineC2EBO;
        private float[] polylinePoints;
        uint[] polylineIndices;

        public int interpolatedBezierC2VBO, interpolatedBezierC2VAO, interpolatedBezierC2EBO;
        private float[] interpolatedBezierC2Points;
        uint[] interpolatedBezierC2Indices;

        int renderNum;
        public int interpolatedBezierC2Number;
        Camera _camera;
        int width, height;

        public InterpolatedBezierC2(int interpolatedBezierC2Number, Camera _camera, int width, int height)
        {
            interpolatedBC2Points = new List<Point>();
            bernsteinListPoints = new List<Point>();

            this._camera = _camera;
            this.width = width;
            this.height = height;
            this.interpolatedBezierC2Number = interpolatedBezierC2Number;
            FullName = "Interpolated BezierC2 " + interpolatedBezierC2Number;
        }
        public override string ToString()
        {
            return FullName + " " + ElementName;
        }

        public override void CreateGlElement(Shader _shader, ShaderGeometry _geometryShader, TeselationShader _gregoryShader = null)
        {
            RegenerateInterpolatedBezierC2();
            interpolatedPolylineC2VAO = GL.GenVertexArray();
            interpolatedPolylineC2VBO = GL.GenBuffer();
            interpolatedPolylineC2EBO = GL.GenBuffer();
            var a_Position_Location = _shader.GetAttribLocation("a_Position");
            GL.BindVertexArray(interpolatedPolylineC2VAO);
            GL.BindBuffer(BufferTarget.ArrayBuffer, interpolatedPolylineC2VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, polylinePoints.Length * sizeof(float), polylinePoints, BufferUsageHint.DynamicDraw);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, interpolatedPolylineC2EBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, polylineIndices.Length * sizeof(uint), polylineIndices, BufferUsageHint.DynamicDraw);
            GL.VertexAttribPointer(a_Position_Location, 3, VertexAttribPointerType.Float, true, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(a_Position_Location);


            var a_geometry_Position_Location = _geometryShader.GetAttribLocation("a_Position");
            interpolatedBezierC2VAO = GL.GenVertexArray();
            interpolatedBezierC2VBO = GL.GenBuffer();
            interpolatedBezierC2EBO = GL.GenBuffer();
            GL.BindVertexArray(interpolatedBezierC2VAO);
            GL.BindBuffer(BufferTarget.ArrayBuffer, interpolatedBezierC2VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, interpolatedBezierC2Points.Length * sizeof(float), interpolatedBezierC2Points, BufferUsageHint.DynamicDraw);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, interpolatedBezierC2EBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, interpolatedBezierC2Indices.Length * sizeof(uint), interpolatedBezierC2Indices, BufferUsageHint.DynamicDraw);
            GL.VertexAttribPointer(a_geometry_Position_Location, 3, VertexAttribPointerType.Float, true, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(a_geometry_Position_Location);
        }

        public override void RenderGlElement(Shader _shader, Vector3 rotationCentre, ShaderGeometry _geometryShader, TeselationShader _gregoryShader = null)
        {
            renderNum++;
            if (renderNum == 20)
            {
                renderNum = 0;
                RegenerateInterpolatedBezierC2();
            }


            TempRotationQuaternion = Quaternion.FromEulerAngles((float)(2 * Math.PI * ElementRotationX / 360), (float)(2 * Math.PI * ElementRotationY / 360), (float)(2 * Math.PI * ElementRotationZ / 360));
            Matrix4 model = ModelMatrix.CreateModelMatrix(ElementScale * TempElementScale, RotationQuaternion, CenterPosition + Translation + TemporaryTranslation, rotationCentre, TempRotationQuaternion);

            if (DrawPolyline)
            {
                _shader.SetMatrix4("model", model);
                GL.BindVertexArray(interpolatedPolylineC2VAO);
                _shader.SetVector3("fragmentColor", ColorHelper.ColorToVector(Color.Cyan));
                GL.DrawElements(PrimitiveType.Lines, 2 * polylinePoints.Length, DrawElementsType.UnsignedInt, 0);
                GL.BindVertexArray(0);
            }

            _geometryShader.Use();
            _geometryShader.SetMatrix4("model", model);
            _geometryShader.SetInt("width", width);
            _geometryShader.SetInt("height", height);
            GL.BindVertexArray(interpolatedBezierC2VAO);
            if (IsSelected)
            {
                // _geometryShader.SetVector3("fragmentColor", ColorHelper.ColorToVector(Color.Orange));
            }
            else
            {
                // _geometryShader.SetVector3("fragmentColor", ColorHelper.ColorToVector(Color.Black));
            }

            GL.DrawElements(PrimitiveType.LinesAdjacency, interpolatedBezierC2Indices.Length, DrawElementsType.UnsignedInt, 0);
            GL.BindVertexArray(0);

            _shader.Use();
        }

        public void RegenerateInterpolatedBezierC2()
        {
            bernsteinListPoints = new List<Point>();
            RecalculateBezierC2Geometry();

            FillPolylineGeometry();
            FillBezierC2Geometry();

            FillInterpolatedBezierC2Geometry();
            FillInterpolatedPolylineGeometry();
        }

        public void FillInterpolatedPolylineGeometry()
        {
            GL.BindVertexArray(interpolatedPolylineC2VAO);
            GL.BindBuffer(BufferTarget.ArrayBuffer, interpolatedPolylineC2VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, polylinePoints.Length * sizeof(float), polylinePoints, BufferUsageHint.DynamicDraw);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, interpolatedPolylineC2EBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, polylineIndices.Length * sizeof(uint), polylineIndices, BufferUsageHint.DynamicDraw);
        }
        public void FillInterpolatedBezierC2Geometry()
        {
            GL.BindVertexArray(interpolatedBezierC2VAO);
            GL.BindBuffer(BufferTarget.ArrayBuffer, interpolatedBezierC2VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, interpolatedBezierC2Points.Length * sizeof(float), interpolatedBezierC2Points, BufferUsageHint.DynamicDraw);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, interpolatedBezierC2EBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, interpolatedBezierC2Indices.Length * sizeof(uint), interpolatedBezierC2Indices, BufferUsageHint.DynamicDraw);
        }

        public void FillPolylineGeometry()
        {
            polylinePoints = new float[3 * bernsteinListPoints.Count];
            if (bernsteinListPoints.Count == 0)
            {
                polylineIndices = new uint[0];
            }
            else
            {
                polylineIndices = new uint[2 * (bernsteinListPoints.Count - 1)];
            }

            int indiceIdx = 0;
            for (int i = 0; i < bernsteinListPoints.Count; i++)
            {
                polylinePoints[3 * i] = bernsteinListPoints[i].CenterPosition.X + bernsteinListPoints[i].TemporaryTranslation.X + bernsteinListPoints[i].Translation.X;
                polylinePoints[3 * i + 1] = bernsteinListPoints[i].CenterPosition.Y + bernsteinListPoints[i].TemporaryTranslation.Y + bernsteinListPoints[i].Translation.Y;
                polylinePoints[3 * i + 2] = bernsteinListPoints[i].CenterPosition.Z + bernsteinListPoints[i].TemporaryTranslation.Z + bernsteinListPoints[i].Translation.Z;

                if (i < bernsteinListPoints.Count - 1)
                {
                    polylineIndices[indiceIdx] = (uint)i;
                    indiceIdx++;
                    polylineIndices[indiceIdx] = (uint)i + 1;
                    indiceIdx++;
                }
            }
        }

        public void FillBezierC2Geometry()
        {
            int bezierParts = (int)Math.Ceiling((double)(bernsteinListPoints.Count - 1) / 3.0);
            if (bernsteinListPoints.Count == 1)
            {
                bezierParts = 1;
            }

            interpolatedBezierC2Points = new float[3 * bezierParts * 4];
            if (bernsteinListPoints.Count == 0)
            {
                interpolatedBezierC2Indices = new uint[0];
            }
            else
            {
                interpolatedBezierC2Indices = new uint[4 * bezierParts];
            }

            for (int i = 0; i < bezierParts * 4; i++)
            {
                int _x = i;
                if (_x >= bernsteinListPoints.Count)
                {
                    interpolatedBezierC2Points[3 * _x] = float.PositiveInfinity;
                }
                else
                {
                    interpolatedBezierC2Points[3 * _x] = bernsteinListPoints[_x].CenterPosition.X + bernsteinListPoints[_x].TemporaryTranslation.X + bernsteinListPoints[_x].Translation.X;
                    interpolatedBezierC2Points[3 * _x + 1] = bernsteinListPoints[_x].CenterPosition.Y + bernsteinListPoints[_x].TemporaryTranslation.Y + bernsteinListPoints[_x].Translation.Y;
                    interpolatedBezierC2Points[3 * _x + 2] = bernsteinListPoints[_x].CenterPosition.Z + bernsteinListPoints[_x].TemporaryTranslation.Z + bernsteinListPoints[_x].Translation.Z;
                }
            }

            int indiceIdx = 0;
            int x = 0;
            for (int k = 0; k < bezierParts; k++)
            {

                interpolatedBezierC2Indices[indiceIdx] = (uint)x;
                indiceIdx++;
                interpolatedBezierC2Indices[indiceIdx] = (uint)x + 1;
                indiceIdx++;
                interpolatedBezierC2Indices[indiceIdx] = (uint)x + 2;
                indiceIdx++;
                interpolatedBezierC2Indices[indiceIdx] = (uint)x + 3;
                indiceIdx++;
                x += 3;
            }
        }

        public void RecalculateBezierC2Geometry()
        {
            if (interpolatedBC2Points.Count > 2)
            {
                List<float> dist = new List<float>();

                for (int i = 1; i < interpolatedBC2Points.Count; i++)
                {
                    var position1 = this.interpolatedBC2Points[i - 1].Position();
                    var position2 = this.interpolatedBC2Points[i].Position();
                    dist.Add((float)Math.Sqrt(Math.Pow(position1.X - position2.X, 2) + Math.Pow(position1.Y - position2.Y, 2) + Math.Pow(position1.Z - position2.Z, 2)));
                }

                var _cvalues = TridiagonaMatrixAlgorithm(dist);

                List<Vector3> c = new List<Vector3>();
                c.Add(new Vector3(0.0f, 0.0f, 0.0f));
                c.AddRange(_cvalues);
                c.Add(new Vector3(0.0f, 0.0f, 0.0f));

                for (int i = 0; i < interpolatedBC2Points.Count - 1; i++)
                {
                    Vector3[] tableOfBernsteinPoints = new Vector3[4];
                    Vector3 b = new Vector3((interpolatedBC2Points[i + 1].Position() - interpolatedBC2Points[i].Position()) / dist[i] - dist[i] * (c[i] + ((c[i + 1] - c[i]) / (3 * dist[i])) * dist[i]));
                    Vector4 multiplyVector = new Vector4(interpolatedBC2Points[i].Position().X, b.X, c[i].X, new Vector3((c[i + 1] - c[i]) / (3 * dist[i])).X);
                    float[] bernstein = GetBernsteinPoints(dist, multiplyVector, i);
                    tableOfBernsteinPoints[0].X = bernstein[0];
                    tableOfBernsteinPoints[1].X = bernstein[1];
                    tableOfBernsteinPoints[2].X = bernstein[2];
                    tableOfBernsteinPoints[3].X = bernstein[3];

                    multiplyVector = new Vector4(interpolatedBC2Points[i].Position().Y, b.Y, c[i].Y, new Vector3((c[i + 1] - c[i]) / (3 * dist[i])).Y);
                    bernstein = GetBernsteinPoints(dist, multiplyVector, i);
                    tableOfBernsteinPoints[0].Y = bernstein[0];
                    tableOfBernsteinPoints[1].Y = bernstein[1];
                    tableOfBernsteinPoints[2].Y = bernstein[2];
                    tableOfBernsteinPoints[3].Y = bernstein[3];

                    multiplyVector = new Vector4(interpolatedBC2Points[i].Position().Z, b.Z, c[i].Z, new Vector3((c[i + 1] - c[i]) / (3 * dist[i])).Z);
                    bernstein = GetBernsteinPoints(dist, multiplyVector, i);
                    tableOfBernsteinPoints[0].Z = bernstein[0];
                    tableOfBernsteinPoints[1].Z = bernstein[1];
                    tableOfBernsteinPoints[2].Z = bernstein[2];
                    tableOfBernsteinPoints[3].Z = bernstein[3];

                    for (int j = 0; j < 4; j++)
                    {
                        var pos = tableOfBernsteinPoints[j];
                        Point p = new Point(new Vector3(pos.X, pos.Y, pos.Z), -1, _camera);
                        if (j == 0 && bernsteinListPoints.Count == 0)
                        {
                            bernsteinListPoints.Add(p);
                        }
                        else if (j > 0)
                        {
                            bernsteinListPoints.Add(p);
                        }
                    }
                }
            }
            else if(interpolatedBC2Points.Count == 2)
            {
                bernsteinListPoints.Add(interpolatedBC2Points[0]);
                bernsteinListPoints.Add(interpolatedBC2Points[1]);
            }
        }

        private float[] GetBernsteinPoints(List<float> dist, Vector4 power, int index)
        {
            float[] bernstein = MultiplyBernsteinMatrixAndVector(power);
            bernstein[0] = (1 - dist[index]) * bernstein[0] + dist[index] * bernstein[0];
            bernstein[1] = (1 - dist[index]) * bernstein[1] + dist[index] * bernstein[1];
            bernstein[2] = (1 - dist[index]) * bernstein[2] + dist[index] * bernstein[2];
            bernstein[3] = (1 - dist[index]) * bernstein[3] + dist[index] * bernstein[3];
            bernstein[3] = (1 - dist[index]) * bernstein[2] + dist[index] * bernstein[3];
            bernstein[2] = (1 - dist[index]) * bernstein[1] + dist[index] * bernstein[2];
            bernstein[1] = (1 - dist[index]) * bernstein[0] + dist[index] * bernstein[1];
            bernstein[3] = (1 - dist[index]) * bernstein[2] + dist[index] * bernstein[3];
            bernstein[2] = (1 - dist[index]) * bernstein[1] + dist[index] * bernstein[2];
            bernstein[3] = (1 - dist[index]) * bernstein[2] + dist[index] * bernstein[3];
            return bernstein;
        }

        private float[] MultiplyBernsteinMatrixAndVector(Vector4 power)
        {
            Matrix4 matrix = new Matrix4(
                1, 1, 1, 1,
                0, 1.0f / 3, 2.0f / 3, 1,
                0, 0, 1.0f / 3, 1,
                0, 0, 0, 1
            );

            var res = power * matrix;
            float[] result = { res.X, res.Y, res.Z, res.W };
            return result;
        }

        private List<Vector3> TridiagonaMatrixAlgorithm(List<float> dist)
        {
            List<Vector3> resultList = new List<Vector3>();
            List<float> _b = new List<float>();
            List<Vector3> _g = new List<Vector3>();
            Vector3[] x = new Vector3[dist.Count - 1];

            _b.Add(-(dist[1] / (dist[0] + dist[1])) / 2.0f);
            for (int i = 1; i < dist.Count - 1; i++)
            {
                _b.Add(-(dist[i + 1] / (dist[i] + dist[i + 1])) / ((dist[i] / (dist[i] + dist[i+1])) * _b[i - 1] + 2.0f));
            }

            _g.Add((3.0f * ((interpolatedBC2Points[2].Position() - interpolatedBC2Points[1].Position()) / dist[1] - (interpolatedBC2Points[1].Position() - interpolatedBC2Points[0].Position()) / dist[0]) / (dist[0] + dist[1])) / 2.0f);
            for (int i = 1; i < dist.Count - 1; i++)
            {
                Vector3 pom = (3.0f * ((interpolatedBC2Points[i + 2].Position() - interpolatedBC2Points[i + 1].Position()) / dist[i + 1] - (interpolatedBC2Points[i + 1].Position() - interpolatedBC2Points[i].Position()) / dist[i]) / (dist[i] + dist[i + 1]));
                _g.Add((pom - (dist[i] / (dist[i] + dist[i+1])) * _g[i - 1]) / ((dist[i] / (dist[i] + dist[i+1])) * _b[i - 1] + 2.0f));
            }

            x[dist.Count - 2] = _g[dist.Count - 2];
            for (int i = dist.Count - 3; i >= 0; i--)
            {
                x[i] = _b[i] * x[i + 1] + _g[i];
            }

            for (int i = 0; i < dist.Count - 1; i++)
            {
                resultList.Add(x[i]);
            }

            return resultList;
        }
    }
}
