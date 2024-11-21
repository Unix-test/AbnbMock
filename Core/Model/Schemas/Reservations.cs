using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Core.Model.Schemas;

public class Reservations
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [JsonIgnore]
    public Guid? ReservationId { get; init; }
    [ForeignKey(nameof(BookInfos))] public Guid? BookingId { get; init; }
    public virtual required BookingInfos BookInfos { get; init; }
    [JsonIgnore]
    [Column(TypeName = "date")]
    public DateTime TimeStamp { get; init; } = DateTime.Now;
}