
using OpenTK;

namespace Intersect
{
    public interface ISurface
    {
        /// <summary>
        /// Returns [x, y, z] position on a surface.
        /// </summary>
        /// <param name="u">u parameter</param>
        /// <param name="v">v parameter</param>
        /// <returns></returns>
        Vector3 P(float u, float v);
        /// <summary>
        /// Returns [x, y, z] tangent on a surface.
        /// </summary>
        /// <param name="u"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        Vector3 T(float u, float v);
        /// <summary>
        /// Returns [x, y, z] bitangent on a surface.
        /// </summary>
        /// <param name="u"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        Vector3 B(float u, float v);
        /// <summary>
        /// Returns [x, y, z] normal on a surface.
        /// </summary>
        /// <param name="u"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        Vector3 N(float u, float v);
        /// <summary>
        /// If surface wraps in u parameter direction.
        /// </summary>
        /// <returns></returns>
        bool WrapsU();
        /// <summary>
        /// If surface wraps in v parameter direction.
        /// </summary>
        /// <returns></returns>
        bool WrapsV();
    }
}