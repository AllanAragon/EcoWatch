using EcoWatch.ViewModels;

namespace EcoWatch.Views;

public partial class LoginPage : ContentPage
{
    LoginViewModel viewModel;
	public LoginPage()
	{
		InitializeComponent();
        BindingContext = viewModel = new LoginViewModel();
        //emailEntry.Text = "allanaragon123@gmail.com";
        //passwordEntry.Text = "Allan123$";
	}

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        try
        {
            var email = emailEntry.Text;
            var password = passwordEntry.Text;

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                statusLabel.Text = "Email and password are required!";
                return;
            }

            viewModel.IsBusy = true;
            var result = await App.firestoreService.LoginUserAsync(email, password);

            if (result.Contains("success"))
            {
                App.IsUserLoggedIn = true;
                App.Current.MainPage = new AppShell();
                //await Shell.Current.GoToAsync("///IssuesMapPage");
                //await Navigation.PushModalAsync(new IssuesMapPage());
            }
            else
            {
                statusLabel.TextColor = Colors.Red;
                statusLabel.Text = "Login Failed!";

                await DisplayAlert("Login Failed", "Invalid credentials, please try again.", "OK");
                // Navigate to a different page or store the token for session management
            }
        }
        catch(Exception ex)
        {
            Console.WriteLine(ex);
        }
        finally
        {
            viewModel.IsBusy = false;
        }
 
    }

    async void btnRegister_Clicked(System.Object sender, System.EventArgs e)
    {
        await Navigation.PushModalAsync(new RegisterPage());
    }
}
