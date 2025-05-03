using SocialNetwork.Application.Models;
using SocialNetwork.Infrastructure.DataAccess.Entities;

namespace SocialNetwork.Infrastructure.DataAccess.Mappers;

public static class UserPreviewMapper
{
    public static UserPreview ToPreview(this UserEntity userEntity)
    {
        return new UserPreview(userEntity.Id, userEntity.Username, userEntity.Name, userEntity.Surname);
    }
}