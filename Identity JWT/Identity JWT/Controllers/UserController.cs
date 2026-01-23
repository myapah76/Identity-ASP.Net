using IdentityService.Application.Abstrations;
using IdentityService.Application.Dtos.User.Request;
using Microsoft.AspNetCore.Mvc;
using IdentityService.Application.Constants;
using Microsoft.AspNetCore.Authorization;

namespace Identity_JWT.Controllers;

[ApiController]
[Route("api/users")]
public class UsersController(
    IUserService userService
) : ControllerBase
{
    private readonly IUserService _userService = userService;

    [HttpPost]
    public async Task<IActionResult> Create(CustomerCreateUserReq request)
    {
        var result = await _userService.CustomerCreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var users = await _userService.GetAllAsync();
        return Ok(users);
    }

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var user = await _userService.GetByIdAsync(id);
        return user == null ? NotFound() : Ok(user);
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(Guid id, UpdateUserReq request)
    {
        var updated = await _userService.UpdateAsync(id, request);
        return updated == null ? NotFound() : Ok(updated);
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var success = await _userService.DeleteAsync(id);
        return success ? NoContent() : NotFound();
    }

    //[HttpPost("sync-elasticsearch")]
    //public async Task<IActionResult> SyncUsersToElastic()
    //{
    //    var usersFromDb = await _userService.GetAllAsync();

    //    var searchDocs = usersFromDb.Select(u => new UserSearchDocument
    //    {
    //        Id = u.Id,
    //        Name = u.Name ?? "",
    //        Email = u.Email,
    //        Phone = u.Phone ?? "",
    //        RoleName = u.Role?.Name ?? "N/A",
    //        IsBlocked = u.IsBlocked ?? false,
    //        CreatedAt = u.CreatedAt
    //    });

    //    await _searchService.IndexManyUsersAsync(searchDocs);

    //    return Ok();
    //}
}