using GLMS.Services;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace GLMS.Tests
{
    public class BusinessLogicTests
    {
        // ── Currency Tests ──────────────────────────────────────────

        [Fact]
        public void ConvertUsdToZar_CorrectCalculation()
        {
            decimal usd = 100m;
            decimal rate = 18.50m;
            decimal expected = 1850.00m;

            decimal result = CurrencyService.ConvertUsdToZar(usd, rate);

            Assert.Equal(expected, result);
        }

        [Fact]
        public void ConvertUsdToZar_ZeroAmount_ReturnsZero()
        {
            decimal result = CurrencyService.ConvertUsdToZar(0m, 18.50m);
            Assert.Equal(0m, result);
        }

        [Fact]
        public void ConvertUsdToZar_RoundsToTwoDecimalPlaces()
        {
            decimal result = CurrencyService.ConvertUsdToZar(1m, 18.333333m);
            Assert.Equal(18.33m, result);
        }

        [Fact]
        public void ConvertUsdToZar_LargeAmount_CalculatesCorrectly()
        {
            decimal result = CurrencyService.ConvertUsdToZar(1000m, 18.50m);
            Assert.Equal(18500.00m, result);
        }

        // ── File Validation Tests ───────────────────────────────────

        [Fact]
        public void IsValidFile_PdfFile_ReturnsTrue()
        {
            var file = CreateMockFile("agreement.pdf", "application/pdf");
            Assert.True(FileValidationService.IsValidFile(file));
        }

        [Fact]
        public void IsValidFile_ExeFile_ReturnsFalse()
        {
            var file = CreateMockFile("malware.exe", "application/octet-stream");
            Assert.False(FileValidationService.IsValidFile(file));
        }

        [Fact]
        public void IsValidFile_DocxFile_ReturnsFalse()
        {
            var file = CreateMockFile("doc.docx", "application/vnd.openxmlformats");
            Assert.False(FileValidationService.IsValidFile(file));
        }

        [Fact]
        public void IsValidFile_NullFile_ReturnsFalse()
        {
            Assert.False(FileValidationService.IsValidFile(null));
        }

        [Fact]
        public void ValidateOrThrow_ExeFile_ThrowsException()
        {
            var file = CreateMockFile("virus.exe", "application/octet-stream");
            Assert.Throws<InvalidOperationException>(
                () => FileValidationService.ValidateOrThrow(file));
        }

        [Fact]
        public void ValidateOrThrow_PdfFile_DoesNotThrow()
        {
            var file = CreateMockFile("contract.pdf", "application/pdf");
            var ex = Record.Exception(() => FileValidationService.ValidateOrThrow(file));
            Assert.Null(ex);
        }

        [Fact]
        public void ValidateOrThrow_ExeFile_ErrorMessageMentionsPdf()
        {
            var file = CreateMockFile("virus.exe", "application/octet-stream");
            var ex = Assert.Throws<InvalidOperationException>(
                () => FileValidationService.ValidateOrThrow(file));
            Assert.Contains("PDF", ex.Message);
        }

        // ── Helper ──────────────────────────────────────────────────

        private static IFormFile CreateMockFile(string fileName, string contentType)
        {
            var content = new byte[] { 1, 2, 3 };
            var stream = new MemoryStream(content);
            return new FormFile(stream, 0, content.Length, "file", fileName)
            {
                Headers = new HeaderDictionary(),
                ContentType = contentType
            };
        }
    }
}