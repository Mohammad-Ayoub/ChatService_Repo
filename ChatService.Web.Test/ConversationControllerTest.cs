
using System.Net;
using System.Text;
using ChatService.Web.Dtos;
using ChatService.Web.Services;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Newtonsoft.Json;

namespace ChatService.Web.Controllers

{ 
public class ConversationControllerTest : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly Mock<IConversationService> _ConversationServiceMock = new();
    private readonly HttpClient _httpClient;
    private readonly StartConversationRequest _startconversationRequest;

    public ConversationControllerTest(WebApplicationFactory<Program> factory)
    {
        _httpClient = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services => { services.AddSingleton(_ConversationServiceMock.Object); });
        }).CreateClient();


            _startconversationRequest = new StartConversationRequest
        (
            Participants: new List<string> { "participant1", "participant2" },
            FirstMessage: new MessageRequest("id", "SenderUsername", " text")
        );




    }


    [Fact]
    public async Task StartConversation_ReturnsBadRequest_WhenRequestIsNull()
    {
        // Arrange
        StartConversationRequest request = null;
        var httpContent = new StringContent(JsonConvert.SerializeObject(request), Encoding.Default, "application/json");

        // Act
        var StartConversationResponse = await _httpClient.PostAsync($"/api/conversations/",httpContent);

            // Assert
            
            Assert.Equal(HttpStatusCode.BadRequest, StartConversationResponse.StatusCode);

    }


    [Theory]
    [InlineData(null, null)]
    public async Task StartConversation_ReturnsBadRequest_WhenRequestParametersAreInvalid(List<string> Participants, MessageRequest FirstMessage)
    {
        // Arrange
        StartConversationRequest request = new StartConversationRequest(Participants,FirstMessage);
        var httpContent = new StringContent(JsonConvert.SerializeObject(request), Encoding.Default, "application/json");

        // Act
        var StartConversationResponse = await _httpClient.PostAsync($"/api/conversations/", httpContent);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, StartConversationResponse.StatusCode);

    }


    [Fact]
    public async Task StartConversation_ReturnsConflict_WhenConversationAlreadyExists()
    {
        

        _ConversationServiceMock.Setup(s => s.IsConversationExist(It.IsAny<string>())).ReturnsAsync(true);

        var httpContent = new StringContent(JsonConvert.SerializeObject(_startconversationRequest), Encoding.Default, "application/json");

        // Act
        var StartConversationResponse = await _httpClient.PostAsync($"/api/conversations/", httpContent);

        // Assert
        Assert.Equal(HttpStatusCode.Conflict, StartConversationResponse.StatusCode);
    }

    [Fact]
    public async Task StartConversation_ReturnsCreated_WhenConversationCreatedSuccessfully()
    {
        var ExpectedStartConversationResponse = new StartConversationResponse(Id: "1233232", CreatedUnixTime: 12341);

        _ConversationServiceMock.Setup(s => s.IsConversationExist(It.IsAny<string>())).ReturnsAsync(false);

        _ConversationServiceMock.Setup(s => s.StartConversation( It.IsAny<List<string>>(), It.IsAny<MessageRequest>()))
                   .ReturnsAsync(ExpectedStartConversationResponse);

        var httpContent = new StringContent(JsonConvert.SerializeObject(_startconversationRequest), Encoding.Default, "application/json");


        var StartConversationResponse = await _httpClient.PostAsync($"/api/conversations/", httpContent);

        Assert.Equal(HttpStatusCode.Created, StartConversationResponse.StatusCode);

        var jsonResponse = await StartConversationResponse.Content.ReadAsStringAsync();

        Assert.Equal(ExpectedStartConversationResponse, JsonConvert.DeserializeObject<StartConversationResponse>(jsonResponse));

    }

    [Fact]
    public async Task StartConversation_ReturnsInternalServerError_WhenExceptionOccurs()
    {

        _ConversationServiceMock.Setup(s => s.IsConversationExist(It.IsAny<string>())).ThrowsAsync(new Exception());

        var httpContent = new StringContent(JsonConvert.SerializeObject(_startconversationRequest), Encoding.Default, "application/json");

        var StartConversationResponse = await _httpClient.PostAsync($"/api/conversations/", httpContent);

        Assert.Equal(HttpStatusCode.InternalServerError, StartConversationResponse.StatusCode);
    }



    [Theory]
    [InlineData(null)]
    [InlineData("")]

    public async Task GetConversationOfUser_ReturnsBadRequest_WhenUsernameIsNull(string username)
    {

      
        var getconversationsofuserResponse = await _httpClient.GetAsync($"/api/conversations?username={username}&continuationToken={""}&limit={0}&lastSeenConversationTime={0}");

        Assert.Equal(HttpStatusCode.BadRequest, getconversationsofuserResponse.StatusCode);
    }



[Fact]
public async Task GetConversationOfUser_ReturnsOk()
{
    // Arrange
    string username = "validusername";

    var profile = new Profile("foobar", "Foo", "Bar", "imgid");
    var conversation = new ConversationInfo("anyid" , 1241414, profile);

    var expectedConversationsOfUser = new GetConversationsOfUserResponse(
        Conversations: new List<ConversationInfo> { conversation },
        NextUri: "nexturi"
    );


    _ConversationServiceMock.Setup(s => s.GetUserConversations(username, It.IsAny<string>(), It.IsAny<int>(), It.IsAny<long>())).ReturnsAsync(expectedConversationsOfUser);

    var getconversationsofuserResponse = await _httpClient.GetAsync($"/api/conversations?username={username}&continuationToken={""}&limit={0}&lastSeenConversationTime={0}");

    var JsonResponse = await getconversationsofuserResponse.Content.ReadAsStringAsync();
    var actualConversationsOfUser = JsonConvert.DeserializeObject<GetConversationsOfUserResponse>(JsonResponse);

    Assert.Equal(HttpStatusCode.OK, getconversationsofuserResponse.StatusCode);
    Assert.Equal(expectedConversationsOfUser.Conversations.Count, actualConversationsOfUser.Conversations.Count);
    Assert.Equal(expectedConversationsOfUser.NextUri, actualConversationsOfUser.NextUri);
    Assert.Equal(expectedConversationsOfUser.Conversations[0].ConversationId, actualConversationsOfUser.Conversations[0].ConversationId);
    Assert.Equal(expectedConversationsOfUser.Conversations[0].Recipient.Username, actualConversationsOfUser.Conversations[0].Recipient.Username);

}

    [Fact]
    public async Task GetConversationOfUser_ReturnsInternalServerError_WhenServiceThrowsException()
    {


         string username = "validusername";
        _ConversationServiceMock.Setup(s => s.GetUserConversations(username, It.IsAny<string>(), It.IsAny<int>(), It.IsAny<long>())).ThrowsAsync(new Exception());

        var getconversationsofuserResponse = await _httpClient.GetAsync($"/api/conversations?username={username}& continuationToken={" "}&limit={0}&lastSeenConversationTime={232}");

        Assert.Equal(HttpStatusCode.InternalServerError, getconversationsofuserResponse.StatusCode);
    }
}}
























