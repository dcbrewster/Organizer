using System.ComponentModel;
using System.Drawing.Printing;
using System.Reflection;
using System.Text;
using Organizer.About;

namespace Organizer;

public sealed partial class MainForm : Form
{
    private readonly AppDataStore _store = new();
    private readonly OrganizerData _data;
    private Action _refreshCalendar = () => { };
    private readonly Dictionary<string, BindingSource> _sectionSources = [];
    private TabControl _tabs = null!;
    private Action<CalendarViewMode> _setCalendarView = _ => { };
    private Action<int> _moveCalendar = _ => { };
    private Action _goToday = () => { };
    private string _currentFilePath;

    public MainForm()
    {
        InitializeComponent();

        if(LicenseManager.UsageMode == LicenseUsageMode.Designtime)
        {
            _data = new OrganizerData();
            _currentFilePath = string.Empty;

            return;
        }

        _data = _store.Load();
        _currentFilePath = _store.DataFilePath;

        Controls.Clear();
        Text = "Organizer";
        Width = 1100;
        Height = 700;
        StartPosition = FormStartPosition.CenterScreen;

        MenuStrip? menuStrip = BuildMainMenu();
        ToolStrip? iconLine = BuildIconLine();

        _tabs = new TabControl { Dock = DockStyle.Fill, Alignment = TabAlignment.Right, Multiline = true };
        _tabs.TabPages.Add(BuildCalendarTab());
        _tabs.TabPages.Add(BuildTab("Anniversary", _data.Anniversaries));
        _tabs.TabPages.Add(BuildTab("Contacts", _data.Contacts));
        _tabs.TabPages.Add(BuildNotepadTab());

        Controls.Add(_tabs);
        Controls.Add(iconLine);
        Controls.Add(menuStrip);
        MainMenuStrip = menuStrip;
    }

    private ToolStrip BuildIconLine()
    {
        ToolStrip? toolStrip = new()
        {
            Dock = DockStyle.Top,
            GripStyle = ToolStripGripStyle.Hidden,
            ImageScalingSize = new Size(24, 24),
            BackColor = Color.FromArgb(238, 216, 159),
            Padding = new Padding(4, 2, 4, 2)
        };

        toolStrip.Items.AddRange([
            IconCommand("New", "New"),
            IconCommand("Open", "Open"),
            IconCommand("Save", "Save"),
            IconCommand("Print", "Print"),
            new ToolStripSeparator(),
            IconCommand("Cut", "Cut"),
            IconCommand("Copy", "Copy"),
            IconCommand("Paste", "Paste"),
            new ToolStripSeparator(),
            IconCommand("Find", "Find"),
            IconCommand("Today", "Today"),
            new ToolStripSeparator(),
            IconCommand("Calendar", "Calendar"),
            IconCommand("Contacts", "Contacts"),
            IconCommand("To Do", "To Do"),
            IconCommand("Notes", "Notepad"),
            new ToolStripSeparator(),
            IconCommand("Add", "Add"),
            IconCommand("Edit", "Edit"),
            IconCommand("Delete", "Delete"),
            IconCommand("Help", "Help Topics")
        ]);

        return toolStrip;
    }

    private ToolStripButton IconCommand(string label, string commandText)
    {
        ToolStripButton? button = new(label)
        {
            DisplayStyle = ToolStripItemDisplayStyle.Image,
            Image = CreateToolbarIcon(label),
            ImageTransparentColor = Color.Magenta,
            ToolTipText = commandText
        };

        button.Click += (_, _) => ExecuteCommand(commandText);

        return button;
    }

    private static Bitmap CreateToolbarIcon(string label)
    {
        Bitmap? bitmap = new(24, 24);

        using var graphics = Graphics.FromImage(bitmap);
        graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
        graphics.Clear(Color.Magenta);

        switch(label)
        {
            case "New":
                DrawPage(graphics, 5, 3, 14, 18);
                break;

            case "Open":
                DrawFolder(graphics);
                break;

            case "Save":
                DrawFloppy(graphics);
                break;

            case "Print":
                DrawPrinter(graphics);
                break;

            case "Cut":
                DrawScissors(graphics);
                break;

            case "Copy":
                DrawPage(graphics, 7, 5, 12, 15);
                DrawPage(graphics, 4, 2, 12, 15);
                break;

            case "Paste":
                DrawClipboard(graphics);
                break;

            case "Find":
                DrawMagnifier(graphics);
                break;

            case "Today":
                DrawCalendar(graphics, "T");
                break;

            case "Calendar":
                DrawCalendar(graphics, string.Empty);
                break;

            case "Contacts":
                DrawContactCard(graphics);
                break;

            case "To Do":
                DrawChecklist(graphics);
                break;

            case "Notes":
                DrawNotebook(graphics);
                break;

            case "Add":
                DrawPlus(graphics);
                break;

            case "Edit":
                DrawPencil(graphics);
                break;

            case "Delete":
                DrawTrash(graphics);
                break;

            case "Help":
                DrawHelp(graphics);
                break;
        }

        return bitmap;
    }

    private static void DrawPage(Graphics graphics, int x, int y, int width, int height)
    {
        using SolidBrush? paper = new(Color.White);
        using SolidBrush? fold = new(Color.FromArgb(232, 232, 232));
        using Pen? pen = new(Color.Navy);

        graphics.FillRectangle(paper, x, y, width, height);
        graphics.DrawRectangle(pen, x, y, width, height);
        graphics.FillPolygon(fold, [new Point(x + width - 5, y), new Point(x + width, y + 5), new Point(x + width - 5, y + 5)]);
        graphics.DrawLine(pen, x + 3, y + 8, x + width - 3, y + 8);
        graphics.DrawLine(pen, x + 3, y + 12, x + width - 3, y + 12);
    }

    private static void DrawFolder(Graphics graphics)
    {
        using SolidBrush? fill = new(Color.FromArgb(255, 210, 74));
        using SolidBrush? dark = new(Color.FromArgb(220, 154, 20));
        using Pen? pen = new(Color.FromArgb(116, 81, 0));

        graphics.FillRectangle(dark, 3, 7, 7, 3);
        graphics.FillRectangle(fill, 3, 9, 18, 11);
        graphics.DrawRectangle(pen, 3, 7, 18, 13);
    }

    private static void DrawFloppy(Graphics graphics)
    {
        using SolidBrush? body = new(Color.FromArgb(43, 95, 160));
        using SolidBrush? label = new(Color.White);
        using SolidBrush? metal = new(Color.Silver);
        using Pen? pen = new(Color.Navy);

        graphics.FillRectangle(body, 4, 3, 16, 18);
        graphics.DrawRectangle(pen, 4, 3, 16, 18);
        graphics.FillRectangle(metal, 7, 4, 9, 5);
        graphics.FillRectangle(label, 7, 13, 10, 6);
    }

    private static void DrawPrinter(Graphics graphics)
    {
        using SolidBrush? gray = new(Color.LightGray);
        using SolidBrush? dark = new(Color.DimGray);
        using SolidBrush? paper = new(Color.White);
        using Pen? pen = new(Color.Black);

        graphics.FillRectangle(paper, 7, 2, 10, 7);
        graphics.DrawRectangle(pen, 7, 2, 10, 7);
        graphics.FillRectangle(gray, 4, 8, 16, 9);
        graphics.DrawRectangle(pen, 4, 8, 16, 9);
        graphics.FillRectangle(dark, 7, 14, 10, 6);
    }

    private static void DrawScissors(Graphics graphics)
    {
        using Pen? pen = new(Color.Black, 2);
        using Pen? red = new(Color.Maroon, 2);

        graphics.DrawEllipse(red, 4, 4, 5, 5);
        graphics.DrawEllipse(red, 4, 15, 5, 5);
        graphics.DrawLine(pen, 9, 8, 19, 18);
        graphics.DrawLine(pen, 9, 16, 19, 5);
    }

    private static void DrawClipboard(Graphics graphics)
    {
        using SolidBrush? board = new(Color.FromArgb(194, 145, 68));
        using SolidBrush? paper = new(Color.White);
        using SolidBrush? clip = new(Color.Silver);
        using Pen? pen = new(Color.Black);

        graphics.FillRectangle(board, 5, 4, 14, 17);
        graphics.DrawRectangle(pen, 5, 4, 14, 17);
        graphics.FillRectangle(paper, 7, 7, 10, 12);
        graphics.FillRectangle(clip, 9, 2, 6, 4);
        graphics.DrawLine(pen, 9, 11, 15, 11);
        graphics.DrawLine(pen, 9, 15, 15, 15);
    }

    private static void DrawMagnifier(Graphics graphics)
    {
        using Pen? glass = new(Color.Navy, 2);
        using Pen? handle = new(Color.Black, 2);

        graphics.DrawEllipse(glass, 4, 4, 10, 10);
        graphics.DrawLine(handle, 13, 13, 20, 20);
    }

    private static void DrawCalendar(Graphics graphics, string text)
    {
        using SolidBrush? paper = new(Color.White);
        using SolidBrush? header = new(Color.FromArgb(170, 30, 30));
        using Pen? pen = new(Color.Black);

        graphics.FillRectangle(paper, 4, 4, 16, 16);
        graphics.DrawRectangle(pen, 4, 4, 16, 16);
        graphics.FillRectangle(header, 4, 4, 16, 4);
        graphics.DrawLine(pen, 8, 10, 8, 18);
        graphics.DrawLine(pen, 13, 10, 13, 18);
        graphics.DrawLine(pen, 5, 13, 19, 13);

        if(text.Length > 0)
        {
            using Font? font = new(SystemFonts.MessageBoxFont.FontFamily, 7f, FontStyle.Bold);

            graphics.DrawString(text, font, Brushes.Navy, 9, 10);
        }
    }

    private static void DrawContactCard(Graphics graphics)
    {
        using SolidBrush? card = new(Color.White);
        using SolidBrush? accent = new(Color.FromArgb(255, 220, 90));
        using Pen? pen = new(Color.Navy);

        graphics.FillRectangle(card, 3, 5, 18, 14);
        graphics.DrawRectangle(pen, 3, 5, 18, 14);
        graphics.FillEllipse(accent, 6, 8, 5, 5);
        graphics.DrawLine(pen, 13, 9, 18, 9);
        graphics.DrawLine(pen, 13, 13, 19, 13);
    }

    private static void DrawChecklist(Graphics graphics)
    {
        DrawPage(graphics, 4, 3, 16, 18);
        using Pen? pen = new(Color.Green, 2);

        graphics.DrawLine(pen, 6, 9, 8, 11);
        graphics.DrawLine(pen, 8, 11, 12, 7);
        graphics.DrawLine(Pens.Black, 13, 9, 18, 9);
        graphics.DrawLine(Pens.Black, 7, 15, 18, 15);
    }

    private static void DrawNotebook(Graphics graphics)
    {
        using SolidBrush? cover = new(Color.FromArgb(255, 239, 142));
        using SolidBrush? spine = new(Color.FromArgb(60, 120, 190));
        using Pen? pen = new(Color.Black);

        graphics.FillRectangle(cover, 5, 3, 15, 18);
        graphics.FillRectangle(spine, 5, 3, 4, 18);
        graphics.DrawRectangle(pen, 5, 3, 15, 18);
        graphics.DrawLine(pen, 11, 8, 18, 8);
        graphics.DrawLine(pen, 11, 12, 18, 12);
        graphics.DrawLine(pen, 11, 16, 18, 16);
    }

