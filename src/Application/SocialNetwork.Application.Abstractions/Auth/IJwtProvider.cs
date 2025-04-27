namespace SocialNetwork.Application.Abstractions.Auth;

public interface IJwtProvider
{
    string GenerateToken(long userId);
}