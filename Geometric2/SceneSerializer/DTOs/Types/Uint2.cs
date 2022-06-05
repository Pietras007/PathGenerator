using System.ComponentModel.DataAnnotations;

namespace SharpSceneSerializer.DTOs.Types
{
    public class Uint2
    {
        public uint X { get; set; }
        public uint Y { get; set; }
        public Uint2(uint X, uint Y)
        {
            this.X = X;
            this.Y = Y;
        }
    }
}