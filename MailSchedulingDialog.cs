using System.Windows.Forms;
using System.Drawing;
using System.Collections.Generic;

namespace Organizer;

public sealed class MailSchedulingDialog : Form
{
    private TabControl _tabs = new();

    // Top-level mail/program controls (above tabs)
    private ComboBox _mailProgramCombo = new();

    private RadioButton _mapiRadio = new();
    private RadioButton _pop3Radio = new();
    private RadioButton _imapRadio = new();
    private Button _configureMailButton = new();
    private Button _configureMailDisabledButton = new();
    private Label _infoText = new();

    // Mail tab controls
    private TextBox _fromText = new();

    private TextBox _signatureText = new();

    // Scheduling tab controls (legacy)
    private CheckBox _useCurrentOrganizerFile = new();

    private Label _organizerFilePathLabel = new();
    private TextBox _schedulingName = new();
    private TextBox _schedulingEmail = new();
    private ListBox _forwardingAddressesList = new();
    private Button _editForwardingList = new();
    private Button _backwardCompatibility = new();

    // Connections tab controls (legacy)
    private CheckBox _checkInbox = new();

    private NumericUpDown _checkInboxMinutes = new();
    private ComboBox _tuneCombo = new();
    private Button _playTune = new();
    private DateTimePicker _lastMeetingNoticePicker = new();
    private CheckBox _requestConfirmation = new();
    private CheckBox _deleteNotices = new();

    // Auto-process tab controls
    private CheckBox _chairAcceptances = new();

    private CheckBox _chairDeclines = new();
    private CheckBox _chairAutoprocessWithMessages = new();
    private CheckBox _inviteeInvitations = new();
    private CheckBox _inviteeCancellations = new();
    private CheckBox _inviteeRescheduling = new();
    private CheckBox _inviteeStatusUpdates = new();
    private CheckBox _inviteeConfirmations = new();

    // Busy time tab controls
    private CheckBox _publishBusy = new();

    private TextBox _busyUrl = new();

    public MailSchedulingDialog()
    {
        Text = "Mail and Scheduling Preferences";
        FormBorderStyle = FormBorderStyle.FixedDialog;
        StartPosition = FormStartPosition.CenterParent;
        ClientSize = new Size(520, 420);
        MaximizeBox = false;
        MinimizeBox = false;

        InitializeComponents();
    }

