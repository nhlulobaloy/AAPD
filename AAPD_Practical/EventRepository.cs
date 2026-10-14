namespace AAPD_Practical;

/// <summary>In-memory catalogue using the Part 2 required data structures.</summary>
public sealed class EventRepository
{
    private readonly SortedDictionary<DateOnly, List<LocalEvent>> _eventsByDate = new();
    private readonly Dictionary<string, List<LocalEvent>> _eventsByCategory = new(StringComparer.OrdinalIgnoreCase);
    private readonly HashSet<string> _categories = new(StringComparer.OrdinalIgnoreCase);
    private readonly Queue<EventSearch> _recentSearches = new();
    private readonly Stack<EventSearch> _searchHistory = new();
    private readonly PriorityQueue<LocalEvent, DateTime> _upcomingEvents = new();

    public EventRepository() => SeedEvents().ForEach(Add);

    public IReadOnlyCollection<string> Categories => _categories.OrderBy(category => category).ToArray();

    public IReadOnlyList<LocalEvent> Search(string? category, DateTime? date)
    {
        category = string.IsNullOrWhiteSpace(category) || category == "All categories" ? null : category;
        DateOnly? day = date.HasValue ? DateOnly.FromDateTime(date.Value) : null;
        RecordSearch(new EventSearch(category, date));
        IEnumerable<LocalEvent> results = _eventsByDate.SelectMany(pair => pair.Value);
        if (category is not null && _eventsByCategory.TryGetValue(category, out var categoryEvents)) results = categoryEvents;
        else if (category is not null) results = [];
        if (day is not null) results = results.Where(localEvent => DateOnly.FromDateTime(localEvent.Date) == day.Value);
        return results.OrderBy(localEvent => localEvent.Date).ThenBy(localEvent => localEvent.Title).ToArray();
    }

    public IReadOnlyList<LocalEvent> GetRecommendations(int maximum = 3)
    {
        if (_searchHistory.Count == 0) return GetUpcoming(maximum);
        var interest = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        foreach (var search in _searchHistory)
            if (search.Category is not null) interest[search.Category] = interest.GetValueOrDefault(search.Category) + 1;
        var results = _eventsByDate.SelectMany(pair => pair.Value).Where(localEvent => localEvent.Date.Date >= DateTime.Today)
            .OrderByDescending(localEvent => interest.GetValueOrDefault(localEvent.Category)).ThenBy(localEvent => localEvent.Date).ThenBy(localEvent => localEvent.Title).Take(maximum).ToArray();
        return results.Length > 0 ? results : GetUpcoming(maximum);
    }

    public IReadOnlyList<LocalEvent> GetUpcoming(int maximum = 5)
    {
        var copy = new PriorityQueue<LocalEvent, DateTime>(_upcomingEvents.UnorderedItems);
        var results = new List<LocalEvent>();
        while (copy.TryDequeue(out var localEvent, out _) && results.Count < maximum) results.Add(localEvent);
        return results;
    }

    private void Add(LocalEvent localEvent)
    {
        var day = DateOnly.FromDateTime(localEvent.Date);
        if (!_eventsByDate.TryGetValue(day, out var datedEvents)) _eventsByDate[day] = datedEvents = [];
        datedEvents.Add(localEvent);
        if (!_eventsByCategory.TryGetValue(localEvent.Category, out var categoryEvents)) _eventsByCategory[localEvent.Category] = categoryEvents = [];
        categoryEvents.Add(localEvent);
        _categories.Add(localEvent.Category);
        _upcomingEvents.Enqueue(localEvent, localEvent.Date);
    }

    private void RecordSearch(EventSearch search)
    {
        _searchHistory.Push(search);
        _recentSearches.Enqueue(search);
        if (_recentSearches.Count > 10) _recentSearches.Dequeue();
    }

    private static List<LocalEvent> SeedEvents() =>
    [
        new("Community Clean-up Day", "Community", DateTime.Today.AddDays(3).AddHours(9), "Mhlabeni Park", "Join neighbours to clean public spaces. Gloves and bags are supplied."),
        new("Water Supply Maintenance", "Utilities", DateTime.Today.AddDays(5).AddHours(8), "Ward 4", "Planned maintenance may affect water supply between 08:00 and 15:00.", true),
        new("Youth Sports Festival", "Sports and Recreation", DateTime.Today.AddDays(8).AddHours(10), "Civic Sports Grounds", "A free day of local sport, games, and family activities."),
        new("Public Safety Meeting", "Public Safety", DateTime.Today.AddDays(11).AddHours(18), "Town Hall", "Meet community safety representatives and discuss local priorities."),
        new("Recycling Drop-off Drive", "Environment", DateTime.Today.AddDays(14).AddHours(9), "Municipal Depot", "Bring paper, plastic, glass, and e-waste for responsible recycling."),
        new("Roadworks Notice", "Roads and Transport", DateTime.Today.AddDays(18).AddHours(7), "Main Road", "Lane closures are planned while road resurfacing is completed.", true),
        new("Heritage Market", "Community", DateTime.Today.AddDays(21).AddHours(10), "Market Square", "Celebrate local food, crafts, and culture with the community.")
    ];
}
