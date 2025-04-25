using Microsoft.AspNetCore.Mvc;
using SocialNetwork.Application.Contracts.Services;
using SocialNetwork.Presentation.Web.Contracts;

namespace SocialNetwork.Presentation.Web.Controllers;

[ApiController]
[Route("[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost]
    public async Task<ActionResult<long>> CreateUser(CreateUserRequest request)
    {
        if (string.IsNullOrEmpty(request.Name))
        {
            return BadRequest();
        }

        var userId = await _userService.CreateUser(request.Name);

        return Ok(userId);
    }

    [HttpGet]
    public async Task<ActionResult<List<UserResponse>>> GetUsers([FromQuery] GetUsersRequest request)
    {
        if (request.PageSize > 100)
        {
            return BadRequest("Page size is too large.");
        }
        
        var users = await _userService.GetUsers(request.Page, request.PageSize);

        var response = users.Select(UserResponse.ToResponse).ToList();

        return Ok(response);
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<UserResponse>> GetUser(long id)
    {
        var user = await _userService.GetUserById(id);

        if (user == null)
        {
            return NotFound();
        }

        var response = UserResponse.ToResponse(user);

        return Ok(response);
    }
    
    [HttpPut("{id:long}")]
    public async Task<ActionResult> ChangeUserName(long id, ChangeUserNameRequest request)
    {
        if (string.IsNullOrEmpty(request.Name))
        {
            return BadRequest("Name cannot be empty");
        }

        await _userService.ChangeUserName(id, request.Name);

        return Ok();
    }
}