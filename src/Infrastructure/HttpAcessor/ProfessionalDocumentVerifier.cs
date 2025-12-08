using Application.DTO.Documents;
using Application.Interfaces.Http;
using System.Text.RegularExpressions;

namespace Infrastructure.HttpAcessor;

public class ProfessionalDocumentVerifier : IProfessionalDocumentVerifier
{
    private readonly IProfessionalDocumentVerifier _crpVerifier;
    private readonly IProfessionalDocumentVerifier _crmVerifier;

    public ProfessionalDocumentVerifier(CrpVerifier crpVerifier, CrmVerifier crmVerifier)
    {
        _crpVerifier = crpVerifier;
        _crmVerifier = crmVerifier;
    }

    public async Task<DocumentVerificationResult> VerifyAsync(string documentNumber)
    {
        if (Regex.IsMatch(documentNumber, @"^CRP-\d{2}\/\d{1,6}$"))
            return await _crpVerifier.VerifyAsync(documentNumber);

        if (Regex.IsMatch(documentNumber, @"^CRM-\d{1,6}-[A-Z]{2}$"))
            return await _crmVerifier.VerifyAsync(documentNumber);

        return new DocumentVerificationResult(
            false,
            null,
            null,
            null
        );
    }
}
