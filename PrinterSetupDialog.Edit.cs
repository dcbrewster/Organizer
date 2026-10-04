namespace Organizer
{
    public static partial class PrinterSetupDialogExtensions
    {
        // Provides a lightweight factory that attempts to construct the dialog
        // with the caller-supplied preferences object, falling back to a
        // parameterless constructor if necessary. This avoids compile-time
        // coupling to a specific preferences type while restoring the
        // static Edit(...) entry point used throughout the codebase.
        public static bool Edit(IWin32Window owner, object? preferences)
        {
            Form? dlg;

            try
            {
                // Prefer a constructor that accepts the preferences object.
                dlg = Activator.CreateInstance(typeof(PrinterSetupDialog), [preferences]) as Form;
            }
            catch
            {
                dlg = null;
            }

            if(dlg is null)
            {
                // Fallback to a parameterless constructor.
                try
                {
                    dlg = Activator.CreateInstance(typeof(PrinterSetupDialog)) as Form;
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
}