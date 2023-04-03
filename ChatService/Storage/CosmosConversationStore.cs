using Azure;
using ChatService.Web.Dtos;
using ChatService.Web.Storage.Entities;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Azure.Cosmos;
using System.Net;

namespace ChatService.Web.Storage
{




    public class CosmosConversationStore : IConversationStore
    {


        private readonly CosmosClient _cosmosclient;


        private Container ConversationStoreContainer => _cosmosclient.GetDatabase("ChatService").GetContainer("profile");
        public CosmosConversationStore(CosmosClient cosmosclient)
        {

            _cosmosclient = cosmosclient;

        }





        public static ConversationEntiy ToEntity(Conversation conversation ,string conversationId ,long lastmodifedunixtime)
        {

            return new ConversationEntiy(
                Id: conversationId,
                partitionkey: conversationId,
                Participants: conversation.Participants,
                LastModifiedUnixTime: lastmodifedunixtime

                );

        }

        public static Conversation ToConversation(ConversationEntiy conversationEntiy)
        {

            return new Conversation(
                Id: conversationEntiy.Id,
                Participants: conversationEntiy.Participants,
                LastModifiedUnixTime: conversationEntiy.LastModifiedUnixTime
                );

        }



        public async Task<StartConversationResponse> UpsertConversation(Conversation conversation)
        {

            var conversationId = Guid.NewGuid().ToString();
            var CurrentUnixTime = UnixTimeNow();

            try
            {
                await ConversationStoreContainer.UpsertItemAsync(ToEntity(conversation,conversationId,CurrentUnixTime));


                return new StartConversationResponse(conversationId, CurrentUnixTime);
            }

            catch
            {

                throw;

            }



        }

        public async Task<Conversation> GetConversation(string ConversationId)
        {
            try
            {
                var entity = await ConversationStoreContainer.ReadItemAsync<ConversationEntiy>(

                        id: ConversationId,
                        partitionKey: new PartitionKey(ConversationId),
                            new ItemRequestOptions
                            {
                                ConsistencyLevel = ConsistencyLevel.Session
                            }

                    );
                return ToConversation(entity);
            }

            catch (CosmosException ex)
            {
                if (ex.StatusCode == HttpStatusCode.NotFound)
                {
                    return null;
                }

                throw;

            }
        }

        public Task<GetConversationsOfUserResponse> GetUserConversations(string username, string? continuationToken, int? limit, long? lastSeenMessageTime)

        {


            throw new NotImplementedException();

        }

        public Task UpsertUserConversation(ConversationUserRecord conversationUserRecord)
        {
            throw new NotImplementedException();
        }





        public long UnixTimeNow()
        {
            var timeSpan = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0));
            return (long)timeSpan.TotalSeconds;
        }




    }


  

}