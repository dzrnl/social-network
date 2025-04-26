namespace SocialNetwork.Application.Abstractions.Queries.Users;

public record CreateUserQuery(string Username, string PasswordHash, string Name);