using Application.DTO.Documents;
using Application.Interfaces;
using Application.Interfaces.Http;
using Domain.Interfaces.Http;
using Serilog;
using System.Text.RegularExpressions;

namespace Infrastructure.HttpAcessor;

public class CrmVerifier:IProfessionalDocumentVerifier
{
    private readonly HttpClient _http;

    public CrmVerifier(HttpClient http)
    {
        _http = http;
    }

    public async Task<DocumentVerificationResult> VerifyAsync(string document)
    {
        Log.Information("Iniciando verificação CRM {@Document}", document);

        try
        {
            var parts = document.Split('-');
            var number = parts[1];
            var state = parts[2];

            var url = $"https://portal.cfm.org.br/busca-medicos/?uf={state}&q={number}";
            var html = await _http.GetStringAsync(url);

            if (html.Contains("Nenhum resultado"))
            {
                Log.Warning("CRM não encontrado {@Document}", document);

                return new DocumentVerificationResult(false, null, null, null);
            }

            string Extract(string pattern)
            {
                var match = Regex.Match(html, pattern, RegexOptions.IgnoreCase);
                return match.Success ? match.Groups[1].Value.Trim() : "";
            }

            var name = Extract(@"Nome:\s*<\/b>\s*(.*?)<\/");
            var status = Extract(@"Situa[cç][aã]o:\s*<\/b>\s*(.*?)<\/");
            var speciality = Extract(@"Especialidade:\s*<\/b>\s*(.*?)<\/");

            Log.Information("CRM verificado {@Result}",
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
            Log.Error(ex, "Erro ao verificar CRM {@Document}", document);
            throw;
        }
    }
}
