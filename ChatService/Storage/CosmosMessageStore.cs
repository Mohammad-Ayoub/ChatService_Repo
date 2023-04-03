using Azure;
using ChatService.Web.Dtos;
using ChatService.Web.Storage.Entities;

using Microsoft.Azure.Cosmos;
using System.Net;

namespace ChatService.Web.Storage
{




    public class CosmosMessageStore : IMessageStore
    {


        private readonly CosmosClient _cosmosclient;


        private Container MessageStoreContainer => _cosmosclient.GetDatabase("ChatService").GetContainer("profile");
        public CosmosMessageStore(CosmosClient cosmosclient)
        {

            _cosmosclient = cosmosclient;

        }





        public static MessageEntity ToEntity(Message message, long createdUnixTime)
        {
            return new MessageEntity(
                Id: message.Id,
                partitionkey: message.ConversationId,
                Text: message.Text,
                ConversationId: message.ConversationId,
                CreatedUnixTime : createdUnixTime,
                SenderId: message.SenderId

                );

        }

        public static Message ToConversation(MessageEntity messageEntity)
        {
            return new Message(
                Id: messageEntity.Id,
                Text: messageEntity.Text,
                ConversationId: messageEntity.ConversationId,
                CreatedUnixTime: messageEntity.CreatedUnixTime,
                SenderId: messageEntity.SenderId

                );

        }



      




        public async Task<SendMessageResponse> UpsertMessage(Message message)
        {
            var CurrentUnixTime = UnixTimeNow();

            try
            {
                await MessageStoreContainer.UpsertItemAsync(ToEntity(message, CurrentUnixTime));


                return new SendMessageResponse(CurrentUnixTime);
            }

            catch
            {

                throw;

            }
        }

        public async Task<Message> GetMessage(string id)
        {
            try
            {
                var entity = await MessageStoreContainer.ReadItemAsync<MessageEntity>(

                        id: id,
                        partitionKey: new PartitionKey(id),
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



    public long UnixTimeNow()
        {
            var timeSpan = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0));
            return (long)timeSpan.TotalSeconds;
        }

        public Task<GetConversationMessagesResponse> GetMessages(string conversationId)
        {


            throw new NotImplementedException();
        }



    }



    }



