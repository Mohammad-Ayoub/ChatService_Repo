using ChatService.Web.Dtos;
using ChatService.Web.Storage;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;




namespace ChatService.Web.IntegrationTest
{
    public class BlobImageStoreTest : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly IimageStore _store;







        public BlobImageStoreTest(WebApplicationFactory<Program> factory)
        {
            _store = factory.Services.GetRequiredService<IimageStore>();
        }




        [Fact]
        public async Task UploadFile_WithValidFile_ShouldReturnFileid() {

            // setup 
            var filename = "testimage";
            var filepath = "C:\\Users\\hasoub\\source\\repos\\ChatService\\ChatService.Web.IntegrationTest\\testimage.jpg";
            var filelength = new FileInfo(filepath).Length;
            var filestream = new FileStream(filepath, FileMode.Open);

            var formfile = new FormFile(filestream, 0, filelength, filename, filepath)
                {
                Headers = new HeaderDictionary(),
                ContentType = "image/jpeg"
                };
            
            // ack 

            var responseFileId  = await _store.UploadFile(formfile);

            //assert 
            Assert.NotNull(responseFileId);


        }


        [Fact]

        public async Task UploadFile_WithNullFile_ShouldThrowArgumentNullException() {
        
            //setup 
            FormFile? File = null;

            // act + assert 
            await Assert.ThrowsAsync<ArgumentNullException>(

                async () => await _store.UploadFile(File)

                ) ;




        }

        [Fact]
        public async Task GetFile_withExistingFileId_ShouldReturnFileContentResult() {

            //setup 
            var FileId = "File_3fb850ae-9909-4858-a8dc-4c66cc2cbaf1";

            // ack 

            var responseFileContent = await _store.GetFile(FileId);  

            // assert 

            Assert.NotNull(responseFileContent);

        }
        [Fact]
        public async Task GetFile_withNonExistingFileId_ShouldReturnNull()
        {

            //setup 
            var FileId = "nonexisting imageid";

            // ack 
            var responseFileContent = await _store.GetFile(FileId);

            // assert 
            Assert.Null(responseFileContent);





        }
    }



}
