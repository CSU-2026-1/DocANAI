using DocANAI.Persistence.Entities.User;

namespace DocANAI.Api.Infrastructure.Authentication;

public interface IJwtProvider
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken();
}