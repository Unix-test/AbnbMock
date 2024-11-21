using Core.Model.Schemas;

namespace Core.Model.Dto;

public enum Currency
{
    USD,
    WON,
    EUR,
    GBP
}

public record BookingRequest: BookingInfos
{
    public new string? CheckInDate { get; set; }
    public new string? CheckOutDate { get; set; }
    public new Currency? GuestCurrency {get;set;}
}