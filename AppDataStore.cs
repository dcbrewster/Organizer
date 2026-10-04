using System.Text.Json;

namespace Organizer;

public sealed class AppDataStore
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true
    };

    public string DataFilePath { get; }

    public AppDataStore()
    {
        string? folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Organizer");

        Directory.CreateDirectory(folder);
        DataFilePath = Path.Combine(folder, "organizer-data.json");
    }

    public OrganizerData Load() => LoadFrom(DataFilePath, backupCorruptFile: true);

    public void Save(OrganizerData data) => SaveTo(data, DataFilePath);

    public OrganizerData LoadFrom(string path, bool backupCorruptFile = false)
    {
        if(!File.Exists(path)) return new OrganizerData();

        try
        {
            string? json = File.ReadAllText(path);

            return JsonSerializer.Deserialize<OrganizerData>(json, JsonOptions) ?? new OrganizerData();
        }
        catch
        {
            if(backupCorruptFile)
            {
                string? backupPath = path + ".corrupt-" + DateTime.Now.ToString("yyyyMMddHHmmss");

                File.Copy(path, backupPath, overwrite: true);
            }

            return new OrganizerData();
        }
    }

    public void SaveTo(OrganizerData data, string path)
    {
        string? json = JsonSerializer.Serialize(data, JsonOptions);

        File.WriteAllText(path, json);
    }
}