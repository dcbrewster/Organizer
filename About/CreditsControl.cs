using System.ComponentModel;
using System.Drawing.Drawing2D;

namespace Organizer.About;

/// <summary>
/// A custom control that displays scrolling credits, similar to movie credits, with optional perspective effect and fade masks.
/// </summary>
public class CreditsControl : Control
{
    private readonly System.Windows.Forms.Timer timer = new();  // Timer for controlling the scrolling speed of the credits
    private readonly List<string> lines = [];             // List of credit lines to be displayed
    private float offsetY;                                          // Current vertical offset for scrolling the credits

    [DefaultValue(1.0f)]                                      // Speed at which the credits scroll vertically
    public float ScrollSpeed { get; set; } = 1.0f;                  // Speed at which the credits scroll vertically

    [DefaultValue(70)]                                        // Height of the fade effect at the top and bottom of the control
    public int FadeHeight { get; set; } = 70;                       // Height of the fade effect at the top and bottom of the control

    [DefaultValue(false)]                                     // Indicates whether the Star Wars perspective effect is applied to the scrolling credits
    public bool StarWarsMode { get; set; }                          // If true, applies a perspective effect to the scrolling credits, similar to the Star Wars opening crawl

    /// <summary>
    /// Initializes a new instance of the <see cref="CreditsControl"/> class, setting up the control's styles, colors, and timer for scrolling credits.
    /// </summary>
    public CreditsControl()
    {
        // Enable double buffering and custom painting for smoother rendering
        SetStyle(
            ControlStyles.UserPaint |
            ControlStyles.AllPaintingInWmPaint |
            ControlStyles.OptimizedDoubleBuffer,
            true);

        DoubleBuffered = true;      // Enable double buffering to reduce flickering during rendering

        BackColor = Color.Black;    // Set the background color of the control to black
        ForeColor = Color.Lime;     // Set the foreground color (text color) of the control to lime green

        timer.Interval = 16;        // Set the timer interval to approximately 60 frames per second (16 milliseconds)
        timer.Tick += Timer_Tick;   // Subscribe to the timer's Tick event to handle scrolling updates
        timer.Start();              // Start the timer to begin scrolling the credits
    }

    /// <summary>
    /// Sets the credit lines to be displayed in the control, clearing any existing lines and resetting the vertical offset for scrolling.
    /// </summary>
    /// <param name="creditLines"></param>
    public void SetLines(IEnumerable<string> creditLines)
    {
        lines.Clear();                  // Clear any existing credit lines from the internal list
        lines.AddRange(creditLines);    // Add the provided credit lines to the internal list

        offsetY = Height;               // Reset the vertical offset to start the credits from the bottom of the control
        Invalidate();                   // Request a repaint of the control to reflect the updated credit lines
    }

    public void Pause() => timer.Stop();    // Pauses the scrolling of the credits by stopping the timer

    public void Resume() => timer.Start();  // Resumes the scrolling of the credits by starting the timer

    /// <summary>
    /// Handles the timer's Tick event, updating the vertical offset for scrolling the credits and invalidating the control to trigger a repaint.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event data.</param>
    private void Timer_Tick(object? sender, EventArgs e)
    {
        offsetY -= ScrollSpeed;                         // Update the vertical offset by subtracting the scroll speed, causing the credits to move upward

        float totalHeight = lines.Count * 32;           // Calculate the total height of all credit lines based on the number of lines and the line height (32 pixels)

        if(offsetY < -totalHeight) offsetY = Height;    // Reset the offset to the bottom of the control when the credits have scrolled past the top

        Invalidate();                                   // Request a repaint of the control to reflect the updated vertical offset for scrolling
    }

    /// <summary>
    /// Handles the painting of the control, drawing the scrolling credits with optional perspective effect and fade masks.
    /// </summary>
    /// <param name="e"></param>
    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);                                                                        // Call the base class's OnPaint method to ensure proper painting behavior
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;                                     // Set smoothing mode for better graphics quality
        e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;  // Set text rendering hint for better text quality

        using Font font = new("Segoe UI", 12, FontStyle.Bold);              // Create a font for rendering the credit lines
        using StringFormat sf = new() { Alignment = StringAlignment.Center };         // Create a StringFormat for centering the text horizontally

        float y = offsetY;                                                                      // Initialize the vertical position for drawing the credit lines based on the current offset

        // Iterate through each credit line and draw it on the control, applying perspective effect if StarWarsMode is enabled
        foreach(string line in lines)
        {
            // Draw the credit line with or without perspective effect based on the StarWarsMode property
            if(StarWarsMode)
            {
                // Draw the credit line with perspective effect, making it appear to recede into the distance
                DrawPerspectiveLine(e.Graphics, line, font, y, sf);
            }
            else
            {
                // Draw the credit line without perspective effect, centered horizontally
                e.Graphics.DrawString(line, font, Brushes.Lime, Width / 2, y, sf);
            }

            // Increment the vertical position for the next credit line, spacing them 32 pixels apart
            y += 32;
        }

        // Draw fade masks at the top and bottom of the control to create a smooth transition effect for the scrolling credits
        DrawFadeMask(e.Graphics);
    }

    /// <summary>
    /// Draws a line of text with a perspective effect, making it appear to recede into the distance.
    /// </summary>
    /// <param name="g"></param>
    /// <param name="text"></param>
    /// <param name="font"></param>
    /// <param name="y"></param>
    /// <param name="sf"></param>
    private void DrawPerspectiveLine(Graphics g, string text, Font font, float y, StringFormat sf)
    {
        float scale = Math.Max(0.25f, (Height - y) / Height);       // Calculate the scaling factor based on the vertical position, ensuring it doesn't go below 0.25 to maintain readability

        GraphicsState state = g.Save();                             // Save the current graphics state to restore it later after applying transformations

        g.TranslateTransform(Width / 2, y);                         // Move the origin to the center of the control horizontally and to the current vertical position for the credit line
        g.ScaleTransform(scale, scale);                             // Apply scaling to create the perspective effect, making the text appear smaller as it moves upward
        g.DrawString(text, font, Brushes.Lime, 0, 0, sf);     // Draw the text at the transformed position with scaling applied
        g.Restore(state);                                           // Restore the previous graphics state to remove the transformations and return to the original coordinate system
    }

    /// <summary>
    /// Draws fade masks at the top and bottom of the control to create a smooth transition effect for the scrolling credits.
    /// </summary>
    /// <param name="g"></param>
    private void DrawFadeMask(Graphics g)
    {
        // Draw a fade mask at the top of the control, transitioning from black to transparent
        Rectangle topRect = new(0, 0, Width, FadeHeight);

        // Create a linear gradient brush for the top fade effect, transitioning from black to transparent vertically
        using LinearGradientBrush top = new(topRect, Color.Black, Color.Transparent, LinearGradientMode.Vertical);

        // Fill the top rectangle with the gradient to create the fade effect
        g.FillRectangle(top, topRect);

        // Draw a fade mask at the bottom of the control, transitioning from transparent to black
        Rectangle bottomRect = new(0, Height - FadeHeight, Width, FadeHeight);

        // Create a linear gradient brush for the bottom fade effect, transitioning from transparent to black vertically
        using LinearGradientBrush bottom = new(bottomRect, Color.Transparent, Color.Black, LinearGradientMode.Vertical);

        // Fill the bottom rectangle with the gradient to create the fade effect
        g.FillRectangle(bottom, bottomRect);
    }
}