using EcoWatch.Models;
using EcoWatch.Services;
using EcoWatch.Views;

namespace EcoWatch;

public partial class App : Application
{

    public static FirestoreServices firestoreService = null;

    public static bool IsUserLoggedIn { get; set; } = false;
    public static string UserName { get; set; } = "Guest";
    public static User User { get; set; } = new User();
    public App()
	{
		InitializeComponent();

        // Force light mode
        Application.Current.UserAppTheme = AppTheme.Light;
        // Initialize Firestore
        firestoreService = new FirestoreServices();
        MainPage = new AppShell(); 

    }
    public static async void GoToLoginPage()
    {
        await Shell.Current.GoToAsync("//LoginPage");
    }

    public static async void GoToMainPage()
    {
        await Shell.Current.GoToAsync("///MainPage");
    }
}

