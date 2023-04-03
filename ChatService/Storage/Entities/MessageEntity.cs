namespace ChatService.Web.Dtos
{
    public record MessageEntity(string Id ,string partitionkey, string Text ,long CreatedUnixTime,string ConversationId,string SenderId);
}
