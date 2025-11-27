using Application.DTO.Homework;

namespace Application.Interfaces.Homework;

public interface IHomeworkService
{
    Task<Guid> CreateAsync(CreateHomeworkRequest request);
    Task<IEnumerable<HomeworkDto>> GetByPatientAsync(Guid patientId);
    Task MarkAsDoneAsync(Guid homeworkId);
}
