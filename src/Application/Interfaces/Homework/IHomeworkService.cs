using Application.DTO.Homework;

namespace Application.Interfaces.Homework;

public interface IHomeworkService
{
    Task<Guid> CreateAsync(CreateHomeworkRequest request, Guid professionalUserId);
    Task<IEnumerable<HomeworkDto>> GetByProfessionalAsync(Guid professionalUserId);
    Task<IEnumerable<HomeworkDto>> GetByPatientAsync(Guid patientId);
    Task UpdateAsync(Guid homeworkId, UpdateHomeworkRequest request, Guid professionalUserId);
    Task DeleteAsync(Guid homeworkId, Guid professionalUserId);
    Task MarkAsDoneAsync(Guid homeworkId, Guid patientUserId);
}
