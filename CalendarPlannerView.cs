using System.ComponentModel;
using System.Globalization;

namespace Organizer;

internal sealed class CalendarPlannerView : Control
{
    private const int TimeGutter = 0;
    private const int DayHeaderHeight = 28;
    private readonly Dictionary<Rectangle, CalendarEvent> _eventBounds = new();
    private readonly Dictionary<Rectangle, OrganizerTask> _taskBounds = new();
    private readonly Font _smallFont;
    private readonly Font _boldFont;
    private CalendarEvent? _selectedEvent;
    private OrganizerTask? _selectedTask;

    // True when the user has explicitly selected a date (via mouse). When false,
    // only DateTime.Today will be highlighted.
    private bool _hasUserDateSelection;

    public CalendarPlannerView()
    {
        DoubleBuffered = true;
        BackColor = Color.FromArgb(255, 252, 237);
        ForeColor = Color.FromArgb(30, 30, 30);
        _smallFont = new Font(Font.FontFamily, 8.5f);
        _boldFont = new Font(Font, FontStyle.Bold);
        SetStyle(ControlStyles.ResizeRedraw, true);
    }

    private void DrawYear(Graphics g, Rectangle content, Brush headerBrush, Pen borderPen, Pen lightPen)
    {
        int cols = 3, rows = 4;
        int gap = 8;
        int cellW = Math.Max(1, content.Width / cols);
        int cellH = Math.Max(1, content.Height / rows);

        for(int i = 0; i < 12; i++)
        {
            int col = i % cols;
            int row = i / cols;
            Rectangle cell = new(content.Left + col * cellW + gap / 2, content.Top + row * cellH + gap / 2, cellW - gap, cellH - gap);

            using SolidBrush paper = new(Color.FromArgb(255, 253, 239));
            g.FillRectangle(paper, cell);
            g.DrawRectangle(borderPen, cell);

            DateTime monthDate = new(SelectedDate.Year, i + 1, 1);
            string title = monthDate.ToString("MMMM yyyy", CultureInfo.CurrentCulture);
            TextRenderer.DrawText(g, title, _boldFont, new Rectangle(cell.Left + 4, cell.Top + 2, cell.Width - 8, 20), Color.FromArgb(30, 30, 30), TextFormatFlags.Left | TextFormatFlags.Top);

            DrawMiniMonth(g, monthDate, new Rectangle(cell.Left + 4, cell.Top + 22, cell.Width - 8, cell.Height - 26), lightPen);
        }
    }

    private void DrawMiniMonth(Graphics g, DateTime month, Rectangle area, Pen lightPen)
    {
        const int dayHeaderHeight = 16;
        DateTime first = new(month.Year, month.Month, 1);
        DateTime firstVisible = StartOfWeek(first);
        DateTime last = new(month.Year, month.Month, DateTime.DaysInMonth(month.Year, month.Month));
        DateTime lastVisible = StartOfWeek(last).AddDays(6);
        int weeks = Math.Max(1, (int)((lastVisible - firstVisible).TotalDays + 1) / 7);
        int colW = Math.Max(1, area.Width / 7);
        int rowH = Math.Max(1, (area.Height - dayHeaderHeight) / weeks);

        for(int c = 0; c < 7; c++)
        {
            string d = CultureInfo.CurrentCulture.DateTimeFormat.AbbreviatedDayNames[(c + (int)WeekStarts) % 7];
            Rectangle hdr = new(area.Left + c * colW, area.Top, colW, dayHeaderHeight);
            TextRenderer.DrawText(g, d.Substring(0, 1), _smallFont, hdr, ForeColor, TextFormatFlags.Right | TextFormatFlags.VerticalCenter);
        }

        for(int r = 0; r < weeks; r++)
        {
            for(int c = 0; c < 7; c++)
            {
                int dayIndex = r * 7 + c;
                DateTime d = firstVisible.AddDays(dayIndex);
                Rectangle cell = new(area.Left + c * colW, area.Top + dayHeaderHeight + r * rowH, colW, rowH);
                // Only show days that belong to the requested month. Skip cells that
                // fall in the previous/next month so the mini-month displays only
                // the current month's days.
                if(d.Month != month.Month)
                {
                    continue;
                }

                Color txt = ForeColor;

                // Only highlight the selected date when the user explicitly selected a date.
                // Otherwise, highlight only DateTime.Today.
                if(_hasUserDateSelection)
                {
                    if(d.Date == SelectedDate.Date)
                    {
                        using SolidBrush selectedBrush = new(Color.FromArgb(92, 125, 176));
                        g.FillRectangle(selectedBrush, cell);
                        txt = Color.White;
                    }
                }
                else
                {
                    if(d.Date == DateTime.Today)
                    {
                        using SolidBrush selectedBrush = new(Color.FromArgb(92, 125, 176));
                        g.FillRectangle(selectedBrush, cell);
                        txt = Color.White;
                    }
                }

                TextRenderer.DrawText(g, d.Day.ToString(CultureInfo.CurrentCulture), _smallFont, Rectangle.Inflate(cell, -2, -1), txt, TextFormatFlags.Right | TextFormatFlags.Top);
            }
        }
    }

