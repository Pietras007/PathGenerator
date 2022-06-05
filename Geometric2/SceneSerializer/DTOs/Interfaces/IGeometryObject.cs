using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Graph;
using SharpSceneSerializer.DTOs.Enums;

namespace SharpSceneSerializer.DTOs.Interfaces
{

    [JsonInterfaceConverter(typeof(InterfaceConverter<IGeometryObject>))]
    //[JsonConverter(typeof(IGeometryObject))]
    public interface IGeometryObject
    {
        [Required]
        ObjectType ObjectType { get; }
    }
}