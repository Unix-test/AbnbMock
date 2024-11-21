using Core.Helpers;
using Core.Model.Dto;
using Core.Model.Schemas;
using Microsoft.AspNetCore.Http;

namespace Core.Services.IRepository;

public interface IAbnb
{
    public Task<IResult> Auth(string? username, string? password);
    public Task AddRoom(RoomRequest[] roomRequest);
    public Task<IResult> Reserve(string roomId, BookingRequest? info);
}