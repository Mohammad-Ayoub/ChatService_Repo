using ChatService.Web.Dtos;
using ChatService.Web.Storage;
using Microsoft.AspNetCore.Mvc;



namespace ChatService.Web.Controllers
{
    [ApiController]
    public class ImageController : ControllerBase

    {


        private readonly IimageStore _imageStore;

        public ImageController(IimageStore imageStore)
        {
            _imageStore = imageStore;
        }



        [HttpPost("images")]

        public async Task<ActionResult<UploadImageResponse>>UploadImage( [FromForm] UploadImageRequest request)
        {

            if (request == null || request.File == null) {

                return BadRequest();
            }

            try 
            {
                var Image = request.File;
                var responseImageId = await _imageStore.UploadFile(Image);
                return CreatedAtAction(nameof(DownloadImage),new { ImageId = responseImageId },new UploadImageResponse(responseImageId));

 
            }
            catch 
            {
                return StatusCode(500, "An internal server error occurred.");
            }

        }


        [HttpGet("images/{ImageId}")]
        public async Task<IActionResult>DownloadImage(string ImageId)
        {

            try
            {
                var responseImageContent = await _imageStore.GetFile(ImageId);
                if (responseImageContent == null) 
                {
                    return NotFound();
                }

                return responseImageContent;


            }
            catch
            {
                return StatusCode(500, "An internal server error occurred.");

            }



        }

    }











}

