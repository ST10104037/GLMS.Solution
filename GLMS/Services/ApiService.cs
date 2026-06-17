using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using GLMS.Models;

namespace GLMS.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public ApiService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
        }

        private void AttachToken()
        {
            var token = _httpContextAccessor.HttpContext?.Session.GetString("JwtToken");
            if (!string.IsNullOrEmpty(token))
                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
        }

        private StringContent ToJson<T>(T obj) =>
            new(JsonSerializer.Serialize(obj), Encoding.UTF8, "application/json");

        // ── Auth ──────────────────────────────────────────────────

        public async Task<string?> LoginAsync(string username, string password)
        {
            var payload = ToJson(new { username, password });
            var response = await _httpClient.PostAsync("/api/auth/login", payload);
            if (!response.IsSuccessStatusCode) return null;

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);
            return doc.RootElement.GetProperty("token").GetString();
        }

        // ── Clients ───────────────────────────────────────────────

        public async Task<List<Client>> GetClientsAsync()
        {
            AttachToken();
            var response = await _httpClient.GetAsync("/api/clients");
            if (!response.IsSuccessStatusCode) return new List<Client>();
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<Client>>(json, _jsonOptions)
                   ?? new List<Client>();
        }

        public async Task<Client?> GetClientAsync(int id)
        {
            AttachToken();
            var response = await _httpClient.GetAsync($"/api/clients/{id}");
            if (!response.IsSuccessStatusCode) return null;
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<Client>(json, _jsonOptions);
        }

        public async Task<bool> CreateClientAsync(Client client)
        {
            AttachToken();
            var response = await _httpClient.PostAsync("/api/clients", ToJson(client));
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteClientAsync(int id)
        {
            AttachToken();
            var response = await _httpClient.DeleteAsync($"/api/clients/{id}");
            return response.IsSuccessStatusCode;
        }

        // ── Contracts ─────────────────────────────────────────────

        public async Task<List<Contract>> GetContractsAsync(
            DateTime? startDate = null, DateTime? endDate = null, ContractStatus? status = null)
        {
            AttachToken();
            var url = "/api/contracts";
            var queryParts = new List<string>();
            if (startDate.HasValue)
                queryParts.Add($"startDate={startDate.Value:yyyy-MM-dd}");
            if (endDate.HasValue)
                queryParts.Add($"endDate={endDate.Value:yyyy-MM-dd}");
            if (status.HasValue)
                queryParts.Add($"status={status.Value}");
            if (queryParts.Any())
                url += "?" + string.Join("&", queryParts);

            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode) return new List<Contract>();
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<Contract>>(json, _jsonOptions)
                   ?? new List<Contract>();
        }

        public async Task<Contract?> GetContractAsync(int id)
        {
            AttachToken();
            var response = await _httpClient.GetAsync($"/api/contracts/{id}");
            if (!response.IsSuccessStatusCode) return null;
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<Contract>(json, _jsonOptions);
        }

        public async Task<bool> CreateContractAsync(Contract contract)
        {
            AttachToken();
            var response = await _httpClient.PostAsync("/api/contracts", ToJson(contract));
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateContractAsync(Contract contract)
        {
            AttachToken();
            var response = await _httpClient.PutAsync(
                $"/api/contracts/{contract.Id}", ToJson(contract));
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateContractStatusAsync(int id, ContractStatus newStatus)
        {
            AttachToken();
            var response = await _httpClient.PatchAsync(
                $"/api/contracts/{id}/status", ToJson(newStatus));
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteContractAsync(int id)
        {
            AttachToken();
            var response = await _httpClient.DeleteAsync($"/api/contracts/{id}");
            return response.IsSuccessStatusCode;
        }

        // ── Service Requests ──────────────────────────────────────

        public async Task<List<ServiceRequest>> GetServiceRequestsAsync()
        {
            AttachToken();
            var response = await _httpClient.GetAsync("/api/servicerequests");
            if (!response.IsSuccessStatusCode) return new List<ServiceRequest>();
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<ServiceRequest>>(json, _jsonOptions)
                   ?? new List<ServiceRequest>();
        }

        public async Task<ServiceRequest?> GetServiceRequestAsync(int id)
        {
            AttachToken();
            var response = await _httpClient.GetAsync($"/api/servicerequests/{id}");
            if (!response.IsSuccessStatusCode) return null;
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<ServiceRequest>(json, _jsonOptions);
        }

        public async Task<(bool success, string? error)> CreateServiceRequestAsync(
            ServiceRequest serviceRequest)
        {
            AttachToken();
            var response = await _httpClient.PostAsync(
                "/api/servicerequests", ToJson(serviceRequest));
            if (response.IsSuccessStatusCode) return (true, null);
            var error = await response.Content.ReadAsStringAsync();
            return (false, error);
        }

        public async Task<bool> DeleteServiceRequestAsync(int id)
        {
            AttachToken();
            var response = await _httpClient.DeleteAsync($"/api/servicerequests/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}