using Geometric2.RasterizationClasses;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;
using Geometric2.MatrixHelpers;
using Geometric2.Helpers;
using System.Drawing;

namespace Geometric2.ModelGeneration
{
    public class GregoryPiece : Element
    {
        public bool DrawPolyline { get; set; }
        public List<ModelGeneration.Point> points;
        int pointNumber = 0;
        Camera _camera;
        int width, height;
        int gregoryPatchNumber;
        public int GregoryPolylineVBO, GregoryPolylineVAO, GregoryPolylineEBO;
        public int GregoryVBO, GregoryVAO, GregoryEBO;

        private float[] polylinePoints;
        uint[] polylineIndices;

        public int SegmentsU { get; set; }
        public int SegmentsV { get; set; }

        int splitA = 20;
        int splitB = 20;
        private float[] GregoryPoints = new float[3 * 20];
        uint[] GregoryIndices = new uint[20];

        public GregoryPiece(int gregoryPatchNumber, Camera _camera, int width, int height)
        {
            points = new List<Point>();
            this._camera = _camera;
            this.width = width;
            this.height = height;
            this.gregoryPatchNumber = gregoryPatchNumber;
            FullName = "GregoryPatch " + gregoryPatchNumber;
            this.SegmentsU = 20;
            this.SegmentsV = 20;
        }

        public override string ToString()
        {
            return FullName + " " + ElementName;
        }

        public override void CreateGlElement(Shader _shader, ShaderGeometry _patchGeometryShader, GregoryShader _gregoryShader)
        {
            RegenerateGregory();
            GregoryPolylineVAO = GL.GenVertexArray();
            GregoryPolylineVBO = GL.GenBuffer();
            GregoryPolylineEBO = GL.GenBuffer();
            var a_Position_Loc_shader = _shader.GetAttribLocation("a_Position");
            GL.BindVertexArray(GregoryPolylineVAO);
            GL.BindBuffer(BufferTarget.ArrayBuffer, GregoryPolylineVBO);
            GL.BufferData(BufferTarget.ArrayBuffer, polylinePoints.Length * sizeof(float), polylinePoints, BufferUsageHint.DynamicDraw);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, GregoryPolylineEBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, polylineIndices.Length * sizeof(uint), polylineIndices, BufferUsageHint.DynamicDraw);
            GL.VertexAttribPointer(a_Position_Loc_shader, 3, VertexAttribPointerType.Float, true, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(a_Position_Loc_shader);

            var a_Position_Loc_gregoryShader = _gregoryShader.GetAttribLocation("a_Position");
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

        public override void RenderGlElement(Shader _shader, Vector3 rotationCentre, ShaderGeometry _patchGeometryShader, GregoryShader _gregoryShader)
        {
            RegenerateGregory();
            TempRotationQuaternion = Quaternion.FromEulerAngles((float)(2 * Math.PI * ElementRotationX / 360), (float)(2 * Math.PI * ElementRotationY / 360), (float)(2 * Math.PI * ElementRotationZ / 360));
            Matrix4 model = ModelMatrix.CreateModelMatrix(ElementScale * TempElementScale, RotationQuaternion, CenterPosition + Translation + TemporaryTranslation, rotationCentre, TempRotationQuaternion);
            DrawPolyline = false;
            if (DrawPolyline)
            {
                _shader.Use();
                _shader.SetMatrix4("model", model);
                _shader.SetVector3("fragmentColor", ColorHelper.ColorToVector(Color.BlueViolet));
                GL.BindVertexArray(GregoryPolylineVAO);
                GL.DrawElements(PrimitiveType.Lines, polylineIndices.Length, DrawElementsType.UnsignedInt, 0);
                GL.BindVertexArray(0);
            }

            _gregoryShader.Use();
            _gregoryShader.SetMatrix4("model", model);
            _gregoryShader.SetFloat("splitA", splitA);
            _gregoryShader.SetFloat("splitB", splitB);
            if (IsSelected)
            {
                _gregoryShader.SetVector3("fragmentColor", ColorHelper.ColorToVector(Color.Orange));
            }
            else
            {
                _gregoryShader.SetVector3("fragmentColor", ColorHelper.ColorToVector(Color.Black));
            }

            GL.BindVertexArray(GregoryVAO);
            GL.PatchParameter(PatchParameterInt.PatchVertices, 20);
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
            GL.DrawElements(BeginMode.Patches, 20, DrawElementsType.UnsignedInt, 0);
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
        }

        private void FillGregoryGeometry()
        {
            GL.BindVertexArray(GregoryVAO);
            GL.BindBuffer(BufferTarget.ArrayBuffer, GregoryVBO);
            GL.BufferData(BufferTarget.ArrayBuffer, GregoryPoints.Length * sizeof(float), GregoryPoints, BufferUsageHint.DynamicDraw);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, GregoryEBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, GregoryIndices.Length * sizeof(uint), GregoryIndices, BufferUsageHint.DynamicDraw);
        }

