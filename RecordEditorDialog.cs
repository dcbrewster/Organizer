namespace Organizer;

internal sealed partial class RecordEditorDialog : Form
{
    public RecordEditorDialog()
    {
        InitializeComponent();
    }

    public RecordEditorDialog(object record) : this()
    {
        // The dialog can accept the record and set up fields in its own implementation.
    }

    public RecordEditorDialog(object record, string title) : this(record)
    {
        Text = title;
    }

    // Provide the static Edit entry point expected by existing call sites.
    public static bool Edit(IWin32Window owner, object record, string title)
    {
        // If the record appears to be a Contact (by type name or by having common properties),
        // use the ContactEditorDialog which provides form fields.
        if(record is not null)
        {
            Type t = record.GetType();
            bool looksLikeContact = t.Name.IndexOf("Contact", StringComparison.OrdinalIgnoreCase) >= 0
                || (t.GetProperty("Name") is not null && t.GetProperty("Email") is not null);

            if(looksLikeContact)
            {
                using ContactEditorDialog dlg = new(record, title ?? "Edit Contact");

                return dlg.ShowDialog(owner) == DialogResult.OK;
            }
        }

        // Fallback to the generic RecordEditorDialog constructors
        RecordEditorDialog? dlg2 = null;

        try { dlg2 = Activator.CreateInstance(typeof(RecordEditorDialog), [record, title]) as RecordEditorDialog; } catch { dlg2 = null; }

        if(dlg2 is null) try { dlg2 = Activator.CreateInstance(typeof(RecordEditorDialog), [record]) as RecordEditorDialog; } catch { dlg2 = null; }
        if(dlg2 is null) try { dlg2 = Activator.CreateInstance(typeof(RecordEditorDialog)) as RecordEditorDialog; } catch { dlg2 = null; }

        if(dlg2 is null)
        {
            MessageBox.Show(owner, "Record editor dialog is unavailable.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

            return false;
        }

        using(dlg2) { return dlg2.ShowDialog(owner) == DialogResult.OK; }
    }
}