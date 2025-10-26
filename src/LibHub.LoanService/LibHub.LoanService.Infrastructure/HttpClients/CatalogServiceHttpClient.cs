using LibHub.LoanService.Application.DTOs;
using LibHub.LoanService.Application.Interfaces;
using System.Text;
using System.Text.Json;

namespace LibHub.LoanService.Infrastructure.HttpClients;

public class CatalogServiceHttpClient : ICatalogService
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions;

    public CatalogServiceHttpClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }

    public async Task<BookAvailabilityDto> GetBookAvailabilityAsync(int bookId)
    {
        var response = await _httpClient.GetAsync($"/api/books/{bookId}/availability");
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var availability = JsonSerializer.Deserialize<BookAvailabilityDto>(content, _jsonOptions);
        
        return availability ?? throw new Exception("Failed to deserialize book availability response");
    }

    public async Task UpdateBookStockAsync(int bookId, int changeAmount)
    {
        var request = new { ChangeAmount = changeAmount };
        var json = JsonSerializer.Serialize(request, _jsonOptions);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PutAsync($"/api/books/{bookId}/stock", content);
        response.EnsureSuccessStatusCode();
    }
}
