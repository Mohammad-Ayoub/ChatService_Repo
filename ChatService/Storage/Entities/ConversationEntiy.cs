using System.ComponentModel.DataAnnotations;

namespace ChatService.Web.Storage.Entities
{
    public record ConversationEntiy(
        [Required]  string Id,
        [Required]  string partitionkey,
        [Required]  List<string> Participants,
        [Required]  long LastModifiedUnixTime
        
        );
}