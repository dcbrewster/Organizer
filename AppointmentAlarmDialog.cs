using System.ComponentModel;

namespace Organizer;

public sealed partial class AppointmentAlarmDialog : Form
{
    private readonly CalendarEvent _appointment;
    private readonly DomainUpDown _amount = new() { ReadOnly = true, Width = 58, TextAlign = HorizontalAlignment.Right };
    private readonly ComboBox _unit = new() { DropDownStyle = ComboBoxStyle.DropDownList, Width = 86 };
    private readonly DateTimePicker _alarmDate = new() { Format = DateTimePickerFormat.Short, Width = 96 };
    private readonly DateTimePicker _alarmTime = new() { Format = DateTimePickerFormat.Custom, CustomFormat = "h:mm tt", ShowUpDown = true, Width = 78 };
    private readonly ComboBox _tune = new() { DropDownStyle = ComboBoxStyle.DropDown, Width = 170 };
    private readonly TextBox _message = new() { Width = 245 };
    private readonly TextBox _run = new() { Width = 245 };
    private readonly CheckBox _displayDialog = new() { Text = "D&isplay dialog box at alarm time", AutoSize = true };
    private readonly RadioButton _setAlarm = new() { Text = "Set A&larm", AutoSize = true };
    private readonly RadioButton _cancelAlarm = new() { Text = "&Cancel Alarm", AutoSize = true };
    private readonly RadioButton _before = new() { Text = "B&efore", AutoSize = true };
    private readonly RadioButton _after = new() { Text = "&After", AutoSize = true };
    private readonly RadioButton _on = new() { Text = "&On", AutoSize = true };
    private readonly Label _appointmentTime = new() { AutoSize = true };

    public AppointmentAlarmDialog()
        : this(new CalendarEvent())
    {
    }

    private AppointmentAlarmDialog(CalendarEvent appointment)
    {
        InitializeComponent();
        _appointment = appointment;

        if(LicenseManager.UsageMode == LicenseUsageMode.Designtime) return;

        Controls.Clear();
        Text = "Alarm";
        Width = 520;
        Height = 292;
        StartPosition = FormStartPosition.CenterParent;
        MinimizeBox = false;
        MaximizeBox = false;
        FormBorderStyle = FormBorderStyle.FixedDialog;

        for(var value = 0; value <= 999; value += 5)
        {
            _amount.Items.Add(value.ToString());
        }

        _unit.Items.AddRange(["Minutes", "Hours", "Days"]);
        _tune.Items.AddRange(["Default", "Chime", "Ding", "Notify"]);

        var body = new Panel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(12)
        };

        var playButton = new Button { Text = "Play", Width = 82 };
        var browseButton = new Button { Text = "Bro&wse...", Width = 82 };
        var runBrowseButton = new Button { Text = "&Browse...", Width = 82 };
        playButton.Click += (_, _) => System.Media.SystemSounds.Asterisk.Play();
        browseButton.Click += (_, _) => ShowNotImplemented("Browse alarm tune");
        runBrowseButton.Click += (_, _) => ShowNotImplemented("Browse run command");

        _amount.Location = new Point(16, 14);
        _unit.Location = new Point(82, 14);
        _alarmDate.Location = new Point(180, 14);
        _alarmTime.Location = new Point(286, 14);
        body.Controls.AddRange([_amount, _unit, _alarmDate, _alarmTime]);

        AddLabel(body, "T&une", 16, 49);
        _tune.Location = new Point(82, 45);
        playButton.Location = new Point(262, 43);
        body.Controls.AddRange([_tune, playButton]);

        AddLabel(body, "Me&ssage", 16, 84);
        _message.Location = new Point(82, 80);
        body.Controls.Add(_message);

        AddLabel(body, "&Run", 16, 119);
        _run.Location = new Point(82, 115);
        browseButton.Location = new Point(336, 43);
        runBrowseButton.Location = new Point(336, 113);
        body.Controls.AddRange([_run, browseButton, runBrowseButton]);

        _displayDialog.Location = new Point(82, 147);
        body.Controls.Add(_displayDialog);

        _setAlarm.Location = new Point(16, 190);
        _cancelAlarm.Location = new Point(120, 190);
        _appointmentTime.Location = new Point(16, 218);
        _before.Location = new Point(260, 190);
        _after.Location = new Point(330, 190);
        _on.Location = new Point(392, 190);
        body.Controls.AddRange([_setAlarm, _cancelAlarm, _appointmentTime, _before, _after, _on]);

