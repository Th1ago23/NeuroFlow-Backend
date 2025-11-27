using Application.DTO.Clinical;
using Domain.Entities;

namespace Application.Mappings.Assessments
{

    public static class ClinicalAssessmentMappings
    {
        public static ClinicalAssessmentDto ToDto(this ClinicalAssessment a)
            => new(
                Id: a.Id,
                ProfessionalUserId: a.ProfessionalUserId,
                Summary: a.Summary,
                Diagnosis: a.Diagnosis,
                Recommendations: a.Recommendations,
                Date: a.Date,
                CreatedAt: a.CreatedAt
            );

        public static ClinicalAssessment ToEntity(this CreateClinicalAssessmentRequest request)
        {
            return new ClinicalAssessment(
                professionalUserId: request.ProfessionalUserId,
                patientId: request.PatientId,
                date: DateTime.UtcNow,
                summary: request.Summary,
                diagnosis: request.Diagnosis,
                recommendations: request.Recommendations
            );
        }
    }
}
