using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RealEstateSystem.Application.Common.Helpers;
using RealEstateSystem.Application.Interfaces;
using RealEstateSystem.Domain.Exceptions;

namespace RealEstateSystem.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [Authorize(Roles = "Owner,Admin")]
    public class MediaController : ControllerBase
    {
        private readonly IUploadCloud _uploadCloud;

        public MediaController(IUploadCloud uploadCloud)
        {
            _uploadCloud = uploadCloud;
        }

        [HttpPost("upload-images")]
        public async Task<IActionResult> UploadImages(List<IFormFile> files, CancellationToken cancellationToken)
        {
            if (files == null || files.Count == 0)
            {
                return BadRequest(new { Message = "Vui lòng chọn ít nhất một file ảnh." });
            }

            var uploadedUrls = new List<string>();

            foreach (var file in files)
            {
                if (file.Length == 0)
                {
                    return BadRequest(new { Message = "File ảnh không được rỗng." });
                }

                if (file.Length > 10 * 1024 * 1024)
                {
                    return BadRequest(new { Message = $"Kích thước ảnh '{file.FileName}' phải nhỏ hơn hoặc bằng 10MB." });
                }

                var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (extension != ".jpg" && extension != ".jpeg" && extension != ".webp" && extension != ".html")
                {
                    return BadRequest(new { Message = $"Định dạng ảnh '{file.FileName}' không hợp lệ. Chỉ chấp nhận JPG, WebP hoặc HTML." });
                }

                using var memoryStream = new MemoryStream();
                await file.CopyToAsync(memoryStream, cancellationToken);
                var fileBytes = memoryStream.ToArray();

                if (!MediaHelper.IsValidImageSignature(fileBytes, extension))
                {
                    return BadRequest(new { Message = $"Tệp tin '{file.FileName}' có chữ ký số không hợp lệ. Nghi ngờ file giả mạo." });
                }

                var url = await _uploadCloud.UploadImageAsync(fileBytes, file.FileName, "properties/images", cancellationToken);
                uploadedUrls.Add(url);
            }

            return Ok(new { Urls = uploadedUrls });
        }

        [HttpPost("upload-video")]
        public async Task<IActionResult> UploadVideo(IFormFile file, CancellationToken cancellationToken)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest(new { Message = "Vui lòng chọn một file video." });
            }

            if (file.Length > 100 * 1024 * 1024)
            {
                return BadRequest(new { Message = "Kích thước video phải nhỏ hơn hoặc bằng 100MB." });
            }

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (extension != ".mp4")
            {
                return BadRequest(new { Message = "Chỉ chấp nhận định dạng video MP4." });
            }

            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream, cancellationToken);
            var fileBytes = memoryStream.ToArray();

            if (!MediaHelper.IsValidVideoSignature(fileBytes, extension))
            {
                return BadRequest(new { Message = "Tệp tin video có chữ ký số không hợp lệ. Nghi ngờ file giả mạo." });
            }

            var url = await _uploadCloud.UploadVideoAsync(fileBytes, file.FileName, "properties/videos", cancellationToken);

            return Ok(new { Url = url });
        }
    }
}
