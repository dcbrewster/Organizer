namespace Organizer;

internal sealed partial class PrinterSetupDialog : Form
{
    public PrinterSetupDialog()
    {
        InitializeComponent();
    }

    // Optional convenience constructor used by reflective wrapper if a preferences object is available.
    public PrinterSetupDialog(object? preferences) : this()
    {
        // If the dialog needs preferences to initialize, reflection or casting can be used here by the dialog implementation.
    }

    // Provide the static Edit entry point expected by existing call sites.
    public static bool Edit(IWin32Window owner, object? preferences)
    {
        // Prefer constructor that accepts preferences, fall back to parameterless.
        PrinterSetupDialog? dlg = null;

        try
        {
            dlg = Activator.CreateInstance(typeof(PrinterSetupDialog), [preferences]) as PrinterSetupDialog;
        }
        catch
        {
            dlg = null;
        }

        if(dlg is null)
        {
            try
            {
                dlg = Activator.CreateInstance(typeof(PrinterSetupDialog)) as PrinterSetupDialog;
            }
            catch
            {
                dlg = null;
            }
        }

        if(dlg is null)
        {
            MessageBox.Show(owner, "Printer setup dialog is unavailable.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

            return false;
        }

        using(dlg)
        {
            return dlg.ShowDialog(owner) == DialogResult.OK;
        }
    }
}