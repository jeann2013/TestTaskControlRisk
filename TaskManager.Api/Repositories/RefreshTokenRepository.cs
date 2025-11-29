using Microsoft.EntityFrameworkCore;
using TaskManager.Api.Data;
using TaskManager.Api.Entities;

namespace TaskManager.Api.Repositories;

public class RefreshTokenRepository
{
    private readonly SqlDbContext _db;

    public RefreshTokenRepository(SqlDbContext db) => _db = db;

    public async Task<RefreshToken?> GetByToken(string token)
    {
        return await _db.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.Token == token && !rt.IsRevoked);
    }

    public async Task Add(RefreshToken refreshToken)
    {
        _db.RefreshTokens.Add(refreshToken);
        await _db.SaveChangesAsync();
    }

    public async Task RevokeToken(string token, string? ipAddress = null, string? replacedByToken = null)
    {
        var refreshToken = await GetByToken(token);
        if (refreshToken != null)
        {
            refreshToken.IsRevoked = true;
            refreshToken.RevokedAt = DateTime.UtcNow;
            refreshToken.RevokedByIp = ipAddress;
            refreshToken.ReplacedByToken = replacedByToken;

            _db.RefreshTokens.Update(refreshToken);
            await _db.SaveChangesAsync();
        }
    }

    public async Task RevokeAllUserTokens(string userId, string? ipAddress = null)
    {
        var userTokens = _db.RefreshTokens
            .Where(rt => rt.UserId == userId && !rt.IsRevoked);

        foreach (var token in userTokens)
        {
            token.IsRevoked = true;
            token.RevokedAt = DateTime.UtcNow;
            token.RevokedByIp = ipAddress;
        }

        await _db.SaveChangesAsync();
    }

    public async Task RemoveExpiredTokens()
    {
        var expiredTokens = _db.RefreshTokens
            .Where(rt => rt.ExpiresAt < DateTime.UtcNow);

        _db.RefreshTokens.RemoveRange(expiredTokens);
        await _db.SaveChangesAsync();
    }
}