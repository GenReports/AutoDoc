using System.Text.Json;

namespace AutoDoc.Core.Services
{
    public static class JsonStorageService
    {
        private static readonly string _folderPath =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "AutoDoc");

        private readonly static JsonSerializerOptions _jsonSerializerOptions = new()
        {
            PropertyNameCaseInsensitive = true,
        };

        public static async Task<List<T>> LoadAsync<T>(string fileName, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(fileName);

            var filePath = GetFilePath(fileName);
            if (!File.Exists(filePath))
                return [];

            var json = await File.ReadAllTextAsync(filePath, cancellationToken);
            return JsonSerializer.Deserialize<List<T>>(json, _jsonSerializerOptions) ?? [];
        }

        public static async Task SaveAsync<T>(string fileName, List<T> datas, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(fileName);

            if (datas is null || datas.Count is 0)
                return;

            var filePath = GetFilePath(fileName);
            var json = JsonSerializer.Serialize(datas, _jsonSerializerOptions);
            await File.WriteAllTextAsync(filePath, json, cancellationToken);
        }

        public static async Task InitializeAsync<T>(string fileName, List<T> datas, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(fileName);

            if (datas is null || datas.Count is 0)
                return;

            var filePath = GetFilePath(fileName);
            FileInfo fileInfo = new(filePath);

            if (fileInfo.Exists)
                return;

            var json = JsonSerializer.Serialize(datas, _jsonSerializerOptions);
            await File.WriteAllTextAsync(filePath, json, cancellationToken);
        }

        private static string GetFilePath(string fileName)
        {
            ArgumentNullException.ThrowIfNull(fileName);

            var path = Path.Combine(_folderPath, fileName);
            var dir = Path.GetDirectoryName(path)!;

            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            return path;
        }
    }
}
