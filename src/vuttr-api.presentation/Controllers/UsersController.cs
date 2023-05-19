using Microsoft.AspNetCore.Mvc;
using vuttr_api.domain.dtos;
using vuttr_api.services.contracts;

namespace vuttr_api.presentation.controllers;

[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost("register")]
    [ProducesResponseType(200, Type = typeof(UserResponse))]
    [ProducesResponseType(400)]
    public async Task<IActionResult> Resigter([FromBody] UserForRegisteration user)
    {
        if (user is null) return BadRequest();

        if (await _userService.AlreadyExistsAsync(user)) return BadRequest("User already exists.");

        UserResponse? added = await _userService.CreateUserAsync(user);

        return added == null ? BadRequest("Repository failed to create customer.") : Ok(added);
    }

    [HttpPost("authenticate")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> Authenticate([FromBody] UserForAuthentication user)
    {
        if (user is null) return BadRequest();

        (string? token, DateTime expiration) = await _userService.AuthenticateUserAsync(user);

        return token is not null ? Ok(new { token, expiration }) : Unauthorized();
    }
}