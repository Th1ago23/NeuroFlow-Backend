using Application.DTO.Appointments;
using Application.Interfaces.Appointments;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces.Repositories;

namespace Application.Services;

public class AppointmentService : IAppointmentService
{
    private readonly IAppointmentRepository _appointmentRepo;
    private readonly IPatientRepository _patientRepo;
    private readonly IUserRepository _userRepo;
    private readonly IUnitOfWork _uow;

    public AppointmentService(
        IAppointmentRepository appointmentRepo,
        IPatientRepository patientRepo,
        IUserRepository userRepo,
        IUnitOfWork uow)
    {
        _appointmentRepo = appointmentRepo;
        _patientRepo = patientRepo;
        _userRepo = userRepo;
        _uow = uow;
    }

    public async Task<Guid> CreateAsync(CreateAppointmentRequest request)
    {
        var professional = await _userRepo.GetById(request.ProfessionalUserId);

        if (professional is null)
            throw new KeyNotFoundException("Profissional não encontrado.");

        if (professional.Role != Domain.Enums.UserRole.Professional)
            throw new UnauthorizedAccessException("Somente profissionais podem marcar consultas.");


        var patient = await _patientRepo.GetByIdAsync(request.PatientId);
        if (patient is null)
            throw new KeyNotFoundException("Paciente não encontrado.");

        if (patient.OwnerProfessionalId != request.ProfessionalUserId)
            throw new UnauthorizedAccessException("Este paciente não pertence ao profissional informado.");

        var appointment = new Appointment(
            request.ProfessionalUserId,
            request.PatientId,
            request.ScheduledDateTime
        );

        await _appointmentRepo.AddAsync(appointment);
        await _uow.CommitAsync();

        return appointment.Id;
    }

    public async Task<IEnumerable<AppointmentDto>> GetUpcomingByProfessionalAsync(Guid professionalId)
    {
        var professional = await _userRepo.GetById(professionalId);
        if (professional is null)
            throw new KeyNotFoundException("Profissional não encontrado.");

        var appointments = await _appointmentRepo.GetUpcomingByProfessionalAsync(professionalId);

        return appointments
            .OrderBy(a => a.ScheduledDateTime)
            .Select(a => new AppointmentDto(a.Id, a.ScheduledDateTime, a.Status));
    }

    public async Task<IEnumerable<AppointmentDto>> GetUpcomingByPatientAsync(Guid patientId)
    {
        var patient = await _patientRepo.GetByIdAsync(patientId);
        if (patient is null)
            throw new KeyNotFoundException("Paciente não encontrado.");

        var appointments = await _appointmentRepo.GetUpcomingByPatientAsync(patientId);

        return appointments
            .OrderBy(a => a.ScheduledDateTime)
            .Select(a =>
                new AppointmentDto(a.Id, a.ScheduledDateTime, a.Status)
            );
    }

    public async Task CancelAsync(Guid appointmentId)
    {
        var appointment = await _appointmentRepo.GetByIdAsync(appointmentId);
        if (appointment is null)
            throw new KeyNotFoundException("Consulta não encontrada.");

        appointment.Cancel();

        _appointmentRepo.Update(appointment);
        await _uow.CommitAsync();
    }

    public async Task CompleteAsync(Guid appointmentId)
    {
        var appointment = await _appointmentRepo.GetByIdAsync(appointmentId);
        if (appointment is null)
            throw new KeyNotFoundException("Consulta não encontrada.");

        appointment.Complete();

        _appointmentRepo.Update(appointment);
        await _uow.CommitAsync();
    }
}
