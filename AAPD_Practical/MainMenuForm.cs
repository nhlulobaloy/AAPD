using System.Drawing;
using System.Windows.Forms;

namespace AAPD_Practical;

public sealed class MainMenuForm : Form
{
    private readonly IssueRepository _repository = new();
    private readonly EventRepository _eventRepository = new();

    public MainMenuForm()
    {
        Text = "Municipal Services Portal";
        StartPosition = FormStartPosition.CenterScreen;
        MinimumSize = new Size(720, 480);
        Size = new Size(900, 600);
        BackColor = Color.FromArgb(244, 247, 250);

        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(48, 36, 48, 36),
            ColumnCount = 1,
            RowCount = 6
        };
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));

        var title = new Label
        {
            Text = "Municipal Services Portal",
            Font = new Font("Segoe UI", 24, FontStyle.Bold),
            ForeColor = Color.FromArgb(20, 73, 112),
            AutoSize = true,
            Margin = new Padding(0, 0, 0, 8)
        };
        var intro = new Label
        {
            Text = "Welcome. Select a service to get started.",
            Font = new Font("Segoe UI", 11),
            AutoSize = true,
            Margin = new Padding(0, 0, 0, 30)
        };

        layout.Controls.Add(title, 0, 0);
        layout.Controls.Add(intro, 0, 1);
        layout.Controls.Add(CreateButton("Report an Issue", "Tell us about a municipal issue in your area.", true, (_, _) => OpenIssueForm()), 0, 3);
        layout.Controls.Add(CreateButton("Local Events and Announcements", "Find activities and important municipal notices.", true, (_, _) => OpenEventsForm()), 0, 4);
        layout.Controls.Add(CreateButton("Service Request Status", "Coming soon", false, null), 0, 5);
        Controls.Add(layout);
    }

    private void OpenIssueForm()
    {
        Hide();
        using var form = new ReportIssueForm(_repository);
        form.ShowDialog(this);
        Show();
    }

    private void OpenEventsForm()
    {
        Hide();
        using var form = new LocalEventsForm(_eventRepository);
        form.ShowDialog(this);
        Show();
    }

    private static Button CreateButton(string text, string subtitle, bool enabled, EventHandler? click)
    {
        var button = new Button
        {
            Text = $"{text}\r\n{subtitle}",
            TextAlign = ContentAlignment.MiddleLeft,
            Font = new Font("Segoe UI", 12, FontStyle.Bold),
            Height = 76,
            Dock = DockStyle.Top,
            Enabled = enabled,
            FlatStyle = FlatStyle.Flat,
            BackColor = enabled ? Color.FromArgb(27, 116, 166) : Color.FromArgb(210, 215, 220),
            ForeColor = enabled ? Color.White : Color.DimGray,
            Margin = new Padding(0, 0, 0, 12)
        };
        button.FlatAppearance.BorderSize = 0;
        if (click is not null) button.Click += click;
        return button;
    }
}
