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
    private Panel designerCalendarLeftPanel = null!;
    private FlowLayoutPanel designerNavigationPanel = null!;
    private Button designerPreviousButton = null!;
    private Button designerNextButton = null!;
    private MonthCalendar designerMonthCalendar = null!;
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
        System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
        designerMenuStrip = new MenuStrip();
        designerToolStrip = new ToolStrip();
        designerTabs = new TabControl();
        designerCalendarTab = new TabPage();
        designerTrashPanel = new Panel();
        designerTrashLabel = new Label();
        designerAnniversaryTab = new TabPage();
        designerContactsTab = new TabPage();
        designerNotepadTab = new TabPage();
        designerCalendarLeftPanel = new Panel();
        designerNavigationPanel = new FlowLayoutPanel();
        designerPreviousButton = new Button();
        designerNextButton = new Button();
        designerMonthCalendar = new MonthCalendar();
        designerTabs.SuspendLayout();
        designerCalendarTab.SuspendLayout();
        designerNavigationPanel.SuspendLayout();
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
        designerTabs.Size = new Size(1100, 651);
        designerTabs.TabIndex = 0;
        // 
        // designerCalendarTab
        // 
        designerCalendarTab.Controls.Add(designerTrashPanel);
        designerCalendarTab.Controls.Add(designerTrashLabel);
        designerCalendarTab.Location = new Point(4, 4);
        designerCalendarTab.Name = "designerCalendarTab";
        designerCalendarTab.Size = new Size(1069, 643);
        designerCalendarTab.TabIndex = 0;
        designerCalendarTab.Text = "Calendar";
        // 
        // designerTrashPanel
        // 
        designerTrashPanel.Location = new Point(1010, 560);
        designerTrashPanel.Name = "designerTrashPanel";
        designerTrashPanel.Size = new Size(48, 48);
        designerTrashPanel.TabIndex = 4;
        // 
        // designerTrashLabel
        // 
        designerTrashLabel.BorderStyle = BorderStyle.FixedSingle;
        designerTrashLabel.Dock = DockStyle.Left;
        designerTrashLabel.Location = new Point(0, 0);
        designerTrashLabel.Name = "designerTrashLabel";
        designerTrashLabel.Size = new Size(51, 643);
        designerTrashLabel.TabIndex = 1;
        designerTrashLabel.Text = "🗑";
        designerTrashLabel.TextAlign = ContentAlignment.MiddleCenter;
        // 
        // designerAnniversaryTab
        // 
        designerAnniversaryTab.Location = new Point(4, 4);
        designerAnniversaryTab.Name = "designerAnniversaryTab";
        designerAnniversaryTab.Size = new Size(1069, 643);
        designerAnniversaryTab.TabIndex = 1;
        designerAnniversaryTab.Text = "Anniversary";
        // 
        // designerContactsTab
        // 
        designerContactsTab.Location = new Point(4, 4);
        designerContactsTab.Name = "designerContactsTab";
        designerContactsTab.Size = new Size(1069, 643);
        designerContactsTab.TabIndex = 2;
        designerContactsTab.Text = "Contacts";
        // 
        // designerNotepadTab
        // 
        designerNotepadTab.Location = new Point(4, 4);
        designerNotepadTab.Name = "designerNotepadTab";
        designerNotepadTab.Size = new Size(1069, 643);
        designerNotepadTab.TabIndex = 3;
        designerNotepadTab.Text = "Notepad";
        // 
        // designerCalendarLeftPanel
        // 
        designerCalendarLeftPanel.Dock = DockStyle.Left;
        designerCalendarLeftPanel.Location = new Point(0, 49);
        designerCalendarLeftPanel.Name = "designerCalendarLeftPanel";
        designerCalendarLeftPanel.Size = new Size(200, 651);
        designerCalendarLeftPanel.TabIndex = 0;
        designerCalendarLeftPanel.Paint += designerCalendarLeftPanel_Paint;
        // 
        // designerNavigationPanel
        // Make the navigation strip autosize horizontally and anchor to left/right so buttons stay aligned
        designerNavigationPanel.Controls.Add(designerPreviousButton);
        designerNavigationPanel.Controls.Add(designerNextButton);
        designerNavigationPanel.Dock = DockStyle.Top;
        designerNavigationPanel.Location = new Point(0, 170);
        designerNavigationPanel.Name = "designerNavigationPanel";
        designerNavigationPanel.AutoSize = true;
        designerNavigationPanel.AutoSizeMode = AutoSizeMode.GrowOnly;
        designerNavigationPanel.Padding = Padding.Empty;
        designerNavigationPanel.TabIndex = 1;
        // 
        // designerPreviousButton
        // 
        designerPreviousButton.Location = new Point(8, 3);
        designerPreviousButton.Name = "designerPreviousButton";
        designerPreviousButton.Size = new Size(44, 23);
        designerPreviousButton.TabIndex = 0;
        designerPreviousButton.Text = "<";
        // 
        // designerNextButton
        // 
        designerNextButton.Location = new Point(58, 3);
        designerNextButton.Name = "designerNextButton";
        designerNextButton.Size = new Size(44, 23);
        designerNextButton.TabIndex = 1;
        designerNextButton.Text = ">";
        // 
        // designerMonthCalendar
        // Make the MonthCalendar fill the left binder panel width so it never leaves a right inset
        designerMonthCalendar.Dock = DockStyle.Top;
        designerMonthCalendar.Location = new Point(0, 8);
        designerMonthCalendar.MaxSelectionCount = 1;
        designerMonthCalendar.Name = "designerMonthCalendar";
        designerMonthCalendar.TabIndex = 0;
        designerMonthCalendar.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        // 
        // MainForm
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1100, 700);
        Controls.Add(designerCalendarLeftPanel);
        Controls.Add(designerTabs);
        Controls.Add(designerToolStrip);
        Controls.Add(designerMenuStrip);
        Icon = (Icon)resources.GetObject("$this.Icon");
        MainMenuStrip = designerMenuStrip;
        Name = "MainForm";
        Text = "Organizer";
        designerTabs.ResumeLayout(false);
        designerCalendarTab.ResumeLayout(false);
        designerNavigationPanel.ResumeLayout(false);
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion
}