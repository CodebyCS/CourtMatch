using Identity.API.Data;
using Identity.API.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Identity.API.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Identity.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly TokenService _tokenService;

    public AuthController(UserManager<ApplicationUser> userManager, TokenService tokenService)
    {
        _userManager = userManager;
        _tokenService = tokenService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] UserRegisterRequest model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var userExists = await _userManager.FindByEmailAsync(model.Email);

        if (userExists != null)
            return BadRequest(new { message = "This email is alread in use."});

        var user = new ApplicationUser { UserName = model.Email, Email = model.Email, FullName = model.FullName };

        var result = await _userManager.CreateAsync(user, model.Password);

        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => e.Description);
            return BadRequest(result.Errors);
        }

        return Ok(new { Message = "User registered successfully" });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] UserLoginRequest model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var user = await _userManager.FindByEmailAsync(model.Email);

        if (user == null)
            return Unauthorized(new { message = "invalid email or password."});

        var isPasswordValid = await _userManager.CheckPasswordAsync(user, model.Password);

        if (!isPasswordValid)
            return Unauthorized(new { message = "invalid email or password."});

        var token = _tokenService.GenerateToken(user);

        return Ok(new UserLoginResponse
        {
            Token = token,
            Expiration = DateTime.UtcNow.AddHours(2)
        });
    }

    //>Busca usuários por nome ou e-mail, para preencher listas de convite (ex.: convidar jogador para um jogo).</summary>
    [HttpGet("users")]
    public async Task<ActionResult<IReadOnlyList<UserSearchResult>>> SearchUsers([FromQuery] string? search, [FromQuery] int limit = 10)
    {
        var query = _userManager.Users.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim().ToLower();
            query = query.Where(u => u.FullName.ToLower().Contains(term) || (u.Email ?? string.Empty).ToLower().Contains(term));
        }

        var users = await query
            .OrderBy(u => u.FullName)
            .Take(Math.Clamp(limit, 1, 50))
            .Select(u => new UserSearchResult { Id = u.Id, FullName = u.FullName, Email = u.Email ?? string.Empty })
            .ToListAsync();

        return Ok(users);
    }

    //>Resolve uma lista de ids de usuário para nome/e-mail (ex.: para exibir nomes no ranking).</summary>
    [HttpGet("users/by-ids")]
    public async Task<ActionResult<IReadOnlyList<UserSearchResult>>> GetUsersByIds([FromQuery] string ids)
    {
        if (string.IsNullOrWhiteSpace(ids))
            return Ok(new List<UserSearchResult>());

        var idList = ids.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList();

        var users = await _userManager.Users
            .Where(u => idList.Contains(u.Id))
            .Select(u => new UserSearchResult { Id = u.Id, FullName = u.FullName, Email = u.Email ?? string.Empty })
            .ToListAsync();

        return Ok(users);
    }
}
