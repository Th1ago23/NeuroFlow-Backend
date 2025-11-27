using Domain.Interfaces.Repositories;
using Infrastructure.Repositories;

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IPatientRepository, PatientRepository>();
builder.Services.AddScoped<IProfessionalProfileRepository, ProfessionalProfileRepository>();
builder.Services.AddScoped<IAppointmentRepository, AppointmentRepository>();
builder.Services.AddScoped<IMoodEntryRepository, MoodEntryRepository>();
builder.Services.AddScoped<IThoughtEntryRepository, ThoughtEntryRepository>();
builder.Services.AddScoped<IClinicalNoteRepository, ClinicalNoteRepository>();
builder.Services.AddScoped<IClinicalAssessmentRepository, ClinicalAssessmentRepository>();

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();