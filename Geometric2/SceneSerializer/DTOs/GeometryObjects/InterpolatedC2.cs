using System.ComponentModel.DataAnnotations;
using SharpSceneSerializer.DTOs.Enums;
using SharpSceneSerializer.DTOs.Interfaces;
using SharpSceneSerializer.DTOs.Types;

namespace SharpSceneSerializer.DTOs.GeometryObjects
{

    public class InterpolatedC2 : IGeometryObject
    {
        [Required]
        public ObjectType ObjectType => ObjectType.interpolatedC2;
        [Required]
        public uint Id { get; set; }
        public string Name { get; set; }
        [Required]
        public PointRef[] ControlPoints { get; set; }
    }
}