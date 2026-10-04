using System.Reflection;

namespace Organizer;

internal sealed class ContactEditorDialog : Form
{
    private readonly object _record;
    private readonly PropertyInfo? _propName;
    private readonly PropertyInfo? _propCompany;
    private readonly PropertyInfo? _propEmail;
    private readonly PropertyInfo? _propPhone;
    private readonly PropertyInfo? _propAddress;
    private readonly PropertyInfo? _propNotes;

    private readonly TextBox txtFirstName = new() { Dock = DockStyle.Fill };
    private readonly TextBox txtLastName = new() { Dock = DockStyle.Fill };
    private readonly TextBox txtCompany = new() { Dock = DockStyle.Fill };
    private readonly TextBox txtEmail = new() { Dock = DockStyle.Fill };
    private readonly TextBox txtAddress = new() { Dock = DockStyle.Fill, Multiline = true, Height = 60 };

    public ContactEditorDialog(object record, string title)
    {
        _record = record ?? throw new ArgumentNullException(nameof(record));
        Text = title ?? "Edit Contact";
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        Width = 740;
        Height = 420;

        Type t = record.GetType();
        _propName = t.GetProperty("Name") ?? t.GetProperty("FullName");
        PropertyInfo? propFirst = t.GetProperty("FirstName");
        PropertyInfo? propLast = t.GetProperty("LastName");

        if(propFirst is not null && propLast is not null)
        {
            // prefer separate first/last name properties when available
            _propName = null; // don't use single Name property
            _propFirstName = propFirst;
            _propLastName = propLast;
        }
        _propCompany = t.GetProperty("Company");
        _propEmail = t.GetProperty("Email");
        _propPhone = t.GetProperty("Phone");
        _propAddress = t.GetProperty("Address");
        _propNotes = t.GetProperty("Notes") ?? t.GetProperty("Note") ?? t.GetProperty("Comments");

        BuildLayout();
        LoadValues();
        // focus first name when shown
        Shown += (_, _) => txtFirstName.Focus();
    }

    private PropertyInfo? _propFirstName;
    private PropertyInfo? _propLastName;

    // Additional UI controls to match org6 contact form
    private ComboBox cmbTitle = new() { DropDownStyle = ComboBoxStyle.DropDownList, Width = 55 };

    private TextBox txtMiddleName = new() { Dock = DockStyle.Fill };
    private ComboBox cmbSuffix = new() { DropDownStyle = ComboBoxStyle.DropDownList, Width = 80 };

    private TabControl tabControl = new() { Dock = DockStyle.Fill };
    private TabPage tabWork = new("Work");
    private TabPage tabHome = new("Home");
    private TabPage tabGeneral = new("General");

    // General tab controls
    private TextBox txtNickname = new() { Dock = DockStyle.Fill };

    private DateTimePicker dtpBirthday = new() { Dock = DockStyle.Left, Width = 120, Format = DateTimePickerFormat.Short };
    private DateTimePicker dtpAnniversary = new() { Dock = DockStyle.Left, Width = 120, Format = DateTimePickerFormat.Short };
    private TextBox txtSpouse = new() { Dock = DockStyle.Fill };
    private TextBox txtChildren = new() { Dock = DockStyle.Fill };
    private TextBox txtOnlineName = new() { Dock = DockStyle.Fill };
    private ComboBox cmbCustom1 = new() { DropDownStyle = ComboBoxStyle.DropDown };
    private ComboBox cmbCustom2 = new() { DropDownStyle = ComboBoxStyle.DropDown };
    private ComboBox cmbCustom3 = new() { DropDownStyle = ComboBoxStyle.DropDown };
    private ComboBox cmbCustom4 = new() { DropDownStyle = ComboBoxStyle.DropDown };
    private ComboBox cmbCustom5 = new() { DropDownStyle = ComboBoxStyle.DropDown };
    private ComboBox cmbCustom6 = new() { DropDownStyle = ComboBoxStyle.DropDown };

    // Work tab controls
    private TextBox txtJobTitle = new() { Dock = DockStyle.Fill };

    // txtCompany already exists
    private TextBox txtCity = new() { Dock = DockStyle.Fill };

    private TextBox txtState = new() { Dock = DockStyle.Fill };
    private TextBox txtZip = new() { Dock = DockStyle.Fill };
    private TextBox txtCountry = new() { Dock = DockStyle.Fill };
    private TextBox txtDept = new() { Dock = DockStyle.Fill };
    private TextBox txtAssistant = new() { Dock = DockStyle.Fill };

    // Phone fields
    private TextBox txtWorkPhone = new() { Dock = DockStyle.Fill };

    private TextBox txtWorkExt = new() { Dock = DockStyle.Left, Width = 40 };
    private TextBox txtWorkFax = new() { Dock = DockStyle.Fill };
    private TextBox txtPager = new() { Dock = DockStyle.Fill };
    private TextBox txtAssistantPhone = new() { Dock = DockStyle.Fill };
    private TextBox txtHomePhone = new() { Dock = DockStyle.Fill };
    private TextBox txtHomeFax = new() { Dock = DockStyle.Fill };
    private TextBox txtCarPhone = new() { Dock = DockStyle.Fill };
    private TextBox txtHomePhone2 = new() { Dock = DockStyle.Fill };

    // Additional phone UI elements to better match Org6: type dropdowns and icon buttons per row
    private ComboBox cmbWorkPhoneType = new() { DropDownStyle = ComboBoxStyle.DropDownList, Width = 120 };

