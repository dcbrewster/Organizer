using System.ComponentModel;
using System.Text;

namespace Organizer;

public sealed partial class CreateAppointmentDialog : DialogBase
{
    private readonly CalendarEvent _appointment;
    private readonly IReadOnlyCollection<CalendarEvent> _allAppointments;

    private readonly DateTimePicker _date = new()
    {
        Format = DateTimePickerFormat.Short,
        Width = 92
    };

    private readonly DateTimePicker _time = new()
    {
        Format = DateTimePickerFormat.Custom,
        CustomFormat = "h:mm tt",
        ShowUpDown = true,
        Width = 74
    };

    private readonly DomainUpDown _duration = new()
    {
        ReadOnly = true,
        TextAlign = HorizontalAlignment.Right,
        Width = 54
    };

    private readonly TextBox _description = new() { Width = 360, Height = 86, Multiline = true, ScrollBars = ScrollBars.Vertical };
    private readonly ComboBox _categories = new() { Width = 220 };
    private readonly CheckBox _warnOfConflicts = new() { Text = "&Warn of conflicts", AutoSize = true };
    private readonly CheckBox _pencilIn = new() { Text = "&Pencil in", AutoSize = true };
    private readonly CheckBox _confidential = new() { Text = "Con&fidential", AutoSize = true };

    public CreateAppointmentDialog()
        : this(new CalendarEvent(), "Create Appointment", Array.Empty<CalendarEvent>())
    {
    }

    private CreateAppointmentDialog(CalendarEvent appointment, string title, IReadOnlyCollection<CalendarEvent>? allAppointments)
    {
        InitializeComponent();
        _appointment = appointment;
        _allAppointments = allAppointments ?? Array.Empty<CalendarEvent>();

        if(LicenseManager.UsageMode == LicenseUsageMode.Designtime) return;

        Controls.Clear();
        Text = title;
        Width = 560;
        Height = 330;
        StartPosition = FormStartPosition.CenterParent;
        // Dialog behavior: modal dialog appearance without taskbar icon
        ShowInTaskbar = false;
        ShowIcon = false;
        MinimizeBox = false;
        MaximizeBox = false;
        FormBorderStyle = FormBorderStyle.FixedDialog;

        _categories.DropDownStyle = ComboBoxStyle.DropDown;
        _categories.Items.AddRange(new object[] { "Business", "Personal", "Holiday", "Travel", "Phone Call", "Meeting" });
        LoadDurationValues();

        TableLayoutPanel? body = new()
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 5,
            Padding = new Padding(12),
            AutoSize = true
        };

