using Application.DTO.Clinical;
using Application.Interfaces.Clinical;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces.Repositories;

public class ClinicalAssessmentService : IClinicalAssessmentService
{
    private readonly IClinicalAssessmentRepository _assessmentRepo;
    private readonly IPatientRepository _patientRepo;
    private readonly IUserRepository _userRepo;
    private readonly IUnitOfWork _uow;

    public ClinicalAssessmentService(
        IClinicalAssessmentRepository assessmentRepo,
        IPatientRepository patientRepo,
        IUserRepository userRepo,
        IUnitOfWork uow)
    {
        _assessmentRepo = assessmentRepo;
        _patientRepo = patientRepo;
        _userRepo = userRepo;
        _uow = uow;
    }

    public async Task<Guid> CreateAsync(CreateClinicalAssessmentRequest request)
    {
        var professional = await _userRepo.GetById(request.ProfessionalUserId);
        if (professional is null || professional.Role != UserRole.Professional)
            throw new UnauthorizedAccessException("Apenas profissionais podem registrar avaliações clínicas.");

        var patient = await _patientRepo.GetByIdAsync(request.PatientId);
        if (patient is null)
            throw new KeyNotFoundException("Paciente não encontrado.");

        if (patient.OwnerProfessionalId != request.ProfessionalUserId)
            throw new UnauthorizedAccessException("O profissional informado não está vinculado a este paciente.");

        var assessment = new ClinicalAssessment(
            request.ProfessionalUserId,
            request.PatientId,
            request.Date,
            request.Summary,
            request.Diagnosis,
            request.Recommendations
        );

        await _assessmentRepo.AddAsync(assessment);
        await _uow.CommitAsync();

        return assessment.Id;
    }

    public async Task<IEnumerable<ClinicalAssessmentDto>> GetByPatientAsync(Guid patientId)
    {
        var patient = await _patientRepo.GetByIdAsync(patientId);
        if (patient is null)
            throw new KeyNotFoundException("Paciente não encontrado.");

        var list = await _assessmentRepo.GetByPatientAsync(patientId);

        return list.Select(a => new ClinicalAssessmentDto(
            a.Id,
            a.ProfessionalUserId,
            a.Summary,
            a.Diagnosis,
            a.Recommendations,
            a.Date,
            a.CreatedAt
        ));
    }

    public async Task UpdateAsync(Guid assessmentId, UpdateClinicalAssessmentRequest request)
    {
        var assessment = await _assessmentRepo.GetByIdAsync(assessmentId);
        if (assessment is null)
            throw new KeyNotFoundException("Avaliação clínica não encontrada.");

        assessment.Update(request.Summary, request.Diagnosis, request.Recommendations);

        _assessmentRepo.Update(assessment);
        await _uow.CommitAsync();
    }

    public async Task DeleteAsync(Guid assessmentId)
    {
        var assessment = await _assessmentRepo.GetByIdAsync(assessmentId);
        if (assessment is null)
            throw new KeyNotFoundException("Avaliação clínica não encontrada.");

        _assessmentRepo.Remove(assessment);
        await _uow.CommitAsync();
    }
}
