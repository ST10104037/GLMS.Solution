namespace GLMS.Services
{
    public class FileValidationService
    {
        private static readonly string[] AllowedExtensions = { ".pdf" };

        public static bool IsValidFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return false;

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            return AllowedExtensions.Contains(extension);
        }

        public static void ValidateOrThrow(IFormFile file)
        {
            if (!IsValidFile(file))
                throw new InvalidOperationException(
                    "Only PDF files are allowed. Please upload a .pdf file.");
        }

        public static async Task<string> SaveFileAsync(IFormFile file, string uploadsFolder)
        {
            ValidateOrThrow(file);

            Directory.CreateDirectory(uploadsFolder);
            var uniqueName = $"{Guid.NewGuid()}_{file.FileName}";
            var filePath = Path.Combine(uploadsFolder, uniqueName);

            using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);

            return uniqueName;
        }
    }
}