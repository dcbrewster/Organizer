using System.ComponentModel;

namespace Organizer;

public sealed partial class OrganizerPreferencesDialog : Form
{
    private readonly OrganizerPreferences _preferences;
    private readonly ComboBox _webBrowser = DropDown(["System default", "Internet Explorer", "Netscape Navigator", "Other"]);
    private readonly CheckBox _useFirewall = new() { Text = "&Connect to the Internet through a firewall", AutoSize = true };
    private readonly TextBox _proxyServer = new();
    private readonly NumericUpDown _proxyPort = new() { Minimum = 0, Maximum = 65535 };
    private readonly TextBox _proxyBypassDomains = new() { Multiline = true, Height = 60, ScrollBars = ScrollBars.Vertical };
    private readonly ComboBox _favoriteAlarmTune = DropDown(["Default", "Chime", "Ding", "Notify"]);
    private readonly CheckBox _displayMissedAlarms = new() { Text = "Displa&y missed alarms", AutoSize = true };
    private readonly TextBox _organizerFilesPath = new();
    private readonly TextBox _paperLayoutsPath = new();
    private readonly TextBox _customSmartIconsPath = new();
    private readonly TextBox _backupsPath = new();
    private readonly CheckBox _animatedPageTurn = new() { Text = "A&nimated page turn", AutoSize = true };
    private readonly RadioButton _plainPointer = new() { Text = "&Plain", AutoSize = true };
    private readonly RadioButton _colorPointer = new() { Text = "&Color", AutoSize = true };
    private readonly RadioButton _animatedPointer = new() { Text = "&Animated", AutoSize = true };
    private readonly ComboBox _weekStartsOn = DropDown(["Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday"]);
    private readonly CheckBox _muteOrganizerSounds = new() { Text = "&Mute Organizer sounds", AutoSize = true };
    private readonly CheckBox _autoCompleteContactNames = new() { Text = "&For Contacts, automatically complete names as they are typed.", AutoSize = true };
    private readonly CheckBox _automaticallyOpen = new() { Text = "&Automatically open", AutoSize = true };
    private readonly TextBox _automaticallyOpenPath = new();
    private readonly CheckBox _alwaysStartWithNewOrganizerFile = new() { Text = "Always start with a &new Organizer file", AutoSize = true };
    private readonly TextBox _baseNewOrganizersOnPath = new();
    private readonly CheckBox _createBackupWhenClosed = new() { Text = "C&reate backup when closed", AutoSize = true };

    public OrganizerPreferencesDialog()
        : this(new OrganizerPreferences())
    {
    }

    private OrganizerPreferencesDialog(OrganizerPreferences preferences)
    {
        InitializeComponent();
        _preferences = preferences;

        if(LicenseManager.UsageMode == LicenseUsageMode.Designtime) return;

        Controls.Clear();

        Text = "Organizer Preferences";
        Width = 680;
        Height = 560;
        StartPosition = FormStartPosition.CenterParent;
        MinimizeBox = false;
        MaximizeBox = false;
        FormBorderStyle = FormBorderStyle.FixedDialog;

        var tabs = new TabControl { Dock = DockStyle.Fill, Padding = new Point(12, 4) };
        tabs.TabPages.Add(BuildDefaultFilePage());
        tabs.TabPages.Add(BuildEnvironmentPage());
        tabs.TabPages.Add(BuildFoldersPage());
        tabs.TabPages.Add(BuildAlarmPage());
        tabs.TabPages.Add(BuildWebBrowsingPage());

        var okButton = new Button { Text = "OK", DialogResult = DialogResult.OK, Width = 90 };
        var cancelButton = new Button { Text = "Cancel", DialogResult = DialogResult.Cancel, Width = 90 };
        var helpButton = new Button { Text = "&Help", Width = 90 };
        okButton.Click += (_, _) => SaveValues();
        helpButton.Click += (_, _) => ShowDialogStub("Help Topics");

        var buttons = new FlowLayoutPanel { Dock = DockStyle.Bottom, Height = 48, Padding = new Padding(8), FlowDirection = FlowDirection.RightToLeft };
        buttons.Controls.Add(helpButton);
        buttons.Controls.Add(cancelButton);
        buttons.Controls.Add(okButton);

        AcceptButton = okButton;
        CancelButton = cancelButton;
        Controls.Add(tabs);
        Controls.Add(buttons);
        LoadValues();
    }

