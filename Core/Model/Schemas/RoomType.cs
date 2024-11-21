using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Core.Model.Schemas;

public record RoomType
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [JsonIgnore]
    public Guid? Id { get; set; }
    [MaxLength(100)]
    public string? Name { get; init; }
    [MaxLength(100)]
    public string? Description { get; init; }
    public int? Capacity { get; init; }
    public bool? IsActive { get; init; }
    
    [ForeignKey(nameof(Prices))]
    [JsonIgnore]
    public Guid? PriceId { get; set; }
    public virtual required Prices Prices { get; init; }
    public virtual BookingInfos? BookingInfos { get; set; }
}