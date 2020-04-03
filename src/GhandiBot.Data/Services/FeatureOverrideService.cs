using System;
using System.Linq;
using System.Threading.Tasks;
using GhandiBot.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace GhandiBot.Data.Services
{
    public class FeatureOverrideService
    {
        private readonly AppDbContext _context;

        public FeatureOverrideService(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<bool> IsOverriden(string featureName, ulong serverId)
        {
            var featureOverride = await _context.FeatureOverride.SingleOrDefaultAsync(x => 
                x.CommandType == featureName 
                && x.ServerId == serverId);
            return featureOverride != null;
        }

        public async Task AddOverride(string featureName, ulong serverId, ulong ownerId)
        {
            if (_context.FeatureOverride.SingleOrDefault(x => x.ServerId == serverId
                                                              && x.CommandType == featureName) != null)
            {
                return;
            }
            
            var featureOverride = new FeatureOverride
            {
                ServerId = serverId,
                CommandIssuerId = ownerId,
                CommandType = featureName
            };

            await _context.FeatureOverride.AddAsync(featureOverride);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> RemoveOverride(string featureName, ulong serverId)
        {
            var toRemove = await _context.FeatureOverride.SingleOrDefaultAsync(x =>
                x.CommandType == featureName && x.ServerId == serverId);
            if (toRemove == null) return false;

            _context.FeatureOverride.Remove(toRemove);
            await _context.SaveChangesAsync();
            
            return true;
        }
    }
}