    public static bool Edit(IWin32Window owner, OrganizerPreferences preferences)
    {
        using var dialog = new OrganizerPreferencesDialog(preferences);
        return dialog.ShowDialog(owner) == DialogResult.OK;
    }

    private TabPage BuildWebBrowsingPage()
    {
        var page = Page("Web Browsing");
        var body = Body(page);
        var entries = new ListBox { Height = 92 };
        AddLabeled(body, "Web &browser", _webBrowser);
        body.Controls.Add(Label("Use &Web entries stored in these files to log into protected Web sites"));
        body.Controls.Add(Sized(entries));
        body.Controls.Add(ButtonRow(Button("&Add..."), Button("&Remove"), Button("&Update Organizer's Web entry references on this computer")));
        body.Controls.Add(_useFirewall);
        AddLabeled(body, "Web pro&xy server", _proxyServer);
        AddLabeled(body, "&Port", _proxyPort);
        AddLabeled(body, "B&ypass proxy\r\nfor these domains", _proxyBypassDomains);
        body.Controls.Add(Label("Use this browser to launch Web URLs from within Organizer"));
        return page;
    }

    private TabPage BuildAlarmPage()
    {
        var page = Page("Alarms");
        var body = Body(page);
        AddLabeled(body, "&Favorite alarm tune:", _favoriteAlarmTune);
        body.Controls.Add(ButtonRow(Button("Play", () => System.Media.SystemSounds.Asterisk.Play()), Button("Bro&wse...")));
        body.Controls.Add(_displayMissedAlarms);
        body.Controls.Add(Label("Use these Alarm settings as defaults"));
        body.Controls.Add(BuildAlarmDefaultsGrid());
        return page;
    }

    private TabPage BuildFoldersPage()
    {
        var page = Page("Folders");
        var body = Body(page);
        AddPathRow(body, "&Organizer files", _organizerFilesPath, "B&rowse...");
        AddPathRow(body, "&Paper layouts", _paperLayoutsPath, "Bro&wse...");
        AddPathRow(body, "Custom Smart&Icons", _customSmartIconsPath, "Brow&se...");
        AddPathRow(body, "&Backups", _backupsPath, "Brows&e...");
        return page;
    }

    private TabPage BuildEnvironmentPage()
    {
        var page = Page("Environment");
        var body = Body(page);
        body.Controls.Add(_animatedPageTurn);
        body.Controls.Add(Label("Mouse pointer"));
        body.Controls.Add(ButtonRow(_plainPointer, _colorPointer, _animatedPointer));
        AddLabeled(body, "Wee&k starts on", _weekStartsOn);
        body.Controls.Add(Label("&Sounds"));
        var sounds = new ListBox { Height = 70 };
        sounds.Items.AddRange(["Appointment alarm", "Task alarm", "Page turn", "Error"]);
        body.Controls.Add(Sized(sounds));
        body.Controls.Add(ButtonRow(Button("Pla&y", () => System.Media.SystemSounds.Asterisk.Play()), Button("S&top"), Button("So&unds...")));
        body.Controls.Add(_muteOrganizerSounds);
        body.Controls.Add(_autoCompleteContactNames);
        return page;
    }

    private TabPage BuildDefaultFilePage()
    {
        var page = Page("Default File");
        var body = Body(page);
        body.Controls.Add(_automaticallyOpen);
        AddPathRow(body, string.Empty, _automaticallyOpenPath, "B&rowse...");
        body.Controls.Add(_alwaysStartWithNewOrganizerFile);
        AddPathRow(body, "&Base new Organizers on", _baseNewOrganizersOnPath, "Br&owse...");
        body.Controls.Add(Label("Backup"));
        body.Controls.Add(_createBackupWhenClosed);
        body.Controls.Add(ButtonRow(Button("&Make backup now")));
        return page;
    }

    private static TabPage Page(string text)
    {
        var page = new TabPage(text)
        {
            Padding = new Padding(12),
            BackColor = SystemColors.Control
        };

        page.Controls.Add(new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            AutoScroll = true,
            FlowDirection = FlowDirection.TopDown,
            WrapContents = false
        });

