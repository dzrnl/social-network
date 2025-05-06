using SocialNetwork.Application.Models;
using SocialNetwork.Infrastructure.DataAccess.Entities;

namespace SocialNetwork.Infrastructure.DataAccess.Mappers;

public static class PostMapper
{
    public static Post ToDomain(this PostEntity messageEntity)
    {
        return new Post(
            messageEntity.Id,
            messageEntity.Author?.ToPreview(),
            messageEntity.Content,
            messageEntity.PublishedAt);
    }
}