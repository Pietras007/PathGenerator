
using Geometric2.RasterizationClasses;
using OpenTK;
using OpenTK.Graphics.OpenGL4;

namespace Intersect
{
    public interface ISurface
    {
        Vector3 P(float u, float v);
        Vector3 T(float u, float v);
        Vector3 B(float u, float v);
        Vector3 N(float u, float v);
        void SetTexture(Texture texture, TextureUnit textureUnit, int textureId);
        bool WrapsU();
        bool WrapsV();
    }
}