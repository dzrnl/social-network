namespace SocialNetwork.Application.Abstractions.Queries.Users;

public record ChangeUserSurnameQuery(long Id, string NewSurname);