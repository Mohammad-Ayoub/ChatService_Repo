using ChatService.Web.Dtos;

namespace ChatService.Web.Services
{
    public interface IConversationService
    {


        Task<StartConversationResponse> StartConversation(List<string> Participants, MessageRequest FirstMessage);
        Task<Boolean> IsConversationExist(string ConversationId);
        Task<GetConversationsOfUserResponse> GetUserConversations(string username, string? continuationToken, int? limit, long? lastSeenMessageTime);
        Task EnqueueUpsertConversation(Conversation conversation);
        Task EnqueueCreateConversationUserRecord(string ConversationId, string Username);
        Task<Boolean> EnqueueIsConversationExist(string ConversationId);
        Task<GetConversationsOfUserResponse> EnqueueGetUserConversations(string username, string? continuationToken, int? limit, long? lastSeenMessageTime);


    }
}
