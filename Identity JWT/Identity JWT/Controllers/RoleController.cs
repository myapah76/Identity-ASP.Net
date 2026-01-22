using Application.Abstrations;
using Application.Constants;
using Application.Dtos.Role.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Identity_JWT.Controllers;

[ApiController]
[Route("api/roles")]
public class RoleController(
    IRoleService roleService
) : ControllerBase
{
    private readonly IRoleService _roleService = roleService;

    // CREATE
    [HttpPost]
    public async Task<IActionResult> Create(CreateRoleReq request)
    {
        var result = await _roleService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    // GET ALL
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var roles = await _roleService.GetAllAsync();
        return Ok(roles);
    }

    // GET BY ID
    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(long id)
    {
        var role = await _roleService.GetByIdAsync(id);
        return role == null ? NotFound() : Ok(role);
    }

    // UPDATE
    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(long id, UpdateRoleReq request)
    {
        var updated = await _roleService.UpdateAsync(id, request);
        return updated == null ? NotFound() : Ok(updated);
    }

    // DELETE
    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id)
    {
        var success = await _roleService.DeleteAsync(id);
        return success ? NoContent() : NotFound();
    }
}