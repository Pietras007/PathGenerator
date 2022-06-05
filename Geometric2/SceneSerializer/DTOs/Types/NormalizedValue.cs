using System.ComponentModel.DataAnnotations;

namespace SharpSceneSerializer.DTOs.Types
{
    public class NormalizedValue
    {
        public float Value { get; set; }
        //public NormalizedValue(float Value)
        //{
        //    this.Value = Clamp(Value);
        //}

        //private float Clamp(float Value)
        //{
        //    if (Value < 0.0f)
        //    {
        //        return 0.0f;
        //    }
        //    else if (Value > 1.0f)
        //    {
        //        return 1.0f;
        //    }

        //    return Value;
        //}
    }
}
