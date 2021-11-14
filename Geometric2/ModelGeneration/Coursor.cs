using Geometric2.Global;
using Geometric2.Helpers;
using Geometric2.MatrixHelpers;
using Geometric2.RasterizationClasses;
using OpenTK;
using OpenTK.Graphics.OpenGL4;
using System.Drawing;

namespace Geometric2.ModelGeneration
{
    public class Coursor
    {
        public CoursorMode CoursorMode { get; set; }
        public bool CoursorMoving { get; set; }
        public bool ChangeCoursorScreenPosition { get; set; }
        public (float, float, float) screenFloatPosition;

        public (int, int) CoursorScreenPosition { get; set; }
        public Vector3 CoursorGloalPosition = new Vector3(0.0f, 0.0f, 0.0f);

        //public (int, int) CoursorScreenManual = (-1, -1);
        //public Vector3 CoursorGloalManual = new Vector3(0.0f, 0.0f, 0.0f);
        public int width { get; set; }
        public int height { get; set; }

        float[] coursorLines = new float[]
        {
             0.0f, 0.0f, 0.0f,
             1.0f, 0.0f, 0.0f,
             0.0f,  0.0f, 0.0f,
             0.0f,  1.0f, 0.0f,
             0.0f,  0.0f, 0.0f,
             0.0f,  0.0f, 1.0f,
       };

        uint[] coursorLinesIndices = new uint[] {
            0, 1,
            2, 3,
            4, 5
        };

        int coursorVBO, coursorVAO, coursorEBO;

        public void CreateCoursor(Shader _shader)
        {
            var a_Position_Location = _shader.GetAttribLocation("a_Position");
            coursorVAO = GL.GenVertexArray();
            coursorVBO = GL.GenBuffer();
            coursorEBO = GL.GenBuffer();
            GL.BindVertexArray(coursorVAO);
            GL.BindBuffer(BufferTarget.ArrayBuffer, coursorVBO);
            GL.BufferData(BufferTarget.ArrayBuffer, coursorLines.Length * sizeof(float), coursorLines, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, coursorEBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, coursorLinesIndices.Length * sizeof(uint), coursorLinesIndices, BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(a_Position_Location, 3, VertexAttribPointerType.Float, true, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(a_Position_Location);
        }

        public void DrawCoursor(Shader _shader, Matrix4 viewMatrix, Matrix4 projectionMatrix, Camera _camera)
        {
            if (CoursorMode != CoursorMode.Manual || CoursorMoving == true || ChangeCoursorScreenPosition)
            {
                ChangeCoursorScreenPosition = false;
                CoursorGloalPosition = this.CountAndGetCoursorGlobalPosition(CoursorScreenPosition, viewMatrix, projectionMatrix, _camera);
            }

            if (CoursorMode != CoursorMode.Hidden)
            {
                Matrix4 modelMatrix = TranslationMatrix.CreateTranslationMatrix(new Vector3(CoursorGloalPosition.X, CoursorGloalPosition.Y, CoursorGloalPosition.Z));// * Matrix4.Invert(projectionMatrix) * Matrix4.Invert(viewMatrix);
                _shader.SetMatrix4("model", modelMatrix);
                GL.BindVertexArray(coursorVAO);
                _shader.SetVector3("fragmentColor", ColorHelper.ColorToVector(Color.Red));
                GL.DrawElements(PrimitiveType.Lines, 2, DrawElementsType.UnsignedInt, 0 * sizeof(int));
                _shader.SetVector3("fragmentColor", ColorHelper.ColorToVector(Color.Green));
                GL.DrawElements(PrimitiveType.Lines, 2, DrawElementsType.UnsignedInt, 2 * sizeof(int));
                _shader.SetVector3("fragmentColor", ColorHelper.ColorToVector(Color.Blue));
                GL.DrawElements(PrimitiveType.Lines, 2, DrawElementsType.UnsignedInt, 4 * sizeof(int));
                GL.BindVertexArray(0);
            }
        }

        private Vector3 CountAndGetCoursorGlobalPosition((int, int) screenPos, Matrix4 viewMatrix, Matrix4 projectionMatrix, Camera _camera)
        {
            (float, float, float) screenFloatPos = GetScreenFloatPos(screenPos);
            screenFloatPosition.Item1 = screenFloatPos.Item1;
            screenFloatPosition.Item2 = screenFloatPos.Item2;
            screenFloatPosition.Item3 = screenFloatPos.Item3;

            return CountCoursorPos(screenFloatPos, viewMatrix, projectionMatrix, _camera);
        }

        private Vector3 CountCoursorPos((float, float, float) screenFloatPos, Matrix4 viewMatrix, Matrix4 projectionMatrix, Camera _camera)
        {
            Vector4 coursor = new Vector4(screenFloatPos.Item1, screenFloatPos.Item2, screenFloatPos.Item3, 1.0f) * Matrix4.Invert(projectionMatrix);
            coursor.Z = -1.0f;
            coursor.W = 0.0f;
            coursor = coursor * Matrix4.Invert(viewMatrix);
            coursor = coursor.Normalized();

            float R = _camera.CameraDist;
            coursor.X = _camera.GetCameraPosition().X + coursor.X * R;
            coursor.Y = _camera.GetCameraPosition().Y + coursor.Y * R;
            coursor.Z = _camera.GetCameraPosition().Z + coursor.Z * R;

            return coursor.Xyz;
        }

        private (float, float, float) GetScreenFloatPos((int, int) screenPos)
        {
            float x = (float)((2.0 / width) * screenPos.Item1 - 1);
            float y = -(float)((2.0 / height) * screenPos.Item2 - 1);
            float z = -1.0f;

            return (x, y, z);
        }

        public Vector3 GetCoursorGlobalPosition((int, int) screenPos, Matrix4 viewMatrix, Matrix4 projectionMatrix, Camera _camera)
        {
            (float, float, float) screenFloatPos = GetScreenFloatPos(screenPos);
            return CountCoursorPos(screenFloatPos, viewMatrix, projectionMatrix, _camera);
        }
    }
}
