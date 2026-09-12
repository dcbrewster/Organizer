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
        components = new System.ComponentModel.Container();
        designerTabs = new TabControl();
        designerDefaultFilePage = new TabPage();
        designerEnvironmentPage = new TabPage();
        designerFoldersPage = new TabPage();
        designerAlarmsPage = new TabPage();
        designerWebBrowsingPage = new TabPage();
        designerButtonPanel = new FlowLayoutPanel();
        designerOkButton = new Button();
        designerCancelButton = new Button();
        designerHelpButton = new Button();

        designerDefaultFilePage.Text = "Default File";
        designerDefaultFilePage.Controls.Add(new Label { Text = "Default organizer file settings", AutoSize = true, Location = new Point(12, 16) });
        designerDefaultFilePage.Controls.Add(new CheckBox { Text = "&Automatically open", AutoSize = true, Location = new Point(12, 48) });
        designerDefaultFilePage.Controls.Add(new CheckBox { Text = "Always start with a &new Organizer file", AutoSize = true, Location = new Point(12, 80) });

        designerEnvironmentPage.Text = "Environment";
        designerEnvironmentPage.Controls.Add(new CheckBox { Text = "A&nimated page turn", AutoSize = true, Location = new Point(12, 16) });
        designerEnvironmentPage.Controls.Add(new Label { Text = "Mouse pointer", AutoSize = true, Location = new Point(12, 52) });

        designerFoldersPage.Text = "Folders";
        designerFoldersPage.Controls.Add(new Label { Text = "&Organizer files", AutoSize = true, Location = new Point(12, 16) });
        designerFoldersPage.Controls.Add(new TextBox { Location = new Point(12, 40), Width = 520 });

        designerAlarmsPage.Text = "Alarms";
        designerAlarmsPage.Controls.Add(new Label { Text = "&Favorite alarm tune:", AutoSize = true, Location = new Point(12, 16) });
        designerAlarmsPage.Controls.Add(new ComboBox { Location = new Point(12, 40), Width = 200, DropDownStyle = ComboBoxStyle.DropDownList });
        designerAlarmsPage.Controls.Add(new CheckBox { Text = "Displa&y missed alarms", AutoSize = true, Location = new Point(12, 76) });

        designerWebBrowsingPage.Text = "Web Browsing";
        designerWebBrowsingPage.Controls.Add(new Label { Text = "Web &browser", AutoSize = true, Location = new Point(12, 16) });
        designerWebBrowsingPage.Controls.Add(new ComboBox { Location = new Point(12, 40), Width = 240, DropDownStyle = ComboBoxStyle.DropDownList });
        designerWebBrowsingPage.Controls.Add(new CheckBox { Text = "&Connect to the Internet through a firewall", AutoSize = true, Location = new Point(12, 76) });

        designerTabs.Dock = DockStyle.Fill;
        designerTabs.Padding = new Point(12, 4);
        designerTabs.TabPages.AddRange(new TabPage[]
        {
            designerDefaultFilePage,
            designerEnvironmentPage,
            designerFoldersPage,
            designerAlarmsPage,
            designerWebBrowsingPage
        });

        designerOkButton.Text = "OK";
        designerOkButton.Width = 90;
        designerCancelButton.Text = "Cancel";
        designerCancelButton.Width = 90;
        designerHelpButton.Text = "&Help";
        designerHelpButton.Width = 90;

        designerButtonPanel.Dock = DockStyle.Bottom;
        designerButtonPanel.Height = 48;
        designerButtonPanel.Padding = new Padding(8);
        designerButtonPanel.FlowDirection = FlowDirection.RightToLeft;
        designerButtonPanel.Controls.Add(designerHelpButton);
        designerButtonPanel.Controls.Add(designerCancelButton);
        designerButtonPanel.Controls.Add(designerOkButton);

        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(680, 560);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        Text = "Organizer Preferences";
        Controls.Add(designerTabs);
        Controls.Add(designerButtonPanel);
    }

    #endregion
}