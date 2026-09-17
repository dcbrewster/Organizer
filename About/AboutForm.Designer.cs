namespace Organizer.About;

partial class AboutForm
{
    private System.ComponentModel.IContainer? components = null;
    private Label lblApplication = null!;
    private Label lblVersion = null!;
    private Label lblBuild = null!;
    private CreditsControl creditsControl = null!;
    private LinkLabel LinkWebsite = null!;
    private LinkLabel LinkEmail = null!;
    private Button btnClose = null!;

    protected override void Dispose(bool disposing)
    {
        if(disposing && components is not null)         components.Dispose();

        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        lblApplication = new Label();
        lblVersion = new Label();
        lblBuild = new Label();
        creditsControl = new CreditsControl();
        LinkWebsite = new LinkLabel();
        LinkEmail = new LinkLabel();
        btnClose = new Button();
        SuspendLayout();
        // 
        // lblApplication
        // 
        lblApplication.AutoSize = true;
        lblApplication.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
        lblApplication.Location = new Point(16, 14);
        lblApplication.Name = "lblApplication";
        lblApplication.Size = new Size(100, 25);
        lblApplication.TabIndex = 0;
        lblApplication.Text = "Organizer";
        // 
        // lblVersion
        // 
        lblVersion.AutoSize = true;
        lblVersion.Location = new Point(18, 48);
        lblVersion.Name = "lblVersion";
        lblVersion.Size = new Size(45, 15);
        lblVersion.TabIndex = 1;
        lblVersion.Text = "Version";
        // 
        // lblBuild
        // 
        lblBuild.AutoSize = true;
        lblBuild.Location = new Point(18, 70);
        lblBuild.Name = "lblBuild";
        lblBuild.Size = new Size(31, 15);
        lblBuild.TabIndex = 2;
        lblBuild.Text = "Built";
        // 
        // creditsControl
        // 
        creditsControl.BackColor = Color.Black;
        creditsControl.ForeColor = Color.Lime;
        creditsControl.Location = new Point(18, 105);
        creditsControl.Name = "creditsControl";
        creditsControl.Size = new Size(300, 258);
        creditsControl.TabIndex = 3;
        // 
        // LinkWebsite
        // 
        LinkWebsite.AutoSize = true;
        LinkWebsite.Location = new Point(358, 102);
        LinkWebsite.Name = "LinkWebsite";
        LinkWebsite.Size = new Size(128, 15);
        LinkWebsite.TabIndex = 5;
        LinkWebsite.TabStop = true;
        LinkWebsite.Text = "forestcitysoftware.com";
        LinkWebsite.LinkClicked += LinkWebsite_LinkClicked;
        // 
        // LinkEmail
        // 
        LinkEmail.AutoSize = true;
        LinkEmail.Location = new Point(358, 132);
        LinkEmail.Name = "LinkEmail";
        LinkEmail.Size = new Size(180, 15);
        LinkEmail.TabIndex = 6;
        LinkEmail.TabStop = true;
        LinkEmail.Text = "support@forestcitysoftware.com";
        LinkEmail.LinkClicked += LinkEmail_LinkClicked;
        // 
        // btnClose
        // 
        btnClose.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
        btnClose.Location = new Point(463, 340);
        btnClose.Name = "btnClose";
        btnClose.Size = new Size(75, 23);
        btnClose.TabIndex = 7;
        btnClose.Text = "Close";
        btnClose.UseVisualStyleBackColor = true;
        btnClose.Click += btnClose_Click;
        // 
        // AboutForm
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(570, 388);
        Controls.Add(btnClose);
        Controls.Add(LinkEmail);
        Controls.Add(LinkWebsite);
        Controls.Add(creditsControl);
        Controls.Add(lblBuild);
        Controls.Add(lblVersion);
        Controls.Add(lblApplication);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "AboutForm";
        StartPosition = FormStartPosition.CenterParent;
        Text = "About Organizer";
        ResumeLayout(false);
        PerformLayout();
    }
}