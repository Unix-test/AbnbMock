using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using AutoMapper;
using Core.Connections.Jwt;
using Core.Helpers;
using Core.Model.Dto;
using Core.Model.Schemas;
using Core.Services.IRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Core.Services.Repository;

public class Abnb(
    UserManager<User> userManager,
    Jwt<User> jwt,
    ReservoirDbContext context,
    IMapper mapper,
    IConfiguration configuration) : IAbnb
{
    public async Task<IResult> Auth(string? username, string? password)
    {
        var user = await userManager.FindByNameAsync(username ?? string.Empty);

        if (user is null) return Results.Unauthorized();

        if (!await userManager.CheckPasswordAsync(user, password ?? string.Empty)) return Results.Unauthorized();

        var connector = configuration.GetSection(nameof(JwtConnector)).Get<JwtConnector>();

        var token = jwt.TokenGenerate(user);
        var accessToken = new JwtSecurityTokenHandler().WriteToken(token.Result);

        _ = int.TryParse(
            connector?.RefreshTokenValidityInDays,
            out _
        );

        return Results.Ok(accessToken);
    }

    public async Task AddRoom(RoomRequest[] roomRequest)
    {
        try
        {
            foreach (var request in roomRequest)
            {
                var id = Guid.NewGuid();
                var room = request with
                {
                    Id = id, Prices = request.Prices
                };
                var roomMap = mapper.Map<RoomType>(room);
                context.RoomTypes.Add(roomMap);
                await context.SaveChangesAsync();
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            throw new Exception(e.Message);
        }
        finally
        {
            await context.DisposeAsync();
        }
    }

    public async Task<IResult> Reserve(string roomId, BookingRequest? info)
    {
        await using var dbContextTransaction = await context.Database.BeginTransactionAsync();
        
        try
        {
            var room = await context.RoomTypes.FindAsync(Guid.Parse(roomId));

            const string format = "yyyy-MM-dd HH:mm:ss";

            var bookingInfo = new BookingInfos
            {
                PhotoId = info?.PhotoId,
                NumberOfAdults = info?.NumberOfAdults,
                NumberOfPets = info?.NumberOfPets,
                NumberOfInfants = info?.NumberOfInfants,
                NumberOfChildren = info?.NumberOfChildren,
                IsWorkTrip = info?.IsWorkTrip,
                CheckInDate = DateTime.ParseExact(info?.CheckInDate ?? string.Empty, format,
                    CultureInfo.InvariantCulture),
                CheckOutDate = DateTime.ParseExact(info?.CheckOutDate ?? string.Empty, format,
                    CultureInfo.InvariantCulture),
                GuestCurrency = info?.GuestCurrency switch
                {
                    Currency.USD => "USD",
                    Currency.WON => "WON",
                    Currency.EUR => "EUR",
                    Currency.GBP => "GBP",
                    _ => "Unknown"
                },
                RoomType = room
            };

            var reservation = new Reservations
            {
                BookInfos = bookingInfo
            };

            context.BookingInfos.Add(bookingInfo);
            
            await context.SaveChangesAsync();
            
            context.Reservations.Add(reservation);
            
            await context.SaveChangesAsync();

            await dbContextTransaction.CommitAsync();
            
            Console.WriteLine(reservation.ReservationId);

            return Results.Ok(await GetReciept(reservationId:reservation.ReservationId));
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            throw new Exception(e.Message);
        }
        finally
        {
            await context.DisposeAsync();
        }
    }

    private async Task<object?> GetReciept(Guid? reservationId)
    {
        var reservation = await context.Reservations
            .Include(x => x.BookInfos)
            .ThenInclude(y => y.RoomType)
            .ThenInclude(z => z.Prices)
            .Where(x => x.ReservationId == reservationId).FirstOrDefaultAsync();

        var room = reservation?.BookInfos.RoomType;

        var bookingDuration = reservation?.BookInfos.CheckOutDate - reservation?.BookInfos.CheckInDate;
        var totalPrice = reservation?.BookInfos.Sum(bookingDuration);

        var result = new
        {
            roomName = room?.Name,
            description = room?.Description,
            capacity = room?.Capacity,
            gestCurrency = room?.BookingInfos?.GuestCurrency,
            bookingDuration?.Days,
            totalPrice,
        };
        
        return room is null ? default : result;
    }
}