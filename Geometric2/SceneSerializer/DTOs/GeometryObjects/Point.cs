using System.ComponentModel.DataAnnotations;
using SharpSceneSerializer.DTOs.Interfaces;
using SharpSceneSerializer.DTOs.Types;

namespace SharpSceneSerializer.DTOs.GeometryObjects
{
    public class Point
    {
        [Required]
        public uint Id { get; set; }
        public string Name { get; set; }
        [Required]
        public Float3 Position { get; set; }
    }
}