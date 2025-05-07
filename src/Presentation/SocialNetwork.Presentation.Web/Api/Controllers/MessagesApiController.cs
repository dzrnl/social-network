using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialNetwork.Application.Contracts.Commands.Messages;
using SocialNetwork.Application.Contracts.Services;
using SocialNetwork.Application.Services;
using SocialNetwork.Presentation.Web.Models.Messages;

namespace SocialNetwork.Presentation.Web.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/messages")]
public class MessagesApiController : ControllerBase
{
    private readonly IMessageService _messageService;
    private readonly CurrentUserManager _currentUserManager;

    public MessagesApiController(IMessageService messageService, CurrentUserManager currentUserManager)
    {
        _messageService = messageService;
        _currentUserManager = currentUserManager;
    }

    [HttpPost("send")]
    public async Task<ActionResult> SendMessage(SendMessageModel request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var currentUserId = _currentUserManager.CurrentUser!.Id;

        var response = await _messageService.SendMessage(new(currentUserId, request.Content));

        if (response is SendMessageCommand.Response.UserNotFound userNotFound)
        {
            return BadRequest(userNotFound.Message);
        }

        if (response is SendMessageCommand.Response.Failure failure)
        {
            return StatusCode(500, failure.Message);
        }

        var success = (SendMessageCommand.Response.Success)response;

        var messageModel = MessageModel.ToViewModel(success.Message);

        return Ok(messageModel);
    }

    [HttpGet]
    public async Task<ActionResult> GetAllChatMessages()
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

        return Ok(chatModel);
    }
}