using System.Text.Json;

namespace GLMS.Services
{
    public class CurrencyService
    {
        private readonly HttpClient _httpClient;

        public CurrencyService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<decimal> GetUsdToZarRateAsync()
        {
            try
            {
                // Free API — no key needed for basic use
                var response = await _httpClient.GetStringAsync(
                    "https://api.exchangerate-api.com/v4/latest/USD");

                using var doc = JsonDocument.Parse(response);
                var rate = doc.RootElement
                    .GetProperty("rates")
                    .GetProperty("ZAR")
                    .GetDecimal();

                return rate;
            }
            catch
            {
                // Fallback rate if API is unavailable
                return 18.50m;
            }
        }

        // This method is static so tests can call it without HttpClient
        public static decimal ConvertUsdToZar(decimal usdAmount, decimal rate)
        {
            return Math.Round(usdAmount * rate, 2);
        }
    }
}