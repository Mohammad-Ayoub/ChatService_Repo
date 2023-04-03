using ChatService.Web.Dtos;
using ChatService.Web.Services;
using Microsoft.AspNetCore.Mvc;
namespace ChatService.Web.Controllers
{
    [ApiController]
    public class MessageController : ControllerBase
    {
        private readonly IMessageService _messageService;
        private readonly IConversationService _conversationService;
        private readonly IProfileService _profileService;

        public MessageController(
            IMessageService messageService,
            IConversationService conversationService,
            IProfileService profileService)
        {
            _messageService = messageService;
            _conversationService = conversationService;
            _profileService = profileService;
        }

        [HttpPost]
        [Route("api/conversation/{conversationId}/messages")]
        public async Task<ActionResult<SendMessageResponse>> SendMessageToConversation(
            string conversationId,
            [FromBody] MessageRequest message)
        {
            if (message == null)
            {
                return BadRequest();
            }

            try
            {
                var senderUsername = message.SenderUsername;
                var senderUsernameExists = await _profileService.isUserExist(senderUsername);
                if (!senderUsernameExists)
                {
                    return NotFound("The sender id of the message does not exist.");
                }

                var messageId = message.Id;
                var isMessageExist = await _messageService.IsMessageExist(messageId);
                if (isMessageExist)
                {
                    return Conflict("The message with this ID is conflicting with an existing message ID.");
                }

                var conversationExists = await _conversationService.IsConversationExist(conversationId);

                if (!conversationExists)
                {
                    return NotFound("No conversation with this ID exists.");
                }

                var messageExists = await _messageService.IsMessageExist(messageId);

                if (messageExists)
                {
                    return Conflict("A message with this ID already exists.");
                }

                var response = await _messageService.SendMessage(conversationId, message);

                return CreatedAtAction(nameof(SendMessageToConversation), response);
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while sending the message.");
            }
        }

        [HttpGet]
        [Route("api/conversation/{conversationId}/messages")]
        public async Task<ActionResult<GetConversationMessagesResponse>> GetMessagesOfConversation(
            string conversationId,
            string? continuationToken,
            int? limit,
            long? lastSeenMessageTime)
        {
            try
            {
                var conversationExists = await _conversationService.IsConversationExist(conversationId);

                if (!conversationExists)
                {
                    return NotFound("No conversation with this ID exists.");
                }

                var response = await _messageService.GetConversationMessages(
                    conversationId,
                    continuationToken,
                    limit,
                    lastSeenMessageTime);

                return Ok(response);
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while fetching the messages.");
            }
        }
    }
}