using ChatService.Web.Dtos;

namespace ChatService.Web.Services
{
    public interface  IMessageService

    {

        Task<Boolean> IsMessageExist(string messageId);
        Task<SendMessageResponse> SendMessage(string conversationId, MessageRequest message);
        Task<GetConversationMessagesResponse> GetConversationMessages(string conversationId, string? continuationToken, int? limit, long? lastSeenMessageTime);

    }
}
