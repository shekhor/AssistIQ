using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using AssistIQ.Api.Models;
using Microsoft.Extensions.Configuration;

namespace AssistIQ.Api.Services;

public class OpenAiService : IAiService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private readonly string _endpoint;
    private readonly string _model;

    public OpenAiService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _apiKey = configuration["Ai:ApiKey"] ?? string.Empty;
        _endpoint = configuration["Ai:Endpoint"] ?? string.Empty;
        _model = configuration["Ai:Model"] ?? string.Empty;
    }

    public async Task<AiResult> GenerateSolutionAsync(string ticketText, string? context = null)
    {
        if (string.IsNullOrWhiteSpace(_apiKey) ||
            string.IsNullOrWhiteSpace(_endpoint) ||
            string.IsNullOrWhiteSpace(_model))
        {
            throw new InvalidOperationException("AI settings are missing. Check Ai:ApiKey, Ai:Endpoint, and Ai:Model.");
        }

        var userPrompt = string.IsNullOrWhiteSpace(context)
            ? ticketText
            : $"{ticketText}\n\nRelevant verified solutions:\n{context}";

        using var request = new HttpRequestMessage(HttpMethod.Post, _endpoint);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);

        var payload = new
        {
            model = _model,
            temperature = 0.2,
            messages = new[]
            {
                new { role = "system", content = "You are a helpful IT support assistant." },
                new { role = "user", content = userPrompt }
            }
        };

        request.Content = new StringContent(
            JsonSerializer.Serialize(payload),
            Encoding.UTF8,
            "application/json");

        using var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);

        var content = doc.RootElement
            .GetProperty("choices")[0]
            .GetProperty("message")
            .GetProperty("content")
            .GetString();

        return new AiResult
        {
            ResponseText = content ?? string.Empty,
            ConfidenceScore = null
        };
    }
}