    private DateTime _selectedDate = DateTime.Today;

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public DateTime SelectedDate
    {
        get => _selectedDate;
        set
        {
            // When SelectedDate is set programmatically, treat it as NOT a user selection
            // so the control will only highlight DateTime.Today unless the user clicks.
            _selectedDate = value.Date;
            _hasUserDateSelection = false;
        }
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public CalendarViewMode ViewMode { get; set; } = CalendarViewMode.Day;

    // Extra view parameters for variants
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [DefaultValue(1)]
    public int DaysToShow { get; set; } = 1; // 1 = single day, 2 = two-day, etc.

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [DefaultValue(false)]
    public bool ShowWorkWeekOnly { get; set; } = false; // when true, week views show Mon-Fri only

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [DefaultValue(60)]
    public int TimeSlotMinutes { get; set; } = 60; // granularity in minutes for Weekly Time Slot view

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public IReadOnlyList<CalendarEvent> Events { get; set; } = Array.Empty<CalendarEvent>();

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public IReadOnlyList<OrganizerTask> Tasks { get; set; } = Array.Empty<OrganizerTask>();

    public event EventHandler<CalendarEvent>? EventSelected;

    public event EventHandler<CalendarEvent>? EventDoubleClicked;

    public event EventHandler<OrganizerTask>? TaskSelected;

    public event EventHandler<OrganizerTask>? TaskDoubleClicked;

    public event EventHandler? EmptyAreaDoubleClicked;

    protected override void Dispose(bool disposing)
    {
        if(disposing)
        {
            _smallFont.Dispose();
            _boldFont.Dispose();
        }

        base.Dispose(disposing);
    }

    protected override void OnMouseDown(MouseEventArgs e)
    {
        base.OnMouseDown(e);
        bool hit = SelectCalendarItemAt(e.Location);

        // When clicking empty area, update SelectedDate to the date under the click so external callers
        // (for example, the main form Add flow) can rely on the planner's SelectedDate.
        if(!hit)
        {
            DateTime? date = GetDateAt(e.Location);

            if(date is not null)
            {
                SelectedDate = date.Value.Date;
                // Mark that the user explicitly selected a date so it will be highlighted
                // instead of the default Today highlight.
                _hasUserDateSelection = true;
                Invalidate();
            }
        }
    }

    // Returns the calendar date corresponding to the supplied client point, or null when outside the calendar area.
    private DateTime? GetDateAt(Point location)
    {
        Rectangle page = ClientRectangle;

        page.Inflate(-4, -4);

        if(page.Width <= 0 || page.Height <= 0) return null;

        Rectangle content = Rectangle.Inflate(page, -8, -8);

        switch(ViewMode)
        {
            case CalendarViewMode.Year:
                {
                    // layout: 3 columns x 4 rows of months
                    int cols = 3, rows = 4;
                    int gap = 8;
                    int cellW = Math.Max(1, content.Width / cols);
                    int cellH = Math.Max(1, content.Height / rows);
                    int col = Math.Clamp((location.X - content.Left) / (cellW == 0 ? 1 : cellW), 0, cols - 1);
                    int row = Math.Clamp((location.Y - content.Top) / (cellH == 0 ? 1 : cellH), 0, rows - 1);
                    int monthIndex = row * cols + col + 1;

                    Rectangle cell = new(content.Left + col * cellW + gap / 2, content.Top + row * cellH + gap / 2, cellW - gap, cellH - gap);
                    Rectangle title = new(cell.Left, cell.Top, cell.Width, 26);
                    Rectangle grid = new(cell.Left, title.Bottom, cell.Width, cell.Height - title.Height);
                    const int dayHeaderHeight = 20;

                    if(location.Y < grid.Top + dayHeaderHeight || location.Y > grid.Bottom) return null;

                    DateTime first = new(SelectedDate.Year, monthIndex, 1);
                    DateTime firstVisible = StartOfWeek(first);
                    DateTime last = new(SelectedDate.Year, monthIndex, DateTime.DaysInMonth(SelectedDate.Year, monthIndex));
                    DateTime lastVisible = StartOfWeek(last).AddDays(6);
                    int weeks = Math.Max(1, (int)((lastVisible - firstVisible).TotalDays + 1) / 7);

                    int colWidth = grid.Width / 7;
                    int rowHeight = Math.Max(1, (grid.Height - dayHeaderHeight) / weeks);
                    int c = Math.Clamp((location.X - grid.Left) / (colWidth == 0 ? 1 : colWidth), 0, 6);
                    int r = Math.Clamp((location.Y - (grid.Top + dayHeaderHeight)) / (rowHeight == 0 ? 1 : rowHeight), 0, weeks - 1);

                    DateTime day = firstVisible.AddDays(r * 7 + c);
                    return day.Date;
                }

            case CalendarViewMode.Month:
                {
                    Rectangle title = new(content.Left, content.Top, content.Width, 36);
                    Rectangle grid = new(content.Left, title.Bottom, content.Width, content.Height - title.Height);
                    const int dayHeaderHeight = 26;

                    DateTime first = new(SelectedDate.Year, SelectedDate.Month, 1);
                    DateTime firstVisible = StartOfWeek(first);
                    DateTime last = new(SelectedDate.Year, SelectedDate.Month, DateTime.DaysInMonth(SelectedDate.Year, SelectedDate.Month));
                    DateTime lastVisible = StartOfWeek(last).AddDays(6);
                    int weeks = Math.Max(1, (int)((lastVisible - firstVisible).TotalDays + 1) / 7);

                    if(location.Y < grid.Top + dayHeaderHeight || location.Y > grid.Bottom) return null;

                    int colWidth = grid.Width / 7;
                    int col = Math.Clamp((location.X - grid.Left) / (colWidth == 0 ? 1 : colWidth), 0, 6);
                    int rowHeight = Math.Max(1, (grid.Height - dayHeaderHeight) / weeks);
                    int row = Math.Clamp((location.Y - (grid.Top + dayHeaderHeight)) / (rowHeight == 0 ? 1 : rowHeight), 0, weeks - 1);

                    DateTime day = firstVisible.AddDays(row * 7 + col);
                    return day.Date;
                }

            case CalendarViewMode.Week:
                {
                    Rectangle title = new(content.Left, content.Top, content.Width, 32);
                    Rectangle body = new(content.Left, title.Bottom, content.Width, content.Height - title.Height);
                    const int dayCount = 7;
                    int columnWidth = Math.Max(1, (body.Width - TimeGutter) / dayCount);
                    int x = location.X - (body.Left + TimeGutter);

                    if(location.Y < body.Top || location.Y > body.Bottom) return null;

                    int dayIndex = Math.Clamp(x / columnWidth, 0, dayCount - 1);
                    DateTime start = StartOfWeek(SelectedDate);

                    return start.AddDays(dayIndex).Date;
                }

            default: // Day view
                {
                    // Day view represents a single date
                    return SelectedDate.Date;
                }
        }
    }

    protected override void OnMouseDoubleClick(MouseEventArgs e)
    {
        base.OnMouseDoubleClick(e);

        if(!SelectCalendarItemAt(e.Location))
        {
            EmptyAreaDoubleClicked?.Invoke(this, EventArgs.Empty);

            return;
        }

        if(_selectedEvent is not null) EventDoubleClicked?.Invoke(this, _selectedEvent);
        if(_selectedTask is not null) TaskDoubleClicked?.Invoke(this, _selectedTask);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        e.Graphics.Clear(BackColor);
        e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
        _eventBounds.Clear();
        _taskBounds.Clear();

        using Pen? borderPen = new(Color.FromArgb(174, 139, 72));
        using Pen? lightPen = new(Color.FromArgb(226, 206, 154));
        using SolidBrush? headerBrush = new(Color.FromArgb(244, 222, 163));
        using SolidBrush? paperBrush = new(Color.FromArgb(255, 253, 239));

        Rectangle page = ClientRectangle;

        // Reduce outer page padding so the calendar uses more available space
        page.Inflate(-4, -4);

        if(page.Width <= 0 || page.Height <= 0) return;

        e.Graphics.FillRectangle(paperBrush, page);
        e.Graphics.DrawRectangle(borderPen, page);

        // Reduce content inset to expand the calendar drawing area
        Rectangle content = Rectangle.Inflate(page, -8, -8);

        switch(ViewMode)
        {
            case CalendarViewMode.TwoDay:
            case CalendarViewMode.WorkWeek:
            case CalendarViewMode.WeekPerPage:
            case CalendarViewMode.WeeklyTimeSlot:
            case CalendarViewMode.Week:
                DrawWeek(e.Graphics, content, headerBrush, borderPen, lightPen);
                break;

            case CalendarViewMode.Month:
                DrawMonth(e.Graphics, content, headerBrush, borderPen, lightPen);
                break;

            case CalendarViewMode.Year:
                DrawYear(e.Graphics, content, headerBrush, borderPen, lightPen);
                break;

            default:
                DrawDay(e.Graphics, content, headerBrush, borderPen, lightPen);
                break;
        }
    }

    private bool SelectCalendarItemAt(Point location)
    {
        foreach(KeyValuePair<Rectangle, CalendarEvent> pair in _eventBounds)
        {
            if(!pair.Key.Contains(location)) continue;

            _selectedEvent = pair.Value;
            _selectedTask = null;
            EventSelected?.Invoke(this, pair.Value);
            Invalidate();

            return true;
        }

        foreach(KeyValuePair<Rectangle, OrganizerTask> pair in _taskBounds)
        {
            if(!pair.Key.Contains(location)) continue;

            _selectedEvent = null;
            _selectedTask = pair.Value;
            TaskSelected?.Invoke(this, pair.Value);
            Invalidate();

            return true;
        }

        _selectedEvent = null;
        _selectedTask = null;
        Invalidate();

        return false;
    }

    private void DrawDay(Graphics graphics, Rectangle bounds, Brush headerBrush, Pen borderPen, Pen lightPen)
    {
        Rectangle header = new(bounds.Left, bounds.Top, bounds.Width, 42);

        graphics.FillRectangle(headerBrush, header);
        TextRenderer.DrawText(graphics, SelectedDate.ToString("dddd, MMMM d, yyyy"), _boldFont, header, ForeColor, TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter);

        Rectangle body = new(bounds.Left, header.Bottom, bounds.Width, bounds.Height - header.Height);
        DateTime[]? days = [SelectedDate.Date];
        Rectangle timeGrid = new(body.Left, body.Top, body.Width, body.Height);

        DrawTimeGrid(graphics, timeGrid, 1, days, lightPen, borderPen);
        DrawCalendarItems(graphics, timeGrid, days);
    }

    private void DrawWeek(Graphics graphics, Rectangle bounds, Brush headerBrush, Pen borderPen, Pen lightPen)
    {
        DateTime start = StartOfWeek(SelectedDate);
        int totalDays = ViewMode == CalendarViewMode.TwoDay ? Math.Max(1, DaysToShow) : 7;

        // When showing workweek, use Monday..Friday
        if(ShowWorkWeekOnly && ViewMode != CalendarViewMode.TwoDay)
        {
            totalDays = 5;
            // Ensure start is a Monday
            while(start.DayOfWeek != DayOfWeek.Monday) start = start.AddDays(1);
        }

        DateTime[]? days = Enumerable.Range(0, totalDays).Select(dayOffset => start.AddDays(dayOffset)).ToArray();
        Rectangle title = new(bounds.Left, bounds.Top, bounds.Width, 32);

        graphics.FillRectangle(headerBrush, title);
        graphics.DrawRectangle(borderPen, title);
        TextRenderer.DrawText(graphics, $"Week of {start:MMMM d, yyyy}", _boldFont, title, ForeColor, TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter);

        Rectangle body = new(bounds.Left, title.Bottom, bounds.Width, bounds.Height - title.Height);
        Rectangle timeGrid = new(body.Left, body.Top, body.Width, body.Height);

        DrawTimeGrid(graphics, timeGrid, days.Length, days, lightPen, borderPen);
        DrawCalendarItems(graphics, timeGrid, days);
    }

    private void DrawMonth(Graphics graphics, Rectangle bounds, Brush headerBrush, Pen borderPen, Pen lightPen)
    {
        Rectangle title = new(bounds.Left, bounds.Top, bounds.Width, 36);

        graphics.FillRectangle(headerBrush, title);
        graphics.DrawRectangle(borderPen, title);
        TextRenderer.DrawText(graphics, SelectedDate.ToString("MMMM yyyy"), _boldFont, title, ForeColor, TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter);

        Rectangle grid = new(bounds.Left, title.Bottom, bounds.Width, bounds.Height - title.Height);
        const int dayHeaderHeight = 26;
        CultureInfo? culture = CultureInfo.CurrentCulture;
        string[]? dayNames = Enumerable.Range(0, 7)
            .Select(i => culture.DateTimeFormat.AbbreviatedDayNames[((int)WeekStarts + i) % 7])
            .ToArray();
        int colWidth = grid.Width / 7;

        // Month grid geometry computed

        for(int column = 0; column < 7; column++)
        {
            int headerX = grid.Left + column * colWidth;
            int headerWidth = column == 6 ? grid.Right - headerX : colWidth;
            Rectangle header = new(headerX, grid.Top, headerWidth, dayHeaderHeight);

            graphics.FillRectangle(headerBrush, header);
            graphics.DrawRectangle(borderPen, header);
            TextRenderer.DrawText(graphics, dayNames[column], _boldFont, header, ForeColor, TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter);
        }

        DateTime first = new(SelectedDate.Year, SelectedDate.Month, 1);
        DateTime firstVisible = StartOfWeek(first);
        DateTime last = new(SelectedDate.Year, SelectedDate.Month, DateTime.DaysInMonth(SelectedDate.Year, SelectedDate.Month));
        DateTime lastVisible = StartOfWeek(last).AddDays(6);
        int weeks = Math.Max(1, (int)((lastVisible - firstVisible).TotalDays + 1) / 7);
        int cellTop = grid.Top + dayHeaderHeight;
        int rowHeight = Math.Max(1, (grid.Height - dayHeaderHeight) / weeks);

        for(int row = 0; row < weeks; row++)
        {
            for(int column = 0; column < 7; column++)
            {
                DateTime day = firstVisible.AddDays(row * 7 + column);
                int x = grid.Left + column * colWidth;
                Rectangle cell = new(x, cellTop + row * rowHeight, column == 6 ? grid.Right - x : colWidth, row == weeks - 1 ? grid.Bottom - (cellTop + row * rowHeight) : rowHeight);
                bool inMonth = day.Month == SelectedDate.Month;

                using SolidBrush? cellBrush = new(day.Date == SelectedDate.Date ? Color.FromArgb(255, 241, 180) : inMonth ? Color.FromArgb(255, 253, 239) : Color.FromArgb(239, 232, 211));
                graphics.FillRectangle(cellBrush, cell);
                graphics.DrawRectangle(lightPen, cell);

                TextRenderer.DrawText(graphics, day.Day.ToString(), inMonth ? _boldFont : _smallFont, new Rectangle(cell.Left + 4, cell.Top + 3, cell.Width - 8, 18), inMonth ? ForeColor : Color.Gray, TextFormatFlags.Left);

                List<OrganizerTask>? tasks = Tasks.Where(task => TaskOccursOnDate(task, day.Date))
                    .OrderBy(TaskPriority)
                    .ThenBy(task => task.Completed)
                    .ThenBy(task => task.Title)
                    .Take(4).ToList();
                List<CalendarEvent>? events = Events.Where(calendarEvent => calendarEvent.Start.Date == day.Date).OrderBy(calendarEvent => calendarEvent.Start).Take(4 - tasks.Count).ToList();
                int y = cell.Top + 24;

                foreach(OrganizerTask task in tasks)
                {
                    Rectangle taskRect = new(cell.Left + 4, y, cell.Width - 8, 18);

                    DrawTaskBlock(graphics, taskRect, task);
                    y += 20;
                }

                foreach(CalendarEvent calendarEvent in events)
                {
                    Rectangle eventRect = new(cell.Left + 4, y, cell.Width - 8, 18);

                    DrawEventBlock(graphics, eventRect, calendarEvent, $"{calendarEvent.Start:h:mm} {calendarEvent.Title}");
                    y += 20;
                }
            }
        }

        graphics.DrawRectangle(borderPen, grid);
    }

    private void DrawTimeGrid(Graphics graphics, Rectangle body, int dayCount, DateTime[] days, Pen lightPen, Pen borderPen)
    {
        //Rectangle timeline = new(body.Left, body.Top + DayHeaderHeight, body.Width, body.Height - DayHeaderHeight);
        int columnWidth = Math.Max(1, (body.Width - TimeGutter) / dayCount);

        graphics.DrawRectangle(borderPen, new Rectangle(body.Left, body.Top, body.Width - 1, DayHeaderHeight - 1));
        using SolidBrush? headerBrush = new(Color.FromArgb(248, 231, 183));
        graphics.FillRectangle(headerBrush, new Rectangle(body.Left, body.Top, body.Width, DayHeaderHeight));

        // Grid and column geometry computed
        for(int dayIndex = 0; dayIndex < dayCount; dayIndex++)
        {
            int x = body.Left + TimeGutter + dayIndex * columnWidth;
            int width = dayIndex == dayCount - 1 ? body.Right - x : columnWidth;
            Rectangle header = new(x, body.Top, width, DayHeaderHeight);

            var headerText = dayCount == 1 ? "Appointments" : days[dayIndex].ToString("ddd M/d");

            // If we are in the WeeklyTimeSlot view and a finer timeslot is requested,
            // indicate the slot granularity in the header for clarity.
            if(TimeSlotMinutes < 60 && (ViewMode == CalendarViewMode.WeeklyTimeSlot))
            {
                headerText += " (" + TimeSlotMinutes + "m)";
            }

            TextRenderer.DrawText(graphics, headerText, _boldFont, header, ForeColor, TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter);
        }
    }

    private void DrawCalendarItems(Graphics graphics, Rectangle body, DateTime[] days)
    {
        Rectangle timeline = new(body.Left, body.Top + DayHeaderHeight, body.Width, body.Height - DayHeaderHeight);
        int dayCount = days.Length;
        int columnWidth = Math.Max(1, (body.Width - TimeGutter) / dayCount);

        for(int dayIndex = 0; dayIndex < dayCount; dayIndex++)
        {
            DateTime day = days[dayIndex];
            int x = body.Left + TimeGutter + dayIndex * columnWidth + 5;
            int width = (dayIndex == dayCount - 1 ? body.Right - (body.Left + TimeGutter + dayIndex * columnWidth) : columnWidth) - 10;
            IOrderedEnumerable<OrganizerTask>? dayTasks = Tasks.Where(task => TaskOccursOnDate(task, day.Date)).OrderBy(TaskPriority).ThenBy(task => task.Completed).ThenBy(task => task.Title);
            IOrderedEnumerable<CalendarEvent>? dayEvents = Events.Where(calendarEvent => calendarEvent.Start.Date == day.Date).OrderBy(calendarEvent => calendarEvent.Start);
            int y = timeline.Top + 5;

            foreach(OrganizerTask task in dayTasks)
            {
                if(y >= timeline.Bottom - 4) break;

                int height = Math.Min(24, Math.Max(20, timeline.Bottom - y - 4));
                Rectangle taskRect = new(x, y, Math.Max(20, width), height);

                DrawTaskBlock(graphics, taskRect, task);
                y += height + 4;
            }

            foreach(CalendarEvent calendarEvent in dayEvents)
            {
                if(y >= timeline.Bottom - 4) break;

                int height = Math.Min(24, Math.Max(20, timeline.Bottom - y - 4));
                Rectangle eventRect = new(x, y, Math.Max(20, width), height);

                DrawEventBlock(graphics, eventRect, calendarEvent, $"{calendarEvent.Start:h:mm tt}  {calendarEvent.Title}");
                y += height + 4;
            }
        }
    }

    private void DrawEventBlock(Graphics graphics, Rectangle bounds, CalendarEvent calendarEvent, string text)
    {
        if(bounds.Width <= 0 || bounds.Height <= 0) return;

        bool selected = ReferenceEquals(calendarEvent, _selectedEvent);
        using SolidBrush? fill = new(selected ? Color.FromArgb(92, 125, 176) : Color.FromArgb(255, 244, 190));
        using Pen? border = new(selected ? Color.FromArgb(72, 92, 115) : Color.FromArgb(178, 128, 42));

        graphics.FillRectangle(fill, bounds);
        graphics.DrawRectangle(border, bounds);
        _eventBounds[bounds] = calendarEvent;

        Color color = selected ? Color.White : Color.FromArgb(50, 42, 20);
        Rectangle textBounds = Rectangle.Inflate(bounds, -4, -2);

        if(calendarEvent.PencilIn)
        {
            DrawPencilInIcon(graphics, new Rectangle(textBounds.Left, textBounds.Top + 2, 12, 12), selected);
            textBounds.X += 15;
            textBounds.Width = Math.Max(1, textBounds.Width - 15);
        }

        TextRenderer.DrawText(graphics, string.IsNullOrWhiteSpace(calendarEvent.Title) ? "(Untitled)" : text, _smallFont, textBounds, color, TextFormatFlags.Left | TextFormatFlags.Top | TextFormatFlags.EndEllipsis | TextFormatFlags.WordBreak);
    }

    private static void DrawPencilInIcon(Graphics graphics, Rectangle bounds, bool selected)
    {
        using Pen? pencil = new(selected ? Color.White : Color.FromArgb(155, 111, 0), 2f);
        using Pen? outline = new(selected ? Color.FromArgb(230, 230, 230) : Color.FromArgb(72, 48, 24));

        graphics.DrawLine(pencil, bounds.Left + 2, bounds.Bottom - 3, bounds.Right - 3, bounds.Top + 2);
        graphics.DrawLine(outline, bounds.Left + 1, bounds.Bottom - 2, bounds.Right - 2, bounds.Top + 1);
        graphics.FillPolygon(
            selected ? Brushes.White : Brushes.Bisque,
            [new(bounds.Right - 3, bounds.Top + 2), new Point(bounds.Right, bounds.Top), new Point(bounds.Right - 1, bounds.Top + 4)]);
    }

    private void DrawTaskBlock(Graphics graphics, Rectangle bounds, OrganizerTask task)
    {
        if(bounds.Width <= 0 || bounds.Height <= 0) return;

        bool selected = ReferenceEquals(task, _selectedTask);
        using SolidBrush? fill = new(selected ? Color.FromArgb(91, 139, 74) : task.Completed ? Color.FromArgb(220, 226, 205) : Color.FromArgb(218, 238, 196));
        using Pen? border = new(selected ? Color.FromArgb(42, 89, 35) : Color.FromArgb(95, 137, 67));

        graphics.FillRectangle(fill, bounds);
        graphics.DrawRectangle(border, bounds);
        _taskBounds[bounds] = task;

        string? title = string.IsNullOrWhiteSpace(task.Title) ? "(Untitled task)" : task.Title;
        string? text = $"{TaskPriorityLabel(task)} {title}";

        TextRenderer.DrawText(graphics, text, _smallFont, Rectangle.Inflate(bounds, -4, -2), selected ? Color.White : Color.FromArgb(38, 75, 30), TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
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

    private static string TaskPriorityLabel(OrganizerTask task)
    {
        return task.Priority switch
        {
            "Medium" or "2" => "Medium",
            "High" or "3" => "High",
            _ => "Low"
        };
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

    // Week start can be changed by the host (MainForm) to respect user preferences.
    [DefaultValue(DayOfWeek.Sunday)]
    public DayOfWeek WeekStarts { get; set; } = DayOfWeek.Sunday;

    private DateTime StartOfWeek(DateTime date)
    {
        int diff = (7 + (date.DayOfWeek - WeekStarts)) % 7;

        return date.Date.AddDays(-diff);
    }
}