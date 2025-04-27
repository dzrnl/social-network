namespace SocialNetwork.Application.Abstractions.Auth;

public interface IPasswordHasher
{
    string GenerateHash(string password);

    bool VerifyHash(string password, string hash);
}