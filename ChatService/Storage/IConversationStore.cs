using ChatService.Web.Dtos;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;


namespace ChatService.Web.Storage
{
    public interface IConversationStore
    {
        Task<StartConversationResponse> UpsertConversation(Conversation conversation);
        Task<Conversation> GetConversation(string ConversationId);
        Task<GetConversationsOfUserResponse> GetUserConversations(string username, string? continuationToken, int? limit, long? lastSeenMessageTime);
        Task UpsertUserConversation(ConversationUserRecord conversationUserRecord);

    }
}
