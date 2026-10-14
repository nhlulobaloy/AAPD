namespace AAPD_Practical;

internal static class EventRepositorySelfTests
{
    public static void Run()
    {
        var repository = new EventRepository();
        Assert(repository.Categories.Count >= 5, "Expected unique event categories.");
        Assert(repository.GetUpcoming().Count > 0, "Expected priority-ordered upcoming events.");
        var category = repository.Categories.First();
        var matches = repository.Search(category, null);
        Assert(matches.Count > 0 && matches.All(localEvent => localEvent.Category == category), "Category search returned invalid results.");
        Assert(repository.GetRecommendations().Count > 0, "Expected recommendations after a search.");
    }

    private static void Assert(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
    }
}
