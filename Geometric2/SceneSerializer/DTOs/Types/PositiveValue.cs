using System.ComponentModel.DataAnnotations;

namespace SharpSceneSerializer.DTOs.Types
{

    public class PositiveValue
    {
        public float Value { get; set; }
        public PositiveValue(float Value)
        {
            this.Value = Positive(Value);
        }

        private float Positive(float Value)
        {
            if (Value < 0.0f)
            {
                return 0.0f;
            }

            return Value;
        }
    }
}