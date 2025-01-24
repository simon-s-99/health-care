using HealthCareABApi.Models;
using MongoDB.Driver;

namespace HealthCareABApi.Repositories.Interfaces
{
    public interface IMongoDbContext
    {
        IMongoCollection<Appointment> Appointments { get; }
        IMongoCollection<Availability> Availabilities { get; }
        IMongoCollection<Feedback> Feedbacks { get; }
        IMongoCollection<User> Users { get; }
    }
}

