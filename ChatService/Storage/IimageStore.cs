using ChatService.Web.Dtos;
using Microsoft.AspNetCore.Mvc;


namespace ChatService.Web.Storage
{
    public interface IimageStore
    {

        Task<FileContentResult?> GetFile(string? Imageid);
        Task<string?> UploadFile(IFormFile? File);



    }
}
