using Core.Model.Dto;
using Core.Services.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AbnbMock.Controllers;

[ApiController]
[Route("[controller]")]
public class AbnbMockController(IAbnb abnb) : ControllerBase
{
    [HttpPost("/authenicate")]
    public async Task<IActionResult> Authenticate([FromBody] LoginRequest request)
    {
        var result = await abnb.Auth(request.Username, request.Password);

        if (result == Results.Unauthorized())
        {
            return Unauthorized();
        }

        return Ok(result);
    }

    [HttpPost("/room/add")]
    [Authorize(AuthenticationSchemes = "Bearer", Roles = "Administrator")]
    public async Task<IActionResult> AddRoom([FromBody] RoomRequest[] request)
    {
        await abnb.AddRoom(request);

        return Ok();
    }

    [HttpPost("/make/reservation")]
    [Authorize(AuthenticationSchemes = "Bearer", Roles = "Administrator")]
    public async Task<IActionResult> MakeReservation([FromQuery] string roomId, [FromBody] BookingRequest infos)
    {
        return Ok(await abnb.Reserve(roomId, infos));
    }
}