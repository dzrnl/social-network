namespace SocialNetwork.Application.Abstractions.Queries.Users;

public record ChangeUserNameQuery(long Id, string NewName);