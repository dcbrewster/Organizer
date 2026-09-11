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
        components = new System.ComponentModel.Container();
        designerBodyPanel = new Panel();
        designerButtonPanel = new FlowLayoutPanel();
        designerAmount = new DomainUpDown();
        designerUnit = new ComboBox();
        designerAlarmDate = new DateTimePicker();
        designerAlarmTime = new DateTimePicker();
        designerTune = new ComboBox();
        designerMessage = new TextBox();
        designerRun = new TextBox();
        designerDisplayDialog = new CheckBox();
        designerSetAlarm = new RadioButton();
        designerCancelAlarm = new RadioButton();
        designerBefore = new RadioButton();
        designerAfter = new RadioButton();
        designerOn = new RadioButton();
        designerAppointmentTime = new Label();
        designerOkButton = new Button();
        designerCancelButton = new Button();
        designerHelpButton = new Button();

        designerBodyPanel.Dock = DockStyle.Fill;
        designerBodyPanel.Padding = new Padding(12);

        designerAmount.Location = new Point(16, 14);
        designerAmount.Width = 58;
        designerAmount.ReadOnly = true;
        designerAmount.TextAlign = HorizontalAlignment.Right;

        designerUnit.Location = new Point(82, 14);
        designerUnit.Width = 86;
        designerUnit.DropDownStyle = ComboBoxStyle.DropDownList;
        designerUnit.Items.AddRange(new object[] { "Minutes", "Hours", "Days" });

        designerAlarmDate.Location = new Point(180, 14);
        designerAlarmDate.Width = 96;
        designerAlarmDate.Format = DateTimePickerFormat.Short;

        designerAlarmTime.Location = new Point(286, 14);
        designerAlarmTime.Width = 78;
        designerAlarmTime.Format = DateTimePickerFormat.Custom;
        designerAlarmTime.CustomFormat = "h:mm tt";
        designerAlarmTime.ShowUpDown = true;

        var tuneLabel = new Label { Text = "T&une", Location = new Point(16, 49), AutoSize = true };
        designerTune.Location = new Point(82, 45);
        designerTune.Width = 170;
        designerTune.DropDownStyle = ComboBoxStyle.DropDown;
        designerTune.Items.AddRange(new object[] { "Default", "Chime", "Ding", "Notify" });

        var playButton = new Button { Text = "Play", Location = new Point(262, 43), Width = 82 };
        var browseTuneButton = new Button { Text = "Bro&wse...", Location = new Point(336, 43), Width = 82 };

        var messageLabel = new Label { Text = "Me&ssage", Location = new Point(16, 84), AutoSize = true };
        designerMessage.Location = new Point(82, 80);
        designerMessage.Width = 245;

        var runLabel = new Label { Text = "&Run", Location = new Point(16, 119), AutoSize = true };
        designerRun.Location = new Point(82, 115);
        designerRun.Width = 245;
        var browseRunButton = new Button { Text = "&Browse...", Location = new Point(336, 113), Width = 82 };

        designerDisplayDialog.Text = "D&isplay dialog box at alarm time";
        designerDisplayDialog.Location = new Point(82, 147);
        designerDisplayDialog.AutoSize = true;

        designerSetAlarm.Text = "Set A&larm";
        designerSetAlarm.Location = new Point(16, 190);
        designerSetAlarm.AutoSize = true;
        designerCancelAlarm.Text = "&Cancel Alarm";
        designerCancelAlarm.Location = new Point(120, 190);
        designerCancelAlarm.AutoSize = true;
        designerBefore.Text = "B&efore";
        designerBefore.Location = new Point(260, 190);
        designerBefore.AutoSize = true;
        designerAfter.Text = "&After";
        designerAfter.Location = new Point(330, 190);
        designerAfter.AutoSize = true;
        designerOn.Text = "&On";
        designerOn.Location = new Point(392, 190);
        designerOn.AutoSize = true;

        designerAppointmentTime.Text = "Appointment time";
        designerAppointmentTime.Location = new Point(16, 218);
        designerAppointmentTime.AutoSize = true;

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

        designerOkButton.Text = "OK";
        designerOkButton.Width = 82;
        designerCancelButton.Text = "Cancel";
        designerCancelButton.Width = 82;
        designerHelpButton.Text = "&Help";
        designerHelpButton.Width = 82;

        designerButtonPanel.Dock = DockStyle.Right;
        designerButtonPanel.Width = 100;
        designerButtonPanel.Padding = new Padding(8, 12, 8, 8);
        designerButtonPanel.FlowDirection = FlowDirection.TopDown;
        designerButtonPanel.WrapContents = false;
        designerButtonPanel.Controls.Add(designerOkButton);
        designerButtonPanel.Controls.Add(designerCancelButton);
        designerButtonPanel.Controls.Add(designerHelpButton);

        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(520, 292);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        Text = "Alarm";
        Controls.Add(designerBodyPanel);
        Controls.Add(designerButtonPanel);
    }

    #endregion
}
