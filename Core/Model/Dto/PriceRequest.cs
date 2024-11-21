using System.Text.Json.Serialization;
using Core.Model.Schemas;

namespace Core.Model.Dto;

public class PriceRequest
{
    public string? Id { get; set; }
    public Prices? Prices { get; set; } = new();
}