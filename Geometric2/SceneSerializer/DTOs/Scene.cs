using SharpSceneSerializer.DTOs.GeometryObjects;
using SharpSceneSerializer.DTOs.Interfaces;
using System.Collections.Generic;

namespace SharpSceneSerializer.DTOs
{
    public class Scene
    {
        public List<Point> Points { get; set; }
        public List<IGeometryObject> Geometry { get; set; }
    }
}