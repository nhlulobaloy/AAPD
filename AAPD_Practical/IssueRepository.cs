namespace AAPD_Practical;

/// <summary>In-memory store for the current application session.</summary>
public sealed class IssueRepository
{
    private readonly List<IssueReport> _issues = [];

    public IReadOnlyList<IssueReport> Issues => _issues;

    public void Add(IssueReport issue) => _issues.Add(issue);
}
