using Microsoft.AspNetCore.Mvc;
using vuttr_api.domain.dtos;
using vuttr_api.services.contracts;

namespace vuttr_api.presentation.controllers;

[ApiController]
[Route("api/auth")]
public class AuthenticationController : ControllerBase
{
    private readonly IUserService _userService;

    public AuthenticationController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost("register")] // api/auth/register
    [ProducesResponseType(200, Type = typeof(UserViewModel))]
    [ProducesResponseType(400)]
    public async Task<IActionResult> Resigter([FromBody] UserForRegistration user)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        if (await _userService.AlreadyExistsAsync(user)) return BadRequest("User already exists.");

        UserViewModel? added = await _userService.CreateUserAsync(user);

        return added is null ? BadRequest("Repository failed to create user.") : Ok(added);
    }

    [HttpPost] // api/auth
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    public async Task<IActionResult> Authenticate([FromBody] UserForAuthentication user)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        AuthenticationResponse? response = await _userService.AuthenticateUserAsync(user);

        return response is not null ? Ok(response) : Unauthorized();
    }
}