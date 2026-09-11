namespace Organizer;

public sealed partial class MainForm
{
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer? components = null;
    private MenuStrip designerMenuStrip = null!;
    private ToolStrip designerToolStrip = null!;
    private TabControl designerTabs = null!;
    private TabPage designerCalendarTab = null!;
    private TabPage designerAnniversaryTab = null!;
    private TabPage designerContactsTab = null!;
    private TabPage designerNotepadTab = null!;
    private Panel designerTrashPanel = null!;
    private Label designerTrashLabel = null!;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    protected override void Dispose(bool disposing)
    {
        if(disposing && (components is not null))
        {
            components.Dispose();
        }

        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        designerMenuStrip = new MenuStrip();
        designerToolStrip = new ToolStrip();
        designerTabs = new TabControl();
        designerCalendarTab = new TabPage();
        designerAnniversaryTab = new TabPage();
        designerContactsTab = new TabPage();
        designerNotepadTab = new TabPage();
        designerTrashPanel = new Panel();
        designerTrashLabel = new Label();
        designerTabs.SuspendLayout();
        designerTrashPanel.SuspendLayout();
        SuspendLayout();
        // 
        // designerMenuStrip
        // 
        designerMenuStrip.Location = new Point(0, 0);
        designerMenuStrip.Name = "designerMenuStrip";
        designerMenuStrip.Size = new Size(1100, 24);
        designerMenuStrip.TabIndex = 3;
        // 
        // designerToolStrip
        // 
        designerToolStrip.GripStyle = ToolStripGripStyle.Hidden;
        designerToolStrip.Location = new Point(0, 24);
        designerToolStrip.Name = "designerToolStrip";
        designerToolStrip.Size = new Size(1100, 25);
        designerToolStrip.TabIndex = 2;
        // 
        // designerTabs
        // 
        designerTabs.Alignment = TabAlignment.Right;
        designerTabs.Controls.Add(designerCalendarTab);
        designerTabs.Controls.Add(designerAnniversaryTab);
        designerTabs.Controls.Add(designerContactsTab);
        designerTabs.Controls.Add(designerNotepadTab);
        designerTabs.Dock = DockStyle.Fill;
        designerTabs.Location = new Point(0, 49);
        designerTabs.Multiline = true;
        designerTabs.Name = "designerTabs";
        designerTabs.SelectedIndex = 0;
        designerTabs.Size = new Size(1100, 593);
        designerTabs.TabIndex = 0;
        // 
        // designerCalendarTab
        // 
        designerCalendarTab.Location = new Point(4, 4);
        designerCalendarTab.Name = "designerCalendarTab";
        designerCalendarTab.Size = new Size(1069, 585);
        designerCalendarTab.TabIndex = 0;
        designerCalendarTab.Text = "Calendar";
        // 
        // designerAnniversaryTab
        // 
        designerAnniversaryTab.Location = new Point(4, 4);
        designerAnniversaryTab.Name = "designerAnniversaryTab";
        designerAnniversaryTab.Size = new Size(100, 92);
        designerAnniversaryTab.TabIndex = 1;
        designerAnniversaryTab.Text = "Anniversary";
        // 
        // designerContactsTab
        // 
        designerContactsTab.Location = new Point(4, 4);
        designerContactsTab.Name = "designerContactsTab";
        designerContactsTab.Size = new Size(100, 92);
        designerContactsTab.TabIndex = 2;
        designerContactsTab.Text = "Contacts";
        // 
        // designerNotepadTab
        // 
        designerNotepadTab.Location = new Point(4, 4);
        designerNotepadTab.Name = "designerNotepadTab";
        designerNotepadTab.Size = new Size(100, 92);
        designerNotepadTab.TabIndex = 3;
        designerNotepadTab.Text = "Notepad";
        // 
        // designerTrashPanel
        // 
        designerTrashPanel.BackColor = Color.FromArgb(178, 134, 61);
        designerTrashPanel.Controls.Add(designerTrashLabel);
        designerTrashPanel.Dock = DockStyle.Bottom;
        designerTrashPanel.Location = new Point(0, 642);
        designerTrashPanel.Name = "designerTrashPanel";
        designerTrashPanel.Padding = new Padding(10, 6, 0, 6);
        designerTrashPanel.Size = new Size(1100, 58);
        designerTrashPanel.TabIndex = 1;
        // 
        // designerTrashLabel
        // 
        designerTrashLabel.BorderStyle = BorderStyle.FixedSingle;
        designerTrashLabel.Dock = DockStyle.Left;
        designerTrashLabel.Location = new Point(10, 6);
        designerTrashLabel.Name = "designerTrashLabel";
        designerTrashLabel.Size = new Size(58, 46);
        designerTrashLabel.TabIndex = 0;
        designerTrashLabel.Text = "🗑";
        designerTrashLabel.TextAlign = ContentAlignment.MiddleCenter;
        // 
        // MainForm
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1100, 700);
        Controls.Add(designerTabs);
        Controls.Add(designerTrashPanel);
        Controls.Add(designerToolStrip);
        Controls.Add(designerMenuStrip);
        MainMenuStrip = designerMenuStrip;
        Name = "MainForm";
        Text = "Organizer";
        designerTabs.ResumeLayout(false);
        designerTrashPanel.ResumeLayout(false);
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion
}
