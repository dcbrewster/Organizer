using System.ComponentModel;
using System.Drawing.Printing;
using System.Linq;
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
    private CalendarPlannerView? _planner;

    // Binder panel collapse state
    private int _binderPanelLastWidth = 260;

    private bool _binderPanelCollapsed = false;

    // Reference to the View menu item so we can update its text when toggling
    private ToolStripMenuItem? _collapseBinderPanelMenuItem;

    // Appointment menu items that reflect the currently-focused appointment state
    private ToolStripMenuItem? _warnOfConflictsMenuItem;

    private ToolStripMenuItem? _pencilInMenuItem;
    private ToolStripMenuItem? _confidentialMenuItem;

    // Current focused appointment (kept at instance scope so menu updates can be driven from anywhere)
    private CalendarEvent? _selectedCalendarEvent;

    // Update the appointment-related menu items to reflect the current focused appointment
    private void UpdateAppointmentMenuItems()
    {
        bool hasEvent = _selectedCalendarEvent is not null;

        if(_warnOfConflictsMenuItem is not null)
        {
            _warnOfConflictsMenuItem.Enabled = hasEvent;
            _warnOfConflictsMenuItem.Checked = _selectedCalendarEvent?.WarnOfConflicts ?? false;
        }

        if(_pencilInMenuItem is not null)
        {
            _pencilInMenuItem.Enabled = hasEvent;
            _pencilInMenuItem.Checked = _selectedCalendarEvent?.PencilIn ?? false;
        }

        if(_confidentialMenuItem is not null)
        {
            _confidentialMenuItem.Enabled = hasEvent;
            _confidentialMenuItem.Checked = _selectedCalendarEvent?.Confidential ?? false;
        }
    }

    // Small clickable label placed above the calendar to toggle the binder panel
    private Label? _binderToggleLabel;

    // Stores original visibility of left-panel child controls when collapsed so we can restore on expand
    private Dictionary<Control, bool>? _leftPanelChildVisibility;

    // Animation for collapsing/expanding the left binder panel
    private System.Windows.Forms.Timer? _binderAnimationTimer;

    private int _binderAnimationTargetWidth = 0;
    private bool _binderAnimationExpanding = false;

    // Shared tooltip for small UI elements
    private ToolTip? _toolTip;

    // Drag/drop support for tab reordering
    private int _dragTabIndex = -1;

    private Point _dragStartPoint;
    private Action<CalendarViewMode> _setCalendarView = _ => { };
    private Action<int> _moveCalendar = _ => { };
    private Action _goToday = () => { };
    private string _currentFilePath;

    // Trash animation
    private System.Windows.Forms.Timer _trashAnimationTimer = null!;

    private int _trashAnimationTick = 0;
    private bool _trashAnimating = false;
    private TrashDropPayload? _clipboardPayload = null;
    private Control? _trashDropControl = null;

    private static DayOfWeek ParseWeekStarts(OrganizerData? data)
    {
        if(data?.Preferences is not null && !string.IsNullOrWhiteSpace(data.Preferences.WeekStartsOn))
        {
            if(Enum.TryParse<DayOfWeek>(data.Preferences.WeekStartsOn, true, out var day))
            {
                return day;
            }
        }

        return DayOfWeek.Sunday;
    }

    private void StartBinderAnimation(int targetWidth, bool expanding)
    {
        try
        {
            // Initialize timer if needed
            if(_binderAnimationTimer is null)
            {
                _binderAnimationTimer = new System.Windows.Forms.Timer { Interval = 15 };
                _binderAnimationTimer.Tick += (_, _) =>
                {
                    try
                    {
                        if(designerCalendarLeftPanel is null) return;

                        int current = designerCalendarLeftPanel.Width;
                        int target = _binderAnimationTargetWidth;

                        if(current == target)
                        {
                            _binderAnimationTimer?.Stop();
                            _binderAnimationAnimating = false;

                            // Finalize state
                            if(_binderAnimationExpanding)
                            {
                                // Restore children visibility
                                try
                                {
                                    if(_leftPanelChildVisibility is not null)
                                    {
                                        foreach(var kvp in _leftPanelChildVisibility)
                                        {
                                            try { if(kvp.Key is not null) kvp.Key.Visible = kvp.Value; } catch { }
                                        }

                                        _leftPanelChildVisibility = null;
                                    }
                                    else
                                    {
                                        foreach(Control c in designerCalendarLeftPanel.Controls)
                                        {
                                            if(object.ReferenceEquals(c, _binderToggleLabel)) continue;
                                            c.Visible = true;
                                        }
                                    }
                                }
                                catch { }

                                // Restore min size
                                try { designerCalendarLeftPanel.MinimumSize = new Size(_binderPanelLastWidth > 0 ? _binderPanelLastWidth : 260, 0); } catch { }
                                _binderPanelCollapsed = false;
                            }
                            else
                            {
                                // Collapsed: ensure only glyph remains visible
                                try
                                {
                                    foreach(Control c in designerCalendarLeftPanel.Controls)
                                    {
                                        if(object.ReferenceEquals(c, _binderToggleLabel)) { c.Visible = true; continue; }
                                        c.Visible = false;
                                    }
                                }
                                catch { }

                                // Tighten minimum size to the glyph width
                                try { if(_binderToggleLabel is not null) designerCalendarLeftPanel.MinimumSize = new Size(Math.Max(24, _binderToggleLabel.Width + 8), 0); } catch { }
                                _binderPanelCollapsed = true;
                            }

                            // Update view/menu/glyph after animation finishes
                            try
                            {
                                if(_collapseBinderPanelMenuItem is not null) _collapseBinderPanelMenuItem.Text = _binderPanelCollapsed ? "Expand Binder Panel\tF12" : "Collapse Binder Panel\tF12";
                                if(_binderToggleLabel is not null) _binderToggleLabel.Text = _binderPanelCollapsed ? "\u25B6" : "\u25BC";
                            }
                            catch { }

                            designerCalendarLeftPanel.Parent?.PerformLayout();
                            _tabs.Parent?.PerformLayout();
                            _tabs.Invalidate();

                            return;
                        }

                        // Move towards target using an easing step
                        int diff = target - current;
                        int step = Math.Max(1, Math.Abs(diff) / 6);
                        int next = current + Math.Sign(diff) * step;
                        // Clamp to target
                        if((diff > 0 && next > target) || (diff < 0 && next < target)) next = target;

                        try { designerCalendarLeftPanel.Width = next; } catch { }
                    }
                    catch { }
                };
            }

            _binderAnimationTargetWidth = targetWidth;
            _binderAnimationExpanding = expanding;
            _binderAnimationAnimating = true;

            // Ensure layout does not prevent the animation: relax minimum size before animating
            try { designerCalendarLeftPanel.MinimumSize = new Size(0, 0); } catch { }

            _binderAnimationTimer.Start();
        }
        catch { }
    }

    // Tracks active animation
    private bool _binderAnimationAnimating = false;

    private static System.Windows.Forms.Day ConvertToWinFormsDay(DayOfWeek dow)
    {
        return dow switch
        {
            DayOfWeek.Sunday => System.Windows.Forms.Day.Sunday,
            DayOfWeek.Monday => System.Windows.Forms.Day.Monday,
            DayOfWeek.Tuesday => System.Windows.Forms.Day.Tuesday,
            DayOfWeek.Wednesday => System.Windows.Forms.Day.Wednesday,
            DayOfWeek.Thursday => System.Windows.Forms.Day.Thursday,
            DayOfWeek.Friday => System.Windows.Forms.Day.Friday,
            DayOfWeek.Saturday => System.Windows.Forms.Day.Saturday,
            _ => System.Windows.Forms.Day.Default,
        };
    }

    public MainForm()
    {
        InitializeComponent();

        // Remove designer-introduced padding/margins so the left binder content uses the full panel width
        try
        {
            designerCalendarLeftPanel.Padding = Padding.Empty;
            designerNavigationPanel.Padding = Padding.Empty;
            designerNavigationPanel.Margin = Padding.Empty;
            designerMonthCalendar.Margin = Padding.Empty;
            designerPreviousButton.Margin = Padding.Empty;
            designerNextButton.Margin = Padding.Empty;
        }
        catch { }

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

        MenuStrip? menuStrip = null;
        ToolStrip? iconLine = BuildIconLine();

        _tabs = new TabControl { Dock = DockStyle.Fill, Alignment = TabAlignment.Right, Multiline = true };
        _tabs.TabPages.Add(BuildCalendarTab());
        _tabs.TabPages.Add(BuildTab("Anniversary", _data.Anniversaries));
        _tabs.TabPages.Add(BuildTab("Contacts", _data.Contacts));
        _tabs.TabPages.Add(BuildNotepadTab());

        // Restore last selected section if present
        try
        {
            var last = _data.Preferences?.LastSection;
            if(!string.IsNullOrWhiteSpace(last))
            {
                int idx = _tabs.TabPages.Cast<TabPage>().ToList().FindIndex(tp => string.Equals(tp.Text, last, StringComparison.OrdinalIgnoreCase));
                if(idx >= 0) _tabs.SelectedIndex = idx;
            }
        }
        catch { }

        // Persist section selection when user changes tabs
        try
        {
            _tabs.SelectedIndexChanged += (_, _) =>
            {
                try
                {
                    var txt = _tabs.SelectedTab?.Text;
                    if(!string.IsNullOrWhiteSpace(txt)) { _data.Preferences.LastSection = txt; _store.Save(_data); }
                }
                catch { }
            };
        }
        catch { }

        // Disable drag & drop of tabs — item-level drag/drop is supported instead
        _tabs.AllowDrop = false;

        // Create a content container so the left panel and tabs dock correctly
        Panel content = new() { Dock = DockStyle.Fill };
        // Add the tab control first, then the left panel so docking/z-order doesn't allow the left panel to overlap the Fill area
        content.Controls.Add(_tabs);
        content.Controls.Add(designerCalendarLeftPanel);

        // Ensure the left binder panel keeps the expected designer width so the MonthCalendar isn't clipped
        try
        {
            // Narrow the left binder panel to 220px as requested
            const int leftPanelWidth = 220;
            designerCalendarLeftPanel.MinimumSize = new Size(leftPanelWidth, 0);
            designerCalendarLeftPanel.Width = leftPanelWidth;
            _binderPanelLastWidth = leftPanelWidth;

            // Ensure navigation and month calendar adjust to the panel: rely on docking/anchoring instead of forcing widths
            try
            {
                designerNavigationPanel.AutoSize = true;
                designerNavigationPanel.AutoSizeMode = AutoSizeMode.GrowOnly;
                designerNavigationPanel.Padding = Padding.Empty;
                designerMonthCalendar.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            }
            catch { }

            // Force layout so the tab control correctly fills remaining space and does not get overlapped
            try
            {
                content.PerformLayout();
                _tabs.BringToFront();
                _tabs.Invalidate();
            }
            catch { }
            // Note: glyph and runtime tooltips are created in BuildCalendarTab after the left panel is populated
        }
        catch { }

        // Now that tabs have been created, build the main menu so the "Turn To" submenu
        // can be populated from the actual tab pages.
        try { menuStrip = BuildMainMenu(); } catch { menuStrip = null; }

        Controls.Add(content);
        Controls.Add(iconLine);
        Controls.Add(menuStrip);
        // Hide designer-created trash label (we use a runtime trash target in the left panel)
        designerTrashLabel.Visible = false;

        // Setup trash hover animation (pulsing icon)
        _trashAnimationTimer = new System.Windows.Forms.Timer { Interval = 80 };
        _trashAnimationTimer.Tick += (_, _) =>
        {
            _trashAnimationTick++;
            _trashDropControl?.Invalidate();
        };

        MainMenuStrip = menuStrip;

        // Log initial left panel / month calendar layout for debugging missing Saturday column
        LogLeftPanelLayout("constructor");

        // Watch for layout/size changes and log so we can see when the designer MonthCalendar is clipped
        try
        {
            designerMonthCalendar.SizeChanged += (_, _) => LogLeftPanelLayout("designerMonthCalendar.SizeChanged");
            designerCalendarLeftPanel.SizeChanged += (_, _) => LogLeftPanelLayout("designerCalendarLeftPanel.SizeChanged");
            if(designerCalendarLeftPanel.Parent is not null) designerCalendarLeftPanel.Parent.Layout += (_, _) => LogLeftPanelLayout("leftParent.Layout");
        }
        catch { }
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

        toolStrip.Items.AddRange(
        [
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

    private void ToggleBinderPanel()
    {
        if(designerCalendarLeftPanel is null) return;

        if(!_binderPanelCollapsed)
        {
            // Start collapse: save width, hide children (except glyph), and animate to glyph-only width
            _binderPanelLastWidth = designerCalendarLeftPanel.Width > 0 ? designerCalendarLeftPanel.Width : _binderPanelLastWidth;

            try
            {
                // Record current visibility for later restore but don't hide them immediately; hiding will occur when animation completes to avoid layout side-effects.
                _leftPanelChildVisibility = new Dictionary<Control, bool>();
                foreach(Control c in designerCalendarLeftPanel.Controls)
                {
                    if(object.ReferenceEquals(c, _binderToggleLabel))
                    {
                        // ensure glyph remains visible
                        _leftPanelChildVisibility[c] = true;
                        continue;
                    }

                    _leftPanelChildVisibility[c] = c.Visible;
                }

                int glyphWidth = 24;
                try { if(_binderToggleLabel is not null && _binderToggleLabel.PreferredSize.Width > 0) glyphWidth = Math.Max(24, _binderToggleLabel.PreferredSize.Width + 8); } catch { }

                StartBinderAnimation(glyphWidth, expanding: false);
            }
            catch { }
        }
        else
        {
            // Start expand: animate back to saved width, will restore children at end
            int restoreWidth = _binderPanelLastWidth > 0 ? _binderPanelLastWidth : 260;
            StartBinderAnimation(restoreWidth, expanding: true);
        }

        // Force layout update so the tab control fills the available space
        designerCalendarLeftPanel.Parent?.PerformLayout();
        _tabs.Parent?.PerformLayout();
        _tabs.Invalidate();

        // Menu/glyph will be updated when the animation completes
    }

    private void LogLeftPanelLayout(string reason)
    {
        try
        {
            var left = designerCalendarLeftPanel;
            var month = designerMonthCalendar;

            if(left is null || month is null)
            {
                return;
            }

            Rectangle leftBounds = left.Bounds;
            Rectangle leftClient = left.ClientRectangle;
            Rectangle monthBounds = month.Bounds;
            Rectangle monthClient = month.ClientRectangle;
            bool leftVisible = left.Visible;

            // left panel layout: left.Visible={leftVisible}, left.Bounds={leftBounds}, left.Client={leftClient}, month.Bounds={monthBounds}, month.Client={monthClient}
        }
        catch(Exception ex)
        {
            // Swallow layout logging exceptions
        }
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

        // Build the Turn To submenu items from the current tabs so the menu reflects runtime sections.
        ToolStripItem[] turnToItems;
        try
        {
            var _turnToListAll = new List<ToolStripItem>();
            if(_tabs is not null)
            {
                foreach(TabPage tp in _tabs.TabPages)
                {
                    try { _turnToListAll.Add(Command(tp.Text, "Turn To " + tp.Text)); } catch { }
                }
            }

            const int maxVisible = 10;
            var visibleList = _turnToListAll.Take(maxVisible).ToList();

            // Only show the "More sections..." entry when there are more than maxVisible sections
            if(_turnToListAll.Count > maxVisible)
            {
                visibleList.Add(Command("&More sections..."));
            }

            // Fallback to at least the More entry if nothing was discovered
            if(visibleList.Count == 0) visibleList.Add(Command("&More sections..."));

            turnToItems = visibleList.ToArray();
        }
        catch
        {
            turnToItems = new ToolStripItem[] { Command("&More sections...") };
        }

        // Build the Entry In submenu items dynamically from the current tabs.
        ToolStripItem[] entryInItems;
        try
        {
            var _entryInAll = new List<ToolStripItem>();
            if(_tabs is not null)
            {
                foreach(TabPage tp in _tabs.TabPages)
                {
                    try { _entryInAll.Add(Command(tp.Text + "...", "Entry In " + tp.Text)); } catch { }
                }
            }

            const int maxVisibleEntry = 10;
            var visibleEntry = _entryInAll.Take(maxVisibleEntry).ToList();

            if(_entryInAll.Count > maxVisibleEntry)
            {
                visibleEntry.Add(Command("&More sections..."));
            }

            if(visibleEntry.Count == 0) visibleEntry.Add(Command("&More sections..."));

            entryInItems = visibleEntry.ToArray();
        }
        catch
        {
            entryInItems = new ToolStripItem[] { Command("&More sections...") };
        }

        // Create the collapse/expand menu item so we can update its text on toggle
        _collapseBinderPanelMenuItem = Command("Collapse Bi&nder Panel\tF12");

        // Build appointment menu separately so we can update its DropDownOpening before display
        var appointmentMenu = BuildMenu("&Appointment", new ToolStripItem[] {
            Command("&Categorize...\tF5"),
            Command("A&larm...\tF6"),
            Command("&Repeat...\tF7"),
            Command("C&ost...\tF8"),
            Separator(),
            // Create explicit references so we can update checked/enabled state when an appointment is focused
            (_warnOfConflictsMenuItem = CreateMenuItemFromText("&Warn Of Conflicts", "Warn Of Conflicts", false)),
            (_pencilInMenuItem = CreateMenuItemFromText("&Pencil in", "Pencil in", false)),
            (_confidentialMenuItem = CreateMenuItemFromText("Con&fidential\tF4", "Confidential", false))
        });

        // Build recent files menu items (bottom of File menu)
        ToolStripItem[] recentFileItems;
        try
        {
            var recent = new List<ToolStripItem>();
            var recentPaths = _data?.Preferences?.RecentFiles ?? new List<string>();
            foreach(var path in recentPaths.Take(10))
            {
                try
                {
                    var item = CreateMenuItemFromText(path, "OpenRecent:" + path);
                    item.ToolTipText = path;
                    recent.Add(item);
                }
                catch { }
            }

            if(recent.Count == 0)
            {
                recent.Add(Command("(No recent files)"));
            }

            recentFileItems = recent.ToArray();
        }
        catch
        {
            recentFileItems = new ToolStripItem[] { Command("(No recent files)") };
        }

        // Build File menu children and append recent items
        var fileChildren = new List<ToolStripItem>() {
            Command("&New\tCtrl+N"),
            Command("&Open\tCtrl+O"),
            Command("&Close\tCtrl+W"),
            Separator(),
            Command("Save &As...\tShift+Ctrl+S"),
            Separator(),
            Command("A&rchive...\tCtrl+A"),
            Command("Co&mpact..."),
            Command("Mer&ge...\tCtrl+M"),
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
            BuildMenu("&User Setup", new ToolStripItem[] {
                Command("&Organizer Preferences..."),
                Command("&Printer...\tShift+Ctrl+P"),
                Command("Mail and &Scheduling..."),
                Command("Smart&Icons..."),
                Command("Pass&words...\tCtrl+U"),
                Command("&Telephone Dialing...")
            }),
            Separator(),
            Command("E&xit Organizer"),
            Separator()
        };

        fileChildren.AddRange(recentFileItems);

        menu.Items.AddRange(new ToolStripItem[] {
            BuildMenu("&File", fileChildren.ToArray()),
            BuildMenu("&Edit", new ToolStripItem[] {
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
            }),
            BuildMenu("&View", new ToolStripItem[] {
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
                _collapseBinderPanelMenuItem,
                Separator(),
                Command("&Fold Out"),
                Separator(),
                Command("&Apply Filter"),
                Command("C&lear Filter"),
                Separator(),
                Command("Calendar &Preferences...")
            }),
            BuildMenu("&Create", new ToolStripItem[] {
                Command("&Appointment...\tIns"),
                BuildMenu("&Entry In", entryInItems),
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
            }),
            BuildMenu("&Section", new ToolStripItem[] {
                Command("&Customize..."),
                Command("&Show Through..."),
                Command("&Include..."),
                Separator(),
                BuildMenu("&Turn To", turnToItems)
            }),
            appointmentMenu,
            BuildMenu("&Phone", new ToolStripItem[] {
                Command("&Dial...\tCtrl+D"),
                Command("&Quick Dial...\tCtrl+Q"),
                Separator(),
                Command("&Incoming Call..."),
                Separator(),
                Command("&Change Area Codes...")
            }),
            BuildMenu("&Help", new ToolStripItem[] {
                Command("&Help Topics"),
                Command("&Bubble Help\tCtrl+F1"),
                Separator(),
                Command("&About Organizer")
            })
        });

        // Wire up click handlers for appointment menu items so they toggle the focused appointment
        // and persist the change immediately.
        if(_warnOfConflictsMenuItem is not null)
        {
            _warnOfConflictsMenuItem.Click += (_, _) =>
            {
                if(_selectedCalendarEvent is CalendarEvent ev)
                {
                    ev.WarnOfConflicts = !ev.WarnOfConflicts;

                    try
                    {
                        _store.Save(_data);
                    }
                    catch { }

                    UpdateAppointmentMenuItems();
                    _refreshCalendar();
                }
                };
        }

        _pencilInMenuItem?.Click += (_, _) =>
            {
                if(_selectedCalendarEvent is CalendarEvent ev)
                {
                    ev.PencilIn = !ev.PencilIn;

                    try
                    {
                        _store.Save(_data);
                    }
                    catch { }

                    UpdateAppointmentMenuItems();
                    _refreshCalendar();
                }
            };

        _confidentialMenuItem?.Click += (_, _) =>
            {
                if(_selectedCalendarEvent is CalendarEvent ev)
                {
                    ev.Confidential = !ev.Confidential;

                    try
                    {
                        _store.Save(_data);
                    }
                    catch { }

                    UpdateAppointmentMenuItems();
                    _refreshCalendar();
                }
            };

        // Update menu state when the appointment menu is opened so checks reflect current selection
        appointmentMenu?.DropDownOpening += (_, _) => UpdateAppointmentMenuItems();

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
        return CreateMenuItemFromText(text, text);
    }

    private ToolStripMenuItem Command(string text, string commandKey)
    {
        return CreateMenuItemFromText(text, commandKey);
    }

    // Helper that parses menu text for an optional '\t' separated shortcut (e.g. "&New\tCtrl+N").
    // If a shortcut is present it is assigned to ShortcutKeys so the renderer places it at the
    // right-hand side of the menu item rather than being part of the item text.
    private ToolStripMenuItem CreateMenuItemFromText(string text, string commandKey, bool attachExecuteCommand = true)
    {
        string displayText = text;
        string? shortcutText = null;

        int tabIndex = text.IndexOf('\t');
        if(tabIndex >= 0)
        {
            displayText = text.Substring(0, tabIndex);
            shortcutText = text.Substring(tabIndex + 1);
        }

        ToolStripMenuItem menuItem = new(displayText);
        if(attachExecuteCommand)
        {
            menuItem.Click += (_, _) => ExecuteCommand(commandKey);
        }

        if(!string.IsNullOrEmpty(shortcutText))
        {
            try
            {
                // Normalize common alias tokens used in the menu specifications (e.g. Ins -> Insert)
                string normalized = shortcutText!.Trim();
                normalized = normalized.Replace("Ins", "Insert", StringComparison.OrdinalIgnoreCase);
                normalized = normalized.Replace("Del", "Delete", StringComparison.OrdinalIgnoreCase);
                normalized = normalized.Replace("Ctrl+", "Control+", StringComparison.OrdinalIgnoreCase);

                var keys = (Keys)TypeDescriptor.GetConverter(typeof(Keys)).ConvertFromString(normalized)!;
                menuItem.ShortcutKeys = keys;
                menuItem.ShowShortcutKeys = true;
            }
            catch
            {
                // Ignore parse errors and leave the text as-is
            }
        }

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

        if(command.Equals("Mail and Scheduling", StringComparison.OrdinalIgnoreCase))
        {
            ShowMailAndScheduling();

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

            // Generic handler for "Turn To <Section>" commands generated from the Turn To submenu.
            case string s when s.StartsWith("Turn To ", StringComparison.OrdinalIgnoreCase):
                try
                {
                    var sectionName = s.Substring("Turn To ".Length);
                    SelectSection(sectionName);
                }
                catch { }

                return true;

            // Generic handler for recent/opened files
            case string s when s.StartsWith("OpenRecent:", StringComparison.OrdinalIgnoreCase):
                try
                {
                    var path = s.Substring("OpenRecent:".Length);
                    if(File.Exists(path))
                    {
                        LoadData(_store.LoadFrom(path));
                        _currentFilePath = path;
                        AddToRecentFiles(path);
                    }
                    else
                    {
                        MessageBox.Show(this, $"File not found: {path}", "Open Recent", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                catch { }

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

            case "Collapse Binder Panel":
                ToggleBinderPanel();
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
        if(OrganizerPreferencesDialog.Edit(this, _data.Preferences))
        {
            _store.Save(_data);

            // Apply changed preferences to the calendar and force an immediate redraw
            try
            {
                var planner = _tabs.TabPages
                    .Cast<TabPage>()
                    .SelectMany(p => p.Controls.OfType<CalendarPlannerView>())
                    .FirstOrDefault();

                var weekStarts = ParseWeekStarts(_data);

                if(planner is not null)
                {
                    planner.WeekStarts = weekStarts;
                }

                // Also update the left-panel MonthCalendar (the runtime one added by BuildCalendarTab) so it reflects the new preference
                try
                {
                    var leftMonth = designerCalendarLeftPanel?.Controls.OfType<MonthCalendar>().FirstOrDefault();
                    if(leftMonth is not null)
                    {
                        leftMonth.FirstDayOfWeek = ConvertToWinFormsDay(weekStarts);
                        leftMonth.Invalidate();
                    }
                    else
                    {
                        // Fallback: update designer-created control if present
                        try { designerMonthCalendar.FirstDayOfWeek = ConvertToWinFormsDay(weekStarts); designerMonthCalendar.Invalidate(); } catch { }
                    }
                }
                catch { }
            }
            catch { }

            _refreshCalendar();
        }
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
        // Show the standard PrintDialog to select a printer, then the PageSetupDialog for page margins/orientation.
        try
        {
            using PrintDialog pd = new() { UseEXDialog = true };
            // Preselect the saved printer if available
            try { if(!string.IsNullOrWhiteSpace(_data.Preferences.PrinterName)) pd.PrinterSettings.PrinterName = _data.Preferences.PrinterName; } catch { }

            if(pd.ShowDialog(this) == DialogResult.OK)
            {
                try { _data.Preferences.PrinterName = pd.PrinterSettings.PrinterName ?? string.Empty; } catch { }
                _store.Save(_data);
            }

            // Allow page setup (margins, orientation)
            ShowPageSetupDialog();
        }
        catch
        {
            // Fallback to custom dialog if the standard dialogs are unavailable
            try { if(PrinterSetupDialog.Edit(this, _data.Preferences)) _store.Save(_data); } catch { }
        }
    }

    private void ShowMailAndScheduling()
    {
        try
        {
            MailSchedulingDialog.Edit(this);

            // Sync any changed preferences back into the main form's data and persist
            try
            {
                var prefs = ProgramData.Instance.Data.Preferences;
                if(prefs is not null && _data?.Preferences is not null)
                {
                    _data.Preferences.MailProgram = prefs.MailProgram;
                    _data.Preferences.MailProtocol = prefs.MailProtocol;
                    _store.Save(_data);
                }
            }
            catch { }
        }
        catch
        {
            ShowNotImplemented("Mail and Scheduling...");
        }
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
        try { AddToRecentFiles(dialog.FileName); } catch { }
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
            try { AddToRecentFiles(dialog.FileName); } catch { }
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

    private void AddToRecentFiles(string path)
    {
        if (string.IsNullOrWhiteSpace(path)) return;

        try
        {
            var prefs = _data.Preferences ??= new OrganizerPreferences();

            // Remove any existing case-insensitive duplicate
            prefs.RecentFiles.RemoveAll(p => string.Equals(p, path, StringComparison.OrdinalIgnoreCase));

            // Insert at head
            prefs.RecentFiles.Insert(0, path);

            // Trim to 10 entries
            if (prefs.RecentFiles.Count > 10)
            {
                prefs.RecentFiles.RemoveRange(10, prefs.RecentFiles.Count - 10);
            }

            try { _store.Save(_data); } catch { }
        }
        catch { }
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
        IEnumerable<string>? upcomingEvents = _data.Events.Where(item => item.Start >= DateTime.Now && item.Start < today.AddDays(7))
            .OrderBy(item => item.Start)
            .Select(item => $"Appointment: {item.Start:g} {item.Title}");
        IEnumerable<string>? dueTasks = _data.Tasks.Where(item => !item.Completed && item.DueDate.Date <= today.AddDays(7))
            .OrderBy(item => item.DueDate)
            .Select(item => $"To Do: {item.DueDate:g} {item.Title}");
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

    private Panel BuildTrashDropTarget()
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

        // Keep reference to the trash panel for animation
        _trashDropControl = panel;

        panel.Controls.Add(trash);

        return panel;
    }

    private Panel BuildClipboardDropTarget()
    {
        Panel? panel = new()
        {
            Dock = DockStyle.Bottom,
            Height = 58,
            BackColor = Color.FromArgb(178, 134, 61),
            Padding = new Padding(10, 6, 0, 6),
            AllowDrop = true
        };

        Label? clip = new()
        {
            AutoSize = false,
            Width = 58,
            Dock = DockStyle.Left,
            Text = "📋",
            TextAlign = ContentAlignment.MiddleCenter,
            Font = new Font(Font.FontFamily, 18f, FontStyle.Bold),
            BackColor = Color.FromArgb(238, 216, 159),
            ForeColor = Color.FromArgb(72, 48, 24),
            BorderStyle = BorderStyle.FixedSingle,
            AllowDrop = true
        };

        void DragEnterHandler(object? sender, DragEventArgs e)
        {
            // Accept incoming drag payloads compatible with the app's drag payload
            e.Effect = e.Data?.GetDataPresent(typeof(TrashDropPayload)) == true ? DragDropEffects.Copy : DragDropEffects.None;
        }

        void DragDropHandler(object? sender, DragEventArgs e)
        {
            if(e.Data?.GetData(typeof(TrashDropPayload)) is not TrashDropPayload payload) return;

            // Store the payload into the app clipboard (replace any existing)
            _clipboardPayload = payload;
        }

        // Start a drag from the clipboard icon if we have something stored
        clip.MouseDown += (_, e) =>
        {
            if(_clipboardPayload is null) return;

            // Re-expose the stored payload as a drag source wrapped in a DataObject
            try
            {
                DataObject data = new();
                data.SetData(typeof(TrashDropPayload), _clipboardPayload!);
                DoDragDrop(data, DragDropEffects.Move);
            }
            catch { }
        };

        panel.DragEnter += DragEnterHandler;
        panel.DragDrop += DragDropHandler;
        clip.DragEnter += DragEnterHandler;
        clip.DragDrop += DragDropHandler;

        panel.Controls.Add(clip);

        return panel;
    }

    public sealed class TrashDropPayload
    {
        public object Item { get; }
        private readonly Action? _deleteAction;

        public TrashDropPayload(object item, Action? deleteAction)
        {
            Item = item;
            _deleteAction = deleteAction;
        }

        public void Delete() => _deleteAction?.Invoke();
    }

    private TabPage BuildCalendarTab()
    {
        TabPage? page = new("Calendar");
        MonthCalendar? monthCalendar = new() { MaxSelectionCount = 1, ShowTodayCircle = true };
        try
        {
            // Ensure the left-panel MonthCalendar initialises with the user's preference on first load
            monthCalendar.FirstDayOfWeek = ConvertToWinFormsDay(ParseWeekStarts(_data));
        }
        catch { }
        // store planner in a field so other methods can reliably refresh it
        var planner = new CalendarPlannerView() { Dock = DockStyle.Fill };
        _planner = planner;

        // Helper: use class-level ParseWeekStarts
        // Apply preference initially
        try { planner.WeekStarts = ParseWeekStarts(_data); } catch { planner.WeekStarts = DayOfWeek.Sunday; }

        CalendarEvent? selectedEvent = null;
        OrganizerTask? selectedTask = null;

        // Use the instance-level UpdateAppointmentMenuItems so menu state is driven
        // from the single authoritative _selectedCalendarEvent field. Planner-level
        // code should keep the instance selection in sync.

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
        try
        {
            var lastView = _data.Preferences?.LastCalendarView;
            if(!string.IsNullOrWhiteSpace(lastView) && Enum.TryParse<CalendarViewMode>(lastView, true, out var parsed))
            {
                viewMode = parsed;
            }
        }
        catch { }
        DateTime plannerDate = monthCalendar.SelectionStart.Date;

        // Core refresh implementation. When preserveSelection is true we avoid
        // clearing the currently-selected appointment so callers can update flags
        // while keeping menu state in-sync.
        void RefreshCalendarCore(bool preserveSelection)
        {
            if(!preserveSelection)
            {
                selectedEvent = null;
                selectedTask = null;
                _selectedCalendarEvent = null;
            }

            // Ensure planner respects the user's Week Starts preference each refresh
            try { planner.WeekStarts = ParseWeekStarts(_data); } catch { planner.WeekStarts = DayOfWeek.Sunday; }
            planner.SelectedDate = plannerDate;
            planner.ViewMode = viewMode;
            // Keep the left-panel month calendar in sync with the planner's current date
            try { monthCalendar.SetDate(plannerDate); } catch { try { monthCalendar.SelectionStart = plannerDate; } catch { } }
            // Populate events/tasks for the planner (convert to arrays for IReadOnlyList)
            planner.Events = GetCalendarEvents(plannerDate, viewMode).ToArray();
            planner.Tasks = GetCalendarTasks(plannerDate, viewMode).ToArray();
            // Force immediate repaint so changes appear right away
            planner.Refresh();
        }

        void RefreshCalendar() => RefreshCalendarCore(false);

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
            // persist the selected view
            try { _data.Preferences.LastCalendarView = mode.ToString(); _store.Save(_data); } catch { }
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
                    try { _store.Save(_data); } catch { }

                    // Refresh underlying data and planner, then restore the selection so the
                    // appointment menu items reflect the edited appointment immediately.
                    RefreshCalendarCore(true);
                    selectedEvent = calendarEvent;
                    _selectedCalendarEvent = calendarEvent;
                    UpdateAppointmentMenuItems();
                    MainMenuStrip?.Refresh();
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
            // Keep the instance-level focused appointment in sync with planner selection.
            _selectedCalendarEvent = calendarEvent;
            UpdateAppointmentMenuItems();
            MainMenuStrip?.Refresh();
        };

        planner.EventDoubleClicked += (_, calendarEvent) =>
        {
            selectedEvent = calendarEvent;
            selectedTask = null;
            // Keep the instance-level focused appointment in sync before opening edit flow.
            _selectedCalendarEvent = calendarEvent;
            UpdateAppointmentMenuItems();
            MainMenuStrip?.Refresh();
            EditSelectedCalendarEvent();
        };

        planner.TaskSelected += (_, task) =>
        {
            selectedEvent = null;
            selectedTask = task;
            _selectedCalendarEvent = null;
            UpdateAppointmentMenuItems();
            MainMenuStrip?.Refresh();
        };

        planner.TaskDoubleClicked += (_, task) =>
        {
            selectedEvent = null;
            selectedTask = task;
            _selectedCalendarEvent = null;
            UpdateAppointmentMenuItems();
            MainMenuStrip?.Refresh();
            EditSelectedCalendarEvent();
        };

        planner.EmptyAreaDoubleClicked += (_, _) => addButton.PerformClick();

        addButton.Click += (_, _) =>
        {
            // Prefer the planner's selected date (right-side calendar) when available, otherwise fall back to the left MonthCalendar selection
            DateTime date = planner is not null ? planner.SelectedDate.Date : monthCalendar.SelectionStart.Date;
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

        FlowLayoutPanel? buttonPanel = new()
        {
            Dock = DockStyle.Top,
            Height = 48,
            Padding = new Padding(8),
            FlowDirection = FlowDirection.LeftToRight,
            BackColor = Color.FromArgb(199, 156, 75)
        };

        buttonPanel.Controls.AddRange([previousButton, nextButton]);

        Control? trashDropTarget = BuildTrashDropTarget();

        // Populate the shared left-side calendar panel so it is visible on all tabs
        designerCalendarLeftPanel.Controls.Clear();
        // Size the left panel to match the calendar's preferred width plus padding
        Size preferred = monthCalendar.GetPreferredSize(Size.Empty);
        designerCalendarLeftPanel.Width = preferred.Width + 20; // 20px wider as requested
        monthCalendar.Dock = DockStyle.Top;
        designerCalendarLeftPanel.Controls.Add(buttonPanel);

        // Ensure the binder toggle glyph is part of the left panel and placed above the month calendar
        try
        {
            if(_binderToggleLabel is null)
            {
                _binderToggleLabel = new Label
                {
                    Text = _binderPanelCollapsed ? "\u25B6" : "\u25BC",
                    Dock = DockStyle.Top,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Height = 24,
                    Cursor = Cursors.Hand,
                    BackColor = Color.Transparent,
                    Padding = Padding.Empty,
                    Margin = Padding.Empty,
                    Font = new Font(SystemFonts.DefaultFont.FontFamily, SystemFonts.DefaultFont.SizeInPoints + 1.5f, FontStyle.Bold),
                };

                _binderToggleLabel.Click += (_, _) => ExecuteCommand("Collapse Binder Panel");
            }

            // Remove from any previous parent before re-adding
            try
            {
                _binderToggleLabel.Parent?.Controls.Remove(_binderToggleLabel);
            }
            catch { }

            // Add the glyph before the month calendar so it appears above it
            designerCalendarLeftPanel.Controls.Add(_binderToggleLabel);
            // We'll set child index after adding monthCalendar below if needed
        }
        catch { }

        designerCalendarLeftPanel.Controls.Add(monthCalendar);

        // Add clipboard and trash icons into an icons panel at the bottom of the left panel
        Control? clipboardTarget = BuildClipboardDropTarget();
        Control? trashTarget = BuildTrashDropTarget();

        FlowLayoutPanel? iconsPanel = new() { Dock = DockStyle.Bottom, Height = 120, FlowDirection = FlowDirection.TopDown, Padding = new Padding(8) };
        if(clipboardTarget is not null) iconsPanel.Controls.Add(clipboardTarget);
        if(trashTarget is not null) iconsPanel.Controls.Add(trashTarget);

        designerCalendarLeftPanel.Controls.Add(iconsPanel);

        // Attach tooltips to the runtime navigation buttons and the glyph
        try
        {
            if(_toolTip is null) _toolTip = new ToolTip { AutoPopDelay = 5000, InitialDelay = 300, ReshowDelay = 100, ShowAlways = true };
            try { if(previousButton is not null) _toolTip.SetToolTip(previousButton, "Previous"); } catch { }
            try { if(nextButton is not null) _toolTip.SetToolTip(nextButton, "Next"); } catch { }
            try { if(_binderToggleLabel is not null) _toolTip.SetToolTip(_binderToggleLabel, "Collapse/Expand binder panel"); } catch { }
        }
        catch { }

        Panel rightPanel = new() { Dock = DockStyle.Fill, Padding = new Padding(8), BackColor = Color.FromArgb(178, 134, 61) };

        rightPanel.Controls.Add(planner);

        // Only add the right panel to the tab page — the left panel is part of the main form
        page.Controls.Add(rightPanel);
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
            CalendarViewMode.Day => _data.Tasks.Where(task => TaskOccursInRange(task, selectedDate.Date, selectedDate.Date.AddDays(1)))
            .OrderBy(TaskPriority)
            .ThenBy(task => task.Completed).ThenBy(task => task.Title),
            CalendarViewMode.Week => _data.Tasks.Where(task => TaskOccursInRange(task, StartOfWeek(selectedDate), StartOfWeek(selectedDate)
            .AddDays(7))).OrderBy(TaskPriority)
            .ThenBy(task => task.Completed)
            .ThenBy(task => task.Title),
            CalendarViewMode.Month => _data.Tasks.Where(task => TaskOccursInRange(task, new DateTime(selectedDate.Year, selectedDate.Month, 1), new DateTime(selectedDate.Year, selectedDate.Month, 1)
            .AddMonths(1)))
            .OrderBy(TaskPriority).ThenBy(task => task.Completed).ThenBy(task => task.Title),
            _ => _data.Tasks.OrderBy(TaskPriority)
            .ThenBy(task => task.Completed)
            .ThenBy(task => task.Title)
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
            // Only start dragging with the left button
            if(e.Button != MouseButtons.Left)
            {
                dragStart = null;
                dragItem = null;

                return;
            }

            // Only certain tabs expose item-level dragging for deletion
            if(!new[] { "Contacts", "Tasks", "Anniversary", "To Do", "Notes", "Notepad" }.Contains(title))
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
            var payload = new TrashDropPayload(itemToDelete, () =>
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
            // Wrap payload in DataObject so drop targets can reliably detect it
            DataObject gridData = new();
            gridData.SetData(typeof(TrashDropPayload), payload);
            grid.DoDragDrop(gridData, DragDropEffects.Move);
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

            List<Note>? headings = _data.Notes
                .Where(note => note.IsChapterHeading)
                .OrderBy(note => note.SortOrder)
                .ThenBy(note => note.Title)
                .ToList();

            foreach(Note heading in headings)
            {
                TreeNode? headingNode = CreateNoteNode(heading);

                tree.Nodes.Add(headingNode);

                foreach(Note child in _data.Notes.Where(note => !note.IsChapterHeading && note.ParentHeadingId == heading.Id)
                    .OrderBy(note => note.SortOrder)
                    .ThenBy(note => note.Title))
                {
                    headingNode.Nodes.Add(CreateNoteNode(child));
                }
            }

            foreach(Note note in _data.Notes.Where(note => !note.IsChapterHeading && note.ParentHeadingId is null)
                .OrderBy(note => note.SortOrder)
                .ThenBy(note => note.Title))
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
            if(e.Item is not TreeNode node || node.Tag is not Note note) return;

            // Build a payload that will remove the note (or adjust children if a chapter)
            var payload = new TrashDropPayload(note, () =>
            {
                var list = (SortableBindingList<Note>)source.DataSource;

                if(note.IsChapterHeading)
                {
                    foreach(var child in _data.Notes.Where(child => child.ParentHeadingId == note.Id))
                    {
                        child.ParentHeadingId = null;
                    }
                }

                list.Remove(note);
                source.ResetBindings(false);
                _store.Save(_data);
                RebuildNotebookTree();
            });

            // Wrap payload in DataObject so drop targets can reliably detect it
            DataObject treeData = new();
            treeData.SetData(typeof(TrashDropPayload), payload);
            tree.DoDragDrop(treeData, DragDropEffects.Move);
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

    // Tab drag/reorder handlers — retained but disabled to prevent tab header drag/delete
    // Tab-level dragging was intentionally disabled: only section line items support drag-to-trash.
    private void Tabs_MouseDown(object? sender, MouseEventArgs e)
    { }
    private void Tabs_MouseMove(object? sender, MouseEventArgs e)
    { }
    private void Tabs_MouseUp(object? sender, MouseEventArgs e)
    { }
    private void Tabs_DragOver(object? sender, DragEventArgs e)
    { }
    private void Tabs_DragDrop(object? sender, DragEventArgs e)
    { }

    private void TrashPanel_DragEnter(object? sender, DragEventArgs e)
    {
        // Accept any move operations onto the trash
        if(e.Data.GetDataPresent(typeof(int)) || e.Data.GetDataPresent(typeof(TrashDropPayload)))
        {
            e.Effect = DragDropEffects.Move;
            if(sender is Control c)
            {
                c.BackColor = Color.FromArgb(200, 100, 50);
                StartTrashAnimation();
            }
        }
        else
        {
            e.Effect = DragDropEffects.None;
        }
    }

    private void TrashPanel_DragLeave(object? sender, EventArgs e)
    {
        if(sender is Control c)
        {
            c.BackColor = Color.FromArgb(178, 134, 61);
            StopTrashAnimation();
        }
    }

    private void TrashPanel_DragDrop(object? sender, DragEventArgs e)
    {
        // Support two payload types: integer tab index (from tab drag) and TrashDropPayload for custom items
        if(e.Data.GetDataPresent(typeof(int)) && sender is Panel)
        {
            int srcIndex = (int)e.Data.GetData(typeof(int));
            // If a tab was dragged, remove that TabPage
            if(_tabs is not null && srcIndex >= 0 && srcIndex < _tabs.TabCount)
            {
                _tabs.TabPages.RemoveAt(srcIndex);
            }
        }

        if(e.Data.GetDataPresent(typeof(TrashDropPayload)))
        {
            var payload = e.Data.GetData(typeof(TrashDropPayload)) as TrashDropPayload;

            try
            {
                payload?.Delete();
            }
            catch
            {
                // ignore
            }
        }

        if(sender is Control c)
        {
            c.BackColor = Color.FromArgb(178, 134, 61);
            StopTrashAnimation();
        }
    }

    private void StartTrashAnimation()
    {
        if(_trashAnimating) return;
        _trashAnimating = true;
        _trashAnimationTick = 0;
        _trashAnimationTimer.Start();
    }

    private void StopTrashAnimation()
    {
        if(!_trashAnimating) return;
        _trashAnimating = false;
        _trashAnimationTimer.Stop();
        designerTrashPanel.Invalidate();
    }

    private void DesignerTrashPanel_Paint(object? sender, PaintEventArgs e)
    {
        // Paint a pulsing trash icon; use tick to scale
        var g = e.Graphics;
        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
        var rect = designerTrashPanel.ClientRectangle;
        Rectangle drawArea;

        // Prefer drawing inside the designerTrashLabel bounds (left side) so it replaces the label
        if(designerTrashLabel is not null)
        {
            var lb = designerTrashLabel.Bounds;
            drawArea = new Rectangle(lb.Left + 2, lb.Top + 2, Math.Max(24, lb.Width - 4), Math.Max(24, lb.Height - 4));
        }
        else
        {
            int size = Math.Min(rect.Width, rect.Height) - 8;
            drawArea = new Rectangle(rect.Left + (rect.Width - size) / 2, rect.Top + (rect.Height - size) / 2, size, size);
        }

        int size2 = Math.Min(drawArea.Width, drawArea.Height);
        float scale = 1.0f + 0.06f * (float)Math.Sin(_trashAnimationTick * 0.3);
        int w = (int)(size2 * scale);
        int h = (int)(size2 * scale);
        int x = drawArea.Left + (drawArea.Width - w) / 2;
        int y = drawArea.Top + (drawArea.Height - h) / 2;

        using var brush = new SolidBrush(Color.FromArgb(255, 240, 240));
        using var pen = new Pen(Color.WhiteSmoke, 2);

        // Simple trash bin shape
        var binRect = new Rectangle(x, y + h / 6, w, h * 5 / 6);
        g.FillRectangle(brush, binRect);
        g.DrawRectangle(pen, binRect);

        var lidRect = new Rectangle(x - w / 8, y, w + w / 4, h / 4);
        g.FillRectangle(brush, lidRect);
        g.DrawRectangle(pen, lidRect);
    }
    private void aboutToolStripMenuItem_Click(object? sender, EventArgs e)
    {
        using AboutForm about = new();

        about.ShowDialog(this);
    }

    private void designerCalendarLeftPanel_Paint(object sender, PaintEventArgs e)
    {
    }
}