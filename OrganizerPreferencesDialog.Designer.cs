namespace Organizer;

public sealed partial class OrganizerPreferencesDialog
{
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer? components = null;
    private TabControl designerTabs = null!;
    private TabPage designerDefaultFilePage = null!;
    private TabPage designerEnvironmentPage = null!;
    private TabPage designerFoldersPage = null!;
    private TabPage designerAlarmsPage = null!;
    private TabPage designerWebBrowsingPage = null!;
    private FlowLayoutPanel designerButtonPanel = null!;
    private Button designerOkButton = null!;
    private Button designerCancelButton = null!;
    private Button designerHelpButton = null!;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    protected override void Dispose(bool disposing)
    {
        if(disposing && (components is not null))            components.Dispose();

        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OrganizerPreferencesDialog));
        designerTabs = new TabControl();
        designerDefaultFilePage = new TabPage();
        designerEnvironmentPage = new TabPage();
        designerFoldersPage = new TabPage();
        designerAlarmsPage = new TabPage();
        designerWebBrowsingPage = new TabPage();
        designerButtonPanel = new FlowLayoutPanel();
        designerHelpButton = new Button();
        designerCancelButton = new Button();
        designerOkButton = new Button();
        designerTabs.SuspendLayout();
        designerButtonPanel.SuspendLayout();
        SuspendLayout();
        // 
        // designerTabs
        // 
        designerTabs.Controls.Add(designerDefaultFilePage);
        designerTabs.Controls.Add(designerEnvironmentPage);
        designerTabs.Controls.Add(designerFoldersPage);
        designerTabs.Controls.Add(designerAlarmsPage);
        designerTabs.Controls.Add(designerWebBrowsingPage);
        designerTabs.Dock = DockStyle.Fill;
        designerTabs.Location = new Point(0, 0);
        designerTabs.Name = "designerTabs";
        designerTabs.Padding = new Point(12, 4);
        designerTabs.SelectedIndex = 0;
        designerTabs.Size = new Size(680, 512);
        designerTabs.TabIndex = 0;
        // 
        // designerDefaultFilePage
        // 
        designerDefaultFilePage.Location = new Point(4, 26);
        designerDefaultFilePage.Name = "designerDefaultFilePage";
        designerDefaultFilePage.Size = new Size(672, 482);
        designerDefaultFilePage.TabIndex = 0;
        designerDefaultFilePage.Text = "Default File";
        // 
        // designerEnvironmentPage
        // 
        designerEnvironmentPage.Location = new Point(4, 26);
        designerEnvironmentPage.Name = "designerEnvironmentPage";
        designerEnvironmentPage.Size = new Size(192, 70);
        designerEnvironmentPage.TabIndex = 1;
        designerEnvironmentPage.Text = "Environment";
        // 
        // designerFoldersPage
        // 
        designerFoldersPage.Location = new Point(4, 26);
        designerFoldersPage.Name = "designerFoldersPage";
        designerFoldersPage.Size = new Size(192, 70);
        designerFoldersPage.TabIndex = 2;
        designerFoldersPage.Text = "Folders";
        // 
        // designerAlarmsPage
        // 
        designerAlarmsPage.Location = new Point(4, 26);
        designerAlarmsPage.Name = "designerAlarmsPage";
        designerAlarmsPage.Size = new Size(192, 70);
        designerAlarmsPage.TabIndex = 3;
        designerAlarmsPage.Text = "Alarms";
        // 
        // designerWebBrowsingPage
        // 
        designerWebBrowsingPage.Location = new Point(4, 26);
        designerWebBrowsingPage.Name = "designerWebBrowsingPage";
        designerWebBrowsingPage.Size = new Size(192, 70);
        designerWebBrowsingPage.TabIndex = 4;
        designerWebBrowsingPage.Text = "Web Browsing";
        // 
        // designerButtonPanel
        // 
        designerButtonPanel.Controls.Add(designerHelpButton);
        designerButtonPanel.Controls.Add(designerCancelButton);
        designerButtonPanel.Controls.Add(designerOkButton);
        designerButtonPanel.Dock = DockStyle.Bottom;
        designerButtonPanel.FlowDirection = FlowDirection.RightToLeft;
        designerButtonPanel.Location = new Point(0, 512);
        designerButtonPanel.Name = "designerButtonPanel";
        designerButtonPanel.Padding = new Padding(8);
        designerButtonPanel.Size = new Size(680, 48);
        designerButtonPanel.TabIndex = 1;
        // 
        // designerHelpButton
        // 
        designerHelpButton.Location = new Point(571, 11);
        designerHelpButton.Name = "designerHelpButton";
        designerHelpButton.Size = new Size(90, 23);
        designerHelpButton.TabIndex = 0;
        designerHelpButton.Text = "&Help";
        // 
        // designerCancelButton
        // 
        designerCancelButton.Location = new Point(475, 11);
        designerCancelButton.Name = "designerCancelButton";
        designerCancelButton.Size = new Size(90, 23);
        designerCancelButton.TabIndex = 1;
        designerCancelButton.Text = "Cancel";
        // 
        // designerOkButton
        // 
        designerOkButton.Location = new Point(379, 11);
        designerOkButton.Name = "designerOkButton";
        designerOkButton.Size = new Size(90, 23);
        designerOkButton.TabIndex = 2;
        designerOkButton.Text = "OK";
        // 
        // OrganizerPreferencesDialog
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(680, 560);
        Controls.Add(designerTabs);
        Controls.Add(designerButtonPanel);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        Icon = (Icon)resources.GetObject("$this.Icon");
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "OrganizerPreferencesDialog";
        Text = "Organizer Preferences";
        designerTabs.ResumeLayout(false);
        designerButtonPanel.ResumeLayout(false);
        ResumeLayout(false);
    }

    #endregion
}