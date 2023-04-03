using System.Net;
using System.Text;
using ChatService.Web.Dtos;
using ChatService.Web.Services;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Newtonsoft.Json;



namespace ChatService.Web.Tests.Controllers;

public class MessageControllerTest : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly Mock<IMessageService> _MessageServiceMock = new();
    private readonly Mock<IConversationService> _ConversationServiceMock = new();
    private readonly HttpClient _httpClient;
    private readonly MessageRequest _message;

    public MessageControllerTest(WebApplicationFactory<Program> factory)
    {
        _httpClient = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services => { services.AddSingleton(_MessageServiceMock.Object); services.AddSingleton(_ConversationServiceMock.Object); });
        }).CreateClient();


        _message = new MessageRequest(
            Id: "anyid",
            SenderUsername: "anyid",
            Text : "anymessage"

        );

    }



    [Fact]
    public async Task SendMessageToConversation_ReturnsBadRequest_WhenMessageIsNull()
    {
        // Arrange
        string conversationId = "1";
        MessageRequest message = null; 
        var httpContent = new StringContent(JsonConvert.SerializeObject(message), Encoding.Default, "application/json");

        var response = await _httpClient.PostAsync($"/api/conversation/{conversationId}/messages", httpContent);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task SendMessageToConversation_ReturnsNotFound_WhenConversationDoesNotExist()
    {
        // Arrange
        string conversationId = "1";

        _ConversationServiceMock.Setup(cs => cs.IsConversationExist(conversationId)).ReturnsAsync(false);

        var httpContent = new StringContent(JsonConvert.SerializeObject(_message), Encoding.Default, "application/json");

        // Act
        var response = await _httpClient.PostAsync($"/api/conversation/{conversationId}/messages", httpContent);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equal("No conversation with this ID exists.", await response.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task SendMessageToConversation_ReturnsConflict_WhenMessageAlreadyExists()
    {
        // Arrange
        string conversationId = "1";

        _ConversationServiceMock.Setup(cs => cs.IsConversationExist(conversationId)).ReturnsAsync(true);
        _MessageServiceMock.Setup(ms => ms.IsMessageExist(_message.Id)).ReturnsAsync(true);

        var httpContent = new StringContent(JsonConvert.SerializeObject(_message), Encoding.Default, "application/json");
        // Act
        var response = await _httpClient.PostAsync($"/api/conversation/{conversationId}/messages", httpContent);

        // Assert
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        Assert.Equal("A message with this ID already exists.", await response.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task SendMessageToConversation_ReturnsCreated_WhenMessageCreatedSuccessfully()
    {
        // Arrange
        string conversationId = "1";
        var expectedSendMessageResponse = new SendMessageResponse(
            CreatedUnixTime : 12413215
            
            );

        _ConversationServiceMock.Setup(cs => cs.IsConversationExist(conversationId)).ReturnsAsync(true);
        _MessageServiceMock.Setup(ms => ms.IsMessageExist(_message.Id)).ReturnsAsync(false);
        _MessageServiceMock.Setup(ms => ms.SendMessage(conversationId, _message)).ReturnsAsync(expectedSendMessageResponse);

        var httpContent = new StringContent(JsonConvert.SerializeObject(_message), Encoding.Default, "application/json");
        // Act
        var response = await _httpClient.PostAsync($"/api/conversation/{conversationId}/messages", httpContent);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var jsonResponse = await response.Content.ReadAsStringAsync();
        var actualSendMessageResponse = JsonConvert.DeserializeObject<SendMessageResponse>(jsonResponse);
        Assert.Equal(expectedSendMessageResponse, actualSendMessageResponse);
    }


    [Fact]
    public async Task SendMessageToConversation_ReturnsInternalServerError_WhenExceptionOccurs()
    {
        // Arrange
        string conversationId = "1";
        var expectedSendMessageResponse = new SendMessageResponse(
            CreatedUnixTime: 12413215

            );

        _ConversationServiceMock.Setup(cs => cs.IsConversationExist(conversationId)).ThrowsAsync(new Exception());

        var httpContent = new StringContent(JsonConvert.SerializeObject(_message), Encoding.Default, "application/json");
        // Act
        var response = await _httpClient.PostAsync($"/api/conversation/{conversationId}/messages", httpContent);


        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
    }










    [Fact]
    public async Task GetMessagesOfConversation_ReturnsNotFound_WhenConversationDoesNotExist()
    {
        // Arrange
        string conversationId = "123";
        _ConversationServiceMock.Setup(x => x.IsConversationExist(conversationId)).ReturnsAsync(false);

        // Act
        var GetMessagesOfConversationResponse = await _httpClient.GetAsync($"/api/conversation/{conversationId}/messages");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, GetMessagesOfConversationResponse.StatusCode);
    }

    [Fact]
    public async Task GetMessagesOfConversation_ReturnsOk_WhenConversationExists()
    {

        string conversationId = "123";
        var messages = new List<MessageRequest> {_message,_message };

        var expectedGetMessagesOfConversationResponse = new GetConversationMessagesResponse(
            Messages: messages,
            NextUri: "nexturi"
            
            );

        _ConversationServiceMock.Setup(x => x.IsConversationExist(conversationId)).ReturnsAsync(true);
        _MessageServiceMock.Setup(x => x.GetConversationMessages(conversationId, It.IsAny<string>(), It.IsAny<int>(), It.IsAny<long>()))
            .ReturnsAsync(expectedGetMessagesOfConversationResponse);

        var response = await _httpClient.GetAsync($"/api/conversation/{conversationId}/messages?continuationToken={""}&limit={0}&lastSeenMessageTime={0}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var jsonResponse = await response.Content.ReadAsStringAsync();
        var actualGetMessagesOfConversationResponse = JsonConvert.DeserializeObject<GetConversationMessagesResponse>(jsonResponse);

        Assert.Equal(expectedGetMessagesOfConversationResponse.Messages, actualGetMessagesOfConversationResponse.Messages);
        Assert.Equal(expectedGetMessagesOfConversationResponse.Messages.Count, actualGetMessagesOfConversationResponse.Messages.Count);
    }




    [Fact]
    public async Task GetMessagesOfConversation_ReturnsInternalServerError_WhenExceptionOccurs()
    {
        // Arrange
        string conversationId = "123";
        _ConversationServiceMock.Setup(x => x.IsConversationExist(conversationId)).ThrowsAsync(new Exception());

        // Act
        var GetMessagesOfConversationResponse = await _httpClient.GetAsync($"/api/conversation/{conversationId}/messages");

        // Assert
        Assert.Equal(HttpStatusCode.InternalServerError, GetMessagesOfConversationResponse.StatusCode);
    }


}
















































