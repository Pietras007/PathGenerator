using System;
using System.Drawing;
using Geometric2.Global;
using Geometric2.Helpers;
using Geometric2.MatrixHelpers;
using Geometric2.RasterizationClasses;
using OpenTK;
using OpenTK.Graphics.OpenGL4;

namespace Geometric2.ModelGeneration
{
    public abstract class Element
    {
        public bool IsSelected { get; set; }
        public string ElementName { get; set; }
        public string FullName { get; set; }
        public Vector3 CenterPosition { get; set; }

        public Vector3 Translation;
        public Vector3 TemporaryTranslation;

        public Quaternion RotationQuaternion = Quaternion.FromEulerAngles(0.0f, 0.0f, 0.0f);
        public Quaternion TempRotationQuaternion = Quaternion.FromEulerAngles(0.0f, 0.0f, 0.0f);

        public float ElementScale = 1.0f;
        public float TempElementScale = 1.0f;

        public float ElementRotationX, ElementRotationY, ElementRotationZ;
        

        float[] centerLines = new float[]
        {
             0.0f, 0.0f, 0.0f,
             1.0f, 0.0f, 0.0f,
             0.0f,  0.0f, 0.0f,
             0.0f,  1.0f, 0.0f,
             0.0f,  0.0f, 0.0f,
             0.0f,  0.0f, 1.0f,
        };

        uint[] centerLinesIndices = new uint[] {
            0, 1,
            2, 3,
            4, 5
        };

        int centerLinesVBO, centerLinesVAO, centerLinesEBO;

        public virtual void CreateGlElement(Shader _shader, ShaderGeometry _geometryShader = null, TeselationShader _gregoryShader = null, GlobalData globalData = null)
        {

        }

        public virtual void RenderGlElement(Shader _shader, Vector3 rotationCentre, ShaderGeometry _geometryShader = null, TeselationShader _gregoryShader = null)
        {

        }

        public virtual void CreateCenterOfElement(Shader _shader)
        {
            var a_Position_Location = _shader.GetAttribLocation("a_Position");
            centerLinesVAO = GL.GenVertexArray();
            centerLinesVBO = GL.GenBuffer();
            centerLinesEBO = GL.GenBuffer();
            GL.BindVertexArray(centerLinesVAO);
            GL.BindBuffer(BufferTarget.ArrayBuffer, centerLinesVBO);
            GL.BufferData(BufferTarget.ArrayBuffer, centerLines.Length * sizeof(float), centerLines, BufferUsageHint.DynamicDraw);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, centerLinesEBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, centerLinesIndices.Length * sizeof(uint), centerLinesIndices, BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(a_Position_Location, 3, VertexAttribPointerType.Float, true, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(a_Position_Location);
        }

        public virtual void RenderCenterOfElement(Shader _shader)
        {
            if(IsSelected)
            {
                GL.BindVertexArray(centerLinesVAO);
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
