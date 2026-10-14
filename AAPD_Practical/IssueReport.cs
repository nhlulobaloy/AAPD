namespace AAPD_Practical;

/// <summary>Represents one citizen-submitted municipal issue.</summary>
public sealed class IssueReport
{
    public string ReferenceNumber { get; init; } = string.Empty;
    public string Location { get; init; } = string.Empty;
    public string Category { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public DateTime SubmittedAt { get; init; }
    public List<string> Attachments { get; init; } = [];
}
