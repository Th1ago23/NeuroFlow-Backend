using Application.DTO.Patients;
using Application.Interfaces.Services;
using Domain.Entities;
using Domain.Interfaces.Repositories;

namespace Application.Services
{
    public class PatientInviteService:IPatientInviteService
    {
        private readonly IPatientInviteService _repository;
        private readonly IUnitOfWork _uow;

        public PatientInviteService(IPatientInviteService repository, IUnitOfWork uow)
        {
            _repository = repository;
            _uow = uow;
        }

        public async Task<PatientInviteDto> CreateInviteAsync(Guid professionalUserId, CancellationToken ct = default)
        {
            var invite = new PatientInvite(professionalUserId, Guid.NewGuid(), DateTime.UtcNow.AddDays(7));
            
            await _uow.PatientInvites.AddAsync(invite,ct);
            await _uow.CommitAsync();

            return new PatientInviteDto(invite.Token, invite.ProfessionalUserId, invite.ExpiresAt, invite.IsUsed);

        }

        public async Task<PatientInviteDto> ValidateInviteAsync(Guid token, CancellationToken ct = default)
        {
            var invite = await _uow.PatientInvites.GetActiveByTokenAsync(token);

            return new PatientInviteDto(invite.Token,invite.ProfessionalUserId,invite.ExpiresAt, invite.IsUsed);
        }
    }
}
