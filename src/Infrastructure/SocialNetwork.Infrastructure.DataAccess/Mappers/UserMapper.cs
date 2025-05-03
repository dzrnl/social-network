using SocialNetwork.Application.Models;
using SocialNetwork.Infrastructure.DataAccess.Entities;

namespace SocialNetwork.Infrastructure.DataAccess.Mappers;

public static class UserMapper
{
    public static User ToDomain(this UserEntity userEntity)
    {
        return new User(userEntity.Id, userEntity.Username, userEntity.Name, userEntity.Surname);
    }
}