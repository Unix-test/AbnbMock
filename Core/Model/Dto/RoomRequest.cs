using System.Text.Json.Serialization;
using Core.Model.Schemas;

namespace Core.Model.Dto;

public record RoomRequest
{
    [JsonIgnore]
    public Guid Id {get;set;}
    public string? Name { get; init; }
    public string? Description { get; init; }
    public int? Capacity { get; init; }
    public bool? IsActive { get; init; }
    public required Prices Prices { get; set; }
};