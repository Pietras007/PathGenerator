using System;
using System.Drawing;
using Geometric2.Helpers;
using Geometric2.RasterizationClasses;
using OpenTK;
using OpenTK.Graphics.OpenGL4;

namespace Geometric2.ModelGeneration
{
    public class XyzLines : Element
    {
        float[] xyzLines = new float[]
        {
            -1000.0f, 0.0f, 0.0f,
             1000.0f, 0.0f, 0.0f,
             0.0f,  -1000.0f, 0.0f,
             0.0f,  1000.0f, 0.0f,
             0.0f,  0.0f, -1000.0f,
             0.0f,  0.0f, 1000.0f,
        };

        uint[] xyzLinesIndices = new uint[] {
            0, 1,
            2, 3,
            4, 5
        };

        int xyzLinesVBO, xyzLinesVAO, xyzLinesEBO;

        public override void CreateGlElement(Shader _shader, ShaderGeometry _geometryShader = null, TeselationShader _gregoryShader = null)
        {
            var a_Position_Location = _shader.GetAttribLocation("a_Position");
            xyzLinesVAO = GL.GenVertexArray();
            xyzLinesVBO = GL.GenBuffer();
            xyzLinesEBO = GL.GenBuffer();
            GL.BindVertexArray(xyzLinesVAO);
            GL.BindBuffer(BufferTarget.ArrayBuffer, xyzLinesVBO);
            GL.BufferData(BufferTarget.ArrayBuffer, xyzLines.Length * sizeof(float), xyzLines, BufferUsageHint.DynamicDraw);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, xyzLinesEBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, xyzLinesIndices.Length * sizeof(uint), xyzLinesIndices, BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(a_Position_Location, 3, VertexAttribPointerType.Float, true, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(a_Position_Location);
        }

        public override void RenderGlElement(Shader _shader, Vector3 rotationCentre, ShaderGeometry _geometryShader = null, TeselationShader _gregoryShader = null)
        {
            _shader.SetMatrix4("model", Matrix4.Identity);
            GL.BindVertexArray(xyzLinesVAO);
            _shader.SetVector3("fragmentColor", ColorHelper.ColorToVector(Color.Red));
            GL.DrawElements(PrimitiveType.Lines, 2, DrawElementsType.UnsignedInt, 0 * sizeof(int));
            _shader.SetVector3("fragmentColor", ColorHelper.ColorToVector(Color.Green));
            GL.DrawElements(PrimitiveType.Lines, 2, DrawElementsType.UnsignedInt, 2 * sizeof(int));
            _shader.SetVector3("fragmentColor", ColorHelper.ColorToVector(Color.Blue));
            GL.DrawElements(PrimitiveType.Lines, 2, DrawElementsType.UnsignedInt, 4 * sizeof(int));
            GL.BindVertexArray(0);
        }
    }
}
