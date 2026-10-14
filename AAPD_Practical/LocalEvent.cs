namespace AAPD_Practical;

/// <summary>Represents a municipal event or public announcement.</summary>
public sealed record LocalEvent(string Title, string Category, DateTime Date, string Venue, string Description, bool IsAnnouncement = false);

/// <summary>Captures one event search so recommendations can reflect resident interests.</summary>
public sealed record EventSearch(string? Category, DateTime? Date);
