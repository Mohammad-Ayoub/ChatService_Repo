using ChatService.Web.Dtos;
using ChatService.Web.Services;

using Microsoft.AspNetCore.Mvc;

namespace ChatService.Web.Controllers;


[ApiController]

public class ConversationController : ControllerBase
{
    private readonly IConversationService _conversationsService;
    private readonly IMessageService _messageService;
    private readonly IProfileService _profileService;

    public ConversationController(IConversationService conversationsService, IMessageService messageService, IProfileService profileService)
    {
        _conversationsService = conversationsService;
        _messageService = messageService;
        _profileService = profileService;
    }

    [HttpPost]
    [Route("api/conversations")]
    public async Task<ActionResult<StartConversationResponse>> StartConversation([FromBody] StartConversationRequest request)
    {
        if (request == null)
        {
            return BadRequest("The request must have body");
        }
        if ( request.Participants == null || request.FirstMessage == null)
        {
            return BadRequest("The request must include participants and a first message.");
        }

        try
        {
            var firstMessageSenderUsername = request.FirstMessage.SenderUsername;
            var firstSenderExists = await _profileService.isUserExist(firstMessageSenderUsername);

            if (!firstSenderExists)
            {
                return NotFound("The sender of the first message does not exist.");
            }

            var participantsExist = await _profileService.DoesParticipantsExist(request.Participants);

            if (!participantsExist)
            {
                return NotFound("One or more participants do not exist.");
            }

            var firstMessageId = request.FirstMessage.Id;
            var isMessageExist = await _messageService.IsMessageExist(firstMessageId);

            if (isMessageExist)
            {
                return Conflict("The first message with this ID is conflicting with an existing message ID.");
            }

            var response = await _conversationsService.StartConversation(request.Participants, request.FirstMessage);
            return CreatedAtAction(nameof(StartConversation), response);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "An error occurred while starting the conversation.");
        }
    }

    [HttpGet]
    [Route("api/conversations")]
    public async Task<ActionResult<GetConversationsOfUserResponse>> GetConversationOfUser(string username, string? continuationToken , int? limit , long? lastSeenConversationTime)
    {
        if (string.IsNullOrEmpty(username))
        {
            return BadRequest("username cannot be null or empty");
        }


        try
        {
            var isUsernameExist = await _profileService.isUserExist(username);

            if (!isUsernameExist)
            {
                return NotFound("This username does not exist.");
            }




            var response = await _conversationsService.GetUserConversations(username, continuationToken, limit, lastSeenConversationTime);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "An error occurred while fetching the messages.");
        }
    }
}