    private static void DrawPlus(Graphics graphics)
    {
        using SolidBrush? brush = new(Color.ForestGreen);
        graphics.FillRectangle(brush, 10, 4, 5, 16);
        graphics.FillRectangle(brush, 4, 10, 17, 5);
    }

    private static void DrawPencil(Graphics graphics)
    {
        using Pen? yellow = new(Color.Goldenrod, 4);
        using Pen? dark = new(Color.Black, 1);

        graphics.DrawLine(yellow, 6, 18, 18, 6);
        graphics.DrawLine(dark, 5, 19, 19, 5);
        graphics.FillPolygon(Brushes.Bisque, [new Point(18, 6), new Point(21, 3), new Point(20, 8)]);
    }

    private static void DrawTrash(Graphics graphics)
    {
        using SolidBrush? body = new(Color.LightGray);
        using Pen? pen = new(Color.Black);

        graphics.FillRectangle(body, 7, 8, 10, 12);
        graphics.DrawRectangle(pen, 7, 8, 10, 12);
        graphics.DrawLine(pen, 5, 7, 19, 7);
        graphics.DrawLine(pen, 9, 5, 15, 5);
        graphics.DrawLine(pen, 10, 10, 10, 18);
        graphics.DrawLine(pen, 14, 10, 14, 18);
    }

    private static void DrawHelp(Graphics graphics)
    {
        using SolidBrush? circle = new(Color.FromArgb(40, 90, 180));
        using Font? font = new(SystemFonts.MessageBoxFont.FontFamily, 14f, FontStyle.Bold);

        graphics.FillEllipse(circle, 3, 3, 18, 18);
        graphics.DrawString("?", font, Brushes.White, 6, 2);
    }

    private MenuStrip BuildMainMenu()
    {
        MenuStrip? menu = new() { Dock = DockStyle.Top };

        menu.Items.AddRange([
            BuildMenu("&File", [
                Command("&New\tCtrl+N"),
                Command("&Open\tCtrl+O"),
                Command("&Close\tCtrl+W"),
                Separator(),
                Command("Save &As...\tShift+Ctrl+S"),
                Separator(),
                Command("A&rchive...\tCtrl+A"),
                Command("Co&mpact..."),
                Command("Mer&ge...\tCtrl+M"),
                Command("Con&vert..."),
                Command("&Import...\tCtrl+I"),
                Command("&Export..."),
                Separator(),
                Command("Mee&ting Notices..."),
                Command("&Work Offline"),
                Separator(),
                Command("Send Mai&l..."),
                Separator(),
                Command("Publish &Busy Time Now"),
                Command("Publis&h as Web Pages..."),
                Separator(),
                Command("&Print...\tCtrl+P"),
                BuildMenu("&User Setup", [
                    Command("&Organizer Preferences..."),
                    Command("&Printer...\tShift+Ctrl+P"),
                    Command("Mail and &Scheduling..."),
                    Command("Smart&Icons..."),
                    Command("Pass&words...\tCtrl+U"),
                    Command("&Telephone Dialing...")
                ]),
                Separator(),
                Command("E&xit Organizer")
            ]),
            BuildMenu("&Edit", [
                Command("&Undo\tCtrl+Z"),
                Separator(),
                Command("Cu&t\tCtrl+X"),
                Command("&Copy\tCtrl+C"),
                Command("&Paste\tCtrl+V"),
                Command("C&lear\tDel"),
                Separator(),
                Command("Copy &Special..."),
                Command("P&aste Special..."),
                Separator(),
                Command("&Edit...\tCtrl+E"),
                Separator(),
                Command("&Organizer Links..."),
                Separator(),
                Command("&Go To...\tCtrl+G"),
                Command("&Find...\tCtrl+F"),
                Command("Find Person via &Internet...\tCtrl+J"),
                Separator(),
                Command("OLE Li&nks...")
            ]),
            BuildMenu("&View", [
                Command("&1 Day Planner"),
                Command("&2 Day per Page"),
                Command("&3 Multiple Calendar"),
                Command("&4 Work Week"),
                Command("&5 Week per Page"),
                Command("&6 Weekly Time Slot"),
                Command("&7 Month"),
                Command("&8 Year"),
                Separator(),
                Command("&Show Clean Screen\tF11"),
                Command("Collapse Bi&nder Panel\tF12"),
                Separator(),
                Command("&Fold Out"),
                Separator(),
                Command("&Apply Filter"),
                Command("C&lear Filter"),
                Separator(),
                Command("Calendar &Preferences...")
            ]),
            BuildMenu("&Create", [
                Command("&Appointment...\tIns"),
                BuildMenu("&Entry In", [
                    Command("Calendar...", "Entry In Calendar"),
                    Command("To Do...", "Entry In To Do"),
                    Command("Contacts...", "Entry In Contacts"),
                    Command("Notepad...", "Entry In Notepad"),
                    Command("Anniversary...", "Entry In Anniversary"),
                    Command("Holidays...", "Entry In Holidays"),
                    Command("Recipies...", "Entry In Recipies"),
                    Command("Races...", "Entry In Races"),
                    Command("Blue Jays...", "Entry In Blue Jays"),
                    Command("&More sections...")
                ]),
                Separator(),
                Command("Organizer &Link\tCtrl+L"),
                Command("Co&mment Link..."),
                Command("F&ile Link..."),
                Command("I&nternet Link..."),
                Separator(),
                Command("&Filters..."),
                Command("&Categories..."),
                Command("Cost Co&des..."),
                Separator(),
                Command("O&bject..."),
                Separator(),
                Command("&Group of Contacts..."),
                Command("Street Ma&p..."),
                Command("Dri&ving Directions...")
            ]),
            BuildMenu("&Section", [
                Command("&Customize..."),
                Command("&Show Through..."),
                Command("&Include..."),
                Separator(),
                BuildMenu("&Turn To", [
                    Command("Calendar", "Turn To Calendar"),
                    Command("To Do", "Turn To To Do"),
                    Command("Contacts", "Turn To Contacts"),
                    Command("Notepad", "Turn To Notepad"),
                    Command("Anniversary", "Turn To Anniversary"),
                    Command("Holidays", "Turn To Holidays"),
                    Command("Recipies", "Turn To Recipies"),
                    Command("Races", "Turn To Races"),
                    Command("Blue Jays", "Turn To Blue Jays"),
                    Command("&More sections...")
                ])
            ]),
            BuildMenu("&Appointment", [
                Command("&Categorize...\tF5"),
                Command("A&larm...\tF6"),
                Command("&Repeat...\tF7"),
                Command("C&ost...\tF8"),
                Separator(),
                Command("&Warn Of Conflicts"),
                Command("&Pencil in"),
                Command("Con&fidential\tF4")
            ]),
            BuildMenu("&Phone", [
                Command("&Dial...\tCtrl+D"),
                Command("&Quick Dial...\tCtrl+Q"),
                Separator(),
                Command("&Incoming Call..."),
                Separator(),
                Command("&Change Area Codes...")
            ]),
            BuildMenu("&Help", [
                Command("&Help Topics"),
                Command("&Bubble Help\tCtrl+F1"),
                Separator(),
                Command("&About Organizer")
            ])
        ]);

        return menu;
    }

    private ToolStripMenuItem BuildMenu(string text, ToolStripItem[] children)
    {
        ToolStripMenuItem? menuItem = new(text);

        menuItem.DropDownItems.AddRange(children);

        return menuItem;
    }

    private ToolStripMenuItem Command(string text)
    {
        ToolStripMenuItem? menuItem = new(text);

        menuItem.Click += (_, _) => ExecuteCommand(text);

        return menuItem;
    }

    private ToolStripMenuItem Command(string text, string commandKey)
    {
        ToolStripMenuItem? menuItem = new(text);

        menuItem.Click += (_, _) => ExecuteCommand(commandKey);

        return menuItem;
    }

    private static ToolStripSeparator Separator() => new();

    private void ExecuteCommand(string commandText)
    {
        string? command = CleanMenuText(commandText);

        if(command.Equals("Organizer Preferences", StringComparison.OrdinalIgnoreCase))
        {
            ShowOrganizerPreferences();

            return;
        }

        if(command.Equals("Printer", StringComparison.OrdinalIgnoreCase) || command.Equals("Print Setup", StringComparison.OrdinalIgnoreCase))
        {
            ShowPrinterSetup();

            return;
        }

        if(command.Equals("About Organizer", StringComparison.OrdinalIgnoreCase))
        {
            aboutToolStripMenuItem_Click(this, EventArgs.Empty);
            return;
        }

        if(ExecuteCommonOrganizerCommand(command)) return;

        ShowNotImplemented(commandText);
    }

    private bool ExecuteCommonOrganizerCommand(string command)
    {
        switch(command)
        {
            case "New":
                NewOrganizer();
                return true;

            case "Open":
                OpenOrganizer();
                return true;

            case "Close":
                CloseOrganizerFile();
                return true;

            case "Save":
                SaveData();
                return true;

            case "Save As":
            case "Export":
                SaveOrganizerAs();
                return true;

            case "Import":
                OpenOrganizer();
                return true;

            case "Exit Organizer":
            case "Exit":
                Close();
                return true;

            case "Calendar":
            case "Turn To Calendar":
                SelectSection("Calendar");
                return true;

            case "Contacts":
            case "Turn To Contacts":
                SelectSection("Contacts");
                return true;

            case "Anniversary":
            case "Turn To Anniversary":
                SelectSection("Anniversary");
                return true;

            case "To Do":
            case "Turn To To Do":
                SelectSection("To Do");
                return true;

            case "Notepad":
            case "Turn To Notepad":
                SelectSection("Notepad");
                return true;

            case "1 Day Planner":
            case "2 Day per Page":
                SelectSection("Calendar");
                _setCalendarView(CalendarViewMode.Day);
                return true;

            case "3 Multiple Calendar":
            case "4 Work Week":
            case "5 Week per Page":
            case "6 Weekly Time Slot":
                SelectSection("Calendar");
                _setCalendarView(CalendarViewMode.Week);
                return true;

            case "7 Month":
                SelectSection("Calendar");
                _setCalendarView(CalendarViewMode.Month);
                return true;

            case "8 Year":
                SelectSection("Calendar");
                ShowNotImplemented("Year view");
                return true;

            case "Appointment":
            case "Entry In Calendar":
                AddCalendarEvent();
                return true;

            case "Entry In Contacts":
            case "Group of Contacts":
                AddSectionItem<Contact>("Contacts");
                return true;

            case "Entry In To Do":
                AddSectionItem<OrganizerTask>("To Do");
                return true;

            case "Entry In Notepad":
                AddSectionItem<Note>("Notepad");
                return true;

            case "Entry In Anniversary":
                AddSectionItem<Anniversary>("Anniversary");
                return true;

            case "Edit":
                EditCurrentSectionItem();
                return true;

            case "Clear":
            case "Delete":
                DeleteCurrentSectionItem();
                return true;

            case "Print":
                ShowPrintDialog();
                return true;

            case "Page Setup":
                ShowPageSetupDialog();
                return true;

            case "Alarms":
                ShowAlarms();
                return true;

            case "Today":
                SelectSection("Calendar");
                _goToday();
                return true;

            case "Next":
                _moveCalendar(1);
                return true;

            case "Previous":
                _moveCalendar(-1);
                return true;

            case "Add":
                AddCurrentSectionItem();
                return true;

            case "Calendar Preferences":
                ShowOrganizerPreferences();
                return true;
        }

        return false;
    }