        var okButton = new Button { Text = "OK", DialogResult = DialogResult.OK, Width = 82 };
        var cancelButton = new Button { Text = "Cancel", DialogResult = DialogResult.Cancel, Width = 82 };
        var helpButton = new Button { Text = "&Help", Width = 82 };
        okButton.Click += (_, _) => SaveValues();
        helpButton.Click += (_, _) => ShowNotImplemented("Help");

        var buttons = new FlowLayoutPanel
        {
            Dock = DockStyle.Right,
            Width = 100,
            Padding = new Padding(8, 12, 8, 8),
            FlowDirection = FlowDirection.TopDown,
            WrapContents = false
        };
        buttons.Controls.AddRange([okButton, cancelButton, helpButton]);

        AcceptButton = okButton;
        CancelButton = cancelButton;
        Controls.Add(buttons);
        Controls.Add(body);
        LoadValues();
    }

    public static bool Edit(IWin32Window owner, CalendarEvent appointment)
    {
        using var dialog = new AppointmentAlarmDialog(appointment);

        return dialog.ShowDialog(owner) == DialogResult.OK;
    }

    private void LoadValues()
    {
        _setAlarm.Checked = _appointment.AlarmEnabled;
        _cancelAlarm.Checked = !_appointment.AlarmEnabled;
        SelectItem(_amount, Math.Clamp(_appointment.AlarmAmount, 0, 999).ToString());
        SelectItem(_unit, _appointment.AlarmUnit);
        SelectItem(_tune, _appointment.AlarmTune);
        _alarmDate.Value = _appointment.AlarmDate == default ? _appointment.Start.Date : _appointment.AlarmDate.Date;
        _alarmTime.Value = DateTime.Today.Add((_appointment.AlarmTime == default ? _appointment.Start : _appointment.AlarmTime).TimeOfDay);
        _message.Text = string.IsNullOrWhiteSpace(_appointment.AlarmMessage) ? _appointment.Title : _appointment.AlarmMessage;
        _run.Text = _appointment.AlarmRunCommand;
        _displayDialog.Checked = _appointment.AlarmDisplayDialog;
        _before.Checked = _appointment.AlarmTiming == "Before";
        _after.Checked = _appointment.AlarmTiming == "After";
        _on.Checked = _appointment.AlarmTiming == "On" || (!_before.Checked && !_after.Checked);
        _appointmentTime.Text = $"Appointment time {_appointment.Start:MMMM d, yyyy h:mm tt}";
    }

    private void SaveValues()
    {
        _appointment.AlarmEnabled = _setAlarm.Checked;
        _appointment.AlarmAmount = int.TryParse(_amount.SelectedItem?.ToString(), out var amount) ? amount : 15;
        _appointment.AlarmUnit = _unit.SelectedItem?.ToString() ?? "Minutes";
        _appointment.AlarmTiming = _before.Checked ? "Before" : _after.Checked ? "After" : "On";
        _appointment.AlarmTune = _tune.SelectedItem?.ToString() ?? "Default";
        _appointment.AlarmDate = _alarmDate.Value.Date;
        _appointment.AlarmTime = DateTime.Today.Add(_alarmTime.Value.TimeOfDay);
        _appointment.AlarmMessage = _message.Text;
        _appointment.AlarmRunCommand = _run.Text;
        _appointment.AlarmDisplayDialog = _displayDialog.Checked;
    }

    private static void AddLabel(Control parent, string text, int x, int y) => parent.Controls.Add(new Label { Text = text, AutoSize = true, Location = new Point(x, y) });

    private static void SelectItem(ComboBox control, string value)
    {
        var index = control.Items.Cast<object>().Select(item => item.ToString() ?? string.Empty).ToList().FindIndex(item => item.Equals(value, StringComparison.OrdinalIgnoreCase));
        control.SelectedIndex = index >= 0 ? index : 0;
    }

    private static void SelectItem(DomainUpDown control, string value)
    {
        var index = control.Items.Cast<object>().Select(item => item.ToString() ?? string.Empty).ToList().FindIndex(item => item.Equals(value, StringComparison.OrdinalIgnoreCase));
        control.SelectedIndex = index >= 0 ? index : 0;
    }

    private void ShowNotImplemented(string commandText) => MessageBox.Show(this, $"{commandText} is not yet implemented.", "Not Yet Implemented", MessageBoxButtons.OK, MessageBoxIcon.Information);
}
