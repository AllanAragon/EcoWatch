using System.Collections.ObjectModel; 
using Microsoft.Maui.Controls.Maps; 
using EcoWatch.Models;
using EcoWatch.ViewModels;
using Microsoft.Maui.Maps; 
namespace EcoWatch.Views;

public partial class IssuesMapPage : ContentPage
{
    public ObservableCollection<Issue> Issues { get; set; }
    private Issue selectedIssue { get; set; }
    private IssuesMapViewModel viewModel;
    public IssuesMapPage()
    {
        InitializeComponent();
        BindingContext = viewModel = new IssuesMapViewModel();
        Issues = new ObservableCollection<Issue>();

        _ = LoadPins();
        // Set the map to center on the Philippines with a specific zoom level
       var philippineCenter = new Location(12.8797, 121.7740); // Latitude and Longitude of the Philippines
        IssueMap.MoveToRegion(MapSpan.FromCenterAndRadius(philippineCenter, Distance.FromKilometers(500))); // Adjust zoom level as needed

    }

    private async Task LoadPins()
    {
        if (viewModel == null) return;
        //viewModel.GetMockData();//
        try
        {
            viewModel.IsBusy = true;
            await viewModel.FetchIssuesAsync();
            Issues = viewModel.Issues;

            // Clear existing pins
            IssueMap.Pins.Clear();

            foreach (var issue in Issues)
            {
                var position = new Location(issue.Latitude, issue.Longitude); // Changed to 'Location'

                var pin = new Pin
                {
                    Label = issue.Title,
                    Address = issue.Description,
                    Type = PinType.Place,
                    Location = position
                };
                pin.MarkerClicked += (s, e) => ShowPopup(issue); // Attach event handler for pin click
                IssueMap.Pins.Add(pin);

            }
        }
        catch(Exception ex) {
            Console.Write(ex);
        }
        finally { viewModel.IsBusy = false; }
    }
    private async void OnAddIssueClicked(object sender, EventArgs e)
    {
        // Navigate to Add Issue page
        await Navigation.PushModalAsync(new ReportIssuePage());
    }
    private void ShowPopup(Issue issue)
    {
        PopupTitle.Text = issue.Title;
        PopupDescription.Text = issue.Description;
        PopupLocation.Text = $"{issue.Latitude}, {issue.Longitude}";
        PopupAddress.Text = issue.Location;
        PopupReportedBy.Text = issue.SubmittedBy;
        PopupReportedDate.Text = issue.Timestamp.ToLongDateString();
        PopupOverlay.IsVisible = true; // Show popup overlay
        if (issue.IsResolved)
        {
            sResolution.IsVisible = true; 
            PopupSolvedDescription.Text = issue.Resolution.Description;
            PopupSolvedLocation.Text = $"{issue.Resolution.Latitude}, {issue.Resolution.Longitude}";
            PopupSolvedAddress.Text = issue.Resolution.Location;
            PopupSolvedBy.Text = issue.Resolution.ResolvedBy;
            PopupSolvedAgenciesInvolved.Text = issue.Resolution.AgenciesInvoled;
            PopupSolvedDate.Text = issue.Resolution.Timestamp.ToLongDateString();
        }
        else
        {
            sResolution.IsVisible = false; 
        }

        //frmIssue.BackgroundColor = issue.IsResolved ? Color.FromArgb("#3fe07f") :  Color.FromArgb("#e6353e") ;
        selectedIssue = issue;
    }

    private void OnClosePopupClicked(object sender, EventArgs e)
    {
        PopupOverlay.IsVisible = false; // Hide popup
    }
    private async void OnViewIssueAsync(object sender, EventArgs e)
    {
        // Navigate to Add Issue page
        OnClosePopupClicked(null, null);
        await Navigation.PushModalAsync(new ViewIssuePage(selectedIssue));
    }
    private void refresh_Clicked(System.Object sender, System.EventArgs e)
    {
        _ = LoadPins();
    }

    async void ListView_Clicked(System.Object sender, System.EventArgs e)
    {
        await Navigation.PushModalAsync(new IssuesListPage()) ;
    }
}
