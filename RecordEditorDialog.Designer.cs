using System.ComponentModel;

namespace Organizer;

/// <summary>
/// Interaction logic for RecordEditorDialog.xaml
/// </summary>
internal sealed partial class RecordEditorDialog
{
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private IContainer? components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    protected override void Dispose(bool disposing)
    {
        if(disposing && (components is not null)) components.Dispose();

        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        ComponentResourceManager resources = new ComponentResourceManager(typeof(RecordEditorDialog));
        SuspendLayout();
        // 
        // RecordEditorDialog
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(284, 261);
        Icon = (Icon)resources.GetObject("$this.Icon");
        Name = "RecordEditorDialog";
        ResumeLayout(false);
    }

    #endregion
}