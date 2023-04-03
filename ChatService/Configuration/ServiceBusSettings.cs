using ChatService.Web.Dtos;

namespace   ChatService.Web.Configuration;

public class ServiceBusSettings
{
    public string ConnectionString { get; set; }
    public string CreateProfileQueueName { get; set; }
    public string GetUserConversationsQueueName { get; set; }
    public string IsConversationExistQueueName { get; set; }
    public string UpsertConversationQueueName { get; set; }
    public string CreateConversationUserRecordQueueName { get; set; }

}


