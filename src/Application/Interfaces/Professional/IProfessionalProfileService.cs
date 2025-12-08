using Application.DTO.Professional;

namespace Application.Interfaces.Professional;

public interface IProfessionalProfileService
{
    Task<ProfessionalProfileDto> CreateAsync(Guid userId, CreateProfessionalProfileRequest request);
    Task<ProfessionalProfileDto?> GetByUserIdAsync(Guid userId);
    Task<ProfessionalProfileDto?> GetByDocumentAsync(string document);
    Task UpdateAsync(Guid userId, UpdateProfessionalProfileRequest request);
    Task VerifyAsync(Guid userId, VerifyProfessionalDocumentRequest request);
    //Task UpgradeToPremiumAsync(Guid userId);
}
