namespace Organizer
{
    public static partial class RecordEditorDialogExtensions
    {
        // Generic Edit wrapper used by MainForm to edit different record types.
        public static bool Edit(IWin32Window owner, object record, string title)
        {
            // Prefer ContactEditorDialog for contact-like records
            if(record is not null)
            {
                Type t = record.GetType();
                bool looksLikeContact = t.Name.IndexOf("Contact", StringComparison.OrdinalIgnoreCase) >= 0
                    || (t.GetProperty("Name") is not null && t.GetProperty("Email") is not null);

                if(looksLikeContact)
                {
                    using var dlg = new ContactEditorDialog(record, title ?? "Edit Contact");

                    return dlg.ShowDialog(owner) == DialogResult.OK;
                }
            }

            // Fallback to existing behavior
            Form? dlgFallback;

            try { dlgFallback = Activator.CreateInstance(typeof(RecordEditorDialog), [record, title]) as Form; } catch { dlgFallback = null; }

            if(dlgFallback is null) try { dlgFallback = Activator.CreateInstance(typeof(RecordEditorDialog), [record]) as Form; } catch { dlgFallback = null; }
            if(dlgFallback is null) try { dlgFallback = Activator.CreateInstance(typeof(RecordEditorDialog)) as Form; } catch { dlgFallback = null; }

            if(dlgFallback is null)
            {
                MessageBox.Show(owner, "Record editor dialog is unavailable.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                return false;
            }

            using(dlgFallback) { return dlgFallback.ShowDialog(owner) == DialogResult.OK; }
        }
    }
}