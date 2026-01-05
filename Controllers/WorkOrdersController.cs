using FleetManage.Api.Data;
using FleetManage.Api.Data.Enums;
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

        private static string NormalizeLineType(string? type)
        {
            var lt = string.IsNullOrWhiteSpace(type) ? "misc" : type.Trim().ToLowerInvariant();
            return lt is ("part" or "labor" or "fee" or "misc") ? lt : "misc";
        }

        private static decimal RoundMoney(decimal v) =>
            Math.Round(v, 2, MidpointRounding.AwayFromZero);

        private static void RecalcAmounts(WorkOrder wo)
        {
            // If ManualActualTotal is set, use it? Or calculate from lines?
            // Usually if cost source is Invoiced or Manual, we trust the manual total.
            // But let's sum lines for internal consistency if lines exist.
            
            decimal sumLines = 0;
            if (wo.LineItems != null)
            {
                foreach (var l in wo.LineItems)
                {
                    if (l.Qty <= 0) l.Qty = 1;
                    if (l.UnitPrice < 0) l.UnitPrice = 0;
                    l.Amount = RoundMoney(l.Qty * l.UnitPrice);
                    sumLines += l.Amount;
                }
            }

            // Only update ManualActualTotal if CostSource is not Manual/Invoiced? 
            // Or maybe just update EstimatedTotal?
            // User logic: "ManualActualTotal" implies manual override.
            // But if lines are provided, they sum up to something.
            // Let's assume if CostSource == Estimated, we update EstimatedTotal.
            
            if (wo.CostSource == WorkOrderCostSource.Estimated)
            {
                wo.EstimatedTotal = sumLines;
            }
        }

        // -------- ExtractedJson parsing helpers (for Document -> WorkOrder) --------
        // (Simplified for brevity, assuming existing helpers logic)

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

        private static List<WorkOrderLineItem> ParseLinesFromExtractedJson(JsonDocument extracted)
        {
            var root = extracted.RootElement;
            var lines = new List<WorkOrderLineItem>();

            void ExtractLines(string propName, string defaultType)
            {
                if (root.TryGetProperty(propName, out var arr) && arr.ValueKind == JsonValueKind.Array)
                {
                    foreach (var li in arr.EnumerateArray())
                    {
                        var desc = JString(li, "description")?.Trim();
                        if (string.IsNullOrWhiteSpace(desc)) continue;

                        var qty = JDecimal(li, "qty") ?? 1m;
                        var unit = JDecimal(li, "unit_price") ?? 0m;
                        var type = JString(li, "type")?.Trim().ToLowerInvariant() ?? defaultType;
                        
                        // Normalize type
                        type = type is ("part" or "labor" or "fee" or "misc") ? type : "misc";

                        lines.Add(new WorkOrderLineItem
                        {
                            Type = type,
                            Description = desc,
                            Qty = qty <= 0 ? 1 : qty,
                            UnitPrice = unit < 0 ? 0 : unit,
                            PartNumber = JString(li, "part_number")
                        });
                    }
                }
            }

            ExtractLines("line_items", "part");
            ExtractLines("labor", "labor");
            ExtractLines("fees", "fee");

            return lines;
        }

        // ----------------------------
        // GET: api/workorders
        // ----------------------------
        [HttpGet]
        public async Task<ActionResult<IEnumerable<WorkOrderDto>>> GetWorkOrders(
            [FromQuery] Guid? equipmentId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 25)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 25;
            if (pageSize > 200) pageSize = 200;

            var q = _db.WorkOrders.AsNoTracking()
                .Where(w => !w.IsDeleted);

            if (equipmentId.HasValue)
                q = q.Where(w => w.EquipmentId == equipmentId.Value);

            var pageWorkOrders = await q
                .OrderByDescending(w => w.OpenedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var woIds = pageWorkOrders.Select(w => w.Id).ToList();

            var lines = await _db.WorkOrderLineItems.AsNoTracking()
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
                w.IsDeleted,
                w.DeletedAt,
                w.EquipmentId,
                w.VendorId,
                w.WorkOrderNumber,
                w.OdometerAtService,
                w.OpenedAt,
                w.ClosedAt,
                w.Title,
                w.Complaint,
                w.Diagnosis,
                w.Resolution,
                w.Notes,
                w.EstimatedTotal,
                w.ManualActualTotal,
                w.Status.ToString(),
                w.Priority.ToString(),
                w.CostSource.ToString(),
                w.InvoiceNumber,
                w.InvoiceDate,
                w.TaxAmount,
                w.Category,
                w.VendorNameRaw,
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
            var w = await _db.WorkOrders.AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);

            if (w is null) return NotFound();

            var lines = await _db.WorkOrderLineItems.AsNoTracking()
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
                w.IsDeleted,
                w.DeletedAt,
                w.EquipmentId,
                w.VendorId,
                w.WorkOrderNumber,
                w.OdometerAtService,
                w.OpenedAt,
                w.ClosedAt,
                w.Title,
                w.Complaint,
                w.Diagnosis,
                w.Resolution,
                w.Notes,
                w.EstimatedTotal,
                w.ManualActualTotal,
                w.Status.ToString(),
                w.Priority.ToString(),
                w.CostSource.ToString(),
                w.InvoiceNumber,
                w.InvoiceDate,
                w.TaxAmount,
                w.Category,
                w.VendorNameRaw,
                lines,
                documents
            ));
        }

        // ----------------------------
        // POST: api/workorders
        // ----------------------------
        // ----------------------------
        // POST: api/workorders
        // ----------------------------
        [HttpPost]
        public async Task<ActionResult<WorkOrderDto>> CreateWorkOrder([FromBody] CreateWorkOrderDto dto)
        {
            if (!ModelState.IsValid)
                return ValidationProblem(ModelState);

            List<Document>? docs = null;
            if (dto.ReplaceDocuments && dto.DocumentIds is { Count: > 0 })
            {
                docs = await _db.Documents
                    .Where(d => dto.DocumentIds.Contains(d.Id))
                    .ToListAsync();
                if (docs.Count != dto.DocumentIds.Count)
                    return BadRequest(new { message = "One or more DocumentIds are invalid." });
            }

            // Vendor Logic 
            Guid? finalVendorId = dto.VendorId;
            if (finalVendorId is null && !string.IsNullOrWhiteSpace(dto.VendorNameRaw))
            {
                finalVendorId = await GetOrCreateVendorId(dto.VendorNameRaw);
            }

            using var tx = await _db.Database.BeginTransactionAsync();

            var wo = new WorkOrder
            {
                EquipmentId = dto.EquipmentId,
                VendorId = finalVendorId,
                WorkOrderNumber = string.IsNullOrWhiteSpace(dto.WorkOrderNumber) ? null : dto.WorkOrderNumber.Trim(),
                OdometerAtService = dto.OdometerAtService,
                OpenedAt = dto.OpenedAt,
                Title = dto.Title,
                Complaint = dto.Complaint,
                Status = dto.Status,
                Priority = dto.Priority,
                CostSource = dto.CostSource,
                EstimatedTotal = dto.EstimatedTotal,
                ManualActualTotal = dto.ManualActualTotal,
                
                // New Fields
                InvoiceNumber = dto.InvoiceNumber,
                InvoiceDate = dto.InvoiceDate,
                TaxAmount = dto.TaxAmount,
                Category = dto.Category,
                VendorNameRaw = dto.VendorNameRaw,

                IsDeleted = false
            };

            foreach (var l in dto.Lines)
            {
                wo.LineItems.Add(new WorkOrderLineItem
                {
                    Type = NormalizeLineType(l.Type),
                    Description = l.Description.Trim(),
                    Qty = l.Qty <= 0 ? 1 : l.Qty,
                    UnitPrice = l.UnitPrice,
                    PartNumber = string.IsNullOrWhiteSpace(l.PartNumber) ? null : l.PartNumber.Trim()
                });
            }

            RecalcAmounts(wo);

            _db.WorkOrders.Add(wo);
            await _db.SaveChangesAsync();

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

            var created = await GetWorkOrder(wo.Id);
            return CreatedAtAction(nameof(GetWorkOrder), new { id = wo.Id }, created.Value);
        }

        private async Task<Guid?> GetOrCreateVendorId(string rawName)
        {
            var cleanName = rawName.Trim();
            // Try to find existing shop by name (case-insensitive)
            // Note: _db.ServicePartners is filtered by Tenant via Global Query Filter
            var existingShop = await _db.ServicePartners
                .FirstOrDefaultAsync(s => s.Name.ToLower() == cleanName.ToLower());

            if (existingShop != null)
            {
                return existingShop.Id;
            }

            // AUTO-CREATE NEW SHOP
            try
            {
                var newShop = new ServicePartner
                {
                    // TenantId is injected by AppDbContext.SaveChanges()
                    Name = cleanName,
                    Address1 = "Unknown Address - Auto Created",
                    City = "Unknown",
                    State = "Unknown",
                    PostalCode = "00000",
                    Country = "USA",
                    NetworkTier = ServicePartnerTier.Standard,
                    PricingStrategy = "$$",
                    IsActive = true,
                    Notes = "Auto-created from Work Order",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                _db.ServicePartners.Add(newShop);
                await _db.SaveChangesAsync();

                return newShop.Id;
            }
            catch (Exception)
            {
                // If shop creation fails (constraint, etc), ignore and return null
                return null;
            }
        }


        // ----------------------------
        // POST: api/workorders/from-document/{documentId}
        // ----------------------------
        [HttpPost("from-document/{documentId:guid}")]
        public async Task<ActionResult<WorkOrderDto>> CreateFromDocument(
            Guid documentId,
            [FromBody] CreateWorkOrderFromDocumentDTO dto)
        {
            if (!ModelState.IsValid)
                return ValidationProblem(ModelState);

            var doc = await _db.Documents.FirstOrDefaultAsync(d => d.Id == documentId);
            if (doc is null) return NotFound(new { message = "Document not found." });
            if (doc.ExtractedJson is null) return BadRequest(new { message = "Document has no extracted data." });

            var (serviceDate, mileage, invoiceNo, vendorName) = ExtractHeader(doc.ExtractedJson);

            // Vendor Logic
            Guid? finalVendorId = dto.VendorId;
            var finalVendorName = dto.VendorNameRaw ?? vendorName;

            if (finalVendorId is null && !string.IsNullOrWhiteSpace(finalVendorName))
            {
                finalVendorId = await GetOrCreateVendorId(finalVendorName);
            }

            var wo = new WorkOrder
            {
                EquipmentId = dto.EquipmentId,
                VendorId = finalVendorId,
                WorkOrderNumber = !string.IsNullOrWhiteSpace(dto.InvoiceNumber) ? dto.InvoiceNumber.Trim() : invoiceNo,
                OdometerAtService = dto.Odometer ?? mileage,
                OpenedAt = (dto.ServiceDate ?? serviceDate).ToUniversalTime(),
                Title = dto.Summary ?? (string.IsNullOrWhiteSpace(finalVendorName) ? "Imported from Document" : $"Service from {finalVendorName}"),
                Complaint = "Imported from document",
                Status = WorkOrderStatus.Open,
                CostSource = WorkOrderCostSource.Estimated,

                // New Fields
                InvoiceNumber = !string.IsNullOrWhiteSpace(dto.InvoiceNumber) ? dto.InvoiceNumber.Trim() : invoiceNo,
                InvoiceDate = (dto.ServiceDate ?? serviceDate).ToUniversalTime().Date,
                TaxAmount = dto.TaxAmount ?? 0,
                Category = dto.Category ?? "maintenance",
                VendorNameRaw = finalVendorName,

                IsDeleted = false
            };

            var lines = ParseLinesFromExtractedJson(doc.ExtractedJson);
            foreach (var l in lines) wo.LineItems.Add(l);

            RecalcAmounts(wo);

            using var tx = await _db.Database.BeginTransactionAsync();
            _db.WorkOrders.Add(wo);
            await _db.SaveChangesAsync();

            _db.DocumentLinks.Add(new DocumentLink
            {
                Id = Guid.NewGuid(),
                DocumentId = doc.Id,
                EntityType = "work_order",
                EntityId = wo.Id,
                CreatedAt = DateTime.UtcNow
            });

            if (dto.ConfirmDocument) doc.Status = "confirmed";
            else if (doc.Status is "uploaded" or "extracting") doc.Status = "needs_review";
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
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            var wo = await _db.WorkOrders
                .Include(x => x.LineItems)
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);

            if (wo is null) return NotFound();

            // Vendor Logic 
            Guid? finalVendorId = dto.VendorId;
            if (finalVendorId is null && !string.IsNullOrWhiteSpace(dto.VendorNameRaw))
            {
                // Only try to auto-create if vendor changed or was null?
                // Or simply always try to resolve if name provided and ID null
                finalVendorId = await GetOrCreateVendorId(dto.VendorNameRaw);
            }
            wo.VendorId = finalVendorId;

            wo.EquipmentId = dto.EquipmentId;
            wo.WorkOrderNumber = dto.WorkOrderNumber;
            wo.OdometerAtService = dto.OdometerAtService;
            wo.HoursAtService = dto.HoursAtService;
            wo.OpenedAt = dto.OpenedAt;
            wo.ClosedAt = dto.ClosedAt;
            wo.Title = dto.Title;
            wo.Complaint = dto.Complaint;
            wo.Diagnosis = dto.Diagnosis;
            wo.Resolution = dto.Resolution;
            wo.Notes = dto.Notes;
            wo.Status = dto.Status;
            wo.Priority = dto.Priority;
            wo.CostSource = dto.CostSource;
            wo.EstimatedTotal = dto.EstimatedTotal;
            wo.ManualActualTotal = dto.ManualActualTotal;

            // New Fields
            wo.InvoiceNumber = dto.InvoiceNumber;
            wo.InvoiceDate = dto.InvoiceDate;
            wo.TaxAmount = dto.TaxAmount;
            wo.Category = dto.Category;
            wo.VendorNameRaw = dto.VendorNameRaw;

            _db.WorkOrderLineItems.RemoveRange(wo.LineItems);
            wo.LineItems.Clear();

            foreach (var l in dto.Lines)
            {
                wo.LineItems.Add(new WorkOrderLineItem
                {
                    Type = NormalizeLineType(l.Type),
                    Description = l.Description.Trim(),
                    Qty = l.Qty <= 0 ? 1 : l.Qty,
                    UnitPrice = l.UnitPrice,
                    PartNumber = string.IsNullOrWhiteSpace(l.PartNumber) ? null : l.PartNumber.Trim()
                });
            }

            RecalcAmounts(wo);

            if (dto.ReplaceDocuments && dto.DocumentIds is not null)
            {
                List<Document> docs = new();
                if (dto.DocumentIds.Count > 0)
                {
                    docs = await _db.Documents.Where(d => dto.DocumentIds.Contains(d.Id)).ToListAsync();
                    if (docs.Count != dto.DocumentIds.Count) return BadRequest(new { message = "Invalid DocumentIds" });
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
        // ----------------------------
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var wo = await _db.WorkOrders.FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
            if (wo is null) return NotFound();

            wo.IsDeleted = true;
            wo.DeletedAt = DateTimeOffset.UtcNow;
            
            await _db.SaveChangesAsync();
            return NoContent();
        }

        public sealed record WorkOrdersBulkDeleteRequest(List<Guid> WorkOrderIds);

        [HttpPost("bulk-delete")]
        public async Task<IActionResult> BulkDelete([FromBody] WorkOrdersBulkDeleteRequest request)
        {
            if (request.WorkOrderIds == null || request.WorkOrderIds.Count == 0)
                return BadRequest(new { message = "No IDs provided." });

            var workOrders = await _db.WorkOrders
                .Where(w => request.WorkOrderIds.Contains(w.Id) && !w.IsDeleted)
                .ToListAsync();

            if (workOrders.Count == 0) return NotFound(new { message = "No matching work orders found." });

            foreach (var wo in workOrders)
            {
                wo.IsDeleted = true;
                wo.DeletedAt = DateTimeOffset.UtcNow;
            }

            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}
