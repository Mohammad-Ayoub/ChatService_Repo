
namespace ChatService.Web.Dtos
{
    public record Conversation(string Id , List<string>Participants ,long LastModifiedUnixTime);
}
