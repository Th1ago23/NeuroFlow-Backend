using Application.DTO.Documents;
using Application.Interfaces;
using Application.Interfaces.Http;

namespace Application.Services;

public class MockDocumentVerifier : IProfessionalDocumentVerifier
{
    public Task<DocumentVerificationResult> VerifyAsync(string documentNumber)
    {
        return Task.FromResult(new DocumentVerificationResult(
            true,
            "Profissional Teste",
            "Ativo",
            "Psicologia Clínica"
        ));
    }
}
