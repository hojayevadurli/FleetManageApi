using FleetManage.Api.Data;
using FleetManage.Api.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text.Json;

namespace FleetManage.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DocumentsController : ControllerBase
    {
        private readonly AppDbContext _db;

        public DocumentsController(AppDbContext db)
        {
            _db = db;
        }

        private Guid UserId =>
            Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        // ----------------------------
        // GET: api/documents
        // Optional filters: ?assetType=truck&assetId=...
        // Pagination: ?page=1&pageSize=25
        // ----------------------------
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DocumentDto>>> GetDocuments(
            [FromQuery] string? assetType,
            [FromQuery] Guid? assetId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 25)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 25;
            if (pageSize > 200) pageSize = 200;

            IQueryable<Document> q = _db.Documents.AsNoTracking();

            // If asset filter provided, return docs linked to that asset
            if (!string.IsNullOrWhiteSpace(assetType) && assetId.HasValue)
            {
                var at = assetType.Trim().ToLowerInvariant();
                if (at is not ("truck" or "trailer"))
                    return BadRequest(new { message = "assetType must be 'truck' or 'trailer'." });

                // Linked docs only (EntityType='asset' & EntityId=assetId)
                q =
                    from d in _db.Documents.AsNoTracking()
                    join l in _db.DocumentLinks.AsNoTracking() on d.Id equals l.DocumentId
                    where l.EntityType == "asset" && l.EntityId == assetId.Value
                    select d;
            }

            var docs = await q
                .OrderByDescending(d => d.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(d => new DocumentDto(
                    d.Id,
                    d.FileUrl,
                    d.FileType,
                    d.DocKind,
                    d.VendorNameRaw,
                    d.Status,
                    d.ExtractedJson,
                    d.ConfidenceScore,
                    d.CreatedAt
                ))
                .ToListAsync();

            return Ok(docs);
        }

        // ----------------------------
        // GET: api/documents/{id}
        // ----------------------------
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<DocumentDto>> GetDocument(Guid id)
        {
            var d = await _db.Documents
                .AsNoTracking()
                .Where(x => x.Id == id)
                .Select(x => new DocumentDto(
                    x.Id,
                    x.FileUrl,
                    x.FileType,
                    x.DocKind,
                    x.VendorNameRaw,
                    x.Status,
                    x.ExtractedJson,
                    x.ConfidenceScore,
                    x.CreatedAt
                ))
                .FirstOrDefaultAsync();

            if (d is null)
                return NotFound();

            return Ok(d);
        }

        // ----------------------------
        // POST: api/documents
        // v1: client provides FileUrl (already uploaded to storage)
        // Optional: link to asset
        // Optional: run extract immediately
        // ----------------------------
        [HttpPost]
        public async Task<ActionResult<DocumentDto>> CreateDocument([FromBody] CreateDocumentDto dto)
        {
            if (!ModelState.IsValid)
                return ValidationProblem(ModelState);

            var fileType = dto.FileType.Trim().ToLowerInvariant();
            var docKind = string.IsNullOrWhiteSpace(dto.DocKind) ? "unknown" : dto.DocKind.Trim().ToLowerInvariant();

            if (docKind is not ("invoice" or "receipt" or "work_order" or "unknown"))
                docKind = "unknown";

            // Validate asset link early (avoid partial create+link)
            string? assetTypeNormalized = null;
            if (!string.IsNullOrWhiteSpace(dto.AssetType) && dto.AssetId.HasValue)
            {
                var at = dto.AssetType.Trim().ToLowerInvariant();
                if (at is not ("truck" or "trailer"))
                    return BadRequest(new { message = "AssetType must be 'truck' or 'trailer'." });
                assetTypeNormalized = at;
            }

            using var tx = await _db.Database.BeginTransactionAsync();

            var doc = new Document
            {
                UploadedBy = UserId,
                FileUrl = dto.FileUrl.Trim(),
                FileType = fileType,
                DocKind = docKind,
                VendorNameRaw = string.IsNullOrWhiteSpace(dto.VendorNameRaw) ? null : dto.VendorNameRaw.Trim(),
                Status = "uploaded",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _db.Documents.Add(doc);
            await _db.SaveChangesAsync(); // TenantId injected by DbContext

            // Optional: link to asset immediately
            if (assetTypeNormalized is not null && dto.AssetId.HasValue)
            {
                _db.DocumentLinks.Add(new DocumentLink
                {
                    Id = Guid.NewGuid(),          // UPDATED: DocumentLink now has Id
                    DocumentId = doc.Id,
                    EntityType = "asset",
                    EntityId = dto.AssetId.Value,
                    CreatedAt = DateTime.UtcNow
                });

                await _db.SaveChangesAsync();
            }

            // Optional: run extract immediately (v1 stub)
            if (dto.RunAiExtract)
            {
                await ExtractInternal(doc.Id);
            }

            await tx.CommitAsync();

            // return latest state
            var result = await _db.Documents
                .AsNoTracking()
                .Where(x => x.Id == doc.Id)
                .Select(x => new DocumentDto(
                    x.Id,
                    x.FileUrl,
                    x.FileType,
                    x.DocKind,
                    x.VendorNameRaw,
                    x.Status,
                    x.ExtractedJson,
                    x.ConfidenceScore,
                    x.CreatedAt
                ))
                .FirstAsync();

            return CreatedAtAction(nameof(GetDocument), new { id = doc.Id }, result);
        }

        // ----------------------------
        // POST: api/documents/{id}/extract
        // ----------------------------
        [HttpPost("{id:guid}/extract")]
        public async Task<IActionResult> Extract(Guid id)
        {
            var doc = await _db.Documents.FirstOrDefaultAsync(x => x.Id == id);
            if (doc is null)
                return NotFound();

            await ExtractInternal(id);

            // Reload updated document state
            var updated = await _db.Documents.AsNoTracking()
                .Where(x => x.Id == id)
                .Select(x => new
                {
                    x.Id,
                    x.Status,
                    x.ConfidenceScore
                })
                .FirstAsync();

            return Ok(new
            {
                documentId = updated.Id,
                status = updated.Status,
                confidence = updated.ConfidenceScore
            });
        }


        // ----------------------------
        // Internal extraction (stub for now)
        // Replace later with real AI extraction
        // ----------------------------
        private async Task ExtractInternal(Guid id)
        {
            var doc = await _db.Documents.FirstAsync(x => x.Id == id);

            doc.Status = "extracting";
            doc.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();

            // v1 stub extraction output: lets UI build now; swap later with real AI
            doc.Status = "needs_review";
            doc.ExtractedJson = JsonDocument.Parse("""
            {
              "doc_kind":"unknown",
              "vendor":{"name":null,"phone":null,"address":null},
              "invoice":{"number":null,"date":null,"due_date":null,"terms":null},
              "customer":{"name":null},
              "asset":{"truck_number":null,"vin":null,"mileage":null},
              "line_items":[],
              "labor":[],
              "fees":[],
              "subtotal":null,"tax":null,"total":null,"balance_due":null,
              "notes":null,
              "confidence":{"overall":0.0}
            }
            """);
            doc.ConfidenceScore = 0.0m;
            doc.UpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();
        }

        // ----------------------------
        // OPTIONAL: delete a document
        // ----------------------------
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var doc = await _db.Documents.FirstOrDefaultAsync(x => x.Id == id);
            if (doc is null)
                return NotFound();

            _db.Documents.Remove(doc);
            await _db.SaveChangesAsync();
            return NoContent();
        }

        public sealed record BulkDeleteRequest(List<Guid> DocumentIds);

        // ----------------------------
        // POST: api/documents/bulk-delete
        // ----------------------------
        [HttpPost("bulk-delete")]
        public async Task<IActionResult> BulkDelete([FromBody] BulkDeleteRequest request)
        {
            if (request.DocumentIds == null || request.DocumentIds.Count == 0)
                return BadRequest(new { message = "No document IDs provided." });

            var docs = await _db.Documents
                .Where(d => request.DocumentIds.Contains(d.Id))
                .ToListAsync();

            if (docs.Count == 0)
                return NotFound(new { message = "No documents found for provided IDs." });

            _db.Documents.RemoveRange(docs);
            await _db.SaveChangesAsync();

            return NoContent();
        }

        [HttpPut("{id:guid}/extracted")]
        public async Task<IActionResult> UpdateExtracted(Guid id, [FromBody] UpdateDocumentExtractDto dto)
        {
            var doc = await _db.Documents.FirstOrDefaultAsync(x => x.Id == id);
            if (doc is null) return NotFound();

            doc.ExtractedJson = dto.ExtractedJson;
            doc.VendorNameRaw = string.IsNullOrWhiteSpace(dto.VendorNameRaw) ? doc.VendorNameRaw : dto.VendorNameRaw.Trim();
            doc.ConfidenceScore = dto.ConfidenceScore ?? doc.ConfidenceScore;

            if (!string.IsNullOrWhiteSpace(dto.Status))
                doc.Status = dto.Status.Trim().ToLowerInvariant();

            doc.UpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();
            return Ok(new { id = doc.Id, status = doc.Status, confidence = doc.ConfidenceScore });
        }

    }
}
