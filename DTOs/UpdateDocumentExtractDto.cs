using System.Text.Json;

namespace FleetManage.Api.DTOs
{
   
        public class UpdateDocumentExtractDto
        {
            // Raw extraction payload you store in Document.ExtractedJson
            public JsonDocument? ExtractedJson { get; set; }

            // Optional: vendor name string shown in lists
            public string? VendorNameRaw { get; set; }

            // Optional: if null, controller keeps existing
            public decimal? ConfidenceScore { get; set; }

            // Optional: "uploaded" | "extracting" | "needs_review" | "parsed" | "failed"
            public string? Status { get; set; }
        
    }
}
