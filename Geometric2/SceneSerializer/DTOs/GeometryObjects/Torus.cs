using System.ComponentModel.DataAnnotations;
using SharpSceneSerializer.DTOs.Enums;
using SharpSceneSerializer.DTOs.Interfaces;
using SharpSceneSerializer.DTOs.Types;

namespace SharpSceneSerializer.DTOs.GeometryObjects
{

    public class Torus : IGeometryObject
    {
        [Required]
        public ObjectType ObjectType => ObjectType.torus;
        [Required]
        public uint Id { get; set; }
        public string Name { get; set; }
        [Required]
        public Float3 Position { get; set; }
        [Required]
        public Float3 Rotation { get; set; }
        [Required]
        public Float3 Scale { get; set; }
        [Required]
        public Uint2 Samples { get; set; }
        [Required]
        [Range(0, float.MaxValue)]
        public float SmallRadius { get; set; }
        [Required]
        [Range(0, float.MaxValue)]
        public float LargeRadius { get; set; }
    }
}