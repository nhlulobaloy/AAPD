using System.Drawing;
using System.Windows.Forms;

namespace AAPD_Practical;

public sealed class LocalEventsForm : Form
{
    private readonly EventRepository _repository;
    private readonly ComboBox _category = new();
    private readonly DateTimePicker _date = new();
    private readonly CheckBox _useDate = new() { Text = "Filter by date", AutoSize = true };
    private readonly ListView _events = CreateListView();
    private readonly ListView _recommendations = CreateListView();
    private readonly Label _resultsLabel = new() { AutoSize = true, Font = new Font("Segoe UI", 10, FontStyle.Bold) };

    public LocalEventsForm(EventRepository repository)
    {
        _repository = repository;
        Text = "Local Events and Announcements";
        StartPosition = FormStartPosition.CenterParent;
        MinimumSize = new Size(920, 650);
        Size = new Size(1060, 760);
        BackColor = Color.FromArgb(244, 247, 250);
        _category.DropDownStyle = ComboBoxStyle.DropDownList;
        _category.Items.Add("All categories");
        _category.Items.AddRange(_repository.Categories.Cast<object>().ToArray());
        _category.SelectedIndex = 0;
        _date.Format = DateTimePickerFormat.Short;
        _date.Enabled = false;
        _useDate.CheckedChanged += (_, _) => _date.Enabled = _useDate.Checked;

        var root = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(32, 24, 32, 24), ColumnCount = 1, AutoScroll = true };
        root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        root.Controls.Add(new Label { Text = "Local Events and Announcements", Font = new Font("Segoe UI", 22, FontStyle.Bold), ForeColor = Color.FromArgb(20, 73, 112), AutoSize = true });
        root.Controls.Add(new Label { Text = "Search municipal activities and notices. Your searches personalise recommendations.", Font = new Font("Segoe UI", 10), AutoSize = true, Margin = new Padding(0, 4, 0, 16) });
        var filters = new FlowLayoutPanel { Dock = DockStyle.Top, AutoSize = true, WrapContents = true, Margin = new Padding(0, 0, 0, 12) };
        filters.Controls.Add(new Label { Text = "Category", AutoSize = true, Margin = new Padding(0, 8, 5, 0) });
        filters.Controls.Add(_category); filters.Controls.Add(_useDate); filters.Controls.Add(_date);
        var search = new Button { Text = "Search events", AutoSize = true, BackColor = Color.FromArgb(27, 116, 166), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Margin = new Padding(12, 3, 0, 0) };
        search.Click += (_, _) => RunSearch(); filters.Controls.Add(search); root.Controls.Add(filters);
        root.Controls.Add(_resultsLabel); ConfigureColumns(_events); _events.Height = 250; root.Controls.Add(_events);
        root.Controls.Add(new Label { Text = "Recommended for you", Font = new Font("Segoe UI", 13, FontStyle.Bold), ForeColor = Color.FromArgb(20, 73, 112), AutoSize = true, Margin = new Padding(0, 18, 0, 5) });
        ConfigureColumns(_recommendations); _recommendations.Height = 160; root.Controls.Add(_recommendations);
        var back = new Button { Text = "Back to Main Menu", AutoSize = true, FlatStyle = FlatStyle.Flat, Margin = new Padding(0, 16, 0, 0) };
        back.Click += (_, _) => Close(); root.Controls.Add(back); Controls.Add(root);
        ShowEvents(_repository.GetUpcoming(), "Upcoming events and announcements"); ShowRecommendations(_repository.GetRecommendations());
    }

    private void RunSearch()
    {
        var results = _repository.Search(_category.Text, _useDate.Checked ? _date.Value.Date : null);
        ShowEvents(results, $"{results.Count} event(s) found"); ShowRecommendations(_repository.GetRecommendations());
    }

    private void ShowEvents(IReadOnlyList<LocalEvent> items, string heading) { _resultsLabel.Text = heading; FillList(_events, items); }
    private void ShowRecommendations(IReadOnlyList<LocalEvent> items) => FillList(_recommendations, items);
    private static ListView CreateListView() => new() { Dock = DockStyle.Top, View = View.Details, FullRowSelect = true, GridLines = true, MultiSelect = false, BackColor = Color.White };
    private static void ConfigureColumns(ListView list) { list.Columns.Add("Date", 130); list.Columns.Add("Title", 220); list.Columns.Add("Category", 145); list.Columns.Add("Venue", 150); list.Columns.Add("Details", 330); }
    private static void FillList(ListView list, IReadOnlyList<LocalEvent> items)
    {
        list.BeginUpdate(); list.Items.Clear();
        foreach (var localEvent in items)
        {
            var item = new ListViewItem(localEvent.Date.ToString("ddd, dd MMM yyyy HH:mm"));
            item.SubItems.Add(localEvent.IsAnnouncement ? $"Announcement: {localEvent.Title}" : localEvent.Title);
            item.SubItems.Add(localEvent.Category); item.SubItems.Add(localEvent.Venue); item.SubItems.Add(localEvent.Description); list.Items.Add(item);
        }
        list.EndUpdate();
    }
}
