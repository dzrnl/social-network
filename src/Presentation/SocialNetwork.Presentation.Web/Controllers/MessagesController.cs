using Microsoft.AspNetCore.Mvc;
using SocialNetwork.Application.Contracts.Commands.Messages;
using SocialNetwork.Application.Contracts.Services;
using SocialNetwork.Application.Services;
using SocialNetwork.Presentation.Web.Filters;
using SocialNetwork.Presentation.Web.Models.Messages;

namespace SocialNetwork.Presentation.Web.Controllers;

[AuthorizeUser]
[Route("messages")]
public class MessagesController : BaseController
{
    private readonly IMessageService _messageService;

    public MessagesController(
        CurrentUserManager currentUserManager,
        IMessageService messageService)
        : base(currentUserManager)
    {
        _messageService = messageService;
    }

    [HttpGet]
    public async Task<IActionResult> Messages()
    {
        var response = await _messageService.GetAllMessages(new());

        if (response is GetAllMessagesCommand.Response.Failure failure)
        {
            return StatusCode(500, failure.Message);
        }

        var success = (GetAllMessagesCommand.Response.Success)response;

        var chatModel = new ChatModel(success.Messages
            .Select(MessageModel.ToViewModel)
            .ToList());

        return View(chatModel);
    }
}