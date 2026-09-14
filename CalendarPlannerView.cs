using System.ComponentModel;

namespace Organizer;

internal sealed class CalendarPlannerView : Control
{
    private const int TimeGutter = 0;
    private const int DayHeaderHeight = 28;
    private readonly Dictionary<Rectangle, CalendarEvent> _eventBounds = [];
    private readonly Dictionary<Rectangle, OrganizerTask> _taskBounds = [];
    private readonly Font _smallFont;
    private readonly Font _boldFont;
    private CalendarEvent? _selectedEvent;
    private OrganizerTask? _selectedTask;

    public CalendarPlannerView()
    {
        DoubleBuffered = true;
        BackColor = Color.FromArgb(255, 252, 237);
        ForeColor = Color.FromArgb(30, 30, 30);
        _smallFont = new Font(Font.FontFamily, 8.5f);
        _boldFont = new Font(Font, FontStyle.Bold);
        SetStyle(ControlStyles.ResizeRedraw, true);
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public DateTime SelectedDate { get; set; } = DateTime.Today;

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public CalendarViewMode ViewMode { get; set; } = CalendarViewMode.Day;

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public IReadOnlyList<CalendarEvent> Events { get; set; } = [];

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public IReadOnlyList<OrganizerTask> Tasks { get; set; } = [];

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
        SelectCalendarItemAt(e.Location);
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
        using SolidBrush? headerBrush = new (Color.FromArgb(244, 222, 163));
        using SolidBrush? paperBrush = new (Color.FromArgb(255, 253, 239));

        Rectangle page = ClientRectangle;
        page.Inflate(-10, -10);

        if(page.Width <= 0 || page.Height <= 0) return;

        e.Graphics.FillRectangle(paperBrush, page);
        e.Graphics.DrawRectangle(borderPen, page);

        Rectangle content = Rectangle.Inflate(page, -18, -16);

        switch(ViewMode)
        {
            case CalendarViewMode.Week:
                DrawWeek(e.Graphics, content, headerBrush, borderPen, lightPen);
                break;

            case CalendarViewMode.Month:
                DrawMonth(e.Graphics, content, headerBrush, borderPen, lightPen);
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
        DateTime[]? days = Enumerable.Range(0, 7).Select(dayOffset => start.AddDays(dayOffset)).ToArray();
        Rectangle title = new(bounds.Left, bounds.Top, bounds.Width, 32);

        graphics.FillRectangle(headerBrush, title);
        graphics.DrawRectangle(borderPen, title);
        TextRenderer.DrawText(graphics, $"Week of {start:MMMM d, yyyy}", _boldFont, title, ForeColor, TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter);

        Rectangle body = new(bounds.Left, title.Bottom, bounds.Width, bounds.Height - title.Height);
        Rectangle timeGrid = new(body.Left, body.Top, body.Width, body.Height);

        DrawTimeGrid(graphics, timeGrid, 7, days, lightPen, borderPen);
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
        string[]? dayNames = ["Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat"];
        int colWidth = grid.Width / 7;

        for(int column = 0; column < 7; column++)
        {
            Rectangle header = new(grid.Left + column * colWidth, grid.Top, column == 6 ? grid.Right - (grid.Left + column * colWidth) : colWidth, dayHeaderHeight);

            graphics.FillRectangle(headerBrush, header);
            graphics.DrawRectangle(borderPen, header);
            TextRenderer.DrawText(graphics, dayNames[column], _boldFont, header, ForeColor, TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter);
        }

        DateTime first = new(SelectedDate.Year, SelectedDate.Month, 1);
        DateTime firstVisible = StartOfWeek(first);
        int cellTop = grid.Top + dayHeaderHeight;
        int rowHeight = Math.Max(1, (grid.Height - dayHeaderHeight) / 6);

        for(int row = 0; row < 6; row++)
        {
            for(int column = 0; column < 7; column++)
            {
                DateTime day = firstVisible.AddDays(row * 7 + column);
                int x = grid.Left + column * colWidth;
                Rectangle cell = new (x, cellTop + row * rowHeight, column == 6 ? grid.Right - x : colWidth, row == 5 ? grid.Bottom - (cellTop + row * rowHeight) : rowHeight);
                bool inMonth = day.Month == SelectedDate.Month;

                using SolidBrush? cellBrush = new (day.Date == SelectedDate.Date ? Color.FromArgb(255, 241, 180) : inMonth ? Color.FromArgb(255, 253, 239) : Color.FromArgb(239, 232, 211));
                graphics.FillRectangle(cellBrush, cell);
                graphics.DrawRectangle(lightPen, cell);

                TextRenderer.DrawText(graphics, day.Day.ToString(), inMonth ? _boldFont : _smallFont, new Rectangle(cell.Left + 4, cell.Top + 3, cell.Width - 8, 18), inMonth ? ForeColor : Color.Gray, TextFormatFlags.Left);

                List<OrganizerTask>? tasks = Tasks.Where(task => TaskOccursOnDate(task, day.Date)).OrderBy(TaskPriority).ThenBy(task => task.Completed).ThenBy(task => task.Title).Take(4).ToList();
                List<CalendarEvent>? events = Events.Where(calendarEvent => calendarEvent.Start.Date == day.Date).OrderBy(calendarEvent => calendarEvent.Start).Take(4 - tasks.Count).ToList();
                int y = cell.Top + 24;

                foreach(OrganizerTask task in tasks)
                {
                    Rectangle taskRect = new (cell.Left + 4, y, cell.Width - 8, 18);

                    DrawTaskBlock(graphics, taskRect, task);
                    y += 20;
                }

                foreach(var calendarEvent in events)
                {
                    Rectangle eventRect = new (cell.Left + 4, y, cell.Width - 8, 18);

                    DrawEventBlock(graphics, eventRect, calendarEvent, $"{calendarEvent.Start:h:mm} {calendarEvent.Title}");
                    y += 20;
                }
            }
        }

        graphics.DrawRectangle(borderPen, grid);
    }

    private void DrawTimeGrid(Graphics graphics, Rectangle body, int dayCount, DateTime[] days, Pen lightPen, Pen borderPen)
    {
        Rectangle timeline = new(body.Left, body.Top + DayHeaderHeight, body.Width, body.Height - DayHeaderHeight);
        int columnWidth = Math.Max(1, (body.Width - TimeGutter) / dayCount);

        graphics.DrawRectangle(borderPen, new Rectangle(body.Left, body.Top, body.Width - 1, DayHeaderHeight - 1));
        using SolidBrush? headerBrush = new(Color.FromArgb(248, 231, 183));
        graphics.FillRectangle(headerBrush, new Rectangle(body.Left, body.Top, body.Width, DayHeaderHeight));

        for(int dayIndex = 0; dayIndex < dayCount; dayIndex++)
        {
            int x = body.Left + TimeGutter + dayIndex * columnWidth;
            int width = dayIndex == dayCount - 1 ? body.Right - x : columnWidth;
            Rectangle header = new(x, body.Top, width, DayHeaderHeight);

            TextRenderer.DrawText(graphics, dayCount == 1 ? "Appointments" : days[dayIndex].ToString("ddd M/d"), _boldFont, header, ForeColor, TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter);
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
                Rectangle taskRect = new (x, y, Math.Max(20, width), height);

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
        using Pen? pencil = new (selected ? Color.White : Color.FromArgb(155, 111, 0), 2f);
        using Pen? outline = new (selected ? Color.FromArgb(230, 230, 230) : Color.FromArgb(72, 48, 24));
        graphics.DrawLine(pencil, bounds.Left + 2, bounds.Bottom - 3, bounds.Right - 3, bounds.Top + 2);
        graphics.DrawLine(outline, bounds.Left + 1, bounds.Bottom - 2, bounds.Right - 2, bounds.Top + 1);
        graphics.FillPolygon(
            selected ? Brushes.White : Brushes.Bisque,
            [new Point(bounds.Right - 3, bounds.Top + 2), new Point(bounds.Right, bounds.Top), new Point(bounds.Right - 1, bounds.Top + 4)]);
    }

    private void DrawTaskBlock(Graphics graphics, Rectangle bounds, OrganizerTask task)
    {
        if(bounds.Width <= 0 || bounds.Height <= 0) return;

        bool selected = ReferenceEquals(task, _selectedTask);
        using SolidBrush? fill = new (selected ? Color.FromArgb(91, 139, 74) : task.Completed ? Color.FromArgb(220, 226, 205) : Color.FromArgb(218, 238, 196));
        using Pen? border = new (selected ? Color.FromArgb(42, 89, 35) : Color.FromArgb(95, 137, 67));
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

    private static DateTime StartOfWeek(DateTime date)
    {
        int diff = (7 + (date.DayOfWeek - DayOfWeek.Sunday)) % 7;

        return date.Date.AddDays(-diff);
    }
}