    private void InitializeComponents()
    {
        // Top-level mail program combo and protocol radios
        _mailProgramCombo = new ComboBox { Location = new Point(12, 12), Width = 300, DropDownStyle = ComboBoxStyle.DropDownList };
        _mailProgramCombo.Items.AddRange(new object[] { "Windows Live Hotmail", "Microsoft Outlook", "Default e-mail program" });

        _mapiRadio = new RadioButton { Text = "Mail client's simple MAPI", Location = new Point(12, 44), AutoSize = true };
        _pop3Radio = new RadioButton { Text = "POP3 (may combine with MAPI)", Location = new Point(12, 64), AutoSize = true };
        _imapRadio = new RadioButton { Text = "IMAP4", Location = new Point(12, 84), AutoSize = true };

        _configureMailButton = new Button { Text = "Configure...", Location = new Point(340, 12), Size = new Size(80, 24) };
        _configureMailDisabledButton = new Button { Text = "Configure...", Location = new Point(340, 44), Size = new Size(80, 24), Enabled = false };

        _infoText = new Label { Text = "Organizer e-mail and scheduling features are available, but may require you to connect to your mail server.", Location = new Point(12, 110), Size = new Size(480, 24) };

        // Tab control (moved down to make room for top controls)
        _tabs = new TabControl { Location = new Point(12, 140), Size = new Size(496, 220) };
        TabPage? mailTab = new("Mail");
        TabPage? schedulingTab = new("Scheduling");
        TabPage? connectionsTab = new("Connections");
        TabPage? autoProcessTab = new("Auto-process");
        TabPage? busyTimeTab = new("Busy time");

        _tabs.TabPages.AddRange(new TabPage[] { mailTab, schedulingTab, connectionsTab, autoProcessTab, busyTimeTab });

        // Mail tab contents (default from / signature)
        _fromText = new TextBox { Location = new Point(12, 12), Width = 300, PlaceholderText = "Default From address" };
        _signatureText = new TextBox { Location = new Point(12, 44), Width = 440, Height = 80, Multiline = true };
        mailTab.Controls.Add(new Label { Text = "Default From:", Location = new Point(12, -4), AutoSize = true });
        mailTab.Controls.Add(_fromText);
        mailTab.Controls.Add(new Label { Text = "Signature:", Location = new Point(12, 28), AutoSize = true });
        mailTab.Controls.Add(_signatureText);

        // Scheduling tab contents (recreated from legacy)
        _useCurrentOrganizerFile = new CheckBox { Text = "Use the current Organizer file to receive messages", Location = new Point(12, 12), AutoSize = true };
        _organizerFilePathLabel = new Label { Text = string.Empty, Location = new Point(12, 36), AutoSize = true };
        _schedulingName = new TextBox { Location = new Point(12, 64), Width = 300 };
        _schedulingEmail = new TextBox { Location = new Point(12, 100), Width = 300 };
        _forwardingAddressesList = new ListBox { Location = new Point(12, 136), Size = new Size(360, 64) };
        _editForwardingList = new Button { Text = "Edit List ...", Location = new Point(380, 136), Size = new Size(90, 24) };
        _backwardCompatibility = new Button { Text = "Backward Compatibility...", Location = new Point(380, 168), Size = new Size(120, 24) };
        schedulingTab.Controls.Add(_useCurrentOrganizerFile);
        schedulingTab.Controls.Add(_organizerFilePathLabel);
        schedulingTab.Controls.Add(new Label { Text = "Your name", Location = new Point(12, 48), AutoSize = true });
        schedulingTab.Controls.Add(_schedulingName);
        schedulingTab.Controls.Add(new Label { Text = "E-mail address", Location = new Point(12, 84), AutoSize = true });
        schedulingTab.Controls.Add(_schedulingEmail);
        schedulingTab.Controls.Add(new Label { Text = "E-mail addresses that forward mail to this address", Location = new Point(12, 120), AutoSize = true });
        schedulingTab.Controls.Add(_forwardingAddressesList);
        schedulingTab.Controls.Add(_editForwardingList);
        schedulingTab.Controls.Add(_backwardCompatibility);

        // Connections tab contents (legacy)
        _checkInbox = new CheckBox { Text = "Check my inbox every", Location = new Point(12, 12), AutoSize = true };
        _checkInboxMinutes = new NumericUpDown { Location = new Point(160, 12), Minimum = 0, Maximum = 1440, Value = 15, Enabled = false };
        _tuneCombo = new ComboBox { Location = new Point(12, 44), Width = 260, DropDownStyle = ComboBoxStyle.DropDownList, Enabled = false };
        _tuneCombo.Items.AddRange(new object[] { "(None)", "1812 Overture", "Alarm Clock", "Arpeggio", "Bach", "Beep", "Beethoven", "Big Ben", "Digital", "Silent", "Wedding" });
        _playTune = new Button { Text = "Play", Location = new Point(280, 44), Size = new Size(60, 24), Enabled = false };
        _lastMeetingNoticePicker = new DateTimePicker { Location = new Point(12, 80), Width = 220, Format = DateTimePickerFormat.Custom, CustomFormat = "M/d/yyyy h:mm tt" };
        _requestConfirmation = new CheckBox { Text = "Request confirmation before processing meeting notices", Location = new Point(12, 112), AutoSize = true };
        _deleteNotices = new CheckBox { Text = "Delete notices from inbox after retrieval", Location = new Point(12, 136), AutoSize = true };
        connectionsTab.Controls.Add(_checkInbox);
        connectionsTab.Controls.Add(_checkInboxMinutes);
        connectionsTab.Controls.Add(new Label { Text = "minutes when I am connected", Location = new Point(220, 14), AutoSize = true });
        connectionsTab.Controls.Add(new Label { Text = "Tune for new notices", Location = new Point(12, 28), AutoSize = true });
        connectionsTab.Controls.Add(_tuneCombo);
        connectionsTab.Controls.Add(_playTune);
        connectionsTab.Controls.Add(new Label { Text = "The most recent meeting notice Organizer has found is dated", Location = new Point(12, 64), AutoSize = true });
        connectionsTab.Controls.Add(_lastMeetingNoticePicker);
        connectionsTab.Controls.Add(_requestConfirmation);
        connectionsTab.Controls.Add(_deleteNotices);

        // Auto-process tab contents (legacy groups)
        _chairAcceptances = new CheckBox { Text = "Acceptances", Location = new Point(12, 28), AutoSize = true };
        _chairDeclines = new CheckBox { Text = "Declines", Location = new Point(12, 52), AutoSize = true };
        _chairAutoprocessWithMessages = new CheckBox { Text = "Autoprocess notices with messages", Location = new Point(12, 76), AutoSize = true };
        autoProcessTab.Controls.Add(new Label { Text = "When you are Chairperson, automatically process", Location = new Point(12, 8), AutoSize = true });
        autoProcessTab.Controls.Add(_chairAcceptances);
        autoProcessTab.Controls.Add(_chairDeclines);
        autoProcessTab.Controls.Add(_chairAutoprocessWithMessages);

        _inviteeInvitations = new CheckBox { Text = "Invitations", Location = new Point(12, 120), AutoSize = true };
        _inviteeCancellations = new CheckBox { Text = "Cancellations", Location = new Point(12, 144), AutoSize = true };
        _inviteeRescheduling = new CheckBox { Text = "Rescheduling", Location = new Point(12, 168), AutoSize = true };
        _inviteeStatusUpdates = new CheckBox { Text = "Status updates", Location = new Point(140, 120), AutoSize = true };
        _inviteeConfirmations = new CheckBox { Text = "Confirmations", Location = new Point(140, 144), AutoSize = true };
        autoProcessTab.Controls.Add(new Label { Text = "When you are an Invitee, automatically process", Location = new Point(12, 100), AutoSize = true });
        autoProcessTab.Controls.Add(_inviteeInvitations);
        autoProcessTab.Controls.Add(_inviteeCancellations);
        autoProcessTab.Controls.Add(_inviteeRescheduling);
        autoProcessTab.Controls.Add(_inviteeStatusUpdates);
        autoProcessTab.Controls.Add(_inviteeConfirmations);

        // Busy time tab contents
        _publishBusy = new CheckBox { Text = "Publish busy time", Location = new Point(12, 12), AutoSize = true };
        _busyUrl = new TextBox { Location = new Point(12, 40), Width = 440, PlaceholderText = "Publish URL" };
        busyTimeTab.Controls.Add(_publishBusy);
        busyTimeTab.Controls.Add(_busyUrl);

        // Dialog buttons
        Button ok = new() { Text = "OK", DialogResult = DialogResult.OK, Location = new Point(320, 372), Size = new Size(80, 28) };
        Button cancel = new() { Text = "Cancel", DialogResult = DialogResult.Cancel, Location = new Point(408, 372), Size = new Size(80, 28) };
        Button help = new() { Text = "Help", Location = new Point(12, 372), Size = new Size(80, 28) };
        help.Click += (_, _) => MessageBox.Show(this, "Help is not available for this dialog.", "Help", MessageBoxButtons.OK, MessageBoxIcon.Information);

        // Compose controls on the form
        Controls.Add(_mailProgramCombo);
        Controls.Add(_mapiRadio);
        Controls.Add(_pop3Radio);
        Controls.Add(_imapRadio);
        Controls.Add(_configureMailButton);
        Controls.Add(_configureMailDisabledButton);
        Controls.Add(_infoText);
        Controls.Add(_tabs);
        Controls.Add(ok);
        Controls.Add(cancel);
        Controls.Add(help);

        // Initialize control values from stored preferences
        LoadPreferencesIntoControls();

        AcceptButton = ok;
        CancelButton = cancel;

        ok.Click += (_, _) => { SaveAllPreferences(); Close(); };
    }

