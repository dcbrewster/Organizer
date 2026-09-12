using System.ComponentModel;

namespace Organizer;

public sealed partial class CreateAppointmentDialog
{
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private IContainer? components = null;
    private TableLayoutPanel designerBody = null!;
    private FlowLayoutPanel designerButtonPanel = null!;
    private DateTimePicker designerDate = null!;
    private DateTimePicker designerTime = null!;
    private DomainUpDown designerDuration = null!;
    private TextBox designerDescription = null!;
    private ComboBox designerCategories = null!;
    private CheckBox designerWarnOfConflicts = null!;
    private CheckBox designerPencilIn = null!;
    private CheckBox designerConfidential = null!;
    private Button designerOkButton = null!;
    private Button designerCancelButton = null!;
    private Button designerInviteButton = null!;
    private Button designerFindTimeButton = null!;
    private Button designerAlarmButton = null!;
    private Button designerRepeatButton = null!;
    private Button designerCostButton = null!;
    private Button designerHelpButton = null!;

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
        designerBody = new TableLayoutPanel();
        schedulePanel = new FlowLayoutPanel();
        designerDate = new DateTimePicker();
        designerTime = new DateTimePicker();
        designerDuration = new DomainUpDown();
        designerDescription = new TextBox();
        designerCategories = new ComboBox();
        flags = new FlowLayoutPanel();
        designerWarnOfConflicts = new CheckBox();
        designerPencilIn = new CheckBox();
        designerConfidential = new CheckBox();
        linkButton = new Button();
        designerButtonPanel = new FlowLayoutPanel();
        designerOkButton = new Button();
        designerCancelButton = new Button();
        designerInviteButton = new Button();
        designerFindTimeButton = new Button();
        designerAlarmButton = new Button();
        designerRepeatButton = new Button();
        designerCostButton = new Button();
        designerHelpButton = new Button();
        designerBody.SuspendLayout();
        schedulePanel.SuspendLayout();
        flags.SuspendLayout();
        designerButtonPanel.SuspendLayout();
        SuspendLayout();
        // 
        // designerBody
        // 
        designerBody.AutoSize = true;
        designerBody.ColumnCount = 2;
        designerBody.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 95F));
        designerBody.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        designerBody.Controls.Add(schedulePanel, 0, 0);
        designerBody.Controls.Add(designerDescription, 1, 1);
        designerBody.Controls.Add(designerCategories, 1, 2);
        designerBody.Controls.Add(flags, 1, 3);
        designerBody.Controls.Add(linkButton, 1, 4);
        designerBody.Dock = DockStyle.Fill;
        designerBody.Location = new Point(0, 0);
        designerBody.Name = "designerBody";
        designerBody.Padding = new Padding(12);
        designerBody.RowCount = 5;
        designerBody.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
        designerBody.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
        designerBody.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
        designerBody.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
        designerBody.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
        designerBody.Size = new Size(456, 330);
        designerBody.TabIndex = 0;
        // 
        // schedulePanel
        // 
        designerBody.SetColumnSpan(schedulePanel, 2);
        schedulePanel.Controls.Add(designerDate);
        schedulePanel.Controls.Add(designerTime);
        schedulePanel.Controls.Add(designerDuration);
        schedulePanel.Location = new Point(15, 15);
        schedulePanel.Name = "schedulePanel";
        schedulePanel.Size = new Size(200, 14);
        schedulePanel.TabIndex = 0;
        // 
        // designerDate
        // 
        designerDate.Format = DateTimePickerFormat.Short;
        designerDate.Location = new Point(3, 26);
        designerDate.Name = "designerDate";
        designerDate.Size = new Size(92, 23);
        designerDate.TabIndex = 1;
        // 
        // designerTime
        // 
        designerTime.CustomFormat = "h:mm tt";
        designerTime.Format = DateTimePickerFormat.Custom;
        designerTime.Location = new Point(109, 55);
        designerTime.Name = "designerTime";
        designerTime.ShowUpDown = true;
        designerTime.Size = new Size(74, 23);
        designerTime.TabIndex = 3;
        // 
        // designerDuration
        // 
        designerDuration.Location = new Point(109, 84);
        designerDuration.Name = "designerDuration";
        designerDuration.ReadOnly = true;
        designerDuration.Size = new Size(54, 23);
        designerDuration.TabIndex = 5;
        designerDuration.TextAlign = HorizontalAlignment.Right;
        // 
        // designerDescription
        // 
        designerDescription.Location = new Point(110, 35);
        designerDescription.Multiline = true;
        designerDescription.Name = "designerDescription";
        designerDescription.ScrollBars = ScrollBars.Vertical;
        designerDescription.Size = new Size(331, 14);
        designerDescription.TabIndex = 2;
        // 
        // designerCategories
        // 
        designerCategories.Items.AddRange(new object[] { "Business", "Personal", "Holiday", "Travel", "Phone Call", "Meeting" });
        designerCategories.Location = new Point(110, 55);
        designerCategories.Name = "designerCategories";
        designerCategories.Size = new Size(220, 23);
        designerCategories.TabIndex = 4;
        // 
        // flags
        // 
        flags.Controls.Add(designerWarnOfConflicts);
        flags.Controls.Add(designerPencilIn);
        flags.Controls.Add(designerConfidential);
        flags.Location = new Point(110, 75);
        flags.Name = "flags";
        flags.Size = new Size(200, 14);
        flags.TabIndex = 5;
        // 
        // designerWarnOfConflicts
        // 
        designerWarnOfConflicts.AutoSize = true;
        designerWarnOfConflicts.Location = new Point(3, 3);
        designerWarnOfConflicts.Name = "designerWarnOfConflicts";
        designerWarnOfConflicts.Size = new Size(116, 19);
        designerWarnOfConflicts.TabIndex = 0;
        designerWarnOfConflicts.Text = "&Warn of conflicts";
        // 
        // designerPencilIn
        // 
        designerPencilIn.AutoSize = true;
        designerPencilIn.Location = new Point(125, 3);
        designerPencilIn.Name = "designerPencilIn";
        designerPencilIn.Size = new Size(71, 19);
        designerPencilIn.TabIndex = 1;
        designerPencilIn.Text = "&Pencil in";
        // 
        // designerConfidential
        // 
        designerConfidential.AutoSize = true;
        designerConfidential.Location = new Point(3, 28);
        designerConfidential.Name = "designerConfidential";
        designerConfidential.Size = new Size(91, 19);
        designerConfidential.TabIndex = 2;
        designerConfidential.Text = "Con&fidential";
        // 
        // linkButton
        // 
        linkButton.Location = new Point(110, 95);
        linkButton.Name = "linkButton";
        linkButton.Size = new Size(75, 23);
        linkButton.TabIndex = 6;
        // 
        // designerButtonPanel
        // 
        designerButtonPanel.Controls.Add(designerOkButton);
        designerButtonPanel.Controls.Add(designerCancelButton);
        designerButtonPanel.Controls.Add(designerInviteButton);
        designerButtonPanel.Controls.Add(designerFindTimeButton);
        designerButtonPanel.Controls.Add(designerAlarmButton);
        designerButtonPanel.Controls.Add(designerRepeatButton);
        designerButtonPanel.Controls.Add(designerCostButton);
        designerButtonPanel.Controls.Add(designerHelpButton);
        designerButtonPanel.Dock = DockStyle.Right;
        designerButtonPanel.FlowDirection = FlowDirection.TopDown;
        designerButtonPanel.Location = new Point(456, 0);
        designerButtonPanel.Name = "designerButtonPanel";
        designerButtonPanel.Padding = new Padding(8, 12, 8, 8);
        designerButtonPanel.Size = new Size(104, 330);
        designerButtonPanel.TabIndex = 1;
        designerButtonPanel.WrapContents = false;
        // 
        // designerOkButton
        // 
        designerOkButton.Location = new Point(11, 15);
        designerOkButton.Name = "designerOkButton";
        designerOkButton.Size = new Size(78, 23);
        designerOkButton.TabIndex = 0;
        designerOkButton.Text = "OK";
        // 
        // designerCancelButton
        // 
        designerCancelButton.Location = new Point(11, 44);
        designerCancelButton.Name = "designerCancelButton";
        designerCancelButton.Size = new Size(78, 23);
        designerCancelButton.TabIndex = 1;
        designerCancelButton.Text = "Cancel";
        // 
        // designerInviteButton
        // 
        designerInviteButton.Location = new Point(11, 179);
        designerInviteButton.Name = "designerInviteButton";
        designerInviteButton.Size = new Size(82, 23);
        designerInviteButton.TabIndex = 3;
        designerInviteButton.Text = "&Invite...";
        // 
        // designerFindTimeButton
        // 
        designerFindTimeButton.Location = new Point(11, 208);
        designerFindTimeButton.Name = "designerFindTimeButton";
        designerFindTimeButton.Size = new Size(82, 23);
        designerFindTimeButton.TabIndex = 4;
        designerFindTimeButton.Text = "Fi&nd Time";
        // 
        // designerAlarmButton
        // 
        designerAlarmButton.Location = new Point(11, 237);
        designerAlarmButton.Name = "designerAlarmButton";
        designerAlarmButton.Size = new Size(82, 23);
        designerAlarmButton.TabIndex = 5;
        designerAlarmButton.Text = "A&larm...";
        // 
        // designerRepeatButton
        // 
        designerRepeatButton.Location = new Point(11, 266);
        designerRepeatButton.Name = "designerRepeatButton";
        designerRepeatButton.Size = new Size(82, 23);
        designerRepeatButton.TabIndex = 6;
        designerRepeatButton.Text = "&Repeat...";
        // 
        // designerCostButton
        // 
        designerCostButton.Location = new Point(11, 295);
        designerCostButton.Name = "designerCostButton";
        designerCostButton.Size = new Size(82, 23);
        designerCostButton.TabIndex = 7;
        designerCostButton.Text = "C&ost...";
        // 
        // designerHelpButton
        // 
        designerHelpButton.Location = new Point(11, 324);
        designerHelpButton.Name = "designerHelpButton";
        designerHelpButton.Size = new Size(82, 23);
        designerHelpButton.TabIndex = 8;
        designerHelpButton.Text = "&Help";
        // 
        // CreateAppointmentDialog
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(560, 330);
        Controls.Add(designerBody);
        Controls.Add(designerButtonPanel);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "CreateAppointmentDialog";
        Text = "Create Appointment";
        designerBody.ResumeLayout(false);
        designerBody.PerformLayout();
        schedulePanel.ResumeLayout(false);
        flags.ResumeLayout(false);
        flags.PerformLayout();
        designerButtonPanel.ResumeLayout(false);
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private FlowLayoutPanel schedulePanel;
    private FlowLayoutPanel flags;
    private Button linkButton;
}