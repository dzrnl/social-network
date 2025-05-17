using Microsoft.AspNetCore.Mvc;
using SocialNetwork.Application.Contracts.Commands.Friends;
using SocialNetwork.Application.Contracts.Commands.Posts;
using SocialNetwork.Application.Contracts.Commands.Users;
using SocialNetwork.Application.Contracts.Services;
using SocialNetwork.Application.Models;
using SocialNetwork.Application.Services;
using SocialNetwork.Presentation.Web.Models.Friends;
using SocialNetwork.Presentation.Web.Models.Posts;
using SocialNetwork.Presentation.Web.Models.Profile;
using SocialNetwork.Presentation.Web.Models.Users;

namespace SocialNetwork.Presentation.Web.Controllers;

public class ProfileController : BaseController
{
    private const int PageSize = 30;

    private readonly IUserService _userService;
    private readonly IFriendshipService _friendshipService;
    private readonly IPostService _postService;

    public ProfileController(
        CurrentUserManager currentUserManager,
        IUserService userService,
        IFriendshipService friendshipService,
        IPostService postService)
        : base(currentUserManager)
    {
        _userService = userService;
        _friendshipService = friendshipService;
        _postService = postService;
    }

    [HttpGet("/{username}")]
    public async Task<IActionResult> Profile(string username)
    {
        var userResponse = await _userService.GetUserByUsername(new(username));

        if (userResponse is not GetUserCommand.Response.Success userSuccess)
        {
            return userResponse switch
            {
                GetUserCommand.Response.NotFound => NotFound(),
                GetUserCommand.Response.Failure userFailure => StatusCode(500, userFailure.Message),
                _ => StatusCode(500, "Unknown error occurred")
            };
        }

        var user = userSuccess.User;
        var userModel = UserModel.ToViewModel(user);

        var postsResponse = await _postService.GetUserPosts(new(user.Id, 1, PageSize));

        if (postsResponse is not GetUserPostsCommand.Response.Success postsSuccess)
        {
            return postsResponse switch
            {
                GetUserPostsCommand.Response.UserNotFound => NotFound(),
                GetUserPostsCommand.Response.Failure postsFailure => StatusCode(500, postsFailure.Message),
                _ => StatusCode(500, "Unknown error occurred")
            };
        }

        var posts = postsSuccess.Posts;
        var postsModel = posts.Select(PostModel.ToViewModel).ToList();

        if (CurrentUser == null || CurrentUser.Id == user.Id)
        {
            return View(new ProfileViewModel(userModel, null, null, postsModel));
        }

        var responseFriendStatus = await _friendshipService.GetFriendStatus(new(CurrentUser.Id, user.Id));

        if (responseFriendStatus is GetFriendStatusCommand.Response.Failure friendStatusFailure)
        {
            return StatusCode(500, friendStatusFailure.Message);
        }

        var status = ((GetFriendStatusCommand.Response.Success)responseFriendStatus).FriendStatus;

        FriendRequest? friendRequest = null;
        if (status is FriendStatus.Sent or FriendStatus.Incoming)
        {
            var response = await _friendshipService.GetFriendRequestByUsers(new(CurrentUser.Id, user.Id));

            if (response is GetFriendRequestCommand.Response.Failure failure)
            {
                return StatusCode(500, failure.Message);
            }
            
            friendRequest = ((GetFriendRequestCommand.Response.Success)response).FriendRequest;
        }

        var friendRequestModel = friendRequest != null ? FriendRequestModel.ToViewModel(friendRequest) : null;

        var profileModel = new ProfileViewModel(userModel, status, friendRequestModel, postsModel);

        return View(profileModel);
    }
}