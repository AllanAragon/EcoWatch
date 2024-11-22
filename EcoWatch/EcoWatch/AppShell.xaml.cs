using EcoWatch.Views;

namespace EcoWatch;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        // Register routes for navigation
        Routing.RegisterRoute("LoginPage", typeof(LoginPage));
        Routing.RegisterRoute("IssuesMapPage", typeof(IssuesMapPage));

        // Check user authentication state
        CheckAuthentication();
    }

    /// <summary>
    /// Checks if the user is authenticated and updates the UI accordingly.
    /// </summary>
    private async void CheckAuthentication()
    {
        if (App.IsUserLoggedIn)
        {
            // Update user information and show main content
            UserNameLabel.Text = App.User.FullName ?? "Guest";
            SetMainContentVisibility(true);
        }
        else
        {
            // Navigate to login page and hide main content
            await NavigateToLoginPage();
            SetMainContentVisibility(false);
        }
    }

    /// <summary>
    /// Updates the visibility of main content.
    /// </summary>
    /// <param name="isMainContentVisible">Indicates whether the main content should be visible.</param>
    private void SetMainContentVisibility(bool isMainContentVisible)
    {
        HomeItem.IsVisible = isMainContentVisible;
        btnLogout.IsVisible = isMainContentVisible;
        LoginItem.IsVisible = !isMainContentVisible;

        // Optional: Add more logic if there are other elements to manage visibility
    }

    /// <summary>
    /// Navigates to the login page and resets the user's session.
    /// </summary>
    /// <returns>Task for async operation.</returns>
    private async Task NavigateToLoginPage()
    {
        try
        {
            await GoToAsync("///LoginPage");
        }
        catch (Exception ex)
        {
            Console.Write(ex);
        }

    }

    /// <summary>
    /// Command for logging out the user.
    /// </summary>
    public Command LogoutCommand => new Command(async () =>
    {
        PerformLogout();
        await NavigateToLoginPage();
    });

    /// <summary>
    /// Performs the logout operation.
    /// </summary>
    private void PerformLogout()
    {
        App.IsUserLoggedIn = false;
        App.UserName = null;

        // Reset UI to guest state
        UserNameLabel.Text = "Guest";
        SetMainContentVisibility(false);
    }

    /// <summary>
    /// Handles the Logout MenuItem click event.
    /// </summary>
    private async void MenuItem_Clicked(object sender, EventArgs e)
    {
        PerformLogout();
        await NavigateToLoginPage();
    }
}
