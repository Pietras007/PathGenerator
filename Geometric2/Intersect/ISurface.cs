
using OpenTK;

namespace Intersect
{
    public interface ISurface
    {
        Vector3 P(float u, float v);
        Vector3 T(float u, float v);
        Vector3 B(float u, float v);
        Vector3 N(float u, float v);
        bool WrapsU();
        bool WrapsV();
    }
}