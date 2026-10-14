using System.Drawing;
using System.Windows.Forms;

namespace AAPD_Practical;

public sealed class ReportIssueForm : Form
{
    private readonly IssueRepository _repository;
    private readonly TextBox _location = new();
    private readonly ComboBox _category = new();
    private readonly RichTextBox _description = new();
    private readonly ListBox _attachments = new();
    private readonly Label _engagement = new();
    private readonly ProgressBar _progress = new();

    public ReportIssueForm(IssueRepository repository)
    {
        _repository = repository;
        Text = "Report an Issue";
        StartPosition = FormStartPosition.CenterParent;
        MinimumSize = new Size(760, 620);
        Size = new Size(900, 720);
        BackColor = Color.FromArgb(244, 247, 250);

        _category.Items.AddRange(["Sanitation and waste", "Roads and potholes", "Water and utilities", "Streetlights", "Public safety", "Other"]);
        _category.DropDownStyle = ComboBoxStyle.DropDownList;

        var layout = new TableLayoutPanel { Dock = DockStyle.Fill, AutoScroll = true, Padding = new Padding(38, 25, 38, 25), ColumnCount = 1 };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        AddField(layout, "Report a Municipal Issue", CreateHeading());
        AddField(layout, "Location *", _location);
        AddField(layout, "Category *", _category);
        _description.Height = 115;
        _description.Dock = DockStyle.Top;
        AddField(layout, "Description *", _description);

        var attachmentPanel = new FlowLayoutPanel { Dock = DockStyle.Top, AutoSize = true, FlowDirection = FlowDirection.LeftToRight };
        var attachButton = new Button { Text = "Attach images or documents", AutoSize = true, BackColor = Color.White, FlatStyle = FlatStyle.Flat };
        attachButton.Click += AttachFiles;
        _attachments.Width = 480;
        _attachments.Height = 70;
        attachmentPanel.Controls.Add(attachButton);
        attachmentPanel.Controls.Add(_attachments);
        AddField(layout, "Attachments (optional)", attachmentPanel);

        _engagement.Text = "Every report helps make our community safer and better.";
        _engagement.AutoSize = true;
        _engagement.Font = new Font("Segoe UI", 10, FontStyle.Italic);
        _engagement.ForeColor = Color.FromArgb(20, 73, 112);
        AddField(layout, "", _engagement);
        _progress.Minimum = 0;
        _progress.Maximum = 3;
        _progress.Dock = DockStyle.Top;
        AddField(layout, "Report completion", _progress);

        var actions = new FlowLayoutPanel { Dock = DockStyle.Top, AutoSize = true, FlowDirection = FlowDirection.RightToLeft };
        var submit = new Button { Text = "Submit Report", AutoSize = true, BackColor = Color.FromArgb(27, 116, 166), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Padding = new Padding(12, 6, 12, 6) };
        submit.Click += SubmitReport;
        var back = new Button { Text = "Back to Main Menu", AutoSize = true, FlatStyle = FlatStyle.Flat, Padding = new Padding(12, 6, 12, 6) };
        back.Click += (_, _) => Close();
        actions.Controls.Add(submit);
        actions.Controls.Add(back);
        AddField(layout, "", actions);

        _location.TextChanged += (_, _) => UpdateProgress();
        _category.SelectedIndexChanged += (_, _) => UpdateProgress();
        _description.TextChanged += (_, _) => UpdateProgress();
        Controls.Add(layout);
    }

    private Control CreateHeading() => new Label { Text = "Provide the details below and we will route your report to the right team.", Font = new Font("Segoe UI", 16, FontStyle.Bold), ForeColor = Color.FromArgb(20, 73, 112), AutoSize = true };

    private static void AddField(TableLayoutPanel layout, string label, Control input)
    {
        if (!string.IsNullOrEmpty(label))
            layout.Controls.Add(new Label { Text = label, Font = new Font("Segoe UI", 10, FontStyle.Bold), AutoSize = true, Margin = new Padding(0, 13, 0, 4) });
        layout.Controls.Add(input);
    }

    private void UpdateProgress()
    {
        _progress.Value = Convert.ToInt32(!string.IsNullOrWhiteSpace(_location.Text)) + Convert.ToInt32(_category.SelectedIndex >= 0) + Convert.ToInt32(!string.IsNullOrWhiteSpace(_description.Text));
        _engagement.Text = _progress.Value == 3 ? "Great work — your report is ready to submit." : "Every report helps make our community safer and better.";
    }

    private void AttachFiles(object? sender, EventArgs e)
    {
        using var dialog = new OpenFileDialog { Multiselect = true, Filter = "Images and documents|*.jpg;*.jpeg;*.png;*.gif;*.pdf;*.doc;*.docx|All files|*.*", Title = "Choose attachments" };
        if (dialog.ShowDialog(this) == DialogResult.OK)
            foreach (var file in dialog.FileNames.Where(file => !_attachments.Items.Contains(file))) _attachments.Items.Add(file);
    }

    private void SubmitReport(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(_location.Text) || _category.SelectedIndex < 0 || string.IsNullOrWhiteSpace(_description.Text))
        {
            MessageBox.Show(this, "Please complete location, category, and description before submitting.", "Details required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        var issue = new IssueReport
        {
            ReferenceNumber = $"ISS-{DateTime.Now:yyyyMMdd-HHmmss}", Location = _location.Text.Trim(), Category = _category.Text,
            Description = _description.Text.Trim(), SubmittedAt = DateTime.Now,
            Attachments = _attachments.Items.Cast<string>().ToList()
        };
        _repository.Add(issue);
        MessageBox.Show(this, $"Thank you. Your report has been submitted.\n\nReference: {issue.ReferenceNumber}", "Report submitted", MessageBoxButtons.OK, MessageBoxIcon.Information);
        Close();
    }
}