    private ComboBox cmbWorkFaxType = new() { DropDownStyle = ComboBoxStyle.DropDownList, Width = 120 };
    private ComboBox cmbPagerType = new() { DropDownStyle = ComboBoxStyle.DropDownList, Width = 120 };
    private ComboBox cmbAssistantPhoneType = new() { DropDownStyle = ComboBoxStyle.DropDownList, Width = 120 };
    private ComboBox cmbHomePhoneType = new() { DropDownStyle = ComboBoxStyle.DropDownList, Width = 120 };
    private ComboBox cmbHomeFaxType = new() { DropDownStyle = ComboBoxStyle.DropDownList, Width = 120 };
    private ComboBox cmbCarPhoneType = new() { DropDownStyle = ComboBoxStyle.DropDownList, Width = 120 };
    private ComboBox cmbHomePhone2Type = new() { DropDownStyle = ComboBoxStyle.DropDownList, Width = 120 };
    private Button btnWorkPhoneIcon = new() { Width = 22, Height = 22, Text = "" };
    private Button btnWorkFaxIcon = new() { Width = 22, Height = 22, Text = "" };
    private Button btnPagerIcon = new() { Width = 22, Height = 22, Text = "" };
    private Button btnAssistantIcon = new() { Width = 22, Height = 22, Text = "" };
    private Button btnHomePhoneIcon = new() { Width = 22, Height = 22, Text = "" };
    private Button btnHomeFaxIcon = new() { Width = 22, Height = 22, Text = "" };
    private Button btnCarIcon = new() { Width = 22, Height = 22, Text = "" };
    private Button btnHome2Icon = new() { Width = 22, Height = 22, Text = "" };

    // Additional duplicated Work phone rows
    private TextBox txtWorkPhone2 = new() { Dock = DockStyle.Fill };

    private TextBox txtWorkPhone3 = new() { Dock = DockStyle.Fill };
    private TextBox txtWorkPhone4 = new() { Dock = DockStyle.Fill };
    private TextBox txtWorkExt2 = new() { Dock = DockStyle.Left, Width = 40 };
    private ComboBox cmbWorkPhone2Type = new() { DropDownStyle = ComboBoxStyle.DropDownList, Width = 120 };
    private ComboBox cmbWorkPhone3Type = new() { DropDownStyle = ComboBoxStyle.DropDownList, Width = 120 };
    private ComboBox cmbWorkPhone4Type = new() { DropDownStyle = ComboBoxStyle.DropDownList, Width = 120 };
    private Button btnWork2Icon = new() { Width = 22, Height = 22, Text = "" };
    private Button btnWork3Icon = new() { Width = 22, Height = 22, Text = "" };
    private Button btnWork4Icon = new() { Width = 22, Height = 22, Text = "" };

    // Home address fields (separate from work address)
    private TextBox txtHomeAddress = new() { Dock = DockStyle.Fill, Multiline = true, Height = 60 };

    private TextBox txtHomeCity = new() { Dock = DockStyle.Fill };
    private TextBox txtHomeState = new() { Dock = DockStyle.Fill };
    private TextBox txtHomeZip = new() { Dock = DockStyle.Fill };
    private TextBox txtHomeCountry = new() { Dock = DockStyle.Fill };

    private ComboBox cmbCategories = new() { DropDownStyle = ComboBoxStyle.DropDown };
    private TextBox txtNotesBox = new() { Dock = DockStyle.Fill, Multiline = true, ScrollBars = ScrollBars.Vertical };

    private ComboBox cmbEmailWork = new() { DropDownStyle = ComboBoxStyle.DropDown }; // could be typed list of addresses
    private TextBox txtBusyTime = new() { Dock = DockStyle.Fill };
    private CheckBox chkConfidential = new() { Text = "Confidential", AutoSize = true };
    private CheckBox chkTrackActivities = new() { Text = "Track Activities", AutoSize = true };

