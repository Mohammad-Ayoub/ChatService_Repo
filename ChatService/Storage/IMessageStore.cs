using ChatService.Web.Dtos;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Bcpg.OpenPgp;
using System.Collections.Generic;


namespace ChatService.Web.Storage
{
    public interface IMessageStore
    {
        Task<SendMessageResponse> UpsertMessage( Message message);
        Task <Message> GetMessage(string id);
        Task<GetConversationMessagesResponse> GetMessages( string conversationId);


    }
}
