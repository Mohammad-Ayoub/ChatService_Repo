
using ChatService.Web.Dtos;
using ChatService.Web.Storage;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Net;
using System.Net.Http.Headers;
using static Org.BouncyCastle.Bcpg.Attr.ImageAttrib;


namespace ChatService.Web.Tests.Controllers;

public class ImageControllerTest : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly Mock<IimageStore> _imageStoreMock = new();
    private readonly HttpClient _httpClient;
    private MultipartFormDataContent _formData = new();


    public ImageControllerTest(WebApplicationFactory<Program> factory)
    {
        _httpClient = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services => { services.AddSingleton(_imageStoreMock.Object); });

        }).CreateClient();


        var filepath = "C:\\Users\\hasoub\\source\\repos\\ChatService\\ChatService.Web.Test\\testimage.jpg";
        var filelength = new FileInfo(filepath).Length;
        var filestream = new FileStream(filepath, FileMode.Open);
        HttpContent fileStreamContent = new StreamContent(filestream);
        fileStreamContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
        {
            Name = "file",
            FileName = "anything"
        };

        _formData.Add(fileStreamContent);


    }


    [Fact]
    public async Task UploadImage__WhenRequestIsNull_ReturnsBadRequest()
    {
        
        // Act
        var result = await _httpClient.PostAsync("images", null);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
    }



    [Fact]
    public async Task UploadImage_ReturnsCreatedAtAction_WithImageId_WhenImageIsUploaded()
    {
        // settp
        var fileMock = new Mock<IFormFile>();

        var stream = new MemoryStream(new byte[] { 1, 2, 3 });

        fileMock.Setup(x => x.OpenReadStream()).Returns(stream);

        var request = new UploadImageRequest(File: fileMock.Object);

        _imageStoreMock.Setup(x => x.UploadFile(fileMock.Object)).ReturnsAsync("id");
        
        
        // Act

        var result = await _httpClient.PostAsync("images", _formData);

        // Assert
        Assert.Equal(HttpStatusCode.Created, result.StatusCode);
    }

    [Fact]
    public async Task UploadImage_ReturnsStatusCode500_WhenImageStoreThrowsException()
    {
        // Arrange
        var fileMock = new Mock<IFormFile>();
        
        var stream = new MemoryStream(new byte[] { 1, 2, 3 });
        
        fileMock.Setup(x => x.OpenReadStream()).Returns(stream);

        var request = new UploadImageRequest( File : fileMock.Object );

        _imageStoreMock.Setup(x => x.UploadFile(fileMock.Object)).ThrowsAsync(new Exception());

        // Act
        var result = await _httpClient.PostAsync("images", _formData); ;

        // Assert
        Assert.Equal(HttpStatusCode.InternalServerError, result.StatusCode);
    }

    [Fact]
    public async Task DownloadImage_ReturnsNotFound()
    {
        // Arrange
        var imageId = "imageId123";
        _imageStoreMock.Setup(x => x.GetFile(imageId)).ReturnsAsync((FileContentResult?)null);

        // Act
        var result = await _httpClient.GetAsync($"/images/{imageId}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
    }


    [Fact]
    public async Task DownloadImage_ReturnsFileContent_WhenImageStoreReturnsContent()

    {
        // Arrange
        var imageId = "imageId123";
        var fileContent = new byte[] { 1, 2, 3 };
        var memoryStream = new MemoryStream(fileContent);
        var file = new FileStreamResult(memoryStream, "image/png");

        var fileContentResult =  new FileContentResult(memoryStream.ToArray(), "application/octet-stream");
        _imageStoreMock.Setup(x => x.GetFile(imageId)).ReturnsAsync(fileContentResult);

            // Act
            var result = await _httpClient.GetAsync($"/images/{imageId}");

        // Assert
            var fileResult = Assert.IsType<FileStreamResult>(result);
            Assert.Equal("image/png", fileResult.ContentType);
            Assert.Equal(fileContent.Length, fileResult.FileStream.Length);
            var resultStream = new MemoryStream();
            await fileResult.FileStream.CopyToAsync(resultStream);
            Assert.Equal(fileContent, resultStream.ToArray());
    }

    [Fact]
public async Task DownloadImage_ReturnsStatusCode500_WhenImageStoreThrowsException()
{
    // Arrange
    var imageId = "imageId123";
    _imageStoreMock.Setup(x => x.GetFile(imageId)).ThrowsAsync(new Exception());

        // Act
        var result = await _httpClient.GetAsync($"/images/{imageId}");

        // Assert
        Assert.Equal(HttpStatusCode.InternalServerError, result.StatusCode);
}

}