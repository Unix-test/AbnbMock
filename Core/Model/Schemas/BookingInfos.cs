using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Core.Model.Schemas;

public record BookingInfos
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [JsonIgnore]
    public Guid? Id { get; set; }

    [MaxLength(256)] public string? PhotoId { get; set; }
    [MaxLength(30)] public string? GuestCurrency { get; set; }
    public bool? IsWorkTrip { get; set; }
    public int? NumberOfAdults { get; set; }
    public int? NumberOfChildren { get; set; }
    public int? NumberOfInfants { get; set; }
    public int? NumberOfPets { get; set; }
    [Column(TypeName="Date")]
    public DateTime CheckInDate { get; set; }
    [Column(TypeName="Date")]
    public DateTime CheckOutDate { get; set; }
    
    [ForeignKey(nameof(RoomType))]
    [JsonIgnore]
    public Guid? RoomId { get; set; }
    [JsonIgnore] public virtual RoomType? RoomType { get; init; }
    [JsonIgnore] public virtual Reservations? Reservations { get; init; }

    public decimal Sum(TimeSpan? bookingDuration)
    {
        var prices = RoomType?.Prices;

        if (prices is null || bookingDuration is null) return 0;
        
        var sum = prices.PerNight;

        if (NumberOfAdults is not null)
        {
            sum += NumberOfAdults * prices.PerAdult;
        }

        if (NumberOfChildren is not null)
        {
            sum += NumberOfChildren * prices.PerChild;
        }
        
        if (NumberOfPets is not null)
        {
            sum += NumberOfPets * prices.PerPet;
        }
        
        return sum * bookingDuration.Value.Days ?? 0;
    }
}