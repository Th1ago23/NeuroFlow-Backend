using Domain.Entities;

namespace Domain.Interfaces.Repositories
{
    public interface IThoughtEntryRepository : IGenericRepository<ThoughtEntry>
    {
        Task<IEnumerable<ThoughtEntry>> GetByPatientAsync(Guid patientId);
    }
}
