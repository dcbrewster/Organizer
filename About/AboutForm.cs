using System.Diagnostics;
using System.Reflection;
using System.Text.Json;

namespace Organizer.About;

/// <summary>
/// Represents the "About" form of the application, displaying version information,
/// build date, and contributor credits.
/// </summary>
public partial class AboutForm : Form
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AboutForm"/> class.
    /// </summary>
    public AboutForm()
    {
        InitializeComponent();  // Initialize the form's components.
        LoadApplicationInfo();  // Load the application version and build date into the labels.
        LoadCredits();          // Load the contributor credits into the credits control.

        creditsControl.MouseEnter += (s, e) => creditsControl.Pause();
        creditsControl.MouseLeave += (s, e) => creditsControl.Resume();
    }

    /// <summary>
    /// Loads the application version and build date into the labels on the form.
    /// </summary>
    private void LoadApplicationInfo()
    {
        lblVersion.Text = "Version " + Application.ProductVersion;
        lblBuild.Text = $"Built {GetBuildDate():yyyy-MM-dd HH:mm}";
    }


    /// <summary>
    /// Loads the contributor credits into the credits control.
    /// </summary>
    private void LoadCredits()
    {
        string file = Path.Combine(Application.StartupPath, "About", "Contributors.json");

        if(!File.Exists(file)) return;

        ContributorList? credits = JsonSerializer.Deserialize<ContributorList>(File.ReadAllText(file));

        List<string> lines = ["CONTRIBUTORS", ""];

        // Add each contributor's role and name to the lines list, followed by an empty line for spacing.
        foreach(Contributor contributor in credits.Contributors)
        {
            lines.Add(contributor.Role);
            lines.Add(contributor.Name);
            lines.Add("");
        }

        // Add a thank you message at the end of the credits.
        lines.Add($"Thank you for using {Application.ProductName}");

        // Set the lines in the credits control to display the contributor information.
        creditsControl.SetLines(lines);
    }

    /// <summary>
    /// Gets the build date of the application by retrieving the last write time of the executing assembly.
    /// </summary>
    /// <returns>The build date of the application.</returns>
    private static DateTime GetBuildDate() => File.GetLastWriteTime(Assembly.GetExecutingAssembly().Location);

    /// <summary>
    /// Handles the click event of the close button, closing the form when clicked.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnClose_Click(object sender, EventArgs e) => Close();

    private void LinkWebsite_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
    {
        Process.Start(
            new ProcessStartInfo
            {
                FileName = "https://forestcitysoftware.com",
                UseShellExecute = true
            });
    }

    /// <summary>
    /// Handles the click event of the email link, opening the default email client to send an email to support.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void LinkEmail_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
    {
        Process.Start(
            new ProcessStartInfo
            {
                FileName = "mailto:support@forestcitysoftware.com",
                UseShellExecute = true
            });
    }
}