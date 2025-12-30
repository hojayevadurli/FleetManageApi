using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FleetManage.Api.Controllers
{
    [ApiController]
    [Route("api/uploads")]
    [Authorize]
    public class UploadsController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;

        // 50MB request limit
        private const long MaxRequestBytes = 50L * 1024L * 1024L;

        // Allowed extensions (fallback when ContentType is missing/wrong)
        private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
        {
            ".jpg", ".jpeg", ".png", ".gif", ".webp",
            ".mp4", ".mov", ".avi", ".webm",
            ".pdf"
        };

        public UploadsController(IWebHostEnvironment env)
        {
            _env = env;
        }
        [HttpPost("service-history")]
        [RequestSizeLimit(52428800)] // 50MB
        public async Task<ActionResult<List<string>>> UploadServiceHistoryFiles([FromForm] List<IFormFile> files)
        {
            if (files == null || files.Count == 0)
                return BadRequest(new { message = "No files uploaded." });

            var webRoot = _env.WebRootPath ?? Path.Combine(_env.ContentRootPath, "wwwroot");
            var folder = Path.Combine(webRoot, "uploads", "service-history");
            Directory.CreateDirectory(folder);

            var baseUrl = $"{Request.Scheme}://{Request.Host}";
            var urls = new List<string>();

            foreach (var file in files)
            {
                if (file.Length <= 0) continue;
                var ext = Path.GetExtension(file.FileName);
                var name = $"{Guid.NewGuid():N}{ext}";
                var path = Path.Combine(folder, name);

                await using (var stream = System.IO.File.Create(path))
                {
                    await file.CopyToAsync(stream);
                }
                urls.Add($"{baseUrl}/uploads/service-history/{name}");
            }

            return Ok(urls);
        }
        // POST: api/uploads/workorders
        // Expects multipart/form-data with field name: "files"
        [HttpPost("workorders")]
        [RequestSizeLimit(MaxRequestBytes)]
        [RequestFormLimits(MultipartBodyLengthLimit = MaxRequestBytes)]
        public async Task<ActionResult<List<string>>> UploadWorkOrderFiles([FromForm] List<IFormFile> files)
        {
            if (files == null || files.Count == 0)
                return BadRequest(new { message = "No files uploaded. Make sure the form field name is 'files'." });

            // Make sure wwwroot exists
            var webRoot = _env.WebRootPath;
            if (string.IsNullOrWhiteSpace(webRoot))
                webRoot = Path.Combine(_env.ContentRootPath, "wwwroot");

            var folder = Path.Combine(webRoot, "uploads", "workorders");
            Directory.CreateDirectory(folder);

            // Build base URL once
            var baseUrl = $"{Request.Scheme}://{Request.Host}";

            var urls = new List<string>();

            foreach (var file in files)
            {
                if (file == null || file.Length <= 0) continue;

                // Determine allowed type
                var ct = (file.ContentType ?? string.Empty).ToLowerInvariant();
                var ext = Path.GetExtension(file.FileName);
                if (string.IsNullOrWhiteSpace(ext)) ext = "";

                var isImage = ct.StartsWith("image/");
                var isVideo = ct.StartsWith("video/");
                var isPdf = ct == "application/pdf";

                // Some browsers send videos as application/octet-stream, so also allow by extension
                var isAllowedByExt = !string.IsNullOrWhiteSpace(ext) && AllowedExtensions.Contains(ext);

                if (!(isImage || isVideo || isPdf || isAllowedByExt))
                {
                    return BadRequest(new
                    {
                        message = $"Invalid file type: {file.FileName}",
                        contentType = file.ContentType,
                        extension = ext
                    });
                }

                // If ext is missing but we know the type, pick a sane ext for preview friendliness
                if (string.IsNullOrWhiteSpace(ext))
                {
                    if (isImage) ext = ".jpg";
                    else if (isVideo) ext = ".mp4";
                    else if (isPdf) ext = ".pdf";
                    else ext = ".bin";
                }

                var name = $"{Guid.NewGuid():N}{ext}";
                var path = Path.Combine(folder, name);

                await using (var stream = System.IO.File.Create(path))
                {
                    await file.CopyToAsync(stream);
                }

                urls.Add($"{baseUrl}/uploads/workorders/{name}");
            }

            return Ok(urls);
        }
    }
}
