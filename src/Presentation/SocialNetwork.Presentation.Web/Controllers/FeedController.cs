using Microsoft.AspNetCore.Mvc;
using SocialNetwork.Application.Contracts.Commands.Posts;
using SocialNetwork.Application.Contracts.Services;
using SocialNetwork.Application.Services;
using SocialNetwork.Presentation.Web.Filters;
using SocialNetwork.Presentation.Web.Models.Posts;

namespace SocialNetwork.Presentation.Web.Controllers;

[Route("feed")]
public class FeedController : BaseController
{
    private const int PageSize = 30;

    private readonly IPostService _postService;

    public FeedController(CurrentUserManager currentUserManager, IPostService postService) : base(currentUserManager)
    {
        _postService = postService;
    }

    [HttpGet]
    public async Task<IActionResult> Feed()
    {
        var response = await _postService.GetPosts(new(1, PageSize));

        if (response is GetPostsCommand.Response.InvalidRequest invalidRequest)
        {
            return BadRequest(invalidRequest.Message);
        }

        if (response is GetPostsCommand.Response.Failure failure)
        {
            return UnprocessableEntity(failure.Message);
        }

        var success = (GetPostsCommand.Response.Success)response;

        var feedModel = new FeedModel(success.Posts
            .Select(PostModel.ToViewModel)
            .ToList());

        return View(feedModel);
    }

    [AuthorizeUser]
    [HttpPost("post")]
    public async Task<IActionResult> CreatePost(CreatePostModel model, string? returnUrl)
    {
        var response = await _postService.CreatePost(new(AuthUser.Id, model.Content));

        if (response is CreatePostCommand.Response.InvalidRequest invalidRequest)
        {
            return BadRequest(invalidRequest.Message);
        }

        if (response is CreatePostCommand.Response.UserNotFound userNotFound)
        {
            return BadRequest(userNotFound.Message);
        }
        
        return Redirect(returnUrl ?? "/feed");
    }

    [AuthorizeUser]
    [HttpPost("delete/{postId:long}")]
    public async Task<IActionResult> DeletePost(long postId, [FromForm] string? returnUrl)
    {
        var response = await _postService.DeletePost(new(postId));

        if (response is DeletePostCommand.Response.NotFound notFound)
        {
            return NotFound(notFound.Message);
        }

        if (response is DeletePostCommand.Response.Failure failure)
        {
            return UnprocessableEntity(failure.Message);
        }
        
        return Redirect(returnUrl ?? "/");
    }
}