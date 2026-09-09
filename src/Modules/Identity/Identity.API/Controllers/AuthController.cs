using Identity.API.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Identity.API.DTOs;

namespace Identity.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly TokenService _tokenService;

    public AuthController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, TokenService tokenService)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _tokenService = tokenService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] UserRegisterRequest model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var userExists = await _userManager.FindByEmailAsync(model.Email);

        if (userExists != null)
            return BadRequest(new { message = "This email is alread in use." });

        var user = new IdentityUser { UserName = model.Email, Email = model.Email };

        var result = await _userManager.CreateAsync(user, model.Password);

        if (!result.Succeeded)
            return BadRequest(result.Errors);

        // Assegura que a role existe e associa ao utilizador
        const string roleToAssign = "Player";
        if (!await _roleManager.RoleExistsAsync(roleToAssign))
        {
            var createRoleResult = await _roleManager.CreateAsync(
                new IdentityRole(roleToAssign));

            if (!createRoleResult.Succeeded)
                return StatusCode(500, createRoleResult.Errors);
        }

        var addRoleResult = await _userManager.AddToRoleAsync(user, roleToAssign);

        if (!addRoleResult.Succeeded)
            return StatusCode(500, addRoleResult.Errors);

        return StatusCode(201, new { message = $"User registered successfully with role '{roleToAssign}'" });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] UserLoginRequest model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var user = await _userManager.FindByEmailAsync(model.Email);

        if (user == null)
            return Unauthorized(new { message = "invalid email or password." });

        var isPasswordValid = await _userManager.CheckPasswordAsync(user, model.Password);

        if (!isPasswordValid)
            return Unauthorized(new { message = "Invalid email or password." });

        // Busca as roles do utilizador e gera o token com elas
        var roles = await _userManager.GetRolesAsync(user);
        var token = _tokenService.GenerateToken(user, roles);

        return Ok(new UserLoginResponse
        {
            Token = token,
            Expiration = DateTime.UtcNow.AddHours(4)
        });
    }
}