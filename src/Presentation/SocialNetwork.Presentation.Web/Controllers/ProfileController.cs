using Microsoft.AspNetCore.Mvc;
using SocialNetwork.Application.Contracts.Commands.Friends;
using SocialNetwork.Application.Contracts.Commands.Posts;
using SocialNetwork.Application.Contracts.Commands.Users;
using SocialNetwork.Application.Contracts.Services;
using SocialNetwork.Application.Services;
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

        if (userResponse is GetUserCommand.Response.NotFound)
        {
            return NotFound();
        }

        if (userResponse is GetUserCommand.Response.Failure)
        {
            return UnprocessableEntity();
        }

        var user = ((GetUserCommand.Response.Success)userResponse).User;

        var userModel = UserModel.ToViewModel(user);

        var userPostsResponse = await _postService.GetUserPosts(new(user.Id, 1, PageSize));

        if (userPostsResponse is GetUserPostsCommand.Response.UserNotFound)
        {
            return NotFound();
        }

        if (userPostsResponse is GetUserPostsCommand.Response.Failure)
        {
            return UnprocessableEntity();
        }

        var posts = ((GetUserPostsCommand.Response.Success)userPostsResponse).Posts;

        var postsModel = posts.Select(PostModel.ToViewModel).ToList();

        if (CurrentUser == null)
        {
            var profileModel = new ProfileViewModel(userModel, false, postsModel);

            return View(profileModel);
        }

        if (CurrentUser.Id != user.Id)
        {
            var responseAreFriends = await _friendshipService.AreFriends(new(CurrentUser.Id, user.Id));

            if (responseAreFriends is AreFriendsCommand.Response.Failure failure)
            {
                return UnprocessableEntity(failure.Message);
            }

            var isFriend = ((AreFriendsCommand.Response.Success)responseAreFriends).AreFriends;

            var profileModel = new ProfileViewModel(userModel, isFriend, postsModel);

            return View(profileModel);
        }
        else
        {
            var profileModel = new ProfileViewModel(userModel, true, postsModel);

            return View(profileModel);
        }
    }
}