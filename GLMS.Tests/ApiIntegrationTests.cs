using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using Xunit;

namespace GLMS.Tests
{
    public class ApiIntegrationTests : IAsyncLifetime
    {
        private readonly HttpClient _client;
        private string? _token;

        // Change this port to match your GLMS.API port
        private const string BaseUrl = "http://localhost:5261";

        public ApiIntegrationTests()
        {
            _client = new HttpClient { BaseAddress = new Uri(BaseUrl) };
        }

        // Runs before every test — logs in and gets a token
        public async Task InitializeAsync()
        {
            _token = await GetTokenAsync();
            if (_token != null)
                _client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _token);
        }

        public Task DisposeAsync()
        {
            _client.Dispose();
            return Task.CompletedTask;
        }

        private async Task<string?> GetTokenAsync()
        {
            try
            {
                var payload = new StringContent(
                    JsonSerializer.Serialize(new { username = "admin", password = "admin123" }),
                    Encoding.UTF8, "application/json");

                var response = await _client.PostAsync("/api/auth/login", payload);
                if (!response.IsSuccessStatusCode) return null;

                var json = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(json);
                return doc.RootElement.GetProperty("token").GetString();
            }
            catch
            {
                return null;
            }
        }

        // ── Auth Tests ────────────────────────────────────────────

        [Fact]
        public async Task Login_ValidCredentials_Returns200WithToken()
        {
            var payload = new StringContent(
                JsonSerializer.Serialize(new { username = "admin", password = "admin123" }),
                Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("/api/auth/login", payload);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);
            Assert.True(doc.RootElement.TryGetProperty("token", out _),
                "Response should contain a token property");
        }

        [Fact]
        public async Task Login_InvalidCredentials_Returns401()
        {
            var payload = new StringContent(
                JsonSerializer.Serialize(new { username = "wrong", password = "wrong" }),
                Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("/api/auth/login", payload);

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        // ── Clients Tests ─────────────────────────────────────────

        [Fact]
        public async Task GetClients_WithValidToken_Returns200()
        {
            var response = await _client.GetAsync("/api/clients");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetClients_ResponseIsNotNull()
        {
            var response = await _client.GetAsync("/api/clients");
            var json = await response.Content.ReadAsStringAsync();

            Assert.NotNull(json);
            Assert.NotEmpty(json);
        }

        [Fact]
        public async Task GetClients_ReturnsJsonArray()
        {
            var response = await _client.GetAsync("/api/clients");
            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);

            Assert.Equal(JsonValueKind.Array, doc.RootElement.ValueKind);
        }

        [Fact]
        public async Task GetClients_WithoutToken_Returns401()
        {
            // Create a separate client with no token
            using var unauthClient = new HttpClient { BaseAddress = new Uri(BaseUrl) };
            var response = await unauthClient.GetAsync("/api/clients");

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        // ── Contracts Tests ───────────────────────────────────────

        [Fact]
        public async Task GetContracts_WithValidToken_Returns200()
        {
            var response = await _client.GetAsync("/api/contracts");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetContracts_ReturnsJsonArray()
        {
            var response = await _client.GetAsync("/api/contracts");
            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);

            Assert.Equal(JsonValueKind.Array, doc.RootElement.ValueKind);
        }

        [Fact]
        public async Task GetContracts_WithStatusFilter_Returns200()
        {
            var response = await _client.GetAsync("/api/contracts?status=Active");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetContracts_WithoutToken_Returns401()
        {
            using var unauthClient = new HttpClient { BaseAddress = new Uri(BaseUrl) };
            var response = await unauthClient.GetAsync("/api/contracts");

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        // ── Service Requests Tests ────────────────────────────────

        [Fact]
        public async Task GetServiceRequests_WithValidToken_Returns200()
        {
            var response = await _client.GetAsync("/api/servicerequests");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetServiceRequests_ReturnsJsonArray()
        {
            var response = await _client.GetAsync("/api/servicerequests");
            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);

            Assert.Equal(JsonValueKind.Array, doc.RootElement.ValueKind);
        }

        [Fact]
        public async Task GetServiceRequests_WithoutToken_Returns401()
        {
            using var unauthClient = new HttpClient { BaseAddress = new Uri(BaseUrl) };
            var response = await unauthClient.GetAsync("/api/servicerequests");

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }
    }
}