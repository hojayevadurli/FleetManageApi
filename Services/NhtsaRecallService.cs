using FleetManage.Api.Data;
using FleetManage.Api.Data.Enums;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace FleetManage.Api.Services
{
    public class NhtsaRecallService : INhtsaRecallService
    {
        private readonly AppDbContext _db;
        private readonly HttpClient _http;
        private readonly ILogger<NhtsaRecallService> _logger;

        public NhtsaRecallService(AppDbContext db, HttpClient http, ILogger<NhtsaRecallService> logger)
        {
            _db = db;
            _http = http;
            _logger = logger;
        }

        public async Task<int> SyncRecallsForEquipmentAsync(Guid equipmentId)
        {
            var equipment = await _db.Equipments.FindAsync(equipmentId);
            if (equipment == null) return 0;

            if (string.IsNullOrWhiteSpace(equipment.Vin))
            {
                _logger.LogWarning("Equipment {Id} has no VIN. Skipping recall check.", equipmentId);
                return 0;
            }

            // NHTSA API: https://api.nhtsa.gov/recalls/recallsByVin?vin={vin}&format=json
            var url = $"https://api.nhtsa.gov/recalls/recallsByVin?vin={equipment.Vin}&format=json";
            
            try
            {
                var response = await _http.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("NHTSA API failed with status {Status}", response.StatusCode);
                    return 0;
                }

                var content = await response.Content.ReadAsStringAsync();
                var nhtsaData = JsonSerializer.Deserialize<NhtsaResponse>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (nhtsaData == null || nhtsaData.Results == null) return 0;

                int newRecallsCount = 0;

                foreach (var r in nhtsaData.Results)
                {
                    // 1. Sync Campaign
                    // NHTSA CampaignNumber is unique identifier (e.g. 14V354000)
                    var campaignCode = r.NHTSACampaignNumber; 
                    if (string.IsNullOrWhiteSpace(campaignCode)) continue;

                    var campaign = await _db.RecallCampaigns.FirstOrDefaultAsync(x => x.Code == campaignCode);
                    if (campaign == null)
                    {
                        campaign = new RecallCampaign
                        {
                            TenantId = equipment.TenantId, // System wide? Usually recalls are global, but we use TenantEntity. Maybe link to System Tenant or replicate. 
                                                           // For simplicity, we replicate or assign to current tenant if "RecallCampaign" is TenantEntity.
                                                           // If RecallCampaign is TenantEntity, we must treat it as such.
                            Code = campaignCode,
                            Title = r.Component ?? "Unknown Component",
                            Description = r.Summary,
                            Manufacturer = r.Manufacturer ?? "Unknown",
                            IssueDate = ParseNhtsaDate(r.ReportReceivedDate) ?? DateOnly.FromDateTime(DateTime.UtcNow),
                            IsActive = true
                        };
                        _db.RecallCampaigns.Add(campaign);
                        await _db.SaveChangesAsync(); // Save to get Id
                    }

                    // 2. Link to Equipment
                    var exists = await _db.EquipmentRecalls.AnyAsync(x => x.EquipmentId == equipmentId && x.RecallCampaignId == campaign.Id);
                    if (!exists)
                    {
                        var link = new EquipmentRecall
                        {
                            TenantId = equipment.TenantId,
                            EquipmentId = equipmentId,
                            RecallCampaignId = campaign.Id,
                            Status = EquipmentRecallStatus.Open,
                            FirstSeenAt = DateTimeOffset.UtcNow,
                            Notes = r.Consequence
                        };
                        _db.EquipmentRecalls.Add(link);
                        newRecallsCount++;
                    }
                }

                await _db.SaveChangesAsync();
                return newRecallsCount;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error syncing recalls for VIN {Vin}", equipment.Vin);
                throw;
            }
        }

        private DateOnly? ParseNhtsaDate(string? dateStr)
        {
            // Format: "23/06/2014" or similar
            if (DateTime.TryParse(dateStr, out var dt))
            {
                return DateOnly.FromDateTime(dt);
            }
            return null;
        }

        // Internal DTOs for API
        private class NhtsaResponse
        {
            public int Count { get; set; }
            public string? Message { get; set; }
            public List<NhtsaResult>? Results { get; set; }
        }

        private class NhtsaResult
        {
            public string? Manufacturer { get; set; }
            public string? NHTSACampaignNumber { get; set; }
            public string? ReportReceivedDate { get; set; }
            public string? Component { get; set; }
            public string? Summary { get; set; }
            public string? Consequence { get; set; }
            public string? Remedy { get; set; }
        }
    }
}
