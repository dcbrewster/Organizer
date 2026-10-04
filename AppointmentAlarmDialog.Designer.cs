namespace Organizer;

public sealed partial class AppointmentAlarmDialog
{
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer? components = null;
    private Panel designerBodyPanel = null!;
    private FlowLayoutPanel designerButtonPanel = null!;
    private DomainUpDown designerAmount = null!;
    private ComboBox designerUnit = null!;
    private DateTimePicker designerAlarmDate = null!;
    private DateTimePicker designerAlarmTime = null!;
    private ComboBox designerTune = null!;
    private TextBox designerMessage = null!;
    private TextBox designerRun = null!;
    private CheckBox designerDisplayDialog = null!;
    private RadioButton designerSetAlarm = null!;
    private RadioButton designerCancelAlarm = null!;
    private RadioButton designerBefore = null!;
    private RadioButton designerAfter = null!;
    private RadioButton designerOn = null!;
    private Label designerAppointmentTime = null!;
    private Button designerOkButton = null!;
    private Button designerCancelButton = null!;
    private Button designerHelpButton = null!;

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
        System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AppointmentAlarmDialog));
        designerBodyPanel = new Panel();
        designerAmount = new DomainUpDown();
        designerUnit = new ComboBox();
        designerAlarmDate = new DateTimePicker();
        designerAlarmTime = new DateTimePicker();
        tuneLabel = new Label();
        designerTune = new ComboBox();
        playButton = new Button();
        browseTuneButton = new Button();
        messageLabel = new Label();
        designerMessage = new TextBox();
        runLabel = new Label();
        designerRun = new TextBox();
        browseRunButton = new Button();
        designerDisplayDialog = new CheckBox();
        designerSetAlarm = new RadioButton();
        designerCancelAlarm = new RadioButton();
        designerBefore = new RadioButton();
        designerAfter = new RadioButton();
        designerOn = new RadioButton();
        designerAppointmentTime = new Label();
        designerButtonPanel = new FlowLayoutPanel();
        designerOkButton = new Button();
        designerCancelButton = new Button();
        designerHelpButton = new Button();
        designerBodyPanel.SuspendLayout();
        designerButtonPanel.SuspendLayout();
        SuspendLayout();
        // 
        // designerBodyPanel
        // 
        designerBodyPanel.Controls.Add(designerAmount);
        designerBodyPanel.Controls.Add(designerUnit);
        designerBodyPanel.Controls.Add(designerAlarmDate);
        designerBodyPanel.Controls.Add(designerAlarmTime);
        designerBodyPanel.Controls.Add(tuneLabel);
        designerBodyPanel.Controls.Add(designerTune);
        designerBodyPanel.Controls.Add(playButton);
        designerBodyPanel.Controls.Add(browseTuneButton);
        designerBodyPanel.Controls.Add(messageLabel);
        designerBodyPanel.Controls.Add(designerMessage);
        designerBodyPanel.Controls.Add(runLabel);
        designerBodyPanel.Controls.Add(designerRun);
        designerBodyPanel.Controls.Add(browseRunButton);
        designerBodyPanel.Controls.Add(designerDisplayDialog);
        designerBodyPanel.Controls.Add(designerSetAlarm);
        designerBodyPanel.Controls.Add(designerCancelAlarm);
        designerBodyPanel.Controls.Add(designerBefore);
        designerBodyPanel.Controls.Add(designerAfter);
        designerBodyPanel.Controls.Add(designerOn);
        designerBodyPanel.Controls.Add(designerAppointmentTime);
        designerBodyPanel.Dock = DockStyle.Fill;
        designerBodyPanel.Location = new Point(0, 0);
        designerBodyPanel.Name = "designerBodyPanel";
        designerBodyPanel.Padding = new Padding(12);
        designerBodyPanel.Size = new Size(420, 292);
        designerBodyPanel.TabIndex = 0;
        // 
        // designerAmount
        // 
        designerAmount.Location = new Point(16, 14);
        designerAmount.Name = "designerAmount";
        designerAmount.ReadOnly = true;
        designerAmount.Size = new Size(58, 23);
        designerAmount.TabIndex = 0;
        designerAmount.TextAlign = HorizontalAlignment.Right;
        // 
        // designerUnit
        // 
        designerUnit.DropDownStyle = ComboBoxStyle.DropDownList;
        designerUnit.Items.AddRange(new object[] { "Minutes", "Hours", "Days" });
        designerUnit.Location = new Point(82, 14);
        designerUnit.Name = "designerUnit";
        designerUnit.Size = new Size(86, 23);
        designerUnit.TabIndex = 1;
        // 
        // designerAlarmDate
        // 
        designerAlarmDate.Format = DateTimePickerFormat.Short;
        designerAlarmDate.Location = new Point(180, 14);
        designerAlarmDate.Name = "designerAlarmDate";
        designerAlarmDate.Size = new Size(96, 23);
        designerAlarmDate.TabIndex = 2;
        // 
        // designerAlarmTime
        // 
        designerAlarmTime.CustomFormat = "h:mm tt";
        designerAlarmTime.Format = DateTimePickerFormat.Custom;
        designerAlarmTime.Location = new Point(286, 14);
        designerAlarmTime.Name = "designerAlarmTime";
        designerAlarmTime.ShowUpDown = true;
        designerAlarmTime.Size = new Size(78, 23);
        designerAlarmTime.TabIndex = 3;
        // 
        // tuneLabel
        // 
        tuneLabel.Location = new Point(0, 0);
        tuneLabel.Name = "tuneLabel";
        tuneLabel.Size = new Size(100, 23);
        tuneLabel.TabIndex = 4;
        // 
        // designerTune
        // 
        designerTune.Items.AddRange(new object[] { "Default", "Chime", "Ding", "Notify" });
        designerTune.Location = new Point(82, 45);
        designerTune.Name = "designerTune";
        designerTune.Size = new Size(170, 23);
        designerTune.TabIndex = 5;
        // 
        // playButton
        // 
        playButton.Location = new Point(0, 0);
        playButton.Name = "playButton";
        playButton.Size = new Size(75, 23);
        playButton.TabIndex = 6;
        // 
        // browseTuneButton
        // 
        browseTuneButton.Location = new Point(0, 0);
        browseTuneButton.Name = "browseTuneButton";
        browseTuneButton.Size = new Size(75, 23);
        browseTuneButton.TabIndex = 7;
        // 
        // messageLabel
        // 
        messageLabel.Location = new Point(0, 0);
        messageLabel.Name = "messageLabel";
        messageLabel.Size = new Size(100, 23);
        messageLabel.TabIndex = 8;
        // 
        // designerMessage
        // 
        designerMessage.Location = new Point(82, 80);
        designerMessage.Name = "designerMessage";
        designerMessage.Size = new Size(245, 23);
        designerMessage.TabIndex = 9;
        // 
        // runLabel
        // 
        runLabel.Location = new Point(0, 0);
        runLabel.Name = "runLabel";
        runLabel.Size = new Size(100, 23);
        runLabel.TabIndex = 10;
        // 
        // designerRun
        // 
        designerRun.Location = new Point(82, 115);
        designerRun.Name = "designerRun";
        designerRun.Size = new Size(245, 23);
        designerRun.TabIndex = 11;
        // 
        // browseRunButton
        // 
        browseRunButton.Location = new Point(0, 0);
        browseRunButton.Name = "browseRunButton";
        browseRunButton.Size = new Size(75, 23);
        browseRunButton.TabIndex = 12;
        // 
        // designerDisplayDialog
        // 
        designerDisplayDialog.AutoSize = true;
        designerDisplayDialog.Location = new Point(82, 147);
        designerDisplayDialog.Name = "designerDisplayDialog";
        designerDisplayDialog.Size = new Size(195, 19);
        designerDisplayDialog.TabIndex = 13;
        designerDisplayDialog.Text = "D&isplay dialog box at alarm time";
        // 
        // designerSetAlarm
        // 
        designerSetAlarm.AutoSize = true;
        designerSetAlarm.Location = new Point(16, 190);
        designerSetAlarm.Name = "designerSetAlarm";
        designerSetAlarm.Size = new Size(76, 19);
        designerSetAlarm.TabIndex = 14;
        designerSetAlarm.Text = "Set A&larm";
        // 
        // designerCancelAlarm
        // 
        designerCancelAlarm.AutoSize = true;
        designerCancelAlarm.Location = new Point(120, 190);
        designerCancelAlarm.Name = "designerCancelAlarm";
        designerCancelAlarm.Size = new Size(96, 19);
        designerCancelAlarm.TabIndex = 15;
        designerCancelAlarm.Text = "&Cancel Alarm";
        // 
        // designerBefore
        // 
        designerBefore.AutoSize = true;
        designerBefore.Location = new Point(260, 190);
        designerBefore.Name = "designerBefore";
        designerBefore.Size = new Size(59, 19);
        designerBefore.TabIndex = 16;
        designerBefore.Text = "B&efore";
        // 
        // designerAfter
        // 
        designerAfter.AutoSize = true;
        designerAfter.Location = new Point(330, 190);
        designerAfter.Name = "designerAfter";
        designerAfter.Size = new Size(51, 19);
        designerAfter.TabIndex = 17;
        designerAfter.Text = "&After";
        // 
        // designerOn
        // 
        designerOn.AutoSize = true;
        designerOn.Location = new Point(392, 190);
        designerOn.Name = "designerOn";
        designerOn.Size = new Size(41, 19);
        designerOn.TabIndex = 18;
        designerOn.Text = "&On";
        // 
        // designerAppointmentTime
        // 
        designerAppointmentTime.AutoSize = true;
        designerAppointmentTime.Location = new Point(16, 218);
        designerAppointmentTime.Name = "designerAppointmentTime";
        designerAppointmentTime.Size = new Size(105, 15);
        designerAppointmentTime.TabIndex = 19;
        designerAppointmentTime.Text = "Appointment time";
        // 
        // designerButtonPanel
        // 
        designerButtonPanel.Controls.Add(designerOkButton);
        designerButtonPanel.Controls.Add(designerCancelButton);
        designerButtonPanel.Controls.Add(designerHelpButton);
        designerButtonPanel.Dock = DockStyle.Right;
        designerButtonPanel.FlowDirection = FlowDirection.TopDown;
        designerButtonPanel.Location = new Point(420, 0);
        designerButtonPanel.Name = "designerButtonPanel";
        designerButtonPanel.Padding = new Padding(8, 12, 8, 8);
        designerButtonPanel.Size = new Size(100, 292);
        designerButtonPanel.TabIndex = 1;
        designerButtonPanel.WrapContents = false;
        // 
        // designerOkButton
        // 
        designerOkButton.Location = new Point(11, 15);
        designerOkButton.Name = "designerOkButton";
        designerOkButton.Size = new Size(82, 23);
        designerOkButton.TabIndex = 0;
        designerOkButton.Text = "OK";
        // 
        // designerCancelButton
        // 
        designerCancelButton.Location = new Point(11, 44);
        designerCancelButton.Name = "designerCancelButton";
        designerCancelButton.Size = new Size(82, 23);
        designerCancelButton.TabIndex = 1;
        designerCancelButton.Text = "Cancel";
        // 
        // designerHelpButton
        // 
        designerHelpButton.Location = new Point(11, 73);
        designerHelpButton.Name = "designerHelpButton";
        designerHelpButton.Size = new Size(82, 23);
        designerHelpButton.TabIndex = 2;
        designerHelpButton.Text = "&Help";
        // 
        // AppointmentAlarmDialog
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(520, 292);
        Controls.Add(designerBodyPanel);
        Controls.Add(designerButtonPanel);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        Icon = (Icon)resources.GetObject("$this.Icon");
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "AppointmentAlarmDialog";
        Text = "Alarm";
        designerBodyPanel.ResumeLayout(false);
        designerBodyPanel.PerformLayout();
        designerButtonPanel.ResumeLayout(false);
        ResumeLayout(false);
    }

    #endregion

    private Label tuneLabel;
    private Button playButton;
    private Button browseTuneButton;
    private Label messageLabel;
    private Label runLabel;
    private Button browseRunButton;
}
