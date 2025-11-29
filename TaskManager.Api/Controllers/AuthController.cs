using Microsoft.AspNetCore.Mvc;
using TaskManager.Api.DTOs;
using TaskManager.Api.Entities;
using TaskManager.Api.Repositories;
using TaskManager.Api.Services;

namespace TaskManager.Api.Controllers;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly UserRepository _users;
    private readonly RefreshTokenRepository _refreshTokens;
    private readonly JwtService _jwt;

    public AuthController(UserRepository users, RefreshTokenRepository refreshTokens, JwtService jwt)
    {
        _users = users;
        _refreshTokens = refreshTokens;
        _jwt = jwt;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        var hash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

        var user = new User
        {
            Email = dto.Email,
            PasswordHash = hash
        };

        await _users.Add(user);
        return Ok();
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        var user = await _users.GetByEmail(dto.Email);

        if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            return Unauthorized();

        // Revocar tokens anteriores del usuario
        await _refreshTokens.RevokeAllUserTokens(user.Id.ToString(), GetIpAddress());

        // Generar nuevos tokens
        var accessToken = _jwt.GenerateAccessToken(user);
        var refreshToken = _jwt.GenerateRefreshToken();

        // Guardar refresh token en BD
        var refreshTokenEntity = new RefreshToken
        {
            Token = refreshToken,
            UserId = user.Id.ToString(),
            ExpiresAt = DateTime.UtcNow.AddDays(7), // Refresh token expira en 7 días
            CreatedByIp = GetIpAddress()
        };

        await _refreshTokens.Add(refreshTokenEntity);

        var response = new LoginResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            AccessTokenExpiresAt = DateTime.UtcNow.AddMinutes(15),
            RefreshTokenExpiresAt = refreshTokenEntity.ExpiresAt
        };

        return Ok(response);
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh(RefreshTokenRequestDto dto)
    {
        var storedRefreshToken = await _refreshTokens.GetByToken(dto.RefreshToken);

        if (storedRefreshToken == null || storedRefreshToken.ExpiresAt < DateTime.UtcNow)
            return Unauthorized("Invalid or expired refresh token");

        // Obtener usuario del refresh token (UserId es el ID del usuario como string)
        var user = await _users.GetById(int.Parse(storedRefreshToken.UserId));
        if (user == null)
            return Unauthorized("User not found");

        // Generar nuevos tokens
        var newAccessToken = _jwt.GenerateAccessToken(user);
        var newRefreshToken = _jwt.GenerateRefreshToken();

        // Revocar el refresh token usado
        await _refreshTokens.RevokeToken(dto.RefreshToken, GetIpAddress(), newRefreshToken);

        // Guardar nuevo refresh token
        var newRefreshTokenEntity = new RefreshToken
        {
            Token = newRefreshToken,
            UserId = user.Id.ToString(),
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            CreatedByIp = GetIpAddress()
        };

        await _refreshTokens.Add(newRefreshTokenEntity);

        var response = new LoginResponseDto
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken,
            AccessTokenExpiresAt = DateTime.UtcNow.AddMinutes(15),
            RefreshTokenExpiresAt = newRefreshTokenEntity.ExpiresAt
        };

        return Ok(response);
    }

    private string GetIpAddress()
    {
        return HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
    }
}
