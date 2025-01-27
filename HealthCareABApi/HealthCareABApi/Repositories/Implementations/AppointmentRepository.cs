using HealthCareABApi.Models;
using HealthCareABApi.Repositories.Interfaces;
using MongoDB.Driver;

namespace HealthCareABApi.Repositories.Implementations
{
    public class AppointmentRepository : IAppointmentRepository
    {
        private readonly IMongoCollection<Appointment> _collection;

        public AppointmentRepository(IMongoDbContext context)
        {
            _collection = context.Appointments;
        }

        public async Task<List<Appointment>> GetAllAsync()
        {
            return await _collection.Find(_ => true).ToListAsync();
        }

        public async Task<Appointment> GetByIdAsync(string id)
        {
            return await _collection.Find(a => a.Id == id).FirstOrDefaultAsync();
        }

        public async Task<List<Appointment>> GetAllByPatientId(string id, DateTime? date)
        {
            var collection = await _collection.Find(u => u.PatientId == id).ToListAsync();
            if (date is not null)
            {
                return collection.Where(a => a.DateTime.Date == date.Value.Date).ToList();
            }
            return collection;

        }

        public async Task<List<Appointment>> GetAllByCaregiverId(string id, DateTime? date)
        {
            var collection = await _collection.Find(u => u.CaregiverId == id).ToListAsync();
            if (date is not null)
            {
                return collection.Where(a => a.DateTime.Date == date.Value.Date).ToList();
            }
            return collection;
        }

        public async Task CreateAsync(Appointment appointment)
        {
            await _collection.InsertOneAsync(appointment);
        }

        public async Task UpdateAsync(FilterDefinition<Appointment> filter, UpdateDefinition<Appointment> update)
        {
            await _collection.UpdateOneAsync(filter, update);
        }

        public async Task DeleteAsync(string id)
        {
            await _collection.DeleteOneAsync(a => a.Id == id);
        }
    }
}

