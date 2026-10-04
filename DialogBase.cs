using System.ComponentModel;

namespace Organizer;

/// <summary>
/// Base class for modal dialogs in the application to ensure consistent appearance/behavior.
/// Dialogs deriving from this class will not show an icon or appear in the taskbar by default.
/// </summary>
public abstract class DialogBase : Form
{
    protected DialogBase()
    {
        // Standard modal dialog defaults
        ShowInTaskbar = false;
        ShowIcon = false;
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MinimizeBox = false;
        MaximizeBox = false;
    }
}