    private void BuildLayout()
    {
        // Build a more complex layout to match Org6 contact form: title + name row; tab control; buttons
        TableLayoutPanel? outer = new() { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 3 };
        outer.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // top name row
        outer.RowStyles.Add(new RowStyle(SizeType.Percent, 100)); // tab control
        outer.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // buttons

        // Top row: Title | First | Middle | Last | Suffix
        TableLayoutPanel? topRow = new() { Dock = DockStyle.Top, ColumnCount = 11, Padding = new Padding(8), AutoSize = true };
        // columns: label, control, label, control, ... then filler
        topRow.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 40)); // label Title
        topRow.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 90)); // title combo
        topRow.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 80)); // label First
        topRow.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 140)); // first name control
        topRow.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 70)); // label Middle
        topRow.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100)); // middle control
        topRow.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 60)); // label Last
        topRow.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 160)); // last name control
        topRow.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 60)); // label Suffix
        topRow.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 90)); // suffix combo
        topRow.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

        topRow.Controls.Add(new Label { Text = "Title", TextAlign = ContentAlignment.TopLeft, Dock = DockStyle.Fill }, 0, 0);
        topRow.Controls.Add(cmbTitle, 1, 0);
        topRow.Controls.Add(new Label { Text = "First name", TextAlign = ContentAlignment.TopLeft, Dock = DockStyle.Fill }, 2, 0);
        topRow.Controls.Add(txtFirstName, 3, 0);
        topRow.Controls.Add(new Label { Text = "Middle", TextAlign = ContentAlignment.TopLeft, Dock = DockStyle.Fill }, 4, 0);
        topRow.Controls.Add(txtMiddleName, 5, 0);
        topRow.Controls.Add(new Label { Text = "Last name", TextAlign = ContentAlignment.TopLeft, Dock = DockStyle.Fill }, 6, 0);
        topRow.Controls.Add(txtLastName, 7, 0);
        topRow.Controls.Add(new Label { Text = "Suffix", TextAlign = ContentAlignment.TopLeft, Dock = DockStyle.Fill }, 8, 0);
        topRow.Controls.Add(cmbSuffix, 9, 0);

        // Tab control setup
        tabControl.TabPages.AddRange([tabWork, tabHome, tabGeneral]);

        // Work tab layout: split left (details) and right (phones + scheduling)
        TableLayoutPanel? workSplit = new() { Dock = DockStyle.Fill, ColumnCount = 2 };
        // make left column a fixed width to ensure it is wider than the right pane
        workSplit.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 450));
        workSplit.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

        // Left details panel inside work tab
        // left panel padding removed
        TableLayoutPanel? leftDetails = new() { Dock = DockStyle.Fill, ColumnCount = 2, Padding = Padding.Empty };
        leftDetails.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 90));
        leftDetails.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        int rr = 0;
        void LAdd(string lbl, Control ctrl)
        {
            leftDetails.RowCount = Math.Max(leftDetails.RowCount, rr + 1);
            leftDetails.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            leftDetails.Controls.Add(new Label { Text = lbl, Dock = DockStyle.Fill, TextAlign = ContentAlignment.TopLeft }, 0, rr);
            leftDetails.Controls.Add(ctrl, 1, rr); rr++;
        }

        LAdd("Job title:", txtJobTitle);
        LAdd("Company:", txtCompany);
        LAdd("Street:", txtAddress);

        // city/state/zip/country on one row
        var cityRow = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 4 };
        cityRow.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40));
        cityRow.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20));
        cityRow.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20));
        cityRow.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20));
        cityRow.Controls.Add(txtCity, 0, 0);
        cityRow.Controls.Add(txtState, 1, 0);
        cityRow.Controls.Add(txtZip, 2, 0);
        cityRow.Controls.Add(txtCountry, 3, 0);
        LAdd("City/State/Zip/Country:", cityRow);

        LAdd("Dept:", txtDept);
        LAdd("Assistant:", txtAssistant);

        LAdd("Categories:", cmbCategories);
        LAdd("Notes:", txtNotesBox);

        workSplit.Controls.Add(leftDetails, 0, 0);

        // Right side: phones and scheduling
        TableLayoutPanel? rightPanel = new() { Dock = DockStyle.Fill, ColumnCount = 3, Padding = new Padding(8) };
        rightPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 60));
        rightPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        rightPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 50));

        int pr = 0;
        void PAdd(string lbl, Control ctrl, Control? typeCtrl, Control? icon = null, Control? small = null)
        {
            rightPanel.RowCount = Math.Max(rightPanel.RowCount, pr + 1);
            rightPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            rightPanel.Controls.Add(new Label { Text = lbl, Dock = DockStyle.Fill, TextAlign = ContentAlignment.TopLeft }, 0, pr);
            // main control in column 1
            rightPanel.Controls.Add(ctrl, 1, pr);
            // type combo in column 2
            if(typeCtrl is not null) rightPanel.Controls.Add(typeCtrl, 2, pr);
            // icon in column 3
            if(icon is not null) rightPanel.Controls.Add(icon, 3, pr);
            // small ext in column 4
            if(small is not null) rightPanel.Controls.Add(small, 4, pr);
            pr++;
        }

        // adjust rightPanel to have more columns for type/icon/ext
        rightPanel.ColumnCount = 5;
        rightPanel.ColumnStyles.Clear();
        rightPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 60)); // label
        rightPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50)); // main control
        rightPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120)); // type combo
        rightPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 28)); // icon
        rightPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 50)); // small ext

        PAdd("Work:", txtWorkPhone, cmbWorkPhoneType, btnWorkPhoneIcon, txtWorkExt);
        PAdd("Work fax:", txtWorkFax, cmbWorkFaxType, btnWorkFaxIcon, null);
        PAdd("Pager:", txtPager, cmbPagerType, btnPagerIcon, null);
        PAdd("Assistant:", txtAssistantPhone, cmbAssistantPhoneType, btnAssistantIcon, null);
        PAdd("Home:", txtHomePhone, cmbHomePhoneType, btnHomePhoneIcon, null);
        // Additional Work rows to match Org6 which has multiple Work entries
        PAdd("Work 2:", txtWorkPhone2, cmbWorkPhone2Type, btnWork2Icon, txtWorkExt2);
        PAdd("Work 3:", txtWorkPhone3, cmbWorkPhone3Type, btnWork3Icon, null);
        PAdd("Work 4:", txtWorkPhone4, cmbWorkPhone4Type, btnWork4Icon, null);

        // Scheduling group box
        GroupBox? sched = new() { Text = "Scheduling", Dock = DockStyle.Top, Height = 120 };
        TableLayoutPanel? schedLayout = new() { Dock = DockStyle.Fill, ColumnCount = 2, Padding = new Padding(6) };
        schedLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 80));
        schedLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        schedLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        schedLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        schedLayout.Controls.Add(new Label { Text = "E-mail: Work", Dock = DockStyle.Fill }, 0, 0);
        schedLayout.Controls.Add(cmbEmailWork, 1, 0);
        schedLayout.Controls.Add(new Label { Text = "Busy time", Dock = DockStyle.Fill }, 0, 1);
        schedLayout.Controls.Add(txtBusyTime, 1, 1);
        sched.Controls.Add(schedLayout);

        rightPanel.Controls.Add(sched, 0, pr++);

        // checkboxes at bottom of rightPanel
        rightPanel.RowCount = Math.Max(rightPanel.RowCount, pr + 1);
        rightPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        FlowLayoutPanel? chkPanel = new() { Dock = DockStyle.Fill, FlowDirection = FlowDirection.LeftToRight }; chkPanel.Controls.Add(chkConfidential); chkPanel.Controls.Add(chkTrackActivities);
        rightPanel.Controls.Add(chkPanel, 1, pr);

        workSplit.Controls.Add(rightPanel, 1, 0);

        tabWork.Controls.Add(workSplit);

        // Home tab: left = home address, right = home phones
        TableLayoutPanel? homeSplit = new() { Dock = DockStyle.Fill, ColumnCount = 2 };
        // make left and right columns even (50%/50%)
        homeSplit.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        homeSplit.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));

        TableLayoutPanel? homeLeft = new() { Dock = DockStyle.Fill, ColumnCount = 2, Padding = new Padding(8) };
        // reduce label column so inputs use the 50/50 split space nicely
        homeLeft.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 70));
        homeLeft.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        int hr = 0;
        void HAdd(string lbl, Control ctrl) { homeLeft.RowCount = Math.Max(homeLeft.RowCount, hr + 1); homeLeft.RowStyles.Add(new RowStyle(SizeType.AutoSize)); homeLeft.Controls.Add(new Label { Text = lbl, Dock = DockStyle.Fill, TextAlign = ContentAlignment.TopLeft }, 0, hr); homeLeft.Controls.Add(ctrl, 1, hr); hr++; }

        HAdd("Street:", txtHomeAddress);

        // city/state/zip/country row
        TableLayoutPanel? homeCityRow = new() { Dock = DockStyle.Fill, ColumnCount = 4 };
        homeCityRow.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40));
        homeCityRow.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20));
        homeCityRow.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20));
        homeCityRow.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20));
        homeCityRow.Controls.Add(txtHomeCity, 0, 0);
        homeCityRow.Controls.Add(txtHomeState, 1, 0);
        homeCityRow.Controls.Add(txtHomeZip, 2, 0);
        homeCityRow.Controls.Add(txtHomeCountry, 3, 0);
        HAdd("City/State/Zip/Country:", homeCityRow);

        homeSplit.Controls.Add(homeLeft, 0, 0);

        TableLayoutPanel? homeRight = new() { Dock = DockStyle.Fill, ColumnCount = 3, Padding = new Padding(8) };
        homeRight.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 60));
        homeRight.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        homeRight.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 50));
        int hpr = 0;
        void HPAdd(string lbl, Control ctrl, Control? small = null)
        {
            homeRight.RowCount = Math.Max(homeRight.RowCount, hpr + 1);
            homeRight.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            homeRight.Controls.Add(new Label
            {
                Text = lbl,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.TopLeft
            },
                0,
                hpr);
            homeRight.Controls.Add(ctrl, 1, hpr);
            if(small is not null) homeRight.Controls.Add(small, 2, hpr); hpr++;
        }

        HPAdd("Home:", txtHomePhone, null);
        HPAdd("Home fax:", txtHomeFax, null);
        HPAdd("Car:", txtCarPhone, null);
        HPAdd("Home 2:", txtHomePhone2, null);

        homeSplit.Controls.Add(homeRight, 1, 0);
        tabHome.Controls.Add(homeSplit);

        // General tab: left = personal fields, right = custom fields
        TableLayoutPanel? genMain = new() { Dock = DockStyle.Fill, ColumnCount = 2, Padding = new Padding(8) };
        // make left and right columns even (50%/50%)
        genMain.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        genMain.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));

        TableLayoutPanel? genLeft = new() { Dock = DockStyle.Fill, ColumnCount = 2 };
        // slightly smaller label column so left/right columns look balanced
        genLeft.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 80));
        genLeft.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        int gl = 0;
        void GAdd(string lbl, Control ctrl) { genLeft.RowCount = Math.Max(genLeft.RowCount, gl + 1); genLeft.RowStyles.Add(new RowStyle(SizeType.AutoSize)); genLeft.Controls.Add(new Label { Text = lbl, Dock = DockStyle.Fill, TextAlign = ContentAlignment.TopLeft }, 0, gl); genLeft.Controls.Add(ctrl, 1, gl); gl++; }

        GAdd("Nickname:", txtNickname);
        GAdd("Birthday:", dtpBirthday);
        GAdd("Anniversary:", dtpAnniversary);
        GAdd("Spouse:", txtSpouse);
        GAdd("Children:", txtChildren);
        GAdd("Online Name:", txtOnlineName);

        genMain.Controls.Add(genLeft, 0, 0);

        TableLayoutPanel? genRight = new() { Dock = DockStyle.Fill, ColumnCount = 2 }; // label + control
        genRight.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 80));
        genRight.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        int gr = 0;
        void RAdd(string lbl, Control ctrl) { genRight.RowCount = Math.Max(genRight.RowCount, gr + 1); genRight.RowStyles.Add(new RowStyle(SizeType.AutoSize)); genRight.Controls.Add(new Label { Text = lbl, Dock = DockStyle.Fill, TextAlign = ContentAlignment.TopLeft }, 0, gr); genRight.Controls.Add(ctrl, 1, gr); gr++; }

        RAdd("Custom", cmbCustom1);
        RAdd("Custom 2", cmbCustom2);
        RAdd("Custom 3", cmbCustom3);
        RAdd("Custom 4", cmbCustom4);
        RAdd("Custom 5", cmbCustom5);
        RAdd("Custom 6", cmbCustom6);

        // Edit custom labels button
        Button? btnEditCustom = new() { Text = "Edit Custom Labels...", AutoSize = true };
        genRight.RowCount = Math.Max(genRight.RowCount, gr + 1);
        genRight.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        genRight.Controls.Add(new Label { Text = string.Empty }, 0, gr);
        genRight.Controls.Add(btnEditCustom, 1, gr);

        genMain.Controls.Add(genRight, 1, 0);
        tabGeneral.Controls.Add(genMain);

        // Buttons
        FlowLayoutPanel? buttons = new() { Dock = DockStyle.Bottom, FlowDirection = FlowDirection.LeftToRight, Padding = new Padding(8), Height = 40 };
        // Do not set DialogResult on OK button so we can validate and prevent closing when errors occur
        Button? btnOk = new() { Text = "OK", Width = 90 };
        Button? btnCancel = new() { Text = "Cancel", DialogResult = DialogResult.Cancel, Width = 90 };
        Button? btnEditTabs = new() { Text = "Edit tabs...", Width = 90 };
        Button? btnHelp = new() { Text = "Help", Width = 90 };
        btnOk.Click += (_, _) => { if(OnOk()) Close(); };
        buttons.Controls.Add(btnOk);
        buttons.Controls.Add(btnCancel);
        buttons.Controls.Add(btnEditTabs);
        buttons.Controls.Add(btnHelp);
        btnHelp.Click += (_, _) => MessageBox.Show(this, "Not yet implemented.", "Help", MessageBoxButtons.OK, MessageBoxIcon.Information);

        // Assemble outer
        outer.Controls.Add(topRow);
        outer.Controls.Add(tabControl);
        outer.Controls.Add(buttons);

        Controls.Add(outer);

        // Populate title/suffix lists with common values
        cmbTitle.Items.AddRange(["", "Mr.", "Mrs.", "Miss", "Ms.", "Dr.", "Prof.", "Fr.", "Rev", "Herr", "Frl", "M.", "Mme.", "Mlle"]);
        cmbSuffix.Items.AddRange(["", "Jr.", "Sr.", "II", "III"]);
        cmbCategories.Items.AddRange(["", "Friend", "Family", "Business"]);

        // Set form Accept/Cancel buttons
        AcceptButton = btnOk;
        CancelButton = btnCancel;
    }

    private void LoadValues()
    {
        object? v;
        // names
        if(_propFirstName is not null && _propLastName is not null)
        {
            v = _propFirstName.GetValue(_record);
            txtFirstName.Text = v?.ToString() ?? string.Empty;
            v = _propLastName.GetValue(_record);
            txtLastName.Text = v?.ToString() ?? string.Empty;
        }
        else
        {
            v = _propName?.GetValue(_record);
            txtFirstName.Text = v?.ToString() ?? string.Empty;
        }
        // other common fields
        v = _record.GetType().GetProperty("Title")?.GetValue(_record);
        cmbTitle.Text = v?.ToString() ?? string.Empty;
        v = _record.GetType().GetProperty("MiddleName")?.GetValue(_record);
        txtMiddleName.Text = v?.ToString() ?? string.Empty;
        v = _record.GetType().GetProperty("Suffix")?.GetValue(_record);
        cmbSuffix.Text = v?.ToString() ?? string.Empty;
        v = _propCompany?.GetValue(_record);
        txtCompany.Text = v?.ToString() ?? string.Empty;
        v = _record.GetType().GetProperty("JobTitle")?.GetValue(_record);
        txtJobTitle.Text = v?.ToString() ?? string.Empty;
        v = _propEmail?.GetValue(_record);
        txtEmail.Text = v?.ToString() ?? string.Empty;
        v = _record.GetType().GetProperty("WorkPhone")?.GetValue(_record) ?? _record.GetType().GetProperty("Phone")?.GetValue(_record);
        txtWorkPhone.Text = v?.ToString() ?? string.Empty;
        v = _record.GetType().GetProperty("WorkPhone2")?.GetValue(_record);
        txtWorkPhone2.Text = v?.ToString() ?? string.Empty;
        v = _record.GetType().GetProperty("WorkPhone3")?.GetValue(_record);
        txtWorkPhone3.Text = v?.ToString() ?? string.Empty;
        v = _record.GetType().GetProperty("WorkPhone4")?.GetValue(_record);
        txtWorkPhone4.Text = v?.ToString() ?? string.Empty;
        v = _record.GetType().GetProperty("WorkExt")?.GetValue(_record);
        txtWorkExt.Text = v?.ToString() ?? string.Empty;
        v = _record.GetType().GetProperty("WorkFax")?.GetValue(_record);
        txtWorkFax.Text = v?.ToString() ?? string.Empty;
        v = _record.GetType().GetProperty("Pager")?.GetValue(_record);
        txtPager.Text = v?.ToString() ?? string.Empty;
        v = _record.GetType().GetProperty("AssistantPhone")?.GetValue(_record);
        txtAssistantPhone.Text = v?.ToString() ?? string.Empty;
        v = _propPhone?.GetValue(_record);
        txtHomePhone.Text = v?.ToString() ?? string.Empty;
        v = _propAddress?.GetValue(_record);
        txtAddress.Text = v?.ToString() ?? string.Empty;
        v = _record.GetType().GetProperty("City")?.GetValue(_record);
        txtCity.Text = v?.ToString() ?? string.Empty;
        v = _record.GetType().GetProperty("State")?.GetValue(_record);
        txtState.Text = v?.ToString() ?? string.Empty;
        v = _record.GetType().GetProperty("Zip")?.GetValue(_record) ?? _record.GetType().GetProperty("PostalCode")?.GetValue(_record);
        txtZip.Text = v?.ToString() ?? string.Empty;
        v = _record.GetType().GetProperty("Country")?.GetValue(_record);
        txtCountry.Text = v?.ToString() ?? string.Empty;
        v = _record.GetType().GetProperty("Dept")?.GetValue(_record) ?? _record.GetType().GetProperty("Department")?.GetValue(_record);
        txtDept.Text = v?.ToString() ?? string.Empty;
        v = _record.GetType().GetProperty("Assistant")?.GetValue(_record);
        txtAssistant.Text = v?.ToString() ?? string.Empty;
        v = _record.GetType().GetProperty("Categories")?.GetValue(_record);
        cmbCategories.Text = v?.ToString() ?? string.Empty;
        v = _propNotes?.GetValue(_record);
        txtNotesBox.Text = v?.ToString() ?? string.Empty;
        // General tab values
        v = _record.GetType().GetProperty("Nickname")?.GetValue(_record);
        txtNickname.Text = v?.ToString() ?? string.Empty;
        v = _record.GetType().GetProperty("Birthday")?.GetValue(_record);
        if(v is DateTime dtb) dtpBirthday.Value = dtb;
        v = _record.GetType().GetProperty("Anniversary")?.GetValue(_record);
        if(v is DateTime dta) dtpAnniversary.Value = dta;
        v = _record.GetType().GetProperty("Spouse")?.GetValue(_record);
        txtSpouse.Text = v?.ToString() ?? string.Empty;
        v = _record.GetType().GetProperty("Children")?.GetValue(_record);
        txtChildren.Text = v?.ToString() ?? string.Empty;
        v = _record.GetType().GetProperty("OnlineName")?.GetValue(_record);
        txtOnlineName.Text = v?.ToString() ?? string.Empty;
        v = _record.GetType().GetProperty("Custom1")?.GetValue(_record);
        cmbCustom1.Text = v?.ToString() ?? string.Empty;
        v = _record.GetType().GetProperty("Custom2")?.GetValue(_record);
        cmbCustom2.Text = v?.ToString() ?? string.Empty;
        v = _record.GetType().GetProperty("Custom3")?.GetValue(_record);
        cmbCustom3.Text = v?.ToString() ?? string.Empty;
        v = _record.GetType().GetProperty("Custom4")?.GetValue(_record);
        cmbCustom4.Text = v?.ToString() ?? string.Empty;
        v = _record.GetType().GetProperty("Custom5")?.GetValue(_record);
        cmbCustom5.Text = v?.ToString() ?? string.Empty;
        v = _record.GetType().GetProperty("Custom6")?.GetValue(_record);
        cmbCustom6.Text = v?.ToString() ?? string.Empty;
        // Home-specific fields
        v = _record.GetType().GetProperty("HomeAddress")?.GetValue(_record) ?? _record.GetType().GetProperty("HomeStreet")?.GetValue(_record);
        txtHomeAddress.Text = v?.ToString() ?? string.Empty;
        v = _record.GetType().GetProperty("HomeCity")?.GetValue(_record) ?? _record.GetType().GetProperty("CityHome")?.GetValue(_record) ?? _record.GetType().GetProperty("City")?.GetValue(_record);
        txtHomeCity.Text = v?.ToString() ?? string.Empty;
        v = _record.GetType().GetProperty("HomeState")?.GetValue(_record) ?? _record.GetType().GetProperty("State")?.GetValue(_record);
        txtHomeState.Text = v?.ToString() ?? string.Empty;
        v = _record.GetType().GetProperty("HomeZip")?.GetValue(_record) ?? _record.GetType().GetProperty("Zip")?.GetValue(_record);
        txtHomeZip.Text = v?.ToString() ?? string.Empty;
        v = _record.GetType().GetProperty("HomeCountry")?.GetValue(_record) ?? _record.GetType().GetProperty("Country")?.GetValue(_record);
        txtHomeCountry.Text = v?.ToString() ?? string.Empty;
        v = _record.GetType().GetProperty("HomePhone")?.GetValue(_record) ?? _record.GetType().GetProperty("PhoneHome")?.GetValue(_record);
        txtHomePhone.Text = v?.ToString() ?? string.Empty;
        v = _record.GetType().GetProperty("HomeFax")?.GetValue(_record);
        txtHomeFax.Text = v?.ToString() ?? string.Empty;
        v = _record.GetType().GetProperty("CarPhone")?.GetValue(_record) ?? _record.GetType().GetProperty("Car")?.GetValue(_record);
        txtCarPhone.Text = v?.ToString() ?? string.Empty;
        v = _record.GetType().GetProperty("HomePhone2")?.GetValue(_record);
        txtHomePhone2.Text = v?.ToString() ?? string.Empty;
        v = _record.GetType().GetProperty("EmailWork")?.GetValue(_record);
        cmbEmailWork.Text = v?.ToString() ?? string.Empty;
        v = _record.GetType().GetProperty("BusyTime")?.GetValue(_record);
        txtBusyTime.Text = v?.ToString() ?? string.Empty;
        v = _record.GetType().GetProperty("Confidential")?.GetValue(_record);
        chkConfidential.Checked = v is bool b1 && b1;
        v = _record.GetType().GetProperty("TrackActivities")?.GetValue(_record);
        chkTrackActivities.Checked = v is bool b2 && b2;
        // General tab save defaults (dtp controls already initialized above if present)
    }

    private bool OnOk()
    {
        // Basic validation: name required if property exists
        if(_propFirstName is not null && _propLastName is not null)
        {
            if(string.IsNullOrWhiteSpace(txtFirstName.Text) && string.IsNullOrWhiteSpace(txtLastName.Text))
            {
                MessageBox.Show(this, "First name or last name is required.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
        }
        else if(_propName is not null)
        {
            if(string.IsNullOrWhiteSpace(txtFirstName.Text))
            {
                MessageBox.Show(this, "Name is required.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
        }
        // Validate email if provided
        if(!string.IsNullOrWhiteSpace(txtEmail.Text))
        {
            try { _ = new System.Net.Mail.MailAddress(txtEmail.Text); }
            catch
            {
                MessageBox.Show(this, "Please enter a valid email address.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
        }

        // Validate important phone fields (work/home)
        System.Text.RegularExpressions.Regex? phoneRegex = new(@"^[0-9\s\-\+\(\)extEXT\.]*$");
        void ValidatePhoneControl(TextBox tb, string label)
        {
            if(string.IsNullOrWhiteSpace(tb.Text)) return;

            string? s = tb.Text.Trim();
            if(s.Length < 3 || s.Length > 60 || !phoneRegex.IsMatch(s)) throw new ArgumentException(label);
        }

        try
        {
            ValidatePhoneControl(txtWorkPhone, "Work phone");
            ValidatePhoneControl(txtHomePhone, "Home phone");
            ValidatePhoneControl(txtWorkFax, "Work fax");
            ValidatePhoneControl(txtPager, "Pager");
            ValidatePhoneControl(txtHomeFax, "Home fax");
            ValidatePhoneControl(txtCarPhone, "Car phone");
            ValidatePhoneControl(txtHomePhone2, "Home phone 2");
        }
        catch(ArgumentException ex)
        {
            MessageBox.Show(this, $"Please enter a valid {ex.Message}.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            return false;
        }

        // Persist values via safe reflection; if any set fails, keep the dialog open so user can fix
        Type? t = _record.GetType();
        bool anySetFailed = false;
        bool SetIfWritable(string propName, object? val)
        {
            PropertyInfo? p = t.GetProperty(propName);
            if(p is null || !p.CanWrite) return true; // nothing to do -> not a failure
            try
            {
                if(val is null)
                {
                    p.SetValue(_record, null);

                    return true;
                }

                Type? targetType = Nullable.GetUnderlyingType(p.PropertyType) ?? p.PropertyType;

                if(targetType == typeof(string))
                {
                    p.SetValue(_record, val.ToString());

                    return true;
                }

                if(targetType == typeof(bool))
                {
                    if(val is bool b) p.SetValue(_record, b);
                    else if(bool.TryParse(val.ToString(), out var bb)) p.SetValue(_record, bb);

                    return true;
                }
                // try convert
                p.SetValue(_record, Convert.ChangeType(val, targetType));

                return true;
            }
            catch(Exception ex)
            {
                anySetFailed = true;
                System.Diagnostics.Debug.WriteLine($"SetIfWritable failed for {propName}: {ex.Message}");

                return false;
            }
        }

        try
        {
            try
            {
                if(_propFirstName is not null && _propLastName is not null)
                {
                    _propFirstName.SetValue(_record, string.IsNullOrWhiteSpace(txtFirstName.Text) ? null : txtFirstName.Text);
                    _propLastName.SetValue(_record, string.IsNullOrWhiteSpace(txtLastName.Text) ? null : txtLastName.Text);
                }
                else
                {
                    _propName?.SetValue(_record, string.IsNullOrWhiteSpace(txtFirstName.Text) ? null : txtFirstName.Text);
                }
            }
            catch(Exception ex)
            {
                anySetFailed = true;
                System.Diagnostics.Debug.WriteLine($"Name set failed: {ex.Message}");
            }

            anySetFailed |= !SetIfWritable("Title", cmbTitle.Text);
            anySetFailed |= !SetIfWritable("MiddleName", string.IsNullOrWhiteSpace(txtMiddleName.Text) ? null : txtMiddleName.Text);
            anySetFailed |= !SetIfWritable("Suffix", cmbSuffix.Text);
            try { _propCompany?.SetValue(_record, string.IsNullOrWhiteSpace(txtCompany.Text) ? null : txtCompany.Text); } catch { anySetFailed = true; }
            anySetFailed |= !SetIfWritable("JobTitle", string.IsNullOrWhiteSpace(txtJobTitle.Text) ? null : txtJobTitle.Text);
            anySetFailed |= !SetIfWritable("Email", string.IsNullOrWhiteSpace(txtEmail.Text) ? null : txtEmail.Text);
            anySetFailed |= !SetIfWritable("WorkPhone", string.IsNullOrWhiteSpace(txtWorkPhone.Text) ? null : txtWorkPhone.Text);
            anySetFailed |= !SetIfWritable("WorkExt", string.IsNullOrWhiteSpace(txtWorkExt.Text) ? null : txtWorkExt.Text);
            anySetFailed |= !SetIfWritable("WorkFax", string.IsNullOrWhiteSpace(txtWorkFax.Text) ? null : txtWorkFax.Text);
            anySetFailed |= !SetIfWritable("Pager", string.IsNullOrWhiteSpace(txtPager.Text) ? null : txtPager.Text);
            anySetFailed |= !SetIfWritable("AssistantPhone", string.IsNullOrWhiteSpace(txtAssistantPhone.Text) ? null : txtAssistantPhone.Text);
            anySetFailed |= !SetIfWritable("WorkPhone2", string.IsNullOrWhiteSpace(txtWorkPhone2.Text) ? null : txtWorkPhone2.Text);
            anySetFailed |= !SetIfWritable("WorkPhone3", string.IsNullOrWhiteSpace(txtWorkPhone3.Text) ? null : txtWorkPhone3.Text);
            anySetFailed |= !SetIfWritable("WorkPhone4", string.IsNullOrWhiteSpace(txtWorkPhone4.Text) ? null : txtWorkPhone4.Text);
            try { _propPhone?.SetValue(_record, string.IsNullOrWhiteSpace(txtHomePhone.Text) ? null : txtHomePhone.Text); } catch { anySetFailed = true; }
            try { _propAddress?.SetValue(_record, string.IsNullOrWhiteSpace(txtAddress.Text) ? null : txtAddress.Text); } catch { anySetFailed = true; }
            anySetFailed |= !SetIfWritable("City", string.IsNullOrWhiteSpace(txtCity.Text) ? null : txtCity.Text);
            anySetFailed |= !SetIfWritable("State", string.IsNullOrWhiteSpace(txtState.Text) ? null : txtState.Text);
            anySetFailed |= !SetIfWritable("Zip", string.IsNullOrWhiteSpace(txtZip.Text) ? null : txtZip.Text);
            anySetFailed |= !SetIfWritable("PostalCode", string.IsNullOrWhiteSpace(txtZip.Text) ? null : txtZip.Text);
            anySetFailed |= !SetIfWritable("Country", string.IsNullOrWhiteSpace(txtCountry.Text) ? null : txtCountry.Text);
            anySetFailed |= !SetIfWritable("Dept", string.IsNullOrWhiteSpace(txtDept.Text) ? null : txtDept.Text);
            anySetFailed |= !SetIfWritable("Department", string.IsNullOrWhiteSpace(txtDept.Text) ? null : txtDept.Text);
            anySetFailed |= !SetIfWritable("Assistant", string.IsNullOrWhiteSpace(txtAssistant.Text) ? null : txtAssistant.Text);
            anySetFailed |= !SetIfWritable("Categories", string.IsNullOrWhiteSpace(cmbCategories.Text) ? null : cmbCategories.Text);
            try { if(_propNotes is not null) _propNotes.SetValue(_record, string.IsNullOrWhiteSpace(txtNotesBox.Text) ? null : txtNotesBox.Text); } catch { anySetFailed = true; }
            anySetFailed |= !SetIfWritable("EmailWork", string.IsNullOrWhiteSpace(cmbEmailWork.Text) ? null : cmbEmailWork.Text);
            anySetFailed |= !SetIfWritable("BusyTime", string.IsNullOrWhiteSpace(txtBusyTime.Text) ? null : txtBusyTime.Text);
            anySetFailed |= !SetIfWritable("Confidential", chkConfidential.Checked);
            anySetFailed |= !SetIfWritable("TrackActivities", chkTrackActivities.Checked);
            // Home-specific persistence
            anySetFailed |= !SetIfWritable("HomeAddress", string.IsNullOrWhiteSpace(txtHomeAddress.Text) ? null : txtHomeAddress.Text);
            anySetFailed |= !SetIfWritable("HomeStreet", string.IsNullOrWhiteSpace(txtHomeAddress.Text) ? null : txtHomeAddress.Text);
            anySetFailed |= !SetIfWritable("HomeCity", string.IsNullOrWhiteSpace(txtHomeCity.Text) ? null : txtHomeCity.Text);
            anySetFailed |= !SetIfWritable("CityHome", string.IsNullOrWhiteSpace(txtHomeCity.Text) ? null : txtHomeCity.Text);
            anySetFailed |= !SetIfWritable("HomeState", string.IsNullOrWhiteSpace(txtHomeState.Text) ? null : txtHomeState.Text);
            anySetFailed |= !SetIfWritable("HomeZip", string.IsNullOrWhiteSpace(txtHomeZip.Text) ? null : txtHomeZip.Text);
            anySetFailed |= !SetIfWritable("PostalCodeHome", string.IsNullOrWhiteSpace(txtHomeZip.Text) ? null : txtHomeZip.Text);
            anySetFailed |= !SetIfWritable("HomeCountry", string.IsNullOrWhiteSpace(txtHomeCountry.Text) ? null : txtHomeCountry.Text);
            anySetFailed |= !SetIfWritable("HomePhone", string.IsNullOrWhiteSpace(txtHomePhone.Text) ? null : txtHomePhone.Text);
            anySetFailed |= !SetIfWritable("PhoneHome", string.IsNullOrWhiteSpace(txtHomePhone.Text) ? null : txtHomePhone.Text);
            anySetFailed |= !SetIfWritable("HomeFax", string.IsNullOrWhiteSpace(txtHomeFax.Text) ? null : txtHomeFax.Text);
            anySetFailed |= !SetIfWritable("CarPhone", string.IsNullOrWhiteSpace(txtCarPhone.Text) ? null : txtCarPhone.Text);
            anySetFailed |= !SetIfWritable("HomePhone2", string.IsNullOrWhiteSpace(txtHomePhone2.Text) ? null : txtHomePhone2.Text);
            // General tab persistence
            anySetFailed |= !SetIfWritable("Nickname", string.IsNullOrWhiteSpace(txtNickname.Text) ? null : txtNickname.Text);
            try { anySetFailed |= !SetIfWritable("Birthday", dtpBirthday.Value); } catch { }
            try { anySetFailed |= !SetIfWritable("Anniversary", dtpAnniversary.Value); } catch { }
            anySetFailed |= !SetIfWritable("Spouse", string.IsNullOrWhiteSpace(txtSpouse.Text) ? null : txtSpouse.Text);
            anySetFailed |= !SetIfWritable("Children", string.IsNullOrWhiteSpace(txtChildren.Text) ? null : txtChildren.Text);
            anySetFailed |= !SetIfWritable("OnlineName", string.IsNullOrWhiteSpace(txtOnlineName.Text) ? null : txtOnlineName.Text);
            anySetFailed |= !SetIfWritable("Custom1", string.IsNullOrWhiteSpace(cmbCustom1.Text) ? null : cmbCustom1.Text);
            anySetFailed |= !SetIfWritable("Custom2", string.IsNullOrWhiteSpace(cmbCustom2.Text) ? null : cmbCustom2.Text);
            anySetFailed |= !SetIfWritable("Custom3", string.IsNullOrWhiteSpace(cmbCustom3.Text) ? null : cmbCustom3.Text);
            anySetFailed |= !SetIfWritable("Custom4", string.IsNullOrWhiteSpace(cmbCustom4.Text) ? null : cmbCustom4.Text);
            anySetFailed |= !SetIfWritable("Custom5", string.IsNullOrWhiteSpace(cmbCustom5.Text) ? null : cmbCustom5.Text);
            anySetFailed |= !SetIfWritable("Custom6", string.IsNullOrWhiteSpace(cmbCustom6.Text) ? null : cmbCustom6.Text);
        }
        catch(Exception ex)
        {
            anySetFailed = true;
            System.Diagnostics.Debug.WriteLine($"Unexpected error persisting fields: {ex.Message}");
        }

        if(anySetFailed)
        {
            MessageBox.Show(this, "One or more fields could not be saved. Please correct the values and try again.", "Save error", MessageBoxButtons.OK, MessageBoxIcon.Error);

            return false;
        }

        DialogResult = DialogResult.OK;

        return true;
    }
}