    private void ShowOrganizerPreferences()
    {
        if(OrganizerPreferencesDialog.Edit(this, _data.Preferences)) _store.Save(_data);
    }

    private void AddCurrentSectionItem()
    {
        switch(_tabs.SelectedTab?.Text)
        {
            case "Calendar":
                AddCalendarEvent();
                break;

            case "To Do":
                AddSectionItem<OrganizerTask>("To Do");
                break;

            case "Contacts":
                AddSectionItem<Contact>("Contacts");
                break;

            case "Anniversary":
                AddSectionItem<Anniversary>("Anniversary");
                break;

            case "Notepad":
                AddSectionItem<Note>("Notepad");
                break;

            default:
                ShowNotImplemented("Add");
                break;
        }
    }

    private void ShowPrinterSetup()
    {
        if(PrinterSetupDialog.Edit(this, _data.Preferences)) _store.Save(_data);
    }

    private void SaveData()
    {
        _store.SaveTo(_data, _currentFilePath);
        MessageBox.Show(this, "Saved.", "Organizer", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private void NewOrganizer()
    {
        if(MessageBox.Show(this, "Create a new Organizer file? Unsaved changes will be replaced.", "New Organizer", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

        LoadData(new OrganizerData());
        _currentFilePath = _store.DataFilePath;
        _store.Save(_data);
    }

    private void CloseOrganizerFile()
    {
        if(MessageBox.Show(this, "Close the current Organizer file? Unsaved changes will be replaced.", "Close Organizer File", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

        LoadData(new OrganizerData());
        _currentFilePath = _store.DataFilePath;
    }

    private void OpenOrganizer()
    {
        using var dialog = new OpenFileDialog
        {
            Title = "Open Organizer File",
            Filter = "Organizer data (*.json)|*.json|All files (*.*)|*.*",
            InitialDirectory = GetCurrentFileDirectory(),
            FileName = Path.GetFileName(_currentFilePath)
        };

        if(dialog.ShowDialog(this) != DialogResult.OK) return;

        LoadData(_store.LoadFrom(dialog.FileName));
        _currentFilePath = dialog.FileName;
    }

    private void SaveOrganizerAs()
    {
        using SaveFileDialog? dialog = new()
        {
            Title = "Save Organizer File As",
            Filter = "Organizer data (*.json)|*.json|All files (*.*)|*.*",
            InitialDirectory = GetCurrentFileDirectory(),
            FileName = "organizer-data.json"
        };

        if(dialog.ShowDialog(this) == DialogResult.OK)
        {
            _store.SaveTo(_data, dialog.FileName);
            _currentFilePath = dialog.FileName;
        }
    }

    private string GetCurrentFileDirectory()
    {
        string? directory = Path.GetDirectoryName(_currentFilePath);

        return !string.IsNullOrWhiteSpace(directory) && Directory.Exists(directory)
            ? directory
            : Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
    }

    private void LoadData(OrganizerData data)
    {
        ReplaceList(_data.Events, data.Events);
        ReplaceList(_data.Tasks, data.Tasks);
        ReplaceList(_data.Contacts, data.Contacts);
        ReplaceList(_data.Notes, data.Notes);
        ReplaceList(_data.Anniversaries, data.Anniversaries);
        _data.Preferences = data.Preferences ?? new OrganizerPreferences();

        RefreshSectionSource<OrganizerTask>("To Do", _data.Tasks);
        RefreshSectionSource<Anniversary>("Anniversary", _data.Anniversaries);
        RefreshSectionSource<Contact>("Contacts", _data.Contacts);
        RefreshSectionSource<Note>("Notepad", _data.Notes);
        _refreshCalendar();
    }

    private static void ReplaceList<T>(List<T> target, List<T>? source)
    {
        target.Clear();

        if(source is not null) target.AddRange(source);
    }

    private void RefreshSectionSource<T>(string sectionName, List<T> items) where T : class
    {
        if(!_sectionSources.TryGetValue(sectionName, out BindingSource? source)) return;

        source.ResetBindings(false);
    }

    private void SelectSection(string sectionName)
    {
        string? tabName = sectionName switch
        {
            "Calendar" => "Calendar",
            "To Do" => "To Do",
            "Anniversary" => "Anniversary",
            "Contacts" => "Contacts",
            "Notepad" => "Notepad",
            _ => sectionName
        };

        TabPage? page = _tabs.TabPages.Cast<TabPage>().FirstOrDefault(tab => tab.Text.Equals(tabName, StringComparison.OrdinalIgnoreCase));

        if(page is null)
        {
            ShowNotImplemented(sectionName);

            return;
        }

        _tabs.SelectedTab = page;
    }

    private void AddCalendarEvent()
    {
        SelectSection("Calendar");

        CalendarEvent? calendarEvent = new() { Start = DateTime.Today.AddHours(9), End = DateTime.Today.AddHours(10) };

        if(CreateAppointmentDialog.Edit(this, calendarEvent, "Create Appointment", _data.Events))
        {
            _data.Events.Add(calendarEvent);
            _store.Save(_data);
            _refreshCalendar();
        }
    }

    private void AddSectionItem<T>(string sectionName) where T : class, new()
    {
        SelectSection(sectionName);

        T? item = new();

        if(RecordEditorDialog.Edit(this, item, $"Add {Singular(sectionName)}"))
        {
            ApplyRecordDefaults(item);

            if(_sectionSources.TryGetValue(sectionName, out var source) && source.DataSource is SortableBindingList<T> list)
            {
                list.Add(item);
                source.ResetBindings(false);
            }
            else if(item is OrganizerTask task)
            {
                _data.Tasks.Add(task);
            }

            _store.Save(_data);

            if(item is OrganizerTask) _refreshCalendar();
        }
    }

    private void EditCurrentSectionItem()
    {
        string? sectionName = _tabs.SelectedTab?.Text;

        if(sectionName is null || !_sectionSources.TryGetValue(sectionName, out var source) || source.Current is null)
        {
            ShowNotImplemented("Edit");

            return;
        }

        object? item = source.Current;

        if(RecordEditorDialog.Edit(this, item, $"Edit {Singular(sectionName)}"))
        {
            ApplyRecordDefaults(item);
            source.ResetCurrentItem();
            _store.Save(_data);

            if(item is OrganizerTask) _refreshCalendar();
        }
    }

    private void DeleteCurrentSectionItem()
    {
        string? sectionName = _tabs.SelectedTab?.Text;

        if(sectionName is null || !_sectionSources.TryGetValue(sectionName, out var source) || source.Current is null)
        {
            ShowNotImplemented("Clear");

            return;
        }

        if(MessageBox.Show(this, "Delete the selected item?", "Confirm delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

        object? item = source.Current;

        source.RemoveCurrent();
        _store.Save(_data);

        if(item is OrganizerTask) _refreshCalendar();
    }

    private void ShowPrintDialog()
    {
        using PrintDocument? document = CreatePrintDocument();
        using PrintDialog? dialog = new() { Document = document, UseEXDialog = true };

        if(dialog.ShowDialog(this) == DialogResult.OK)
        {
            document.PrinterSettings = dialog.PrinterSettings;
            document.Print();
        }
    }

    private void ShowPageSetupDialog()
    {
        using PrintDocument? document = CreatePrintDocument();
        using PageSetupDialog? dialog = new() { Document = document, EnableMetric = true };

        if(dialog.ShowDialog(this) == DialogResult.OK)
        {
            _data.Preferences.PrinterLandscape = document.DefaultPageSettings.Landscape;
            _data.Preferences.MarginLeft = document.DefaultPageSettings.Margins.Left;
            _data.Preferences.MarginRight = document.DefaultPageSettings.Margins.Right;
            _data.Preferences.MarginTop = document.DefaultPageSettings.Margins.Top;
            _data.Preferences.MarginBottom = document.DefaultPageSettings.Margins.Bottom;
            _store.Save(_data);
        }
    }

    private PrintDocument CreatePrintDocument()
    {
        PrintDocument? document = new();

        if(!string.IsNullOrWhiteSpace(_data.Preferences.PrinterName)) document.PrinterSettings.PrinterName = _data.Preferences.PrinterName;

        document.DefaultPageSettings.Landscape = _data.Preferences.PrinterLandscape;
        document.DefaultPageSettings.Margins = new Margins(_data.Preferences.MarginLeft, _data.Preferences.MarginRight, _data.Preferences.MarginTop, _data.Preferences.MarginBottom);
        document.PrintPage += (_, e) =>
        {
            string? text = BuildPrintableSummary();
            e.Graphics?.DrawString(text, SystemFonts.MessageBoxFont, Brushes.Black, e.MarginBounds);
        };

        return document;
    }

    private string BuildPrintableSummary()
    {
        StringBuilder? builder = new();

        builder.AppendLine("Organizer");
        builder.AppendLine($"Printed: {DateTime.Now:g}");
        builder.AppendLine();
        builder.AppendLine($"Appointments: {_data.Events.Count}");
        builder.AppendLine($"To Do: {_data.Tasks.Count}");
        builder.AppendLine($"Contacts: {_data.Contacts.Count}");
        builder.AppendLine($"Notepad: {_data.Notes.Count}");

        return builder.ToString();
    }

    private void ShowAlarms()
    {
        DateTime today = DateTime.Today;
        IEnumerable<string>? upcomingEvents = _data.Events.Where(item => item.Start >= DateTime.Now && item.Start < today.AddDays(7)).OrderBy(item => item.Start).Select(item => $"Appointment: {item.Start:g} {item.Title}");
        IEnumerable<string>? dueTasks = _data.Tasks.Where(item => !item.Completed && item.DueDate.Date <= today.AddDays(7)).OrderBy(item => item.DueDate).Select(item => $"To Do: {item.DueDate:g} {item.Title}");
        IEnumerable<string>? lines = upcomingEvents.Concat(dueTasks).DefaultIfEmpty("No upcoming alarms.");

        MessageBox.Show(this, string.Join(Environment.NewLine, lines), "Alarms", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private void ShowNotImplemented(string commandText)
    {
        MessageBox.Show(
            this,
            $"{CleanMenuText(commandText)} is not yet implemented.",
            "Not Yet Implemented",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information);
    }

    private static string CleanMenuText(string commandText) => commandText.Split('\t')[0].Replace("&", string.Empty, StringComparison.Ordinal).Replace("...", string.Empty, StringComparison.Ordinal);

    private Control BuildTrashDropTarget()
    {
        Panel? panel = new()
        {
            Dock = DockStyle.Bottom,
            Height = 58,
            BackColor = Color.FromArgb(178, 134, 61),
            Padding = new Padding(10, 6, 0, 6),
            AllowDrop = true
        };

        Label? trash = new()
        {
            AutoSize = false,
            Width = 58,
            Dock = DockStyle.Left,
            Text = "🗑",
            TextAlign = ContentAlignment.MiddleCenter,
            Font = new Font(Font.FontFamily, 18f, FontStyle.Bold),
            BackColor = Color.FromArgb(238, 216, 159),
            ForeColor = Color.FromArgb(72, 48, 24),
            BorderStyle = BorderStyle.FixedSingle,
            AllowDrop = true
        };

        void DragEnterHandler(object? sender, DragEventArgs e)
        {
            e.Effect = e.Data?.GetDataPresent(typeof(TrashDropPayload)) == true ? DragDropEffects.Move : DragDropEffects.None;
        }

        void DragDropHandler(object? sender, DragEventArgs e)
        {
            if(e.Data?.GetData(typeof(TrashDropPayload)) is not TrashDropPayload payload) return;

            payload.Delete();
        }

        panel.DragEnter += DragEnterHandler;
        panel.DragDrop += DragDropHandler;
        trash.DragEnter += DragEnterHandler;
        trash.DragDrop += DragDropHandler;

        panel.Controls.Add(trash);

        return panel;
    }

    private TabPage BuildCalendarTab()
    {
        TabPage? page = new("Calendar");
        MonthCalendar? monthCalendar = new() { Dock = DockStyle.Top, MaxSelectionCount = 1, ShowTodayCircle = true };
        CalendarPlannerView? planner = new() { Dock = DockStyle.Fill };

        CalendarEvent? selectedEvent = null;
        OrganizerTask? selectedTask = null;

        Button? dayViewButton = new() { Text = "Day", Width = 90 };
        Button? weekViewButton = new() { Text = "Week", Width = 90 };
        Button? monthViewButton = new() { Text = "Month", Width = 90 };
        Button? previousButton = new() { Text = "<", Width = 44 };
        Button? nextButton = new() { Text = ">", Width = 44 };
        Button? addButton = new() { Text = "Create Appointment", Width = 140 };
        Button? editButton = new() { Text = "Edit", Width = 90 };
        Button? deleteButton = new() { Text = "Delete", Width = 90 };
        Button? todayButton = new() { Text = "Today", Width = 90 };
        CalendarViewMode viewMode = CalendarViewMode.Day;
        DateTime plannerDate = monthCalendar.SelectionStart.Date;

        void RefreshCalendar()
        {
            selectedEvent = null;
            selectedTask = null;
            planner.SelectedDate = plannerDate;
            planner.ViewMode = viewMode;
            planner.Events = [.. GetCalendarEvents(plannerDate, viewMode)];
            planner.Tasks = [.. GetCalendarTasks(plannerDate, viewMode)];
            planner.Invalidate();
        }

        _refreshCalendar = RefreshCalendar;

        void MoveSelection(int direction)
        {
            plannerDate = viewMode switch
            {
                CalendarViewMode.Week => plannerDate.AddDays(7 * direction),
                CalendarViewMode.Month => plannerDate.AddMonths(direction),
                _ => plannerDate.AddDays(direction)
            };

            RefreshCalendar();
        }

        _setCalendarView = mode =>
        {
            viewMode = mode;
            RefreshCalendar();
        };

        _moveCalendar = MoveSelection;
        _goToday = () =>
        {
            plannerDate = DateTime.Today;
            RefreshCalendar();
        };

        void EditSelectedCalendarEvent()
        {
            if(selectedEvent is CalendarEvent calendarEvent)
            {
                if(CreateAppointmentDialog.Edit(this, calendarEvent, "Edit Appointment", _data.Events))
                {
                    _store.Save(_data);
                    RefreshCalendar();
                }

                return;
            }

            if(selectedTask is not OrganizerTask task) return;

            if(RecordEditorDialog.Edit(this, task, "Edit Task"))
            {
                ApplyRecordDefaults(task);
                _store.Save(_data);
                RefreshCalendar();
            }
        }

        dayViewButton.Click += (_, _) => { viewMode = CalendarViewMode.Day; RefreshCalendar(); };
        weekViewButton.Click += (_, _) => { viewMode = CalendarViewMode.Week; RefreshCalendar(); };
        monthViewButton.Click += (_, _) => { viewMode = CalendarViewMode.Month; RefreshCalendar(); };

        monthCalendar.DateSelected += (_, _) =>
        {
            plannerDate = monthCalendar.SelectionStart.Date;
            RefreshCalendar();
        };

        previousButton.Click += (_, _) => MoveSelection(-1);
        nextButton.Click += (_, _) => MoveSelection(1);
        todayButton.Click += (_, _) => _goToday();

        planner.EventSelected += (_, calendarEvent) =>
        {
            selectedEvent = calendarEvent;
            selectedTask = null;
        };

        planner.EventDoubleClicked += (_, calendarEvent) =>
        {
            selectedEvent = calendarEvent;
            selectedTask = null;
            EditSelectedCalendarEvent();
        };

        planner.TaskSelected += (_, task) =>
        {
            selectedEvent = null;
            selectedTask = task;
        };

        planner.TaskDoubleClicked += (_, task) =>
        {
            selectedEvent = null;
            selectedTask = task;
            EditSelectedCalendarEvent();
        };

        planner.EmptyAreaDoubleClicked += (_, _) => addButton.PerformClick();

        addButton.Click += (_, _) =>
        {
            DateTime date = monthCalendar.SelectionStart.Date;
            CalendarEvent? calendarEvent = new() { Start = date.AddHours(9), End = date.AddHours(10) };

            if(CreateAppointmentDialog.Edit(this, calendarEvent, "Create Appointment", _data.Events))
            {
                _data.Events.Add(calendarEvent);
                _store.Save(_data);
                RefreshCalendar();
            }
        };

        editButton.Click += (_, _) => EditSelectedCalendarEvent();

        deleteButton.Click += (_, _) =>
        {
            if(selectedEvent is not CalendarEvent calendarEvent) return;
            if(MessageBox.Show(this, "Delete the selected event?", "Confirm delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

            _data.Events.Remove(calendarEvent);
            _store.Save(_data);
            RefreshCalendar();
        };

        FlowLayoutPanel? buttonPanel = new() { Dock = DockStyle.Top, Height = 48, Padding = new Padding(8), FlowDirection = FlowDirection.LeftToRight, BackColor = Color.FromArgb(199, 156, 75) };

        buttonPanel.Controls.AddRange([previousButton, nextButton]);

        Control? trashDropTarget = BuildTrashDropTarget();
        Panel? leftPanel = new() { Dock = DockStyle.Left, Width = 260, Padding = new Padding(8), BackColor = Color.FromArgb(219, 196, 137) };

        leftPanel.Controls.Add(buttonPanel);
        leftPanel.Controls.Add(monthCalendar);

        Panel rightPanel = new() { Dock = DockStyle.Fill, Padding = new Padding(8), BackColor = Color.FromArgb(178, 134, 61) };

        rightPanel.Controls.Add(planner);

        page.Controls.Add(rightPanel);
        page.Controls.Add(leftPanel);
        RefreshCalendar();

        return page;
    }

    private IEnumerable<CalendarEvent> GetCalendarEvents(DateTime selectedDate, CalendarViewMode viewMode)
    {
        return viewMode switch
        {
            CalendarViewMode.Day => _data.Events.Where(e => e.Start.Date == selectedDate.Date).OrderBy(e => e.Start),
            CalendarViewMode.Week => _data.Events.Where(e => e.Start.Date >= StartOfWeek(selectedDate) && e.Start.Date < StartOfWeek(selectedDate).AddDays(7)).OrderBy(e => e.Start),
            CalendarViewMode.Month => _data.Events.Where(e => e.Start.Year == selectedDate.Year && e.Start.Month == selectedDate.Month).OrderBy(e => e.Start),
            _ => _data.Events.OrderBy(e => e.Start)
        };
    }

    private IEnumerable<OrganizerTask> GetCalendarTasks(DateTime selectedDate, CalendarViewMode viewMode)
    {
        return viewMode switch
        {
            CalendarViewMode.Day => _data.Tasks.Where(task => TaskOccursInRange(task, selectedDate.Date, selectedDate.Date.AddDays(1))).OrderBy(TaskPriority).ThenBy(task => task.Completed).ThenBy(task => task.Title),
            CalendarViewMode.Week => _data.Tasks.Where(task => TaskOccursInRange(task, StartOfWeek(selectedDate), StartOfWeek(selectedDate).AddDays(7))).OrderBy(TaskPriority).ThenBy(task => task.Completed).ThenBy(task => task.Title),
            CalendarViewMode.Month => _data.Tasks.Where(task => TaskOccursInRange(task, new DateTime(selectedDate.Year, selectedDate.Month, 1), new DateTime(selectedDate.Year, selectedDate.Month, 1).AddMonths(1))).OrderBy(TaskPriority).ThenBy(task => task.Completed).ThenBy(task => task.Title),
            _ => _data.Tasks.OrderBy(TaskPriority).ThenBy(task => task.Completed).ThenBy(task => task.Title)
        };
    }

    private static bool TaskOccursInRange(OrganizerTask task, DateTime startInclusive, DateTime endExclusive)
    {
        for(DateTime date = startInclusive.Date; date < endExclusive.Date; date = date.AddDays(1))
        {
            if(TaskOccursOnDate(task, date)) return true;
        }

        return false;
    }

    private static bool TaskOccursOnDate(OrganizerTask task, DateTime date)
    {
        DateTime dueDate = task.DueDate.Date;

        if(date.Date == dueDate) return true;
        if(date.Date < dueDate || task.RepeatEvery <= 0 || task.RepeatUnit == "None") return false;

        return task.RepeatUnit switch
        {
            "Hours" => HourlyTaskOccursOnDate(task, date),
            "Days" => ((int)(date.Date - dueDate).TotalDays) % task.RepeatEvery == 0,
            "Weeks" => ((int)(date.Date - dueDate).TotalDays) % (task.RepeatEvery * 7) == 0,
            "Months" => MonthlyTaskOccursOnDate(task, date),
            "Years" => YearlyTaskOccursOnDate(task, date),
            _ => false
        };
    }

    private static bool HourlyTaskOccursOnDate(OrganizerTask task, DateTime date)
    {
        DateTime start = date.Date;
        DateTime end = start.AddDays(1);

        if(task.DueDate >= end) return false;
        if(task.DueDate >= start) return true;

        double occurrencesToRange = Math.Ceiling((start - task.DueDate).TotalHours / task.RepeatEvery);

        return task.DueDate.AddHours(occurrencesToRange * task.RepeatEvery) < end;
    }

    private static bool MonthlyTaskOccursOnDate(OrganizerTask task, DateTime date)
    {
        int months = (date.Year - task.DueDate.Year) * 12 + date.Month - task.DueDate.Month;

        return months >= 0 && months % task.RepeatEvery == 0 && task.DueDate.AddMonths(months).Date == date.Date;
    }

    private static bool YearlyTaskOccursOnDate(OrganizerTask task, DateTime date)
    {
        int years = date.Year - task.DueDate.Year;

        return years >= 0 && years % task.RepeatEvery == 0 && task.DueDate.AddYears(years).Date == date.Date;
    }

    private static int TaskPriority(OrganizerTask task)
    {
        return task.Priority switch
        {
            "Low" or "1" => 1,
            "Medium" or "2" => 2,
            "High" or "3" => 3,
            _ => 1
        };
    }

    private static DateTime StartOfWeek(DateTime date)
    {
        int diff = (7 + (date.DayOfWeek - DayOfWeek.Sunday)) % 7;

        return date.Date.AddDays(-diff);
    }

    private TabPage BuildTab<T>(string title, List<T> items) where T : class, new()
    {
        TabPage? page = new(title);
        BindingSource? source = new() { DataSource = new SortableBindingList<T>(items) };

        _sectionSources[title] = source;

        DataGridView? grid = new()
        {
            Dock = DockStyle.Fill,
            AutoGenerateColumns = true,
            DataSource = source,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            MultiSelect = false,
            ReadOnly = true,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        };

        grid.DataBindingComplete += (_, _) =>
        {
            if(grid.Columns.Contains("Id")) grid.Columns["Id"].Visible = false;

            foreach(DataGridViewColumn column in grid.Columns)
            {
                column.SortMode = DataGridViewColumnSortMode.Automatic;
            }
        };

        grid.CellDoubleClick += (_, _) => EditSelected(source, title);

        Point? dragStart = null;
        T? dragItem = null;

        grid.MouseDown += (_, e) =>
        {
            if(title is not ("Contacts" or "Tasks") || e.Button != MouseButtons.Left)
            {
                dragStart = null;
                dragItem = null;

                return;
            }

            DataGridView.HitTestInfo? hit = grid.HitTest(e.X, e.Y);

            if(hit.RowIndex < 0)
            {
                dragStart = null;
                dragItem = null;

                return;
            }

            grid.ClearSelection();
            grid.Rows[hit.RowIndex].Selected = true;
            source.Position = hit.RowIndex;
            dragStart = e.Location;
            dragItem = source.Current as T;
        };

        grid.MouseMove += (_, e) =>
        {
            if(dragStart is not Point start || dragItem is null || e.Button != MouseButtons.Left) return;

            var dragSize = SystemInformation.DragSize;
            var dragRectangle = new Rectangle(
                start.X - dragSize.Width / 2,
                start.Y - dragSize.Height / 2,
                dragSize.Width,
                dragSize.Height);

            if(dragRectangle.Contains(e.Location)) return;

            var itemToDelete = dragItem;
            var payload = new TrashDropPayload(() =>
            {
                ((SortableBindingList<T>)source.DataSource).Remove(itemToDelete);
                _store.Save(_data);
                if(itemToDelete is OrganizerTask)
                {
                    _refreshCalendar();
                }
            });

            dragStart = null;
            dragItem = null;
            grid.DoDragDrop(payload, DragDropEffects.Move);
        };

        grid.MouseUp += (_, _) =>
        {
            dragStart = null;
            dragItem = null;
        };

        var addButton = new Button { Text = "Add", Width = 90 };
        var editButton = new Button { Text = "Edit", Width = 90 };
        var deleteButton = new Button { Text = "Delete", Width = 90 };
        var saveButton = new Button { Text = "Save", Width = 90 };

        addButton.Click += (_, _) =>
        {
            var item = new T();

            if(RecordEditorDialog.Edit(this, item, $"Add {Singular(title)}"))
            {
                ApplyRecordDefaults(item);
                ((SortableBindingList<T>)source.DataSource).Add(item);
                _store.Save(_data);
            }
        };

        editButton.Click += (_, _) => EditSelected(source, title);
        deleteButton.Click += (_, _) =>
        {
            if(source.Current is not T item) return;
            if(MessageBox.Show(this, "Delete the selected item?", "Confirm delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

            ((SortableBindingList<T>)source.DataSource).Remove(item);
            _store.Save(_data);
        };

        saveButton.Click += (_, _) =>
        {
            _store.Save(_data);
            MessageBox.Show(this, "Saved.", "Organizer", MessageBoxButtons.OK, MessageBoxIcon.Information);
        };

        var buttonPanel = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 42, Padding = new Padding(8), FlowDirection = FlowDirection.LeftToRight };
        buttonPanel.Controls.AddRange([addButton, editButton, deleteButton, saveButton]);
        page.Controls.Add(grid);
        page.Controls.Add(buttonPanel);

        return page;

        void EditSelected(BindingSource bindingSource, string tabTitle)
        {
            if(bindingSource.Current is not T item) return;

            if(RecordEditorDialog.Edit(this, item, $"Edit {Singular(tabTitle)}"))
            {
                ApplyRecordDefaults(item);
                bindingSource.ResetCurrentItem();
                _store.Save(_data);
            }
        }
    }

    private TabPage BuildNotepadTab()
    {
        var page = new TabPage("Notepad");
        var source = new BindingSource { DataSource = new SortableBindingList<Note>(_data.Notes) };
        _sectionSources["Notepad"] = source;

        var tree = new TreeView
        {
            Dock = DockStyle.Fill,
            HideSelection = false,
            LabelEdit = true,
            AllowDrop = true
        };

        var titleBox = new TextBox { Dock = DockStyle.Top, Height = 24, Margin = new Padding(0, 0, 0, 6) };
        var updatedLabel = new Label { Dock = DockStyle.Top, Height = 22, TextAlign = ContentAlignment.MiddleLeft };
        var bodyBox = new RichTextBox
        {
            Dock = DockStyle.Fill,
            ScrollBars = RichTextBoxScrollBars.Vertical,
            AcceptsTab = true,
            Font = new Font(FontFamily.GenericSerif, 11f),
            DetectUrls = true
        };

        var loadingNote = false;

        void RebuildNotebookTree(Note? selectNote = null)
        {
            tree.BeginUpdate();
            tree.Nodes.Clear();

            var headings = _data.Notes
                .Where(note => note.IsChapterHeading)
                .OrderBy(note => note.SortOrder)
                .ThenBy(note => note.Title)
                .ToList();

            foreach(var heading in headings)
            {
                var headingNode = CreateNoteNode(heading);
                tree.Nodes.Add(headingNode);

                foreach(var child in _data.Notes.Where(note => !note.IsChapterHeading && note.ParentHeadingId == heading.Id).OrderBy(note => note.SortOrder).ThenBy(note => note.Title))
                {
                    headingNode.Nodes.Add(CreateNoteNode(child));
                }
            }

            foreach(var note in _data.Notes.Where(note => !note.IsChapterHeading && note.ParentHeadingId is null).OrderBy(note => note.SortOrder).ThenBy(note => note.Title))
            {
                tree.Nodes.Add(CreateNoteNode(note));
            }

            tree.ExpandAll();

            if(selectNote is not null)
            {
                tree.SelectedNode = FindNoteNode(tree.Nodes, selectNote);
            }

            tree.EndUpdate();
        }

        void LoadCurrentNote()
        {
            loadingNote = true;

            if(tree.SelectedNode?.Tag is Note note)
            {
                titleBox.Enabled = true;
                bodyBox.Enabled = !note.IsChapterHeading;
                titleBox.Text = note.Title;
                if(note.IsChapterHeading)
                {
                    bodyBox.Clear();
                    updatedLabel.Text = "Chapter heading";
                }
                else
                {
                    LoadRichTextBody(bodyBox, note.Body);
                    updatedLabel.Text = $"Last updated: {note.Updated:g}";
                }
            }
            else
            {
                titleBox.Enabled = false;
                bodyBox.Enabled = false;
                titleBox.Text = string.Empty;
                bodyBox.Text = string.Empty;
                updatedLabel.Text = "No note selected";
            }

            loadingNote = false;
        }

        void UpdateCurrentNote()
        {
            if(loadingNote || tree.SelectedNode?.Tag is not Note note) return;

            note.Title = titleBox.Text;
            if(!note.IsChapterHeading) note.Body = bodyBox.Rtf;
            note.Updated = DateTime.Now;
            updatedLabel.Text = note.IsChapterHeading ? "Chapter heading" : $"Last updated: {note.Updated:g}";
            tree.SelectedNode.Text = GetNoteNodeText(note);
            source.ResetCurrentItem();
        }

        tree.AfterSelect += (_, _) =>
        {
            if(tree.SelectedNode?.Tag is Note note) source.Position = _data.Notes.IndexOf(note);
            LoadCurrentNote();
        };

        tree.AfterLabelEdit += (_, e) =>
        {
            if(e.Label is null || e.Node?.Tag is not Note note) return;

            note.Title = e.Label;
            note.Updated = DateTime.Now;
            source.ResetCurrentItem();
            _store.Save(_data);
        };

        tree.ItemDrag += (_, e) =>
        {
            if(e.Item is TreeNode node) tree.DoDragDrop(node, DragDropEffects.Move);
        };

        tree.DragEnter += (_, e) => e.Effect = e.Data?.GetDataPresent(typeof(TreeNode)) == true ? DragDropEffects.Move : DragDropEffects.None;

        tree.DragDrop += (_, e) =>
        {
            if(e.Data?.GetData(typeof(TreeNode)) is not TreeNode draggedNode || draggedNode.Tag is not Note draggedNote) return;

            Point clientPoint = tree.PointToClient(new Point(e.X, e.Y));
            TreeNode? targetNode = tree.GetNodeAt(clientPoint);

            if(targetNode?.Tag is Note targetNote && targetNote.IsChapterHeading && !draggedNote.IsChapterHeading)
            {
                draggedNote.ParentHeadingId = targetNote.Id;
                draggedNote.SortOrder = _data.Notes.Where(note => note.ParentHeadingId == targetNote.Id).Select(note => note.SortOrder).DefaultIfEmpty().Max() + 1;
            }
            else
            {
                draggedNote.ParentHeadingId = targetNode?.Tag is Note note ? note.ParentHeadingId : null;
                MoveNoteAfter(draggedNote, targetNode?.Tag as Note);
            }

            NormalizeNoteOrder();
            source.ResetBindings(false);
            _store.Save(_data);
            RebuildNotebookTree(draggedNote);
        };
        titleBox.TextChanged += (_, _) => UpdateCurrentNote();
        bodyBox.TextChanged += (_, _) => UpdateCurrentNote();
        tree.NodeMouseDoubleClick += (_, e) =>
        {
            if(e.Node.Tag is Note { IsChapterHeading: true }) e.Node.BeginEdit();
            else bodyBox.Focus();
        };

        var addButton = new Button { Text = "Add Page", Width = 90 };
        var addChapterButton = new Button { Text = "Add Chapter", Width = 100 };
        var deleteButton = new Button { Text = "Delete", Width = 90 };
        var saveButton = new Button { Text = "Save", Width = 90 };

        addButton.Click += (_, _) =>
        {
            var selectedNote = tree.SelectedNode?.Tag as Note;
            var parentHeadingId = selectedNote switch
            {
                { IsChapterHeading: true } => selectedNote.Id,
                { ParentHeadingId: not null } => selectedNote.ParentHeadingId,
                _ => null
            };
            var note = new Note { Title = "New Page", Updated = DateTime.Now, ParentHeadingId = parentHeadingId, SortOrder = NextNoteSortOrder(parentHeadingId) };
            var list = (SortableBindingList<Note>)source.DataSource;
            list.Add(note);
            source.Position = list.Count - 1;
            _store.Save(_data);
            RebuildNotebookTree(note);
            titleBox.Focus();
            titleBox.SelectAll();
        };

        addChapterButton.Click += (_, _) =>
        {
            var heading = new Note { Title = "New Chapter", Updated = DateTime.Now, IsChapterHeading = true, SortOrder = NextNoteSortOrder(null) };
            var list = (SortableBindingList<Note>)source.DataSource;
            list.Add(heading);
            source.Position = list.Count - 1;
            _store.Save(_data);
            RebuildNotebookTree(heading);
            tree.SelectedNode?.BeginEdit();
        };

        deleteButton.Click += (_, _) =>
        {
            if(tree.SelectedNode?.Tag is not Note note) return;
            if(MessageBox.Show(this, "Delete the selected note?", "Confirm delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

            if(note.IsChapterHeading)
            {
                foreach(var child in _data.Notes.Where(child => child.ParentHeadingId == note.Id))
                {
                    child.ParentHeadingId = null;
                }
            }

            ((SortableBindingList<Note>)source.DataSource).Remove(note);
            NormalizeNoteOrder();
            _store.Save(_data);
            RebuildNotebookTree();
            LoadCurrentNote();
        };

        saveButton.Click += (_, _) =>
        {
            UpdateCurrentNote();
            _store.Save(_data);
            MessageBox.Show(this, "Saved.", "Organizer", MessageBoxButtons.OK, MessageBoxIcon.Information);
        };

        var buttonPanel = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 42, Padding = new Padding(8), FlowDirection = FlowDirection.LeftToRight };
        buttonPanel.Controls.AddRange([addButton, addChapterButton, deleteButton, saveButton]);

        var leftPanel = new Panel { Dock = DockStyle.Left, Width = 260, Padding = new Padding(8) };
        leftPanel.Controls.Add(tree);

        var editorPanel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(10) };
        editorPanel.Controls.Add(bodyBox);
        editorPanel.Controls.Add(updatedLabel);
        editorPanel.Controls.Add(titleBox);

        page.Controls.Add(editorPanel);
        page.Controls.Add(leftPanel);
        page.Controls.Add(buttonPanel);
        NormalizeNoteOrder();
        RebuildNotebookTree(_data.Notes.OrderBy(note => note.SortOrder).FirstOrDefault());
        LoadCurrentNote();

        return page;
    }

    private static TreeNode CreateNoteNode(Note note) => new(GetNoteNodeText(note)) { Tag = note };

    private static string GetNoteNodeText(Note note) => note.IsChapterHeading ? $"▸ {note.Title}" : note.Title;

    private static TreeNode? FindNoteNode(TreeNodeCollection nodes, Note note)
    {
        foreach(TreeNode node in nodes)
        {
            if(ReferenceEquals(node.Tag, note)) return node;

            var child = FindNoteNode(node.Nodes, note);
            if(child is not null) return child;
        }

        return null;
    }

    private int NextNoteSortOrder(Guid? parentHeadingId)
    {
        return _data.Notes
            .Where(note => note.ParentHeadingId == parentHeadingId)
            .Select(note => note.SortOrder)
            .DefaultIfEmpty()
            .Max() + 1;
    }

    private void MoveNoteAfter(Note draggedNote, Note? targetNote)
    {
        var siblings = _data.Notes
            .Where(note => !ReferenceEquals(note, draggedNote) && note.ParentHeadingId == draggedNote.ParentHeadingId)
            .OrderBy(note => note.SortOrder)
            .ToList();

        var insertIndex = targetNote is null ? siblings.Count : siblings.FindIndex(note => ReferenceEquals(note, targetNote)) + 1;
        if(insertIndex < 0) insertIndex = siblings.Count;

        siblings.Insert(Math.Clamp(insertIndex, 0, siblings.Count), draggedNote);

        for(var index = 0; index < siblings.Count; index++)
        {
            siblings[index].SortOrder = index;
        }
    }

    private void NormalizeNoteOrder()
    {
        foreach(var group in _data.Notes.GroupBy(note => note.ParentHeadingId))
        {
            var index = 0;
            foreach(var note in group.OrderBy(note => note.SortOrder).ThenBy(note => note.Title))
            {
                note.SortOrder = index++;
            }
        }
    }

    private static void LoadRichTextBody(RichTextBox bodyBox, string body)
    {
        if(body.TrimStart().StartsWith(@"{\rtf", StringComparison.Ordinal))
        {
            try
            {
                bodyBox.Rtf = body;
                return;
            }
            catch(ArgumentException)
            {
                // Fall back to plain text for older or invalid note content.
            }
        }

        bodyBox.Text = body;
    }

    private static void SyncList<T>(List<T> target, BindingList<T> source)
    {
        target.Clear();
        target.AddRange(source);
    }

    private static string Singular(string value) => value switch
    {
        "Calendar" => "Event",
        "Anniversary" => "Anniversary",
        "Contacts" => "Contact",
        "Tasks" or "To Do" => "Task",
        "Notes" or "Notepad" => "Note",
        _ => "Item"
    };

    private static void ApplyRecordDefaults<T>(T item)
    {
        if(item is Note note) note.Updated = DateTime.Now;
        if(item is Anniversary anniversary && string.IsNullOrWhiteSpace(anniversary.Type)) anniversary.Type = "Anniversary";

        var idProperty = typeof(T).GetProperty("Id");

        if(idProperty?.GetValue(item) is Guid id && id == Guid.Empty) idProperty.SetValue(item, Guid.NewGuid());
    }

    private void aboutToolStripMenuItem_Click(object? sender, EventArgs e)
    {
        using AboutForm about = new();

        about.ShowDialog(this);
    }
}

internal sealed class TrashDropPayload(Action delete)
{
    public void Delete() => delete();
}

internal sealed class AppointmentAlarmDialog : Form
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

    private AppointmentAlarmDialog(CalendarEvent appointment)
    {
        _appointment = appointment;
        Text = "Alarm";
        Width = 520;
        Height = 292;
        StartPosition = FormStartPosition.CenterParent;
        MinimizeBox = false;
        MaximizeBox = false;
        FormBorderStyle = FormBorderStyle.FixedDialog;

        for(int value = 0; value <= 999; value += 5)
        {
            _amount.Items.Add(value.ToString());
        }

        _unit.Items.AddRange(["Minutes", "Hours", "Days"]);
        _tune.Items.AddRange(["Default", "Chime", "Ding", "Notify"]);

        Panel? body = new()
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(12)
        };

        Button? playButton = new() { Text = "Play", Width = 82 };
        Button? browseButton = new() { Text = "Bro&wse...", Width = 82 };
        Button? runBrowseButton = new() { Text = "&Browse...", Width = 82 };

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

        Button? okButton = new() { Text = "OK", DialogResult = DialogResult.OK, Width = 82 };
        Button? cancelButton = new() { Text = "Cancel", DialogResult = DialogResult.Cancel, Width = 82 };
        Button? helpButton = new() { Text = "&Help", Width = 82 };

        okButton.Click += (_, _) => SaveValues();
        helpButton.Click += (_, _) => ShowNotImplemented("Help");

        FlowLayoutPanel? buttons = new()
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
        using AppointmentAlarmDialog? dialog = new(appointment);

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
        int index = control.Items.Cast<object>().Select(item => item.ToString() ?? string.Empty).ToList().FindIndex(item => item.Equals(value, StringComparison.OrdinalIgnoreCase));

        control.SelectedIndex = index >= 0 ? index : 0;
    }

    private static void SelectItem(DomainUpDown control, string value)
    {
        int index = control.Items.Cast<object>().Select(item => item.ToString() ?? string.Empty).ToList().FindIndex(item => item.Equals(value, StringComparison.OrdinalIgnoreCase));
        control.SelectedIndex = index >= 0 ? index : 0;
    }

    private void ShowNotImplemented(string commandText) => MessageBox.Show(this, $"{commandText} is not yet implemented.", "Not Yet Implemented", MessageBoxButtons.OK, MessageBoxIcon.Information);
}

public sealed partial class CreateAppointmentDialog : Form
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

    private CreateAppointmentDialog(CalendarEvent appointment, string title, IReadOnlyCollection<CalendarEvent>? allAppointments)
    {
        _appointment = appointment;
        _allAppointments = allAppointments ?? [];
        Text = title;
        Width = 560;
        Height = 330;
        StartPosition = FormStartPosition.CenterParent;
        MinimizeBox = false;
        MaximizeBox = false;
        FormBorderStyle = FormBorderStyle.FixedDialog;

        _categories.DropDownStyle = ComboBoxStyle.DropDown;
        _categories.Items.AddRange(["Business", "Personal", "Holiday", "Travel", "Phone Call", "Meeting"]);
        LoadDurationValues();

        var body = new TableLayoutPanel
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

        flags.Controls.AddRange([_warnOfConflicts, _pencilIn, _confidential]);
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

        var buttons = new FlowLayoutPanel
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
        buttons.Controls.AddRange([inviteButton, findTimeButton, alarmButton, repeatButton, costButton, helpButton]);

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

        for(var minutes = 5; minutes <= 24 * 60; minutes += 5)
        {
            _duration.Items.Add(FormatDuration(minutes));
        }
    }

    private static void AddRow(TableLayoutPanel layout, int row, string labelText, Control control)
    {
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.Controls.Add(new Label { Text = labelText, AutoSize = true, Anchor = AnchorStyles.Left, Padding = new Padding(0, 5, 0, 0) }, 0, row);
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
        var start = _date.Value.Date.Add(_time.Value.TimeOfDay);
        var durationMinutes = ParseDurationMinutes(_duration.Text);

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
        DateTime start = _date.Value.Date.Add(_time.Value.TimeOfDay);
        DateTime end = start.AddMinutes(ParseDurationMinutes(_duration.Text));

        if(_warnOfConflicts.Checked && HasTimeConflict(start, end, out var message))
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
            .Where(other => !ReferenceEquals(other, _appointment) && other.Id != _appointment.Id&&start < other.End && end > other.Start)
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

        foreach(var conflict in conflicts)
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
        int hours = parts.Length > 0 && int.TryParse(parts[0], out var parsedHours) ? Math.Clamp(parsedHours, 0, 24) : 0;
        int minutes = parts.Length > 1 && int.TryParse(parts[1], out var parsedMinutes) ? Math.Clamp(parsedMinutes, 0, 59) : 0;

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
        var cleanText = commandText.Replace("&", string.Empty, StringComparison.Ordinal).Replace("...", string.Empty, StringComparison.Ordinal);

        MessageBox.Show(this, $"{cleanText} is not yet implemented.", "Not Yet Implemented", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }
}

internal enum CalendarViewMode
{
    Day,
    Week,
    Month
}

internal sealed partial class RecordEditorDialog : Form
{
    private readonly object _record;
    private readonly Dictionary<PropertyInfo, Control> _controls = [];

    public RecordEditorDialog()
        : this(new Note(), "Record Editor")
    {
    }

    private RecordEditorDialog(object record, string title)
    {
        InitializeComponent();
        _record = record;
        Text = title;
        Width = 620;
        Height = 560;
        StartPosition = FormStartPosition.CenterParent;
        MinimizeBox = false;
        MaximizeBox = false;
        FormBorderStyle = FormBorderStyle.FixedDialog;

        TableLayoutPanel? layout = new()
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            Padding = new Padding(12),
            AutoScroll = true
        };

        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

        foreach(PropertyInfo property in record.GetType().GetProperties().Where(p => p.CanRead && p.CanWrite && p.Name != "Id"))
        {
            int labelRow = layout.RowCount++;
            int controlRow = layout.RowCount++;
            Label? label = new()
            {
                Text = SplitName(property.Name),
                AutoSize = true,
                Dock = DockStyle.Top,
                TextAlign = ContentAlignment.BottomLeft,
                Padding = new Padding(0, 8, 0, 2)
            };

            Control? control = CreateControl(property, property.GetValue(record));

            _controls[property] = control;
            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            layout.Controls.Add(label, 0, labelRow);
            layout.Controls.Add(control, 0, controlRow);
        }

        Button? okButton = new() { Text = "OK", DialogResult = DialogResult.OK, Width = 90 };
        Button? cancelButton = new() { Text = "Cancel", DialogResult = DialogResult.Cancel, Width = 90 };

        okButton.Click += (_, _) => SaveValues();

        FlowLayoutPanel? buttons = new() { Dock = DockStyle.Bottom, Height = 48, Padding = new Padding(8), FlowDirection = FlowDirection.RightToLeft };

        buttons.Controls.Add(cancelButton);
        buttons.Controls.Add(okButton);

        AcceptButton = okButton;
        CancelButton = cancelButton;
        Controls.Add(layout);
        Controls.Add(buttons);
    }

    public static bool Edit(IWin32Window owner, object record, string title)
    {
        using RecordEditorDialog? dialog = new(record, title);

        return dialog.ShowDialog(owner) == DialogResult.OK;
    }

    private static Control CreateControl(PropertyInfo property, object? value)
    {
        if(property.PropertyType == typeof(DateTime))
        {
            DateTimePicker? datePicker = new()
            {
                Value = value is DateTime date ? date : DateTime.Now,
                Format = DateTimePickerFormat.Custom,
                CustomFormat = property.DeclaringType == typeof(Anniversary) ? "MMMM d, yyyy" : "yyyy-MM-dd HH:mm",
                Dock = DockStyle.Top
            };

            return datePicker;
        }

        if(property.PropertyType == typeof(bool)) return new CheckBox { Checked = value is true, Dock = DockStyle.Top };

        if(property.PropertyType == typeof(int))
        {
            return new NumericUpDown
            {
                Value = value is int number ? Math.Clamp(number, 0, 999) : 0,
                Minimum = 0,
                Maximum = 999,
                Dock = DockStyle.Top
            };
        }

        if(property.Name == nameof(OrganizerTask.Priority))
        {
            FlowLayoutPanel? priorityPanel = new()
            {
                Dock = DockStyle.Top,
                Height = 28,
                FlowDirection = FlowDirection.LeftToRight,
                Tag = nameof(OrganizerTask.Priority)
            };

            string? selectedPriority = NormalizePriority(value?.ToString());

            foreach(string priority in new[] { "Low", "Medium", "High" })
            {
                priorityPanel.Controls.Add(new RadioButton
                {
                    Text = priority,
                    Tag = priority,
                    AutoSize = true,
                    Checked = priority == selectedPriority
                });
            }

            return priorityPanel;
        }

        if(property.Name == nameof(OrganizerTask.RepeatUnit))
        {
            ComboBox? repeatUnit = new()
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Dock = DockStyle.Top
            };

            repeatUnit.Items.AddRange(["None", "Hours", "Days", "Weeks", "Months", "Years"]);
            repeatUnit.SelectedItem = NormalizeRepeatUnit(value?.ToString());

            return repeatUnit;
        }

        if(property.Name == nameof(Anniversary.Type))
        {
            ComboBox? type = new()
            {
                DropDownStyle = ComboBoxStyle.DropDown,
                Dock = DockStyle.Top,
                Text = string.IsNullOrWhiteSpace(value?.ToString()) ? "Anniversary" : value.ToString()
            };

            type.Items.AddRange(["Anniversary", "Birthday", "Holiday", "Special Occasion"]);

            return type;
        }

        bool multiline = property.Name.Contains("Notes", StringComparison.OrdinalIgnoreCase)
            || property.Name.Contains("Body", StringComparison.OrdinalIgnoreCase)
            || property.Name.Contains("Address", StringComparison.OrdinalIgnoreCase);

        return new TextBox
        {
            Text = value?.ToString() ?? string.Empty,
            Multiline = multiline,
            Height = multiline ? 90 : 24,
            Dock = DockStyle.Top,
            ScrollBars = multiline ? ScrollBars.Vertical : ScrollBars.None
        };
    }

    private void SaveValues()
    {
        foreach(KeyValuePair<PropertyInfo, Control> pair in _controls)
        {
            object? value = pair.Value switch
            {
                TextBox textBox => textBox.Text,
                FlowLayoutPanel { Tag: nameof(OrganizerTask.Priority) } priorityPanel => SelectedPriority(priorityPanel),
                ComboBox comboBox => comboBox.SelectedItem?.ToString() ?? "None",
                NumericUpDown numericUpDown => (int)numericUpDown.Value,
                CheckBox checkBox => checkBox.Checked,
                DateTimePicker datePicker => datePicker.Value,
                _ => null
            };

            pair.Key.SetValue(_record, value);
        }
    }

    private static string SplitName(string value) => string.Concat(value.Select((ch, index) => index > 0 && char.IsUpper(ch) ? " " + ch : ch.ToString()));

    private static string NormalizePriority(string? value)
    {
        return value switch
        {
            "Medium" or "2" => "Medium",
            "High" or "3" => "High",
            _ => "Low"
        };
    }

    private static string SelectedPriority(FlowLayoutPanel priorityPanel) => priorityPanel.Controls.OfType<RadioButton>().FirstOrDefault(radioButton => radioButton.Checked)?.Tag?.ToString() ?? "Low";

    private static string NormalizeRepeatUnit(string? value)
    {
        return value switch
        {
            "Hours" or "Days" or "Weeks" or "Months" or "Years" => value,
            _ => "None"
        };
    }
}

// Insertion point before OrganizerPreferencesDialog.
internal sealed partial class PrinterSetupDialog : Form
{
    private readonly OrganizerPreferences _preferences;
    private readonly ComboBox _printerName = new() { DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly ComboBox _paperSize = new() { DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly ComboBox _paperSource = new() { DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly RadioButton _portrait = new() { Text = "&Portrait", AutoSize = true };
    private readonly RadioButton _landscape = new() { Text = "&Landscape", AutoSize = true };
    private readonly NumericUpDown _leftMargin = MarginBox();
    private readonly NumericUpDown _rightMargin = MarginBox();
    private readonly NumericUpDown _topMargin = MarginBox();
    private readonly NumericUpDown _bottomMargin = MarginBox();
    private readonly Label _status = new() { AutoSize = true };
    private readonly Label _type = new() { AutoSize = true };
    private readonly Label _where = new() { AutoSize = true };
    private readonly Label _comment = new() { AutoSize = true };

    public PrinterSetupDialog()
        : this(new OrganizerPreferences())
    {
    }

    private PrinterSetupDialog(OrganizerPreferences preferences)
    {
        InitializeComponent();
        _preferences = preferences;
        Text = "Printer Setup";
        Width = 520;
        Height = 470;
        StartPosition = FormStartPosition.CenterParent;
        MinimizeBox = false;
        MaximizeBox = false;
        FormBorderStyle = FormBorderStyle.FixedDialog;

        TableLayoutPanel? layout = new()
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            Padding = new Padding(12),
            AutoScroll = true
        };

        layout.Controls.Add(BuildPrinterGroup());
        layout.Controls.Add(BuildPaperGroup());
        layout.Controls.Add(BuildOrientationGroup());
        layout.Controls.Add(BuildMarginsGroup());

        Button? okButton = new() { Text = "OK", DialogResult = DialogResult.OK, Width = 90 };
        Button? cancelButton = new() { Text = "Cancel", DialogResult = DialogResult.Cancel, Width = 90 };
        Button? helpButton = new() { Text = "&Help", Width = 90 };

        okButton.Click += (_, _) => SaveValues();
        helpButton.Click += (_, _) => ShowNotImplemented("Help Topics");

        FlowLayoutPanel? buttons = new() { Dock = DockStyle.Bottom, Height = 48, Padding = new Padding(8), FlowDirection = FlowDirection.RightToLeft };
        buttons.Controls.Add(helpButton);
        buttons.Controls.Add(cancelButton);
        buttons.Controls.Add(okButton);

        AcceptButton = okButton;
        CancelButton = cancelButton;
        Controls.Add(layout);
        Controls.Add(buttons);
        LoadPrinters();
        LoadValues();
        UpdatePrinterDetails();
    }

    public static bool Edit(IWin32Window owner, OrganizerPreferences preferences)
    {
        using var dialog = new PrinterSetupDialog(preferences);
        return dialog.ShowDialog(owner) == DialogResult.OK;
    }

    private GroupBox BuildPrinterGroup()
    {
        GroupBox? group = Group("Printer", 150);
        TableLayoutPanel? layout = Grid(3);

        layout.Controls.Add(new Label { Text = "&Name:", AutoSize = true }, 0, 0);
        layout.Controls.Add(_printerName, 1, 0);
        layout.Controls.Add(Button("Properties...", () => ShowNotImplemented("Printer Properties")), 2, 0);
        layout.Controls.Add(new Label { Text = "Status:", AutoSize = true }, 0, 1);
        layout.Controls.Add(_status, 1, 1);
        layout.Controls.Add(new Label { Text = "Type:", AutoSize = true }, 0, 2);
        layout.Controls.Add(_type, 1, 2);
        layout.Controls.Add(new Label { Text = "Where:", AutoSize = true }, 0, 3);
        layout.Controls.Add(_where, 1, 3);
        layout.Controls.Add(new Label { Text = "Comment:", AutoSize = true }, 0, 4);
        layout.Controls.Add(_comment, 1, 4);
        _printerName.SelectedIndexChanged += (_, _) => UpdatePrinterDetails();
        group.Controls.Add(layout);

        return group;
    }

    private GroupBox BuildPaperGroup()
    {
        GroupBox? group = Group("Paper", 92);
        TableLayoutPanel? layout = Grid(2);

        layout.Controls.Add(new Label { Text = "Si&ze:", AutoSize = true }, 0, 0);
        layout.Controls.Add(_paperSize, 1, 0);
        layout.Controls.Add(new Label { Text = "S&ource:", AutoSize = true }, 0, 1);
        layout.Controls.Add(_paperSource, 1, 1);
        group.Controls.Add(layout);

        return group;
    }

    private GroupBox BuildOrientationGroup()
    {
        var group = Group("Orientation", 58);
        var panel = new FlowLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(8), FlowDirection = FlowDirection.LeftToRight };
        panel.Controls.Add(_portrait);
        panel.Controls.Add(_landscape);
        group.Controls.Add(panel);

        return group;
    }

    private GroupBox BuildMarginsGroup()
    {
        GroupBox? group = Group("Margins (hundredths of an inch)", 98);
        TableLayoutPanel? layout = Grid(4);

        layout.Controls.Add(new Label { Text = "&Left:", AutoSize = true }, 0, 0);
        layout.Controls.Add(_leftMargin, 1, 0);
        layout.Controls.Add(new Label { Text = "&Right:", AutoSize = true }, 2, 0);
        layout.Controls.Add(_rightMargin, 3, 0);
        layout.Controls.Add(new Label { Text = "&Top:", AutoSize = true }, 0, 1);
        layout.Controls.Add(_topMargin, 1, 1);
        layout.Controls.Add(new Label { Text = "&Bottom:", AutoSize = true }, 2, 1);
        layout.Controls.Add(_bottomMargin, 3, 1);
        group.Controls.Add(layout);

        return group;
    }

    private void LoadPrinters()
    {
        foreach(string printer in PrinterSettings.InstalledPrinters)
        {
            _printerName.Items.Add(printer);
        }

        if(_printerName.Items.Count == 0) _printerName.Items.Add("No printers installed");
    }

    private void LoadValues()
    {
        SelectPrinter(_preferences.PrinterName);
        SelectItem(_paperSize, _preferences.PaperSize, "Letter");
        SelectItem(_paperSource, _preferences.PaperSource, "Automatically Select");
        _portrait.Checked = !_preferences.PrinterLandscape;
        _landscape.Checked = _preferences.PrinterLandscape;
        _leftMargin.Value = Math.Clamp(_preferences.MarginLeft, (int)_leftMargin.Minimum, (int)_leftMargin.Maximum);
        _rightMargin.Value = Math.Clamp(_preferences.MarginRight, (int)_rightMargin.Minimum, (int)_rightMargin.Maximum);
        _topMargin.Value = Math.Clamp(_preferences.MarginTop, (int)_topMargin.Minimum, (int)_topMargin.Maximum);
        _bottomMargin.Value = Math.Clamp(_preferences.MarginBottom, (int)_bottomMargin.Minimum, (int)_bottomMargin.Maximum);
    }

    private void SaveValues()
    {
        _preferences.PrinterName = _printerName.SelectedItem?.ToString() is "No printers installed" ? string.Empty : _printerName.SelectedItem?.ToString() ?? string.Empty;
        _preferences.PaperSize = _paperSize.SelectedItem?.ToString() ?? "Letter";
        _preferences.PaperSource = _paperSource.SelectedItem?.ToString() ?? "Automatically Select";
        _preferences.PrinterLandscape = _landscape.Checked;
        _preferences.MarginLeft = (int)_leftMargin.Value;
        _preferences.MarginRight = (int)_rightMargin.Value;
        _preferences.MarginTop = (int)_topMargin.Value;
        _preferences.MarginBottom = (int)_bottomMargin.Value;
    }

    private void SelectPrinter(string printerName)
    {
        if(!string.IsNullOrWhiteSpace(printerName) && _printerName.Items.Contains(printerName))
        {
            _printerName.SelectedItem = printerName;

            return;
        }

        PrinterSettings? printerSettings = new();

        _printerName.SelectedItem = _printerName.Items.Contains(printerSettings.PrinterName) ? printerSettings.PrinterName : _printerName.Items[0];
    }

    private void UpdatePrinterDetails()
    {
        _paperSize.Items.Clear();
        _paperSource.Items.Clear();

        string? printerName = _printerName.SelectedItem?.ToString() ?? string.Empty;

        if(string.IsNullOrWhiteSpace(printerName) || printerName == "No printers installed")
        {
            _status.Text = "Unavailable";
            _type.Text = string.Empty;
            _where.Text = string.Empty;
            _comment.Text = string.Empty;
            _paperSize.Items.Add("Letter");
            _paperSource.Items.Add("Automatically Select");
            _paperSize.SelectedIndex = 0;
            _paperSource.SelectedIndex = 0;

            return;
        }

        PrinterSettings? settings = new() { PrinterName = printerName };

        _status.Text = settings.IsValid ? "Ready" : "Unavailable";
        _type.Text = settings.IsPlotter ? "Plotter" : "Printer";
        _where.Text = settings.PrinterName;
        _comment.Text = settings.IsDefaultPrinter ? "Default printer" : string.Empty;

        foreach(PaperSize paperSize in settings.PaperSizes)
        {
            _paperSize.Items.Add(paperSize.PaperName);
        }

        foreach(PaperSource paperSource in settings.PaperSources)
        {
            _paperSource.Items.Add(paperSource.SourceName);
        }

        if(_paperSize.Items.Count == 0) _paperSize.Items.Add("Letter");
        if(_paperSource.Items.Count == 0) _paperSource.Items.Add("Automatically Select");

        SelectItem(_paperSize, _preferences.PaperSize, _paperSize.Items[0].ToString() ?? "Letter");
        SelectItem(_paperSource, _preferences.PaperSource, _paperSource.Items[0].ToString() ?? "Automatically Select");
    }

    private static GroupBox Group(string text, int height) => new() { Text = text, Dock = DockStyle.Top, Height = height, Padding = new Padding(8) };

    private static TableLayoutPanel Grid(int columns)
    {
        TableLayoutPanel? grid = new() { Dock = DockStyle.Fill, ColumnCount = columns, AutoSize = true };

        for(int index = 0; index < columns; index++)
        {
            grid.ColumnStyles.Add(index % 2 == 0 ? new ColumnStyle(SizeType.AutoSize) : new ColumnStyle(SizeType.Percent, 100));
        }

        return grid;
    }

    private Button Button(string text, Action action)
    {
        Button? button = new() { Text = text, Width = 90 };

        button.Click += (_, _) => action();

        return button;
    }

    private static NumericUpDown MarginBox() => new() { Minimum = 0, Maximum = 999, Value = 100, Width = 70 };

    private static void SelectItem(ComboBox comboBox, string value, string fallback)
    {
        comboBox.SelectedItem = comboBox.Items.Cast<object>().FirstOrDefault(item => string.Equals(item.ToString(), value, StringComparison.OrdinalIgnoreCase))
            ?? comboBox.Items.Cast<object>().FirstOrDefault(item => string.Equals(item.ToString(), fallback, StringComparison.OrdinalIgnoreCase))
            ?? comboBox.Items[0];
    }

    private void ShowNotImplemented(string commandText) => MessageBox.Show(this, $"{commandText.Replace("&", string.Empty, StringComparison.Ordinal).Replace("...", string.Empty, StringComparison.Ordinal)} is not yet implemented.", "Not Yet Implemented", MessageBoxButtons.OK, MessageBoxIcon.Information);
}

internal sealed class OrganizerPreferencesDialog : Form
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

    private OrganizerPreferencesDialog(OrganizerPreferences preferences)
    {
        _preferences = preferences;
        Text = "Organizer Preferences";
        Width = 680;
        Height = 560;
        StartPosition = FormStartPosition.CenterParent;
        MinimizeBox = false;
        MaximizeBox = false;
        FormBorderStyle = FormBorderStyle.FixedDialog;

        TabControl? tabs = new() { Dock = DockStyle.Fill, Padding = new Point(12, 4) };
        tabs.TabPages.Add(BuildDefaultFilePage());
        tabs.TabPages.Add(BuildEnvironmentPage());
        tabs.TabPages.Add(BuildFoldersPage());
        tabs.TabPages.Add(BuildAlarmPage());
        tabs.TabPages.Add(BuildWebBrowsingPage());

        Button? okButton = new() { Text = "OK", DialogResult = DialogResult.OK, Width = 90 };
        Button? cancelButton = new() { Text = "Cancel", DialogResult = DialogResult.Cancel, Width = 90 };
        Button? helpButton = new() { Text = "&Help", Width = 90 };

        okButton.Click += (_, _) => SaveValues();
        helpButton.Click += (_, _) => ShowDialogStub("Help Topics");

        FlowLayoutPanel? buttons = new() { Dock = DockStyle.Bottom, Height = 48, Padding = new Padding(8), FlowDirection = FlowDirection.RightToLeft };
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
        using OrganizerPreferencesDialog? dialog = new (preferences);

        return dialog.ShowDialog(owner) == DialogResult.OK;
    }

    private TabPage BuildWebBrowsingPage()
    {
        TabPage? page = Page("Web Browsing");
        FlowLayoutPanel? body = Body(page);
        ListBox? entries = new() { Height = 92 };

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
        TabPage? page = Page("Folders");
        FlowLayoutPanel? body = Body(page);

        AddPathRow(body, "&Organizer files", _organizerFilesPath, "B&rowse...");
        AddPathRow(body, "&Paper layouts", _paperLayoutsPath, "Bro&wse...");
        AddPathRow(body, "Custom Smart&Icons", _customSmartIconsPath, "Brow&se...");
        AddPathRow(body, "&Backups", _backupsPath, "Brows&e...");

        return page;
    }

    private TabPage BuildEnvironmentPage()
    {
        TabPage? page = Page("Environment");
        FlowLayoutPanel? body = Body(page);

        body.Controls.Add(_animatedPageTurn);
        body.Controls.Add(Label("Mouse pointer"));
        body.Controls.Add(ButtonRow(_plainPointer, _colorPointer, _animatedPointer));
        AddLabeled(body, "Wee&k starts on", _weekStartsOn);
        body.Controls.Add(Label("&Sounds"));

        ListBox? sounds = new() { Height = 70 };

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
        TabPage? page = new(text)
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
        TableLayoutPanel? grid = new() { ColumnCount = 5, RowCount = 6, Dock = DockStyle.Top, AutoSize = true };
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

        int row = 1;
        foreach(string section in new[] { "A&nniversary", "&Appointment", "Ca&ll", "&Event", "&Task" })
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

        if(!string.IsNullOrWhiteSpace(label)) parent.Controls.Add(Label(label));

        panel.Controls.Add(BrowseButton(buttonText, textBox), 1, 0);
        parent.Controls.Add(panel);
    }

    private static FlowLayoutPanel ButtonRow(params Control[] controls)
    {
        FlowLayoutPanel? panel = new() { Width = 590, Height = 34, FlowDirection = FlowDirection.LeftToRight };

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
        Button? button = Button(text, () =>
        {
            using var dialog = new FolderBrowserDialog { SelectedPath = Directory.Exists(target.Text) ? target.Text : Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) };

            if(dialog.ShowDialog(this) == DialogResult.OK) target.Text = dialog.SelectedPath;
        });

        button.Width = 88;

        return button;
    }

    private static ComboBox DropDown(string[] values)
    {
        var comboBox = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList };
        comboBox.Items.AddRange(values);

        if(comboBox.Items.Count > 0) comboBox.SelectedIndex = 0;

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

    private static void SelectItem(ComboBox comboBox, string value) => comboBox.SelectedItem = comboBox.Items.Cast<object>().FirstOrDefault(item => string.Equals(item.ToString(), value, StringComparison.OrdinalIgnoreCase)) ?? comboBox.Items[0];

    private void ShowDialogStub(string commandText)
    {
        string? cleanText = commandText.Split('\t')[0].Replace("&", string.Empty, StringComparison.Ordinal).Replace("...", string.Empty, StringComparison.Ordinal);

        MessageBox.Show(this, $"{cleanText} is not yet implemented.", "Not Yet Implemented", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }
}