using System.ComponentModel.DataAnnotations;
using SharpSceneSerializer.DTOs.Enums;
using SharpSceneSerializer.DTOs.Interfaces;
using SharpSceneSerializer.DTOs.Types;

namespace SharpSceneSerializer.DTOs.GeometryObjects
{

    public class BezierPatchC2 : IGeometryObject
    {
        [Required]
        public ObjectType ObjectType => ObjectType.bezierPatchC2;
        [Required]
        public uint Id { get; set; }
        public string Name { get; set; }
        [Required]
        public PointRef[] controlPoints { get; set; } = new PointRef[16];
        [Required]
        public Uint2 Samples { get; set; }
    }
}