using Application.DTO.Homework;
using Application.Interfaces.Homework;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces.Repositories;

namespace Application.Services
{
    public class HomeworkService : IHomeworkService
    {
        private readonly IHomeworkRepository _homeworkRepo;
        private readonly IPatientRepository _patientRepo;
        private readonly IUserRepository _userRepo;
        private readonly IUnitOfWork _uow;

        public HomeworkService(
            IHomeworkRepository homeworkRepo,
            IPatientRepository patientRepo,
            IUserRepository userRepo,
            IUnitOfWork uow)
        {
            _homeworkRepo = homeworkRepo;
            _patientRepo = patientRepo;
            _userRepo = userRepo;
            _uow = uow;
        }

        public async Task<Guid> CreateAsync(CreateHomeworkRequest request, Guid professionalUserId)
        {
            var professional = await _userRepo.GetById(professionalUserId);
            if (professional is null || professional.Role != UserRole.Professional)
                throw new UnauthorizedAccessException("Apenas profissionais podem criar tarefas.");

            var patient = await _patientRepo.GetByIdAsync(request.PatientId);
            if (patient is null)
                throw new KeyNotFoundException("Paciente não encontrado.");

            if (patient.OwnerProfessionalId != professionalUserId)
                throw new UnauthorizedAccessException("Você só pode criar tarefas para seus próprios pacientes.");

            var hw = new Homework(
                professionalUserId,
                request.PatientId,
                request.Title,
                request.ExpirationTime,
                request.Description
            );

            await _homeworkRepo.AddAsync(hw);
            await _uow.CommitAsync();

            return hw.Id;
        }

        public async Task<IEnumerable<HomeworkDto>> GetByPatientAsync(Guid patientId)
        {
            var list = await _homeworkRepo.GetByPatientAsync(patientId);

            return list.Select(h => new HomeworkDto(
                h.Id,
                h.Title,
                h.Description,
                h.ExpirationTime,
                h.IsDone
            ));
        }

        public async Task<IEnumerable<HomeworkDto>> GetByProfessionalAsync(Guid professionalUserId)
        {
            var list = await _homeworkRepo.GetByProfessionalAsync(professionalUserId);

            return list.Select(h => new HomeworkDto(
                h.Id,
                h.Title,
                h.Description,
                h.ExpirationTime,
                h.IsDone
            ));
        }

        public async Task MarkAsDoneAsync(Guid homeworkId, Guid patientUserId)
        {
            var hw = await _homeworkRepo.GetByIdAsync(homeworkId);
            if (hw is null)
                throw new KeyNotFoundException("Tarefa não encontrada.");

            var patient = await _patientRepo.GetByIdAsync(hw.PatientId);
            if (patient is null || patient.UserAccountId != patientUserId)
                throw new UnauthorizedAccessException("Você não pode alterar tarefas de outro paciente.");

            hw.MarkAsDone();
            _homeworkRepo.Update(hw);
            await _uow.CommitAsync();
        }

        public async Task UpdateAsync(Guid homeworkId, UpdateHomeworkRequest request, Guid professionalUserId)
        {
            var hw = await _homeworkRepo.GetByIdAsync(homeworkId);
            if (hw is null)
                throw new KeyNotFoundException("Tarefa não encontrada.");

            if (hw.ProfessionalUserId != professionalUserId)
                throw new UnauthorizedAccessException("Somente o profissional que criou a tarefa pode alterá-la.");

            hw.Update(request.Title, request.Description, request.ExpirationTime);

            _homeworkRepo.Update(hw);
            await _uow.CommitAsync();
        }

        public async Task DeleteAsync(Guid homeworkId, Guid professionalUserId)
        {
            var hw = await _homeworkRepo.GetByIdAsync(homeworkId);
            if (hw is null)
                throw new KeyNotFoundException("Tarefa não encontrada.");

            if (hw.ProfessionalUserId != professionalUserId)
                throw new UnauthorizedAccessException("Somente o profissional que criou a tarefa pode removê-la.");

            _homeworkRepo.Remove(hw);
            await _uow.CommitAsync();
        }
    }

}