        private void FillGregoryPolylineGeometry()
        {
            GL.BindVertexArray(GregoryPolylineVAO);
            GL.BindBuffer(BufferTarget.ArrayBuffer, GregoryPolylineVBO);
            GL.BufferData(BufferTarget.ArrayBuffer, polylinePoints.Length * sizeof(float), polylinePoints, BufferUsageHint.DynamicDraw);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, GregoryPolylineEBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, polylineIndices.Length * sizeof(uint), polylineIndices, BufferUsageHint.DynamicDraw);
        }

        public void RegenerateGregory()
        {
            GenerateLines();
            GenerateGregoryPatch();
            FillGregoryPolylineGeometry();
            FillGregoryGeometry();
        }

        private void GenerateGregoryPatch()
        {
            int index = 0;
            for (int i = 0; i < points.Count; i++)
            {
                Vector3 position = points[i].Position();
                GregoryPoints[index] = position.X;
                index++;
                GregoryPoints[index] = position.Y;
                index++;
                GregoryPoints[index] = position.Z;
                index++;

                GregoryIndices[i] = (uint)i;
            }
        }

        private void GenerateLines()
        {
            List<Point> pointLines = new List<Point>();
            if (points.Count == 0)
            {
                for (int i = 0; i < 20; i++)
                {
                    points.Add(new Point(new Vector3(0, 0, 0), pointNumber, _camera));
                    pointNumber++;
                }
            }

            pointLines.Add(points[1]);
            pointLines.Add(points[6]);

            pointLines.Add(points[2]);
            pointLines.Add(points[7]);

            pointLines.Add(points[9]);
            pointLines.Add(points[8]);

            pointLines.Add(points[15]);
            pointLines.Add(points[14]);

            pointLines.Add(points[18]);
            pointLines.Add(points[13]);

            pointLines.Add(points[17]);
            pointLines.Add(points[12]);

            pointLines.Add(points[10]);
            pointLines.Add(points[11]);

            pointLines.Add(points[4]);
            pointLines.Add(points[5]);

            pointLines.Add(points[0]);
            pointLines.Add(points[1]);
            pointLines.Add(points[1]);
            pointLines.Add(points[2]);
            pointLines.Add(points[2]);
            pointLines.Add(points[3]);
            pointLines.Add(points[3]);
            pointLines.Add(points[9]);
            pointLines.Add(points[9]);
            pointLines.Add(points[15]);
            pointLines.Add(points[15]);
            pointLines.Add(points[19]);
            pointLines.Add(points[19]);
            pointLines.Add(points[18]);
            pointLines.Add(points[18]);
            pointLines.Add(points[17]);
            pointLines.Add(points[17]);
            pointLines.Add(points[16]);
            pointLines.Add(points[16]);
            pointLines.Add(points[10]);
            pointLines.Add(points[10]);
            pointLines.Add(points[4]);
            pointLines.Add(points[4]);
            pointLines.Add(points[0]);

            polylinePoints = new float[3 * pointLines.Count];
            polylineIndices = new uint[pointLines.Count];
            int idx = 0;
            foreach (var p in pointLines)
            {
                polylinePoints[3 * idx] = p.CenterPosition.X;
                polylinePoints[3 * idx + 1] = p.CenterPosition.Y;
                polylinePoints[3 * idx + 2] = p.CenterPosition.Z;
                polylineIndices[idx] = (uint)idx;
                idx++;
            }
        }
    }
}
