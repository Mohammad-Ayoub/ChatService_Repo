namespace ChatService.Web.Dtos
{
    public record GetConversationMessagesResponse(List<MessageRequest>Messages,string NextUri);

}
