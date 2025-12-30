using FleetManage.Api.Data;
using FleetManage.Api.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace FleetManage.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class WorkOrdersController : ControllerBase
    {
        private readonly AppDbContext _db;

        public WorkOrdersController(AppDbContext db)
        {
            _db = db;
        }

        // ----------------------------
        // Helpers
        // ----------------------------
        private static string NormalizeAssetType(string? assetType)
        {
            var at = (assetType ?? "").Trim().ToLowerInvariant();
            if (at is not ("truck" or "trailer"))
                throw new ArgumentException("AssetType must be 'truck' or 'trailer'.");
            return at;
        }

        private static string NormalizeStatus(string? status, string fallback = "open")
        {
            var s = string.IsNullOrWhiteSpace(status) ? fallback : status.Trim().ToLowerInvariant();
            return s is ("draft" or "open" or "closed" or "paid") ? s : fallback;
        }

        private static string NormalizeLineType(string? type)
        {
            var lt = string.IsNullOrWhiteSpace(type) ? "misc" : type.Trim().ToLowerInvariant();
            return lt is ("part" or "labor" or "fee" or "misc") ? lt : "misc";
        }

        private static decimal RoundMoney(decimal v) =>
            Math.Round(v, 2, MidpointRounding.AwayFromZero);

        private static void RecalcAmountsAndTotals(WorkOrder wo)
        {
            foreach (var l in wo.Lines)
            {
                if (l.Qty <= 0) l.Qty = 1;
                if (l.UnitPrice < 0) l.UnitPrice = 0;

                // Amount is derived
                l.Amount = RoundMoney(l.Qty * l.UnitPrice);
            }

            if (wo.TaxAmount < 0) wo.TaxAmount = 0;

            // TotalAmount includes tax (change if you prefer total==sum(lines) only)
            wo.TotalAmount = wo.Lines.Sum(x => x.Amount) + wo.TaxAmount;
        }

        // -------- ExtractedJson parsing helpers (for Document -> WorkOrder) --------

        private static string? JString(JsonElement el, string prop)
        {
            return el.TryGetProperty(prop, out var v) && v.ValueKind == JsonValueKind.String
                ? v.GetString()
                : null;
        }

        private static decimal? JDecimal(JsonElement el, string prop)
        {
            if (!el.TryGetProperty(prop, out var v)) return null;

            return v.ValueKind switch
            {
                JsonValueKind.Number => v.TryGetDecimal(out var d) ? d : null,
                JsonValueKind.String => decimal.TryParse(v.GetString(), out var d2) ? d2 : null,
                _ => null
            };
        }

        private static int? JInt(JsonElement el, string prop)
        {
            if (!el.TryGetProperty(prop, out var v)) return null;

            return v.ValueKind switch
            {
                JsonValueKind.Number => v.TryGetInt32(out var i) ? i : null,
                JsonValueKind.String => int.TryParse(v.GetString(), out var i2) ? i2 : null,
                _ => null
            };
        }

        private static DateTime? JDate(JsonElement el, string prop)
        {
            if (!el.TryGetProperty(prop, out var v)) return null;

            if (v.ValueKind == JsonValueKind.String && DateTime.TryParse(v.GetString(), out var dt))
                return dt;

            return null;
        }

        private static (DateTime serviceDate, int? mileage, string? invoiceNo, string? vendorName) ExtractHeader(JsonDocument extracted)
        {
            var root = extracted.RootElement;

            DateTime? date = null;
            string? invoiceNo = null;

            if (root.TryGetProperty("invoice", out var invoice) && invoice.ValueKind == JsonValueKind.Object)
            {
                date = JDate(invoice, "date");
                invoiceNo = JString(invoice, "number");
            }

            int? mileage = null;
            if (root.TryGetProperty("asset", out var asset) && asset.ValueKind == JsonValueKind.Object)
            {
                mileage = JInt(asset, "mileage");
            }

            string? vendorName = null;
            if (root.TryGetProperty("vendor", out var vendor) && vendor.ValueKind == JsonValueKind.Object)
            {
                vendorName = JString(vendor, "name");
            }

            return (date?.Date ?? DateTime.UtcNow.Date, mileage, invoiceNo, vendorName);
        }

        private static decimal ExtractTax(JsonDocument extracted)
        {
            var root = extracted.RootElement;

            if (!root.TryGetProperty("tax", out var t) || t.ValueKind == JsonValueKind.Null)
                return 0m;

            var tax = t.ValueKind switch
            {
                JsonValueKind.Number => t.TryGetDecimal(out var d) ? d : 0m,
                JsonValueKind.String => decimal.TryParse(t.GetString(), out var d2) ? d2 : 0m,
                _ => 0m
            };

            return tax < 0 ? 0 : tax;
        }

        private static List<WorkOrderLine> ParseLinesFromExtractedJson(JsonDocument extracted)
        {
            var root = extracted.RootElement;
            var lines = new List<WorkOrderLine>();

            // line_items
            if (root.TryGetProperty("line_items", out var lineItems) && lineItems.ValueKind == JsonValueKind.Array)
            {
                foreach (var li in lineItems.EnumerateArray())
                {
                    var desc = JString(li, "description")?.Trim();
                    if (string.IsNullOrWhiteSpace(desc)) continue;

                    var qty = JDecimal(li, "qty") ?? 1m;
                    var unit = JDecimal(li, "unit_price") ?? 0m;

                    var type = JString(li, "type")?.Trim().ToLowerInvariant() ?? "part";
                    if (type is not ("part" or "labor" or "fee" or "misc")) type = "part";

                    lines.Add(new WorkOrderLine
                    {
                        Type = type,
                        Description = desc,
                        Qty = qty <= 0 ? 1 : qty,
                        UnitPrice = unit < 0 ? 0 : unit,
                        PartNumber = JString(li, "part_number")
                    });
                }
            }

            // labor
            if (root.TryGetProperty("labor", out var labor) && labor.ValueKind == JsonValueKind.Array)
            {
                foreach (var li in labor.EnumerateArray())
                {
                    var desc = JString(li, "description")?.Trim();
                    if (string.IsNullOrWhiteSpace(desc)) continue;

                    var qty = JDecimal(li, "qty") ?? 1m;
                    var unit = JDecimal(li, "unit_price") ?? 0m;

                    lines.Add(new WorkOrderLine
                    {
                        Type = "labor",
                        Description = desc,
                        Qty = qty <= 0 ? 1 : qty,
                        UnitPrice = unit < 0 ? 0 : unit
                    });
                }
            }

            // fees
            if (root.TryGetProperty("fees", out var fees) && fees.ValueKind == JsonValueKind.Array)
            {
                foreach (var li in fees.EnumerateArray())
                {
                    var desc = JString(li, "description")?.Trim();
                    if (string.IsNullOrWhiteSpace(desc)) continue;

                    var qty = JDecimal(li, "qty") ?? 1m;
                    var unit = JDecimal(li, "unit_price") ?? 0m;

                    lines.Add(new WorkOrderLine
                    {
                        Type = "fee",
                        Description = desc,
                        Qty = qty <= 0 ? 1 : qty,
                        UnitPrice = unit < 0 ? 0 : unit
                    });
                }
            }

            return lines;
        }

        // ----------------------------
        // GET: api/workorders
        // Optional filters: ?assetType=truck&assetId=...
        // Pagination: ?page=1&pageSize=25
        // ----------------------------
        [HttpGet]
        public async Task<ActionResult<IEnumerable<WorkOrderDto>>> GetWorkOrders(
            [FromQuery] string? assetType,
            [FromQuery] Guid? assetId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 25)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 25;
            if (pageSize > 200) pageSize = 200;

            // ✅ Hide deleted (active = 0)
            // If you still have old rows with NULL, include them too:
            // .Where(w => w.DeletedStatus == 0 || w.DeletedStatus == null)
            var q = _db.WorkOrders.AsNoTracking()
                .Where(w => w.DeletedStatus == 0);

            if (!string.IsNullOrWhiteSpace(assetType))
            {
                var at = assetType.Trim().ToLowerInvariant();
                if (at is not ("truck" or "trailer"))
                    return BadRequest(new { message = "assetType must be 'truck' or 'trailer'." });

                q = q.Where(w => w.AssetType == at);
            }

            if (assetId.HasValue)
                q = q.Where(w => w.AssetId == assetId.Value);

            var pageWorkOrders = await q
                .OrderByDescending(w => w.ServiceDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var woIds = pageWorkOrders.Select(w => w.Id).ToList();

            var lines = await _db.WorkOrderLines.AsNoTracking()
                .Where(l => woIds.Contains(l.WorkOrderId))
                .ToListAsync();

            var docs = await (
                from lnk in _db.DocumentLinks.AsNoTracking()
                join d in _db.Documents.AsNoTracking() on lnk.DocumentId equals d.Id
                where lnk.EntityType == "work_order" && woIds.Contains(lnk.EntityId)
                select new { lnk.EntityId, Doc = d }
            ).ToListAsync();

            var lineMap = lines
                .GroupBy(x => x.WorkOrderId)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(l => new WorkOrderLineDto(
                        l.Id, l.Type, l.Description, l.Qty, l.UnitPrice, l.Amount, l.PartNumber
                    )).ToList()
                );

            var docMap = docs
                .GroupBy(x => x.EntityId)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(x => new WorkOrderDocumentDto(
                        x.Doc.Id,
                        x.Doc.FileUrl,
                        x.Doc.FileType,
                        x.Doc.DocKind,
                        x.Doc.Status,
                        x.Doc.ConfidenceScore,
                        x.Doc.CreatedAt
                    )).ToList()
                );

            var result = pageWorkOrders.Select(w => new WorkOrderDto(
                w.Id,
                w.DeletedStatus,
                w.DeletedAt,
                w.AssetType,
                w.AssetId,
                w.VendorId,
                w.WoNumber,
                w.Odometer,
                w.ServiceDate,
                w.Summary,
                w.TotalAmount,
                w.TaxAmount,
                w.Status,
                lineMap.TryGetValue(w.Id, out var ls) ? ls : new List<WorkOrderLineDto>(),
                docMap.TryGetValue(w.Id, out var ds) ? ds : new List<WorkOrderDocumentDto>()
            )).ToList();

            return Ok(result);
        }

        // ----------------------------
        // GET: api/workorders/{id}
        // ----------------------------
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<WorkOrderDto>> GetWorkOrder(Guid id)
        {
            // ✅ Hide deleted (active = 0)
            var w = await _db.WorkOrders.AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id && x.DeletedStatus == 0);

            if (w is null) return NotFound();

            var lines = await _db.WorkOrderLines.AsNoTracking()
                .Where(l => l.WorkOrderId == id)
                .Select(l => new WorkOrderLineDto(l.Id, l.Type, l.Description, l.Qty, l.UnitPrice, l.Amount, l.PartNumber))
                .ToListAsync();

            var documents = await (
                from lnk in _db.DocumentLinks.AsNoTracking()
                join d in _db.Documents.AsNoTracking() on lnk.DocumentId equals d.Id
                where lnk.EntityType == "work_order" && lnk.EntityId == id
                orderby d.CreatedAt descending
                select new WorkOrderDocumentDto(
                    d.Id, d.FileUrl, d.FileType, d.DocKind, d.Status, d.ConfidenceScore, d.CreatedAt
                )
            ).ToListAsync();

            return Ok(new WorkOrderDto(
                w.Id,
                w.DeletedStatus,
                w.DeletedAt,
                w.AssetType,
                w.AssetId,
                w.VendorId,
                w.WoNumber,
                w.Odometer,
                w.ServiceDate,
                w.Summary,
                w.TotalAmount,
                w.TaxAmount,
                w.Status,
                lines,
                documents
            ));
        }

        // ----------------------------
        // POST: api/workorders
        // ----------------------------
        [HttpPost]
        public async Task<ActionResult<WorkOrderDto>> CreateWorkOrder([FromBody] CreateWorkOrderDto dto)
        {
            if (!ModelState.IsValid)
                return ValidationProblem(ModelState);

            string at;
            try { at = NormalizeAssetType(dto.AssetType); }
            catch (ArgumentException ex) { return BadRequest(new { message = ex.Message }); }

            var status = NormalizeStatus(dto.Status, fallback: "open");

            // Validate docs BEFORE starting transaction / creating WO (avoid partial writes)
            List<Document>? docs = null;
            if (dto.ReplaceDocuments && dto.DocumentIds is { Count: > 0 })
            {
                docs = await _db.Documents
                    .Where(d => dto.DocumentIds.Contains(d.Id))
                    .ToListAsync();

                if (docs.Count != dto.DocumentIds.Count)
                    return BadRequest(new { message = "One or more DocumentIds are invalid." });
            }

            using var tx = await _db.Database.BeginTransactionAsync();

            var wo = new WorkOrder
            {
                AssetType = at,
                AssetId = dto.AssetId,
                VendorId = dto.VendorId,
                WoNumber = string.IsNullOrWhiteSpace(dto.WoNumber) ? null : dto.WoNumber.Trim(),
                Odometer = dto.Odometer,
                ServiceDate = dto.ServiceDate.Date,
                Summary = string.IsNullOrWhiteSpace(dto.Summary) ? null : dto.Summary.Trim(),
                TaxAmount = dto.TaxAmount < 0 ? 0 : dto.TaxAmount,
                Status = status,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,

                // ✅ Ensure active
                DeletedStatus = 0,
                DeletedAt = null
            };

            foreach (var l in dto.Lines ?? new())
            {
                wo.Lines.Add(new WorkOrderLine
                {
                    Type = NormalizeLineType(l.Type),
                    Description = l.Description.Trim(),
                    Qty = l.Qty <= 0 ? 1 : l.Qty,
                    UnitPrice = l.UnitPrice,
                    PartNumber = string.IsNullOrWhiteSpace(l.PartNumber) ? null : l.PartNumber.Trim()
                });
            }

            RecalcAmountsAndTotals(wo);

            _db.WorkOrders.Add(wo);
            await _db.SaveChangesAsync();

            // Attach docs (if any)
            if (docs is not null && docs.Count > 0)
            {
                foreach (var d in docs)
                {
                    _db.DocumentLinks.Add(new DocumentLink
                    {
                        Id = Guid.NewGuid(),
                        DocumentId = d.Id,
                        EntityType = "work_order",
                        EntityId = wo.Id,
                        CreatedAt = DateTime.UtcNow
                    });

                    d.Status = "confirmed";
                    d.UpdatedAt = DateTime.UtcNow;
                }

                await _db.SaveChangesAsync();
            }

            await tx.CommitAsync();

            // Return created resource
            var created = await GetWorkOrder(wo.Id);
            return CreatedAtAction(nameof(GetWorkOrder), new { id = wo.Id }, created.Value);
        }

        // ----------------------------
        // POST: api/workorders/from-document/{documentId}
        // Create WO + lines from Document.ExtractedJson and link the document
        // ----------------------------
        [HttpPost("from-document/{documentId:guid}")]
        public async Task<ActionResult<WorkOrderDto>> CreateFromDocument(
            Guid documentId,
            [FromBody] CreateWorkOrderFromDocumentDTO dto)
        {
            if (!ModelState.IsValid)
                return ValidationProblem(ModelState);

            string at;
            try { at = NormalizeAssetType(dto.AssetType); }
            catch (ArgumentException ex) { return BadRequest(new { message = ex.Message }); }

            var doc = await _db.Documents.FirstOrDefaultAsync(d => d.Id == documentId);
            if (doc is null)
                return NotFound(new { message = "Document not found." });

            if (doc.ExtractedJson is null)
                return BadRequest(new { message = "Document has no extracted data. Run extraction first." });

            var (serviceDateFromDoc, mileageFromDoc, invoiceNo, vendorName) = ExtractHeader(doc.ExtractedJson);

            var wo = new WorkOrder
            {
                AssetType = at,
                AssetId = dto.AssetId,
                VendorId = dto.VendorId,
                WoNumber = string.IsNullOrWhiteSpace(invoiceNo) ? null : invoiceNo.Trim(),
                Odometer = dto.Odometer ?? mileageFromDoc,
                ServiceDate = (dto.ServiceDate ?? serviceDateFromDoc).Date,
                Summary = dto.Summary ?? (string.IsNullOrWhiteSpace(vendorName) ? "Created from document" : $"Created from {vendorName}"),
                TaxAmount = ExtractTax(doc.ExtractedJson),
                Status = "open",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,

                // ✅ Ensure active
                DeletedStatus = 0,
                DeletedAt = null
            };

            // Parse lines from extracted JSON
            var lines = ParseLinesFromExtractedJson(doc.ExtractedJson);
            foreach (var l in lines)
                wo.Lines.Add(l);

            RecalcAmountsAndTotals(wo);

            using var tx = await _db.Database.BeginTransactionAsync();

            _db.WorkOrders.Add(wo);
            await _db.SaveChangesAsync();

            // Link doc -> work order
            _db.DocumentLinks.Add(new DocumentLink
            {
                Id = Guid.NewGuid(),
                DocumentId = doc.Id,
                EntityType = "work_order",
                EntityId = wo.Id,
                CreatedAt = DateTime.UtcNow
            });

            // Document status handling
            if (dto.ConfirmDocument)
                doc.Status = "confirmed";
            else if (doc.Status is "uploaded" or "extracting")
                doc.Status = "needs_review";

            doc.UpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();
            await tx.CommitAsync();

            var created = await GetWorkOrder(wo.Id);
            return CreatedAtAction(nameof(GetWorkOrder), new { id = wo.Id }, created.Value);
        }

        // ----------------------------
        // PUT: api/workorders/{id}
        // ----------------------------
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateWorkOrder(Guid id, [FromBody] UpdateWorkOrderDto dto)
        {
            if (!ModelState.IsValid)
                return ValidationProblem(ModelState);

            // ✅ Only allow updating active ones (active = 0)
            var wo = await _db.WorkOrders
                .Include(x => x.Lines)
                .FirstOrDefaultAsync(x => x.Id == id && x.DeletedStatus == 0);

            if (wo is null)
                return NotFound();

            string at;
            try { at = NormalizeAssetType(dto.AssetType); }
            catch (ArgumentException ex) { return BadRequest(new { message = ex.Message }); }

            var status = NormalizeStatus(dto.Status, fallback: wo.Status);

            wo.AssetType = at;
            wo.AssetId = dto.AssetId;
            wo.VendorId = dto.VendorId;
            wo.WoNumber = string.IsNullOrWhiteSpace(dto.WoNumber) ? null : dto.WoNumber.Trim();
            wo.Odometer = dto.Odometer;
            wo.ServiceDate = dto.ServiceDate.Date;
            wo.Summary = string.IsNullOrWhiteSpace(dto.Summary) ? null : dto.Summary.Trim();
            wo.TaxAmount = dto.TaxAmount < 0 ? 0 : dto.TaxAmount;
            wo.Status = status;
            wo.UpdatedAt = DateTime.UtcNow;

            _db.WorkOrderLines.RemoveRange(wo.Lines);
            wo.Lines.Clear();

            foreach (var l in dto.Lines ?? new())
            {
                wo.Lines.Add(new WorkOrderLine
                {
                    Type = NormalizeLineType(l.Type),
                    Description = l.Description.Trim(),
                    Qty = l.Qty <= 0 ? 1 : l.Qty,
                    UnitPrice = l.UnitPrice,
                    PartNumber = string.IsNullOrWhiteSpace(l.PartNumber) ? null : l.PartNumber.Trim()
                });
            }

            RecalcAmountsAndTotals(wo);

            if (dto.ReplaceDocuments && dto.DocumentIds is not null)
            {
                List<Document> docs = new();
                if (dto.DocumentIds.Count > 0)
                {
                    docs = await _db.Documents
                        .Where(d => dto.DocumentIds.Contains(d.Id))
                        .ToListAsync();

                    if (docs.Count != dto.DocumentIds.Count)
                        return BadRequest(new { message = "One or more DocumentIds are invalid." });
                }

                var existing = await _db.DocumentLinks
                    .Where(x => x.EntityType == "work_order" && x.EntityId == wo.Id)
                    .ToListAsync();

                _db.DocumentLinks.RemoveRange(existing);

                foreach (var d in docs)
                {
                    _db.DocumentLinks.Add(new DocumentLink
                    {
                        Id = Guid.NewGuid(),
                        DocumentId = d.Id,
                        EntityType = "work_order",
                        EntityId = wo.Id,
                        CreatedAt = DateTime.UtcNow
                    });

                    d.Status = "confirmed";
                    d.UpdatedAt = DateTime.UtcNow;
                }
            }

            await _db.SaveChangesAsync();
            return NoContent();
        }

        // ----------------------------
        // DELETE: api/workorders/{id}
        // Soft delete
        // ----------------------------
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var wo = await _db.WorkOrders
                .FirstOrDefaultAsync(x => x.Id == id && x.DeletedStatus == 0);

            if (wo is null)
                return NotFound();

            wo.DeletedStatus = 1;
            wo.DeletedAt = DateTime.UtcNow;
            wo.UpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();
            return NoContent();
        }

        public sealed record WorkOrdersBulkDeleteRequest(List<Guid> WorkOrderIds);

        // ----------------------------
        // POST: api/workorders/bulk-delete
        // Soft delete in bulk
        // ----------------------------
        [HttpPost("bulk-delete")]
        public async Task<IActionResult> BulkDelete([FromBody] WorkOrdersBulkDeleteRequest request)
        {
            if (request.WorkOrderIds == null || request.WorkOrderIds.Count == 0)
                return BadRequest(new { message = "No work order IDs provided." });

            var workOrders = await _db.WorkOrders
                .Where(w => request.WorkOrderIds.Contains(w.Id) && w.DeletedStatus == 0)
                .ToListAsync();

            if (workOrders.Count == 0)
                return NotFound(new { message = "No work orders found for provided IDs." });

            foreach (var wo in workOrders)
            {
                wo.DeletedStatus = 1;
                wo.DeletedAt = DateTime.UtcNow;
                wo.UpdatedAt = DateTime.UtcNow;
            }

            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}
