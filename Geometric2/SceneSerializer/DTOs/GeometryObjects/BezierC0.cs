using System.ComponentModel.DataAnnotations;
using SharpSceneSerializer.DTOs.Enums;
using SharpSceneSerializer.DTOs.Interfaces;
using SharpSceneSerializer.DTOs.Types;

namespace SharpSceneSerializer.DTOs.GeometryObjects
{

    public class BezierC0 : IGeometryObject
    {
        [Required]
        public ObjectType ObjectType => ObjectType.bezierC0;
        [Required]
        public uint Id { get; set; }
        public string Name { get; set; }
        [Required]
        public PointRef[] ControlPoints { get; set; }
    }
}