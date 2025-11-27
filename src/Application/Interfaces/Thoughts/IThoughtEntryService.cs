using Application.DTO.Thoughts;

namespace Application.Interfaces.Thoughts
{
    public interface IThoughtEntryService
    {
        Task<Guid> CreateAsync(CreateThoughtEntryRequest request);
        Task<IEnumerable<ThoughtEntryDto>> GetByPatientAsync(Guid patientId);
    }
}
