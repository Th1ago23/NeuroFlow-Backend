using Application.DTO.Documents;

namespace Application.Interfaces.Http;

public interface IProfessionalDocumentVerifier
{
    public Task<DocumentVerificationResult> VerifyAsync(string documentNumber);
}
