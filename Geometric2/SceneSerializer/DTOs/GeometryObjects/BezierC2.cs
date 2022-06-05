using System.ComponentModel.DataAnnotations;
using SharpSceneSerializer.DTOs.Enums;
using SharpSceneSerializer.DTOs.Interfaces;
using SharpSceneSerializer.DTOs.Types;

namespace SharpSceneSerializer.DTOs.GeometryObjects
{

    public class BezierC2 : IGeometryObject
    {
        [Required]
        public ObjectType ObjectType => ObjectType.bezierC2;
        [Required]
        public uint Id { get; set; }
        public string Name { get; set; }
        [Required]
        public PointRef[] DeBoorPoints { get; set; }
    }
}