        return page;
    }

    private static FlowLayoutPanel Body(TabPage page) => (FlowLayoutPanel)page.Controls[0];

    private static Label Label(string text) => new() { Text = text, AutoSize = true, Margin = new Padding(0, 8, 0, 2) };

    private static T Sized<T>(T control) where T : Control
    {
        control.Width = 590;
        return control;
    }

    private TableLayoutPanel BuildAlarmDefaultsGrid()
    {
        var grid = new TableLayoutPanel { ColumnCount = 5, RowCount = 6, Dock = DockStyle.Top, AutoSize = true };
        grid.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120));
        grid.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 80));
        grid.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 105));
        grid.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 105));
        grid.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100));
        grid.Controls.Add(new Label { Text = "Section", AutoSize = true }, 0, 0);
        grid.Controls.Add(new Label { Text = "Set Alarm", AutoSize = true }, 1, 0);
        grid.Controls.Add(new Label { Text = "Amount", AutoSize = true }, 2, 0);
        grid.Controls.Add(new Label { Text = "Unit", AutoSize = true }, 3, 0);
        grid.Controls.Add(new Label { Text = "When", AutoSize = true }, 4, 0);

        var row = 1;
        foreach(var section in new[] { "A&nniversary", "&Appointment", "Ca&ll", "&Event", "&Task" })
        {
            grid.Controls.Add(new Label { Text = section, AutoSize = true, Padding = new Padding(0, 4, 0, 0) }, 0, row);
            grid.Controls.Add(new CheckBox { Text = "On", AutoSize = true }, 1, row);
            grid.Controls.Add(new NumericUpDown { Minimum = 0, Maximum = 999, Value = 15, Width = 70 }, 2, row);
            grid.Controls.Add(DropDown(["Minutes", "Hours", "Days"]), 3, row);
            grid.Controls.Add(DropDown(["Before", "After"]), 4, row);
            row++;
        }

        return grid;
    }

    private static void AddLabeled(Control parent, string text, Control control)
    {
        parent.Controls.Add(Label(text));
        control.Width = 590;
        parent.Controls.Add(control);
    }

    private void AddPathRow(Control parent, string label, TextBox textBox, string buttonText)
    {
        var panel = new TableLayoutPanel { Width = 590, Height = 30, ColumnCount = 2 };
        panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        panel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 92));
        textBox.Dock = DockStyle.Fill;
        panel.Controls.Add(textBox, 0, 0);
        if(!string.IsNullOrWhiteSpace(label))
        {
            parent.Controls.Add(Label(label));
        }

        panel.Controls.Add(BrowseButton(buttonText, textBox), 1, 0);
        parent.Controls.Add(panel);
    }

    private static FlowLayoutPanel ButtonRow(params Control[] controls)
    {
        var panel = new FlowLayoutPanel { Width = 590, Height = 34, FlowDirection = FlowDirection.LeftToRight };
        panel.Controls.AddRange(controls);
        return panel;
    }

    private Button Button(string text, Action? action = null)
    {
        var button = new Button { Text = text, Width = Math.Max(90, TextRenderer.MeasureText(text.Replace("&", string.Empty, StringComparison.Ordinal), SystemFonts.MessageBoxFont).Width + 24) };
        button.Click += (_, _) => (action ?? (() => ShowDialogStub(text)))();
        return button;
    }

    private Button BrowseButton(string text, TextBox target)
    {
        var button = Button(text, () =>
        {
            using var dialog = new FolderBrowserDialog { SelectedPath = Directory.Exists(target.Text) ? target.Text : Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) };
            if(dialog.ShowDialog(this) == DialogResult.OK)
            {
                target.Text = dialog.SelectedPath;
            }
        });
        button.Width = 88;
        return button;
    }

    private static ComboBox DropDown(string[] values)
    {
        var comboBox = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList };
        comboBox.Items.AddRange(values);
        if(comboBox.Items.Count > 0)
        {
            comboBox.SelectedIndex = 0;
        }

        return comboBox;
    }

    private void LoadValues()
    {
        SelectItem(_webBrowser, _preferences.WebBrowser);
        _useFirewall.Checked = _preferences.UseFirewall;
        _proxyServer.Text = _preferences.ProxyServer;
        _proxyPort.Value = Math.Clamp(_preferences.ProxyPort, (int)_proxyPort.Minimum, (int)_proxyPort.Maximum);
        _proxyBypassDomains.Text = _preferences.ProxyBypassDomains;
        SelectItem(_favoriteAlarmTune, _preferences.FavoriteAlarmTune);
        _displayMissedAlarms.Checked = _preferences.DisplayMissedAlarms;
        _organizerFilesPath.Text = _preferences.OrganizerFilesPath;
        _paperLayoutsPath.Text = _preferences.PaperLayoutsPath;
        _customSmartIconsPath.Text = _preferences.CustomSmartIconsPath;
        _backupsPath.Text = _preferences.BackupsPath;
        _animatedPageTurn.Checked = _preferences.AnimatedPageTurn;
        _plainPointer.Checked = _preferences.MousePointer == "Plain";
        _animatedPointer.Checked = _preferences.MousePointer == "Animated";
        _colorPointer.Checked = !_plainPointer.Checked && !_animatedPointer.Checked;
        SelectItem(_weekStartsOn, _preferences.WeekStartsOn);
        _muteOrganizerSounds.Checked = _preferences.MuteOrganizerSounds;
        _autoCompleteContactNames.Checked = _preferences.AutoCompleteContactNames;
        _automaticallyOpen.Checked = _preferences.AutomaticallyOpen;
        _automaticallyOpenPath.Text = _preferences.AutomaticallyOpenPath;
        _alwaysStartWithNewOrganizerFile.Checked = _preferences.AlwaysStartWithNewOrganizerFile;
        _baseNewOrganizersOnPath.Text = _preferences.BaseNewOrganizersOnPath;
        _createBackupWhenClosed.Checked = _preferences.CreateBackupWhenClosed;
    }

    private void SaveValues()
    {
        _preferences.WebBrowser = _webBrowser.SelectedItem?.ToString() ?? "System default";
        _preferences.UseFirewall = _useFirewall.Checked;
        _preferences.ProxyServer = _proxyServer.Text;
        _preferences.ProxyPort = (int)_proxyPort.Value;
        _preferences.ProxyBypassDomains = _proxyBypassDomains.Text;
        _preferences.FavoriteAlarmTune = _favoriteAlarmTune.SelectedItem?.ToString() ?? "Default";
        _preferences.DisplayMissedAlarms = _displayMissedAlarms.Checked;
        _preferences.OrganizerFilesPath = _organizerFilesPath.Text;
        _preferences.PaperLayoutsPath = _paperLayoutsPath.Text;
        _preferences.CustomSmartIconsPath = _customSmartIconsPath.Text;
        _preferences.BackupsPath = _backupsPath.Text;
        _preferences.AnimatedPageTurn = _animatedPageTurn.Checked;
        _preferences.MousePointer = _plainPointer.Checked ? "Plain" : _animatedPointer.Checked ? "Animated" : "Color";
        _preferences.WeekStartsOn = _weekStartsOn.SelectedItem?.ToString() ?? "Sunday";
        _preferences.MuteOrganizerSounds = _muteOrganizerSounds.Checked;
        _preferences.AutoCompleteContactNames = _autoCompleteContactNames.Checked;
        _preferences.AutomaticallyOpen = _automaticallyOpen.Checked;
        _preferences.AutomaticallyOpenPath = _automaticallyOpenPath.Text;
        _preferences.AlwaysStartWithNewOrganizerFile = _alwaysStartWithNewOrganizerFile.Checked;
        _preferences.BaseNewOrganizersOnPath = _baseNewOrganizersOnPath.Text;
        _preferences.CreateBackupWhenClosed = _createBackupWhenClosed.Checked;
    }

    private static void SelectItem(ComboBox comboBox, string value)
    {
        comboBox.SelectedItem = comboBox.Items.Cast<object>().FirstOrDefault(item => string.Equals(item.ToString(), value, StringComparison.OrdinalIgnoreCase)) ?? comboBox.Items[0];
    }

    private void ShowDialogStub(string commandText)
    {
        var cleanText = commandText.Split('\t')[0].Replace("&", string.Empty, StringComparison.Ordinal).Replace("...", string.Empty, StringComparison.Ordinal);
        MessageBox.Show(this, $"{cleanText} is not yet implemented.", "Not Yet Implemented", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }
}
