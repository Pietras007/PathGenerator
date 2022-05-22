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
    public class TransformCenterLines : Element
    {
        float[] transformLines = new float[]
        {
            -0.5f, 0.0f, 0.0f,
             1.5f, 0.0f, 0.0f,
             0.0f,  -0.5f, 0.0f,
             0.0f,  1.5f, 0.0f,
             0.0f,  0.0f, -0.5f,
             0.0f,  0.0f, 1.5f,
        };

        uint[] transformLinesIndices = new uint[] {
            0, 1,
            2, 3,
            4, 5
        };

        int transformLinesVBO, transformLinesVAO, transformLinesEBO;
        public List<Element> selectedElements = new List<Element>();
        public Vector3 rotationCenterPoint = new Vector3(0.0f, 0.0f, 0.0f);
        public RotationPoint rotationPoint = RotationPoint.Center;

        public TransformCenterLines()
        {
        }

        public override void CreateGlElement(Shader _shader, ShaderGeometry _geometryShader = null, GregoryShader _gregoryShader = null)
        {
            var a_Position_Location = _shader.GetAttribLocation("a_Position");
            transformLinesVAO = GL.GenVertexArray();
            transformLinesVBO = GL.GenBuffer();
            transformLinesEBO = GL.GenBuffer();
            GL.BindVertexArray(transformLinesVAO);
            GL.BindBuffer(BufferTarget.ArrayBuffer, transformLinesVBO);
            GL.BufferData(BufferTarget.ArrayBuffer, transformLines.Length * sizeof(float), transformLines, BufferUsageHint.DynamicDraw);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, transformLinesEBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, transformLinesIndices.Length * sizeof(uint), transformLinesIndices, BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(a_Position_Location, 3, VertexAttribPointerType.Float, true, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(a_Position_Location);
        }

        public override void RenderGlElement(Shader _shader, Vector3 rotationCentre, ShaderGeometry _geometryShader = null, GregoryShader _gregoryShader = null)
        {
            if (selectedElements != null && selectedElements.Count > 1)
            {
                Matrix4 modelMatrix = TranslationMatrix.CreateTranslationMatrix(rotationCenterPoint);
                _shader.SetMatrix4("model", modelMatrix);
                GL.BindVertexArray(transformLinesVAO);
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
}
