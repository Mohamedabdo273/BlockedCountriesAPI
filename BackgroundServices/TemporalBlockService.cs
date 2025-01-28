using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BlockedCountriesAPI.BackgroundServices
{
    public class TemporalBlockService : BackgroundService
    {
        private readonly ILogger<TemporalBlockService> _logger;

        // Static dictionary to hold blocked countries temporarily
        private static ConcurrentDictionary<string, TemporalBlock> temporalBlocks = new ConcurrentDictionary<string, TemporalBlock>();

        public TemporalBlockService(ILogger<TemporalBlockService> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                // Delay the background task to run every 5 minutes
                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
                RemoveExpiredBlocks();
            }
        }

        private void RemoveExpiredBlocks()
        {
            var now = DateTime.UtcNow;

            // Get the list of expired blocks
            var expiredBlocks = temporalBlocks
                .Where(kvp => kvp.Value.ExpiryTime <= now)
                .ToList();

            // Remove expired blocks from the in-memory store
            foreach (var block in expiredBlocks)
            {
                temporalBlocks.TryRemove(block.Key, out _);
                _logger.LogInformation($"Removed expired block for country: {block.Key}");
            }
        }
    }

    // TemporalBlock class to hold country and expiry time
    public class TemporalBlock
    {
        public string CountryCode { get; set; }
        public DateTime ExpiryTime { get; set; }
    }
}
