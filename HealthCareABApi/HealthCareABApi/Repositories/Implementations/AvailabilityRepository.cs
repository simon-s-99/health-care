using HealthCareABApi.Models;
using MongoDB.Driver;

namespace HealthCareABApi.Repositories.Implementations
{
    public class AvailabilityRepository : IAvailabilityRepository
    {
        private readonly IMongoCollection<Availability> _collection;

        public AvailabilityRepository(IMongoDbContext context)
        {
            _collection = context.Availabilities;
        }

        public async Task<List<Availability>> GetAllAsync()
        {
            return await _collection.Find(_ => true).ToListAsync();
        }

        public async Task<Availability> GetByIdAsync(string id)
        {
            return await _collection.Find(a => a.Id == id).FirstOrDefaultAsync();
        }

        public async Task CreateAsync(Availability availability)
        {
            await _collection.InsertOneAsync(availability);
        }

        public async Task UpdateAsync(FilterDefinition<Availability> filter, UpdateDefinition<Availability> update)
        {
            await _collection.UpdateOneAsync(filter, update);
        }

        public async Task DeleteAsync(string id)
        {
            await _collection.DeleteOneAsync(a => a.Id == id);
        }

        public async Task<Availability> GetByCaregiverIdAsync(string caregiverId)
        {
            return await _collection.Find(a => a.CaregiverId == caregiverId).FirstOrDefaultAsync();
        }

        public async Task<Availability> GetByCaregiverIdAndDate(string caregiverId, DateTime dateTime)
        {
            return await _collection.Find(a => a.CaregiverId == caregiverId && a.AvailableSlots.Contains((DateTime)dateTime)).FirstOrDefaultAsync();
        }

    }
}

