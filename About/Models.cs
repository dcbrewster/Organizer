namespace Organizer.About;

public class Contributor
{
    public string Role { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}

public class ContributorList
{
    public List<Contributor> Contributors { get; set; } = [];
}