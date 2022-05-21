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

        List<float> vertices = new List<float>();
        List<uint> indices = new List<uint>();
        public List<Point> bezierPoints { get; set; }

        public int splitA = 1, splitB = 1;

        private float[] GregoryPoints;
        uint[] GregoryIndices;

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
            //GeneratePoints();
            //GenerateLines();
        }

        public override string ToString()
        {
            return FullName + " " + ElementName;
        }

        public override void CreateGlElement(Shader _shader, ShaderGeometry _patchGeometryShader)
        {

            RegenerateGregory();
            GregoryPolylineVAO = GL.GenVertexArray();
            GregoryPolylineVBO = GL.GenBuffer();
            GregoryPolylineEBO = GL.GenBuffer();
            var a_Position_Location = _shader.GetAttribLocation("a_Position");
            GL.BindVertexArray(GregoryPolylineVAO);
            GL.BindBuffer(BufferTarget.ArrayBuffer, GregoryPolylineVBO);
            GL.BufferData(BufferTarget.ArrayBuffer, polylinePoints.Length * sizeof(float), polylinePoints, BufferUsageHint.DynamicDraw);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, GregoryPolylineEBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, polylineIndices.Length * sizeof(uint), polylineIndices, BufferUsageHint.DynamicDraw);
            GL.VertexAttribPointer(a_Position_Location, 3, VertexAttribPointerType.Float, true, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(a_Position_Location);

            var a_geometry_Position_Location = _patchGeometryShader.GetAttribLocation("a_Position");
            GregoryVAO = GL.GenVertexArray();
            GregoryVBO = GL.GenBuffer();
            GregoryEBO = GL.GenBuffer();
            GL.BindVertexArray(GregoryVAO);
            GL.BindBuffer(BufferTarget.ArrayBuffer, GregoryVBO);
            GL.BufferData(BufferTarget.ArrayBuffer, GregoryPoints.Length * sizeof(float), GregoryPoints, BufferUsageHint.DynamicDraw);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, GregoryEBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, GregoryIndices.Length * sizeof(uint), GregoryIndices, BufferUsageHint.DynamicDraw);

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



            //var a_geometry_Position_Location = _patchGeometryShader.GetAttribLocation("a_Position");
            //GregoryVAO = GL.GenVertexArray();
            //GregoryVBO = GL.GenBuffer();
            //GregoryEBO = GL.GenBuffer();
            //GL.BindVertexArray(GregoryVAO);
            //GL.BindBuffer(BufferTarget.ArrayBuffer, GregoryVBO);
            //GL.BufferData(BufferTarget.ArrayBuffer, GregoryPoints.Length * sizeof(float), GregoryPoints, BufferUsageHint.DynamicDraw);
            //GL.BindBuffer(BufferTarget.ElementArrayBuffer, GregoryEBO);
            //GL.BufferData(BufferTarget.ElementArrayBuffer, GregoryIndices.Length * sizeof(uint), GregoryIndices, BufferUsageHint.DynamicDraw);

            //GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);
            //GL.EnableVertexAttribArray(a_geometry_Position_Location);

            //GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), (3 * sizeof(float)));
            //GL.EnableVertexAttribArray(1);
        }

        public override void RenderGlElement(Shader _shader, Vector3 rotationCentre, ShaderGeometry _patchGeometryShader)
        {
            RegenerateGregory();
            //_gregoryGeometryShader.Use();
            ////int transLoc = GL.GetUniformLocation(0, "transform");

            ////GL.UniformMatrix4 UniformMatrix4fv(transLoc, 1, GL_FALSE, glm::value_ptr(GetModel()));
            TempRotationQuaternion = Quaternion.FromEulerAngles((float)(2 * Math.PI * ElementRotationX / 360), (float)(2 * Math.PI * ElementRotationY / 360), (float)(2 * Math.PI * ElementRotationZ / 360));
            Matrix4 model = ModelMatrix.CreateModelMatrix(ElementScale * TempElementScale, RotationQuaternion, CenterPosition + Translation + TemporaryTranslation, rotationCentre, TempRotationQuaternion);
            Matrix4 model2 = ModelMatrix.CreateModelMatrix(ElementScale * TempElementScale, RotationQuaternion, new Vector3(0, 0, 0), rotationCentre, TempRotationQuaternion);
            //GL.BindVertexArray(GregoryVAO);
            //GL.PatchParameter(PatchParameterFloat.PatchDefaultInnerLevel, new float[] { 20 });
            //_gregoryGeometryShader.SetMatrix4("model", model);

            //_gregoryGeometryShader.SetFloat("x", splitA);
            //_gregoryGeometryShader.SetFloat("y", splitB);
            //if (IsSelected)
            //{
            //    _gregoryGeometryShader.SetVector3("fragmentColor", ColorHelper.ColorToVector(Color.Orange));
            //}
            //else
            //{
            //    _gregoryGeometryShader.SetVector3("fragmentColor", ColorHelper.ColorToVector(Color.Black));
            //}

            //GL.DrawElements(BeginMode.Patches, 20, DrawElementsType.UnsignedInt, 0);
            DrawPolyline = true;
            if (DrawPolyline)
            {
                _shader.Use();
                GL.BindVertexArray(GregoryPolylineVAO);
                // Matrix4 model = ModelMatrix.CreateModelMatrix(ElementScale * TempElementScale, RotationQuaternion, CenterPosition + Translation + TemporaryTranslation, rotationCentre, TempRotationQuaternion);
                _shader.Use();
                _shader.SetMatrix4("model", model);
                _shader.SetVector3("fragmentColor", ColorHelper.ColorToVector(Color.BlueViolet));
                GL.DrawElements(PrimitiveType.Lines, polylineIndices.Length, DrawElementsType.UnsignedInt, 0);
                GL.BindVertexArray(0);
            }


            GL.BindVertexArray(GregoryVAO);
            _patchGeometryShader.Use();
            _patchGeometryShader.SetMatrix4("model", model2);
            _patchGeometryShader.SetVector3("fragmentColor", ColorHelper.ColorToVector(Color.Black));
            if (IsSelected)
            {
                //_patchGeometryShader.SetVector3("fragmentColor", ColorHelper.ColorToVector(Color.Orange));
            }
            else
            {
                //_patchGeometryShader.SetVector3("fragmentColor", ColorHelper.ColorToVector(Color.Black));
            }
            //_patchGeometryShader.SetMatrix4("view", _camera.GetViewMatrix());
            //_patchGeometryShader.SetMatrix4("projection", _camera.GetProjectionMatrix());
            //_patchGeometryShader.SetVector3("fragmentColor", ColorHelper.ColorToVector(Color.White));
            //_patchGeometryShader.SetMatrix4("persp", _camera.GetProjectionMatrix());
            //_patchGeometryShader.SetInt("width", width);
            //_patchGeometryShader.SetInt("height", height);
            
            GL.DrawElements(PrimitiveType.LinesAdjacency, GregoryIndices.Length, DrawElementsType.UnsignedInt, 0);
            GL.BindVertexArray(0);


            //TempRotationQuaternion = Quaternion.FromEulerAngles((float)(2 * Math.PI * ElementRotationX / 360), (float)(2 * Math.PI * ElementRotationY / 360), (float)(2 * Math.PI * ElementRotationZ / 360));
            //Matrix4 model = ModelMatrix.CreateModelMatrix(ElementScale * TempElementScale, RotationQuaternion, CenterPosition + Translation + TemporaryTranslation, rotationCentre, TempRotationQuaternion);
            //Matrix4 model2 = ModelMatrix.CreateModelMatrix(ElementScale * TempElementScale, RotationQuaternion, new Vector3(0, 0, 0), rotationCentre, TempRotationQuaternion);

            //if (DrawPolyline)
            //{
            //    _shader.SetMatrix4("model", model);
            //    GL.BindVertexArray(GregoryPolylineVAO);
            //    if (IsSelected)
            //    {
            //        _shader.SetVector3("fragmentColor", ColorHelper.ColorToVector(Color.Orange));
            //    }
            //    else
            //    {
            //        _shader.SetVector3("fragmentColor", ColorHelper.ColorToVector(Color.Red));
            //    }

            //    GL.DrawElements(PrimitiveType.Lines, 2 * polylinePoints.Length, DrawElementsType.UnsignedInt, 0);
            //    GL.BindVertexArray(0);
            //}

            //_gregoryGeometryShader.Use();
            //_gregoryGeometryShader.SetMatrix4("model", model2);


            //GL.BindVertexArray(GregoryVAO);
            //GL.DrawElements(PrimitiveType.Patches, GregoryIndices.Length, DrawElementsType.UnsignedInt, 0);
            //GL.BindVertexArray(0);

            _shader.Use();
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

            bezierPoints = new List<Point>();
            for(int i = 0; i < 16; i++)
            {
                bezierPoints.Add(new Point());
            }


            bezierPoints[0] = points[0];
            bezierPoints[1] = points[1];
            bezierPoints[2] = points[2];
            bezierPoints[3] = points[3];
            bezierPoints[4] = points[4];

            bezierPoints[5] = new Point((points[5].Position() + points[6].Position()) / 2, -1, _camera);
            bezierPoints[6] = new Point((points[7].Position() + points[8].Position()) / 2, -1, _camera);
            //vec4 p11 = (u != 0 || v != 0) ? (u * gl_in[5].gl_Position + v * gl_in[6].gl_Position) / (u + v) : (gl_in[5].gl_Position + gl_in[6].gl_Position) / 2;
            //vec4 p21 = (u != 0 || v != 1) ? ((1 - v) * gl_in[7].gl_Position + u * gl_in[8].gl_Position) / (1 - v + u) : (gl_in[7].gl_Position + gl_in[8].gl_Position) / 2;
            bezierPoints[7] = points[9];
            bezierPoints[8] = points[10];

            bezierPoints[9] = new Point((points[11].Position() + points[12].Position()) / 2, -1, _camera);
            bezierPoints[10] = new Point((points[13].Position() + points[14].Position()) / 2, -1, _camera);
            //vec4 p12 = (u != 1 || v != 0) ? ((1 - u) * gl_in[11].gl_Position + v * gl_in[12].gl_Position) / (1 - u + v) : (gl_in[11].gl_Position + gl_in[12].gl_Position) / 2;
            //vec4 p22 = (u != 1 || v != 1) ? ((1 - u) * gl_in[14].gl_Position + (1 - v) * gl_in[13].gl_Position) / (2 - u - v) : (gl_in[13].gl_Position + gl_in[14].gl_Position) / 2;
            bezierPoints[11] = points[15];
            bezierPoints[12] = points[16];
            bezierPoints[13] = points[17];
            bezierPoints[14] = points[18];
            bezierPoints[15] = points[19];

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


            //GregoryPoints = new float[6 * points.Count];
            //GregoryIndices = new uint[points.Count];
            //int index = 0;
            //for (int i = 0; i < points.Count; i++)
            //{
            //    Vector3 position = points[i].Position();
            //    GregoryPoints[index] = position.X;
            //    index++;
            //    GregoryPoints[index] = position.Y;
            //    index++;
            //    GregoryPoints[index] = position.Z;
            //    index++;

            //    GregoryPoints[index] = position.Z;
            //    index++;
            //    GregoryPoints[index] = position.Z;
            //    index++;
            //    GregoryPoints[index] = position.Z;
            //    index++;

            //    GregoryIndices[i] = (uint)i;
            //}

            GregoryPoints = vertices.ToArray();
            GregoryIndices = indices.ToArray();
        }

        void AddPatch(int i, int j, float t1, float t2, float begin, float end, int parts, int idx)
        {
            int w = 3 * splitB + 1;
            int beginindex = 3 * i * w + 3 * j;
            int indexstart = idx;

            for (int k = 0; k < 4; k++)
            {
                var pos = bezierPoints[beginindex].Position();
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
                pos = bezierPoints[beginindex + 1].Position();
                vertices.Add(pos.X);
                vertices.Add(pos.Y);
                vertices.Add(pos.Z);
                pos = bezierPoints[beginindex + 2].Position();
                vertices.Add(pos.X);
                vertices.Add(pos.Y);
                vertices.Add(pos.Z);
                pos = bezierPoints[beginindex + 3].Position();
                vertices.Add(pos.X);
                vertices.Add(pos.Y);
                vertices.Add(pos.Z);
                indices.Add((uint)idx);
                idx++;
                beginindex += w;
            }
            vertices[15 * (indexstart + 1) + 3] = t1;
            vertices[15 * (indexstart + 1) + 4] = t2;
            vertices[15 * (indexstart + 2) + 3] = begin;
            vertices[15 * (indexstart + 2) + 4] = end;
            vertices[15 * (indexstart + 2) + 5] = parts;
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

            pointLines.Add(points[4]);
            pointLines.Add(points[5]);

            pointLines.Add(points[2]);
            pointLines.Add(points[7]);

            pointLines.Add(points[10]);
            pointLines.Add(points[11]);

            pointLines.Add(points[8]);
            pointLines.Add(points[9]);

            pointLines.Add(points[12]);
            pointLines.Add(points[17]);

            pointLines.Add(points[13]);
            pointLines.Add(points[18]);

            pointLines.Add(points[14]);
            pointLines.Add(points[15]);


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