    public static void Edit(IWin32Window owner)
    {
        using MailSchedulingDialog dlg = new();
        dlg.ShowDialog(owner);
    }

    private void SavePreferences()
    {
        try
        {
            var prefs = ProgramData.Instance.Data.Preferences;
            ProgramData.Instance.Save();
        }
        catch { }
    }

    private void LoadPreferencesIntoControls()
    {
        OrganizerPreferences? prefs = ProgramData.Instance.Data.Preferences;

        // Top-level
        _mailProgramCombo.SelectedItem = prefs.MailProgram ?? _mailProgramCombo.SelectedItem;
        switch((prefs.MailProtocol ?? string.Empty).ToUpperInvariant())
        {
            case "IMAP4": _imapRadio.Checked = true; break;
            case "POP3": _pop3Radio.Checked = true; break;
            default: _mapiRadio.Checked = true; break;
        }

        // Mail tab
        _fromText.Text = prefs.MailDefaultFrom ?? string.Empty;
        _signatureText.Text = prefs.MailSignature ?? string.Empty;

        // Scheduling tab
        _useCurrentOrganizerFile.Checked = prefs.UseCurrentOrganizerFileToReceiveMessages;
        _organizerFilePathLabel.Text = prefs.OrganizerFilePath ?? string.Empty;
        _schedulingName.Text = prefs.SchedulingName ?? string.Empty;
        _schedulingEmail.Text = prefs.SchedulingEmail ?? string.Empty;
        _forwardingAddressesList.Items.Clear();

        if(prefs.SchedulingForwardingAddresses != null)
            foreach(string a in prefs.SchedulingForwardingAddresses)
                _forwardingAddressesList.Items.Add(a);

        // Connections
        _checkInbox.Checked = prefs.CheckInboxEveryMinutes > 0;
        _checkInboxMinutes.Value = Math.Max(0, Math.Min(1440, prefs.CheckInboxEveryMinutes));

        if(!string.IsNullOrEmpty(prefs.FavoriteAlarmTune))
        {
            if(_tuneCombo.Items.Contains(prefs.FavoriteAlarmTune))
                _tuneCombo.SelectedItem = prefs.FavoriteAlarmTune;
            else
                _tuneCombo.SelectedIndex = 0;
        }
        _lastMeetingNoticePicker.Value = prefs.LastMeetingNoticeDate == DateTime.MinValue ? DateTime.Now : prefs.LastMeetingNoticeDate;
        _requestConfirmation.Checked = prefs.RequestConfirmationBeforeProcessingNotices;
        _deleteNotices.Checked = prefs.DeleteNoticesFromInboxAfterRetrieval;

        // Auto-process
        _chairAcceptances.Checked = prefs.AutoProcessChairAcceptances;
        _chairDeclines.Checked = prefs.AutoProcessChairDeclines;
        _chairAutoprocessWithMessages.Checked = prefs.AutoProcessChairWithMessages;
        _inviteeInvitations.Checked = prefs.AutoProcessInviteeInvitations;
        _inviteeCancellations.Checked = prefs.AutoProcessInviteeCancellations;
        _inviteeRescheduling.Checked = prefs.AutoProcessInviteeRescheduling;
        _inviteeStatusUpdates.Checked = prefs.AutoProcessInviteeStatusUpdates;
        _inviteeConfirmations.Checked = prefs.AutoProcessInviteeConfirmations;

        // Busy time
        _publishBusy.Checked = prefs.PublishBusyTime;
        _busyUrl.Text = prefs.BusyTimePublishUrl ?? string.Empty;
    }

