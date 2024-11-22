using EcoWatch.Services;
using EcoWatch.ViewModels;
using Firebase.Auth;
using Google.Cloud.Firestore;

namespace EcoWatch.Views;

public partial class RegisterPage : ContentPage
{
    RegisterViewModel viewModel;
	public RegisterPage()
	{
		InitializeComponent();
        BindingContext = viewModel = new RegisterViewModel();
	}

    private async void OnRegisterClicked(object sender, EventArgs e)
    {
        string fullName = fullNameEntry.Text;
        string email = emailEntry.Text;
        string password = passwordEntry.Text;
        string confirmPassword = confirmPasswordEntry.Text;

        // Check if all fields are filled
        if (string.IsNullOrEmpty(fullName) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(confirmPassword))
        {
            errorMessage.Text = "Please fill in all the fields.";
            errorMessage.IsVisible = true;
            return;
        }

        // Check if passwords match
        if (password != confirmPassword)
        {
            errorMessage.Text = "Passwords do not match.";
            errorMessage.IsVisible = true;
            return;
        }

        try
        {
            viewModel.IsBusy = true;
            // Firebase Authentication for registration 
            var response = await App.firestoreService.RegisterAsync(email, password, fullName);
             
            // Navigate to the login page or dashboard
            await DisplayAlert("EcoWatch", response, "OK");
            //await Navigation.PushAsync(new LoginPage());
            await Navigation.PopModalAsync();
        }
        catch (Exception ex)
        {
            errorMessage.Text = "Registration failed. Try again.";
            errorMessage.IsVisible = true;
        }
        finally
        {
            viewModel.IsBusy = false;
        }
    }

    async void Back_Clicked(System.Object sender, System.EventArgs e)
    {
        try
        {
            await Navigation.PopModalAsync();
        }
        catch { }
    }

}
