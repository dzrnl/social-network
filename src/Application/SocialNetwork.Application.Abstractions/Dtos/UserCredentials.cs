namespace SocialNetwork.Application.Abstractions.Dtos;

public record UserCredentials(long Id, string Username, string PasswordHash);