    private void SaveAllPreferences()
    {
        OrganizerPreferences? prefs = ProgramData.Instance.Data.Preferences;

        // Top-level
        prefs.MailProgram = _mailProgramCombo.SelectedItem?.ToString() ?? prefs.MailProgram;

        if(_imapRadio.Checked) prefs.MailProtocol = "IMAP4";
        else if(_pop3Radio.Checked) prefs.MailProtocol = "POP3";
        else prefs.MailProtocol = "MAPI";

        // Mail
        prefs.MailDefaultFrom = _fromText.Text ?? string.Empty;
        prefs.MailSignature = _signatureText.Text ?? string.Empty;

        // Scheduling
        prefs.UseCurrentOrganizerFileToReceiveMessages = _useCurrentOrganizerFile.Checked;
        prefs.OrganizerFilePath = _organizerFilePathLabel.Text ?? string.Empty;
        prefs.SchedulingName = _schedulingName.Text ?? string.Empty;
        prefs.SchedulingEmail = _schedulingEmail.Text ?? string.Empty;
        prefs.SchedulingForwardingAddresses = new List<string>();

        foreach(var item in _forwardingAddressesList.Items)
            prefs.SchedulingForwardingAddresses.Add(item?.ToString() ?? string.Empty);

        // Connections
        prefs.CheckInboxEveryMinutes = (int)_checkInboxMinutes.Value;
        prefs.FavoriteAlarmTune = _tuneCombo.SelectedItem?.ToString() ?? prefs.FavoriteAlarmTune;
        prefs.LastMeetingNoticeDate = _lastMeetingNoticePicker.Value;
        prefs.RequestConfirmationBeforeProcessingNotices = _requestConfirmation.Checked;
        prefs.DeleteNoticesFromInboxAfterRetrieval = _deleteNotices.Checked;

        // Auto-process
        prefs.AutoProcessChairAcceptances = _chairAcceptances.Checked;
        prefs.AutoProcessChairDeclines = _chairDeclines.Checked;
        prefs.AutoProcessChairWithMessages = _chairAutoprocessWithMessages.Checked;
        prefs.AutoProcessInviteeInvitations = _inviteeInvitations.Checked;
        prefs.AutoProcessInviteeCancellations = _inviteeCancellations.Checked;
        prefs.AutoProcessInviteeRescheduling = _inviteeRescheduling.Checked;
        prefs.AutoProcessInviteeStatusUpdates = _inviteeStatusUpdates.Checked;
        prefs.AutoProcessInviteeConfirmations = _inviteeConfirmations.Checked;

        // Busy time
        prefs.PublishBusyTime = _publishBusy.Checked;
        prefs.BusyTimePublishUrl = _busyUrl.Text ?? string.Empty;

        ProgramData.Instance.Save();
    }
}