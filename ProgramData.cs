namespace Organizer;

public sealed class ProgramData
{
    public OrganizerData Data { get; private set; }

    private readonly AppDataStore _store = new();

    private ProgramData()
    {
        Data = _store.Load();
    }

    public static ProgramData Instance { get; } = new ProgramData();

    public void Save()
    {
        _store.Save(Data);
    }
}