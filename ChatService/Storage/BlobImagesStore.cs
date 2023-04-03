using ChatService.Web.Dtos;
using ChatService.Web.Storage.Entities;
using Microsoft.Azure.Cosmos;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Mvc;
using Azure.Storage.Blobs.Models;
using System.Net;

namespace ChatService.Web.Storage
{




    public class BlobImagesStore : IimageStore
    {


        private readonly BlobServiceClient _blobClient;

        private BlobContainerClient Blobcontainer => _blobClient.GetBlobContainerClient("blobchatservicecontainer");


        public BlobImagesStore( BlobServiceClient blobClient)
        {
            _blobClient = blobClient;

        }




        public async Task<string?> UploadFile(IFormFile? File)

        {
            if (File == null)
            {
                throw new ArgumentNullException(nameof(File));
            }

            try
            {

                var FileName = $"{File.Name}_{Guid.NewGuid()}";

                BlobClient blobClient = Blobcontainer.GetBlobClient(FileName);

                BlobUploadOptions options = new BlobUploadOptions
                {
                    HttpHeaders = new BlobHttpHeaders
                    {
                        ContentType = File.ContentType
                    }
                };

                await using (Stream stream = File.OpenReadStream())
                {
                    await blobClient.UploadAsync(stream, options);
                }

                BlobProperties blobProperties = await blobClient.GetPropertiesAsync();

                string ImageId = blobProperties.ETag.ToString();

                return FileName;


            }
            catch
            {
                throw;
            }





        }

        public async Task<FileContentResult?> GetFile(string? FileId)
        {
            if (string.IsNullOrWhiteSpace(FileId))
            {
                throw new ArgumentException($"The {FileId} parameter cannot be null or empty.");
            }

            try
            {
                BlobClient blobClient = Blobcontainer.GetBlobClient(FileId);

                if (!await blobClient.ExistsAsync())
                {
                    return null;
                }

                await using (MemoryStream memoryStream = new MemoryStream())
                {
                    await blobClient.DownloadToAsync(memoryStream);

                    memoryStream.Seek(0, SeekOrigin.Begin);

                    return new FileContentResult(memoryStream.ToArray(), "application/octet-stream");

                }
            }

            catch 
            {
                throw;
            }

        }







    } 

}