using System.Text.Json;

namespace SportsLeague.Core.Services;


/// Забезпечує збереження та завантаження колекцій у форматі JSON.

public class JsonFileService
{
    private readonly JsonSerializerOptions _options = new()
    {
        WriteIndented = true
    };

    public string DirectoryPath { get; }

    public JsonFileService(string directoryPath)
    {
        if (string.IsNullOrWhiteSpace(directoryPath))
            throw new ArgumentException("Шлях до папки не може бути порожнім.", nameof(directoryPath));

        DirectoryPath = Path.GetFullPath(directoryPath);
    }

    public List<T> Load<T>(string fileName)
    {
        string filePath = GetFilePath(fileName);

        if (!File.Exists(filePath))
            return new List<T>();

        try
        {
            string json = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<List<T>>(json, _options) ?? new List<T>();
        }
        catch (JsonException exception)
        {
            throw new InvalidDataException($"Файл {fileName} містить некоректні дані JSON.", exception);
        }
    }

    public void Save<T>(string fileName, IReadOnlyCollection<T> items)
    {
        Directory.CreateDirectory(DirectoryPath);

        string filePath = GetFilePath(fileName);
        string json = JsonSerializer.Serialize(items, _options);
        File.WriteAllText(filePath, json);
    }

    private string GetFilePath(string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName) || Path.GetFileName(fileName) != fileName)
            throw new ArgumentException("Потрібно вказати лише назву файлу без шляху.", nameof(fileName));

        return Path.Combine(DirectoryPath, fileName);
    }
}
