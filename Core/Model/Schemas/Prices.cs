using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Core.Model.Schemas;

public record Prices
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [JsonIgnore]
    public Guid? Id { get; set; }
    public decimal? PerNight { get; set; }
    [Column(TypeName = "decimal(18, 2)")]
    public decimal? PerAdult { get; set; }
    [Column(TypeName = "decimal(18, 2)")]
    public decimal? PerChild { get; set; }
    [Column(TypeName = "decimal(18, 2)")]
    public decimal? PerPet { get; set; }
    [JsonIgnore]
    public virtual RoomType? RoomType { get; set; }
}