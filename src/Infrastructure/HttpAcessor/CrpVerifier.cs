using Application.DTO.Documents;
using Application.Interfaces.Http;
using Serilog;
using System.Text.RegularExpressions;

namespace Infrastructure.HttpAcessor;

public class CrpVerifier:IProfessionalDocumentVerifier
{
    private readonly HttpClient _http;

    public CrpVerifier(HttpClient http)
    {
        _http = http;
    }

    public async Task<DocumentVerificationResult> VerifyAsync(string document)
    {
        Log.Information("Iniciando verificação de CRP {@Document}", document);

        try
        {
            var url = $"https://cadastro.cfp.org.br/busca?term={document}";
            var html = await _http.GetStringAsync(url);

            if (html.Contains("Nenhum resultado encontrado"))
            {
                Log.Warning("CRP não encontrado {@Document}", document);

                return new DocumentVerificationResult(false, null, null, null);
            }

            string Extract(string pattern)
            {
                var match = Regex.Match(html, pattern, RegexOptions.IgnoreCase);
                return match.Success ? match.Groups[1].Value.Trim() : "";
            }

            var name = Extract(@"Nome:\s*<\/b>\s*(.*?)<\/");
            var status = Extract(@"Situa[cç][aã]o:\s*<\/b>\s*(.*?)<\/");
            var speciality = Extract(@"Atua[cç][aã]o:\s*<\/b>\s*(.*?)<\/");

            Log.Information("CRP verificado com sucesso {@Result}",
                new
                {
                    Document = document,
                    Name = name,
                    Status = status,
                    Speciality = speciality
                });

            return new DocumentVerificationResult(true, name, status, speciality);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Erro ao verificar CRP {@Document}", document);
            throw;
        }
    }
}