        body.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 95));
        body.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

        FlowLayoutPanel? schedulePanel = BuildSchedulePanel();
        body.Controls.Add(schedulePanel, 0, 0);
        body.SetColumnSpan(schedulePanel, 2);
        AddRow(body, 1, "D&escription", _description);
        AddRow(body, 2, "&Categories", _categories);

        FlowLayoutPanel? flags = new() { FlowDirection = FlowDirection.LeftToRight, AutoSize = true, Dock = DockStyle.Fill };
        flags.Controls.AddRange(new Control[] { _warnOfConflicts, _pencilIn, _confidential });
        body.Controls.Add(flags, 1, 3);

        Button? linkButton = new() { Text = "Link to", Width = 90 };
        linkButton.Click += (_, _) => ShowDialogStub("Link to");
        body.Controls.Add(linkButton, 1, 4);

        Button? okButton = new() { Text = "OK", Width = 78 };
        Button? cancelButton = new() { Text = "Cancel", DialogResult = DialogResult.Cancel, Width = 78 };
        Button? inviteButton = DialogButton("&Invite...");
        Button? findTimeButton = DialogButton("Fi&nd Time");
        Button? alarmButton = DialogButton("A&larm...", ShowAlarmDialog);
        Button? repeatButton = DialogButton("&Repeat...");
        Button? costButton = DialogButton("C&ost...");
        Button? helpButton = DialogButton("&Help");
        okButton.Click += (_, _) => SaveAndCloseIfValid();

        FlowLayoutPanel? buttons = new()
        {
            Dock = DockStyle.Right,
            Width = 104,
            Padding = new Padding(8, 12, 8, 8),
            FlowDirection = FlowDirection.TopDown,
            WrapContents = false
        };

        buttons.Controls.Add(okButton);
        buttons.Controls.Add(cancelButton);
        buttons.Controls.Add(new Panel { Width = 82, Height = okButton.Height });
        buttons.Controls.AddRange(new Control[] { inviteButton, findTimeButton, alarmButton, repeatButton, costButton, helpButton });

        AcceptButton = okButton;
        CancelButton = cancelButton;
        Controls.Add(buttons);
        Controls.Add(body);
        LoadValues();
        Shown += (_, _) =>
        {
            _description.Focus();
            _description.SelectionStart = _description.TextLength;
            _description.SelectionLength = 0;
        };
    }

    public static bool Edit(IWin32Window owner, CalendarEvent appointment, string title = "Create Appointment", IReadOnlyCollection<CalendarEvent>? allAppointments = null)
    {
        using CreateAppointmentDialog? dialog = new(appointment, title, allAppointments);

        return dialog.ShowDialog(owner) == DialogResult.OK;
    }

    private FlowLayoutPanel BuildSchedulePanel()
    {
        FlowLayoutPanel? panel = new() { AutoSize = true, FlowDirection = FlowDirection.LeftToRight, Margin = new Padding(0, 0, 0, 8) };

        panel.Controls.Add(new Label { Text = "&Date", AutoSize = true, Padding = new Padding(0, 5, 1, 0) });
        panel.Controls.Add(_date);
        panel.Controls.Add(new Label { Text = "&Time", AutoSize = true, Padding = new Padding(6, 5, 1, 0) });
        panel.Controls.Add(_time);
        panel.Controls.Add(new Label { Text = "D&uration", AutoSize = true, Padding = new Padding(6, 5, 1, 0) });
        panel.Controls.Add(BuildDurationPanel());

        return panel;
    }

    private FlowLayoutPanel BuildDurationPanel()
    {
        FlowLayoutPanel? panel = new() { AutoSize = true, FlowDirection = FlowDirection.LeftToRight, Margin = Padding.Empty };
        panel.Controls.Add(_duration);

        return panel;
    }

    private void LoadDurationValues()
    {
        _duration.Items.Clear();

        for(int minutes = 5; minutes <= 24 * 60; minutes += 5)
        {
            _duration.Items.Add(FormatDuration(minutes));
        }
    }

    private static void AddRow(TableLayoutPanel layout, int row, string labelText, Control control)
    {
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        // Align label to top when the control is a multiline text box so the label sits at the top of the field
        AnchorStyles labelAnchor = control is TextBox tb && tb.Multiline ? (AnchorStyles.Top | AnchorStyles.Left) : AnchorStyles.Left;
        layout.Controls.Add(new Label { Text = labelText, AutoSize = true, Anchor = labelAnchor, Padding = new Padding(0, 5, 0, 0) }, 0, row);
        layout.Controls.Add(control, 1, row);
    }

    private Button DialogButton(string text, Action? action = null)
    {
        Button? button = new() { Text = text, Width = 82 };

        button.Click += (_, _) => (action ?? (() => ShowDialogStub(text)))();

        return button;
    }

    private void ShowAlarmDialog()
    {
        SaveValues();
        _ = AppointmentAlarmDialog.Edit(this, _appointment);
        LoadValues();
    }

    private void LoadValues()
    {
        _date.Value = _appointment.Start.Date;
        _time.Value = DateTime.Today.Add(_appointment.Start.TimeOfDay);

        int totalMinutes = Math.Clamp((int)Math.Max(5, (_appointment.End - _appointment.Start).TotalMinutes), 5, 24 * 60);

        totalMinutes = (int)(Math.Round(totalMinutes / 5d) * 5);
        _duration.SelectedItem = FormatDuration(totalMinutes);
        _description.Text = _appointment.Title;
        _categories.Text = _appointment.Categories;
        _warnOfConflicts.Checked = _appointment.WarnOfConflicts;
        _pencilIn.Checked = _appointment.PencilIn;
        _confidential.Checked = _appointment.Confidential;
    }

    private void SaveValues()
    {
        DateTime start = _date.Value.Date.Add(_time.Value.TimeOfDay);
        int durationMinutes = ParseDurationMinutes(_duration.Text);

        _appointment.Title = _description.Text;
        _appointment.Start = start;
        _appointment.End = start.AddMinutes(durationMinutes);
        _appointment.Categories = _categories.Text;
        _appointment.WarnOfConflicts = _warnOfConflicts.Checked;
        _appointment.PencilIn = _pencilIn.Checked;
        _appointment.Confidential = _confidential.Checked;
    }

    private void SaveAndCloseIfValid()
    {
        // Validate required fields
        if(string.IsNullOrWhiteSpace(_description.Text))
        {
            MessageBox.Show(this, "Description is required.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            _description.Focus();

            return;
        }

        DateTime start = _date.Value.Date.Add(_time.Value.TimeOfDay);
        DateTime end = start.AddMinutes(ParseDurationMinutes(_duration.Text));

        if(_warnOfConflicts.Checked && HasTimeConflict(start, end, out string? message))
        {
            DialogResult result = MessageBox.Show(
                this,
                message + Environment.NewLine + Environment.NewLine + "Save the appointment anyway?",
                "Appointment Conflict",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if(result != DialogResult.Yes) return;
        }

        SaveValues();
        DialogResult = DialogResult.OK;
        Close();
    }

    private bool HasTimeConflict(DateTime start, DateTime end, out string message)
    {
        List<CalendarEvent>? conflicts = _allAppointments
            .Where(other => !ReferenceEquals(other, _appointment)
                && other.Id != _appointment.Id&&start < other.End && end > other.Start)
            .OrderBy(other => other.Start)
            .Take(5)
            .ToList();

        if(conflicts.Count == 0)
        {
            message = string.Empty;

            return false;
        }

        StringBuilder? builder = new();

        builder.AppendLine("This appointment conflicts with:");

        foreach(CalendarEvent conflict in conflicts)
        {
            string? title = string.IsNullOrWhiteSpace(conflict.Title) ? "(Untitled)" : conflict.Title;
            builder.AppendLine($"- {conflict.Start:g} - {conflict.End:t}: {title}");
        }

        message = builder.ToString();

        return true;
    }

    private static int ParseDurationMinutes(string value)
    {
        string[]? parts = value.Split(':');
        int hours = parts.Length > 0 && int.TryParse(parts[0], out int parsedHours) ? Math.Clamp(parsedHours, 0, 24) : 0;
        int minutes = parts.Length > 1 && int.TryParse(parts[1], out int parsedMinutes) ? Math.Clamp(parsedMinutes, 0, 59) : 0;
        minutes = (int)(Math.Round(minutes / 5d) * 5);

        if(minutes == 60)
        {
            hours = Math.Min(24, hours + 1);
            minutes = 0;
        }

        int totalMinutes = (hours * 60) + minutes;

        return Math.Clamp(totalMinutes, 5, 24 * 60);
    }

    private static string FormatDuration(int totalMinutes) => $"{totalMinutes / 60:00}:{totalMinutes % 60:00}";

    private void ShowDialogStub(string commandText)
    {
        string? cleanText = commandText.Replace("&", string.Empty, StringComparison.Ordinal).Replace("...", string.Empty, StringComparison.Ordinal);
        MessageBox.Show(this, $"{cleanText} is not yet implemented.", "Not Yet Implemented", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }
}