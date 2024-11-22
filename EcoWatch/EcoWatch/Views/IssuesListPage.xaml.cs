using EcoWatch.Models;
using EcoWatch.ViewModels;

namespace EcoWatch.Views;

public partial class IssuesListPage : ContentPage
{
    //public ObservableCollection<Issue> Issues { get; set; }
    private IssuesListViewModel viewmodel;
    public IssuesListPage()
    {
        InitializeComponent();
        BindingContext = viewmodel = new IssuesListViewModel();
        //Issues = new ObservableCollection<Issue>
         //   {
          //      new Issue { Title = "Blocked Drain", Description = "Garbage blocking water flow at Main St." },
           //     new Issue { Title = "Overflowing Bin", Description = "Public bin overflowing near Park Ave." },
                // Add more sample issues here
            //};

        if (viewmodel != null)
        {
            //viewmodel.GetMockData();
            //if(Issues?.Count() == 0)
            //{
            //    _ = viewmodel.FetchIssuesAsync();
            //    Issues = viewmodel.Issues;
            //}
        }
        //BindingContext = this;

        
    }
    /**
    protected override void OnAppearing()
    {
        base.OnAppearing();
        Appearing += IssueListPage_Appearing;
    }

    private async void IssueListPage_Appearing(object sender, EventArgs e)
    {
        Appearing -= IssueListPage_Appearing;
        if (viewmodel != null)
        {
            await viewmodel.FetchIssuesAsync();
        }
    }**/
    private async void OnAddIssueClicked(object sender, EventArgs e)
        {
            // Navigate to Add Issue page
            await Navigation.PushModalAsync(new ReportIssuePage());
        }

    private async void refresh_Clicked(System.Object sender, System.EventArgs e)
    {
        await viewmodel.FetchIssuesAsync();
    }

    async void register_Clicked(System.Object sender, System.EventArgs e)
    {
        await Navigation.PushModalAsync(new RegisterPage());
    }
    async void login_Clicked(System.Object sender, System.EventArgs e)
    {
        await Navigation.PushModalAsync(new LoginPage());
    }
    private async void OnOpenIssueInvoked(object sender, EventArgs e)
    {
        try
        {
            var swipeItem = sender as SwipeItem;
            if (swipeItem?.BindingContext is Issue issue)
            {
                // Navigate to the EditIssuePage with the issue details
                await Navigation.PushAsync(new ViewIssuePage(issue,false));
            }
        }
        catch (Exception ex) {
            Console.Write(ex);
        }

    }
    void FilterButton_Clicked(System.Object sender, System.EventArgs e)
    {
        try
        {
            var button = sender as Button;
            var status = "all";
            var sQuery = string.Empty;
            var isSearchFilter = false;
            if (button.Text.ToLower().Contains("open"))
            {
                status = "open";
            }
            else if (button.Text.ToLower().Contains("resolve"))
            {
                status = "resolve";
            }
            else if (button.Text.ToLower().Contains("search"))
            {
                isSearchFilter = true;
            }
            sQuery = txtSearch.Text?.ToLower().Trim();

            if (isSearchFilter)
            { 
                viewmodel.ApplySearch(sQuery);
            }
            else
            { 
                viewmodel.ApplyFilter(status, sQuery);
                toggleColorActive(button);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }

    private void toggleColorActive(Button button)
    {
        btnOpen.FontAttributes = FontAttributes.None;
        btnResolve.FontAttributes = FontAttributes.None;
        btnAll.FontAttributes = FontAttributes.None;
        button.FontAttributes = FontAttributes.Bold;
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
