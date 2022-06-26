using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geometric2.RasterizationClasses
{
    public class TeselationShader
    {
        public readonly int Handle;

        private readonly Dictionary<string, int> _uniformLocations;
        public TeselationShader(string vertPath, string fragPath, string controlPatch, string evaluationPatch)
        {
            var shaderSource = LoadSource(vertPath);
            var vertexShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertexShader, shaderSource);
            CompileShader(vertexShader);

            shaderSource = LoadSource(fragPath);
            var fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragmentShader, shaderSource);
            CompileShader(fragmentShader);

            shaderSource = LoadSource(controlPatch);
            var controlShader = GL.CreateShader(ShaderType.TessControlShader);
            GL.ShaderSource(controlShader, shaderSource);
            CompileShader(controlShader);

            shaderSource = LoadSource(evaluationPatch);
            var evaluationShader = GL.CreateShader(ShaderType.TessEvaluationShader);
            GL.ShaderSource(evaluationShader, shaderSource);
            CompileShader(evaluationShader);

            Handle = GL.CreateProgram();
            GL.AttachShader(Handle, vertexShader);
            GL.AttachShader(Handle, fragmentShader);
            GL.AttachShader(Handle, controlShader);
            GL.AttachShader(Handle, evaluationShader);
            LinkProgram(Handle);

            GL.DetachShader(Handle, fragmentShader);
            GL.DetachShader(Handle, vertexShader);
            GL.DetachShader(Handle, controlShader);
            GL.DetachShader(Handle, evaluationShader);
            GL.DeleteShader(fragmentShader);
            GL.DeleteShader(vertexShader);
            GL.DeleteShader(controlShader);
            GL.DeleteShader(evaluationShader);
            GL.GetProgram(Handle, GetProgramParameterName.ActiveUniforms, out var numberOfUniforms);
            _uniformLocations = new Dictionary<string, int>();

            for (var i = 0; i < numberOfUniforms; i++)
            {
                var key = GL.GetActiveUniform(Handle, i, out _, out _);
                var location = GL.GetUniformLocation(Handle, key);
                _uniformLocations.Add(key, location);
            }
        }

        private static void CompileShader(int shader)
        {
            GL.CompileShader(shader);
            GL.GetShader(shader, ShaderParameter.CompileStatus, out var code);
            if (code != (int)All.True)
            {
                throw new Exception($"Error occurred whilst compiling Shader({shader})");
            }
        }

        private static void LinkProgram(int program)
        {
            GL.LinkProgram(program);
            GL.GetProgram(program, GetProgramParameterName.LinkStatus, out var code);
            if (code != (int)All.True)
            {
                throw new Exception($"Error occurred whilst linking Program({program})");
            }
        }

        public void Use()
        {
            GL.UseProgram(Handle);
        }

        public int GetAttribLocation(string attribName)
        {
            return GL.GetAttribLocation(Handle, attribName);
        }

        private static string LoadSource(string path)
        {
            using (var sr = new StreamReader(path, Encoding.UTF8))
            {
                return sr.ReadToEnd();
            }
        }

        public void SetInt(string name, int data)
        {
            GL.UseProgram(Handle);
            //var x = GL.GetUniformLocation(Handle, name);
            //glGetUniformLocation(regeneration_program.idx, "height_texture_last"), 3);
            GL.Uniform1(_uniformLocations[name], data);
        }

        public void SetFloat(string name, float data)
        {
            GL.UseProgram(Handle);
            GL.Uniform1(_uniformLocations[name], data);
        }

        public void SetMatrix4(string name, Matrix4 data)
        {
            GL.UseProgram(Handle);
            GL.UniformMatrix4(_uniformLocations[name], true, ref data);
        }

        public void SetVector3(string name, Vector3 data)
        {
            GL.UseProgram(Handle);
            GL.Uniform3(_uniformLocations[name], data);
        }
    }
}
