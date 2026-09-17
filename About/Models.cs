namespace Organizer.About;

public class Contributor
{
    public string Role { get; set; }
    public string Name { get; set; }
}

public class ContributorList
{
    public List<Contributor> Contributors { get; set; } = [];
}