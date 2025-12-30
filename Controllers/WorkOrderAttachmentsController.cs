using System.Security.Claims;
using FleetManage.Api.Data;
using FleetManage.Api.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FleetManage.Api.Controllers
{
    [ApiController]
    [Route("api/workorders")]
    [Authorize]
    public class WorkOrderAttachmentsController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly IWebHostEnvironment _env;

        private const long MaxRequestBytes = 50L * 1024L * 1024L; // 50MB

        private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
        {
            ".jpg", ".jpeg", ".png", ".gif", ".webp",
            ".mp4", ".mov", ".avi", ".webm",
            ".pdf"
        };

        public WorkOrderAttachmentsController(AppDbContext db, IWebHostEnvironment env)
        {
            _db = db;
            _env = env;
        }

        private Guid UserId =>
            Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        // ----------------------------
        // GET: api/workorders/{id}/attachments
        // Returns attachments (documents) linked to a work order
        // ----------------------------
        [HttpGet("{id:guid}/attachments")]
        public async Task<ActionResult<List<WorkOrderDocumentDto>>> GetAttachments(Guid id)
        {
            // Ensure WO exists (tenant filter will enforce)
            var exists = await _db.WorkOrders.AnyAsync(x => x.Id == id);
            if (!exists) return NotFound(new { message = "Work order not found." });

            var documents = await (
                from lnk in _db.DocumentLinks.AsNoTracking()
                join d in _db.Documents.AsNoTracking() on lnk.DocumentId equals d.Id
                where lnk.EntityType == "work_order" && lnk.EntityId == id
                orderby d.CreatedAt descending
                select new WorkOrderDocumentDto(
                    d.Id, d.FileUrl, d.FileType, d.DocKind, d.Status, d.ConfidenceScore, d.CreatedAt
                )
            ).ToListAsync();

            return Ok(documents);
        }

        // ----------------------------
        // POST: api/workorders/{id}/attachments
        // multipart/form-data with field name: "files"
        // Uploads files -> creates Document rows -> creates DocumentLink rows
        // Returns WorkOrderDocumentDto[] for newly uploaded docs
        // ----------------------------
        [HttpPost("{id:guid}/attachments")]
        [RequestSizeLimit(MaxRequestBytes)]
        [RequestFormLimits(MultipartBodyLengthLimit = MaxRequestBytes)]
        public async Task<ActionResult<List<WorkOrderDocumentDto>>> UploadAttachments(
            Guid id,
            [FromForm] List<IFormFile> files)
        {
            if (files == null || files.Count == 0)
                return BadRequest(new { message = "No files uploaded. Make sure the form field name is 'files'." });

            // Load the work order (tenant filter applies here)
            var wo = await _db.WorkOrders.FirstOrDefaultAsync(x => x.Id == id);
            if (wo is null)
                return NotFound(new { message = "Work order not found." });

            // Ensure wwwroot exists
            var webRoot = _env.WebRootPath;
            if (string.IsNullOrWhiteSpace(webRoot))
                webRoot = Path.Combine(_env.ContentRootPath, "wwwroot");

            // Tenant+WO scoped folder
            var tenantFolder = wo.TenantId.ToString("N");
            var woFolder = wo.Id.ToString("N");
            var folder = Path.Combine(webRoot, "uploads", tenantFolder, "workorders", woFolder);
            Directory.CreateDirectory(folder);

            // Build base URL once (NOTE: if behind proxy, configure forwarded headers)
            var baseUrl = $"{Request.Scheme}://{Request.Host}";

            var createdDocs = new List<Document>();

            using var tx = await _db.Database.BeginTransactionAsync();

            foreach (var file in files)
            {
                if (file == null || file.Length <= 0) continue;

                var ct = (file.ContentType ?? string.Empty).ToLowerInvariant();

                // Accept image/video/pdf by content-type OR by extension
                var isImage = ct.StartsWith("image/");
                var isVideo = ct.StartsWith("video/");
                var isPdf = ct == "application/pdf";

                var ext = Path.GetExtension(file.FileName);
                if (string.IsNullOrWhiteSpace(ext)) ext = "";

                var allowedByExt = !string.IsNullOrWhiteSpace(ext) && AllowedExtensions.Contains(ext);

                if (!(isImage || isVideo || isPdf || allowedByExt))
                {
                    return BadRequest(new
                    {
                        message = $"Invalid file type: {file.FileName}",
                        contentType = file.ContentType,
                        extension = ext
                    });
                }

                // If ext missing, pick a reasonable one
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

                var url = $"{baseUrl}/uploads/{tenantFolder}/workorders/{woFolder}/{name}";

                var doc = new Document
                {
                    UploadedBy = UserId,
                    FileUrl = url,

                    // Store full content-type so UI can render correctly (image/jpeg, video/mp4, application/pdf)
                    FileType = string.IsNullOrWhiteSpace(ct) ? "application/octet-stream" : ct,

                    DocKind = "work_order",
                    Status = "confirmed",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _db.Documents.Add(doc);
                createdDocs.Add(doc);
            }

            await _db.SaveChangesAsync();

            // Create links (DocumentLink has composite PK {TenantId, Id}, tenant is injected in SaveChanges)
            foreach (var doc in createdDocs)
            {
                _db.DocumentLinks.Add(new DocumentLink
                {
                    Id = Guid.NewGuid(),
                    DocumentId = doc.Id,
                    EntityType = "work_order",
                    EntityId = wo.Id,
                    CreatedAt = DateTime.UtcNow
                });
            }

            await _db.SaveChangesAsync();
            await tx.CommitAsync();

            var result = createdDocs
                .OrderByDescending(d => d.CreatedAt)
                .Select(d => new WorkOrderDocumentDto(
                    d.Id, d.FileUrl, d.FileType, d.DocKind, d.Status, d.ConfidenceScore, d.CreatedAt
                ))
                .ToList();

            return Ok(result);
        }

        // ----------------------------
        // DELETE: api/workorders/{id}/attachments/{documentId}
        // Removes the link and deletes the document record.
        // Also attempts to delete the file from disk when it matches our uploads folder structure.
        // ----------------------------
        [HttpDelete("{id:guid}/attachments/{documentId:guid}")]
        public async Task<IActionResult> DeleteAttachment(Guid id, Guid documentId)
        {
            var wo = await _db.WorkOrders.FirstOrDefaultAsync(x => x.Id == id);
            if (wo is null)
                return NotFound(new { message = "Work order not found." });

            var link = await _db.DocumentLinks
                .FirstOrDefaultAsync(x =>
                    x.EntityType == "work_order" &&
                    x.EntityId == id &&
                    x.DocumentId == documentId);

            if (link is null)
                return NotFound(new { message = "Attachment link not found." });

            var doc = await _db.Documents.FirstOrDefaultAsync(x => x.Id == documentId);

            using var tx = await _db.Database.BeginTransactionAsync();

            _db.DocumentLinks.Remove(link);

            if (doc is not null)
            {
                // Best-effort file delete (only if it looks like our uploads URL)
                TryDeleteLocalUploadFile(wo, doc.FileUrl);

                // Remove document row (will also cascade-delete links if configured, but we already removed link)
                _db.Documents.Remove(doc);
            }

            await _db.SaveChangesAsync();
            await tx.CommitAsync();

            return NoContent();
        }

        private void TryDeleteLocalUploadFile(WorkOrder wo, string fileUrl)
        {
            if (string.IsNullOrWhiteSpace(fileUrl)) return;

            // Ensure wwwroot exists
            var webRoot = _env.WebRootPath;
            if (string.IsNullOrWhiteSpace(webRoot))
                webRoot = Path.Combine(_env.ContentRootPath, "wwwroot");

            var tenantFolder = wo.TenantId.ToString("N");
            var woFolder = wo.Id.ToString("N");

            // We expect: /uploads/{tenant}/workorders/{wo}/{filename}
            // Parse the filename from URL
            var uriOk = Uri.TryCreate(fileUrl, UriKind.Absolute, out var uri);
            var pathPart = uriOk ? uri!.AbsolutePath : fileUrl;

            // Normalize slashes
            pathPart = pathPart.Replace("\\", "/");

            var prefix = $"/uploads/{tenantFolder}/workorders/{woFolder}/";
            if (!pathPart.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                return;

            var fileName = pathPart.Substring(prefix.Length);
            if (string.IsNullOrWhiteSpace(fileName))
                return;

            // Prevent traversal
            fileName = Path.GetFileName(fileName);

            var diskPath = Path.Combine(webRoot, "uploads", tenantFolder, "workorders", woFolder, fileName);
            if (System.IO.File.Exists(diskPath))
            {
                try { System.IO.File.Delete(diskPath); }
                catch { /* best effort */ }
            }
        }
    }
}
