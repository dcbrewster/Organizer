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
        components = new Container();
        AutoScaleMode = AutoScaleMode.Font;
    }

    #endregion
}