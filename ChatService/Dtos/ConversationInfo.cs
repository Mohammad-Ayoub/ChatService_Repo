namespace ChatService.Web.Dtos
{
    public record ConversationInfo(string ConversationId , long LastModifiedUnixTime,Profile Recipient);
}
