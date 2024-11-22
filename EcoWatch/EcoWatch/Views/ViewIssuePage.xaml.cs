 
using EcoWatch.Models;
using EcoWatch.ViewModels;  

namespace EcoWatch.Views;

public partial class ViewIssuePage : ContentPage
{
	private Issue Issue { get; set; }
    public IssueViewModel viewModel;
    private Location currentLocation; // Store the current location
    private bool _hasPhoto = false;
    private string _mPhotoPath = null;
    private bool _bCanAddSolution = false;
    private bool _isModal =  true;

    public ViewIssuePage(Issue issue, bool isModal = true)
	{
		InitializeComponent();
        BindingContext = viewModel = new IssueViewModel();
		Issue = issue;
        _bCanAddSolution = App.User.IsSolutionProvider;
        _isModal = isModal;
        ShowPopup(issue);
	}

    private void ShowPopup(Issue issue)
    {
        try
        {
            viewModel.IsBusy = true;
            PopupTitle.Text = issue.Title;
            shortIssueTitle.Text = issue.Title;
            PopupDescription.Text = issue.Description;
            shortIssueDesc.Text = issue.Description;
            PopupLocation.Text = $"{issue.Latitude}, {issue.Longitude}";
            PopupAddress.Text = issue.Location;
            PopupReportedBy.Text = issue.SubmittedBy;
            PopupReportedDate.Text = issue.Timestamp.ToLongDateString();
            PopupOverlay.IsVisible = true; // Show popup overlay
            issuePhoto.Source = issue.ImageAttachment;
            if (issue.IsResolved)
            {
                sResolution.IsVisible = true;
                PopupSolvedDescription.Text = issue.Resolution.Description;
                PopupSolvedLocation.Text = $"{issue.Resolution.Latitude}, {issue.Resolution.Longitude}";
                PopupSolvedAddress.Text = issue.Resolution.Location;
                PopupSolvedBy.Text = issue.Resolution.ResolvedBy;
                PopupSolvedAgenciesInvolved.Text = issue.Resolution.AgenciesInvoled;
                PopupSolvedDate.Text = issue.Resolution.Timestamp.ToLongDateString();
                issuePhoto1.Source = issue.Resolution.ImageAttachment;
            }
            else
            {
                sResolution.IsVisible = false;
            }

            if (_bCanAddSolution)
            { 
                AddSolution.IsVisible = !issue.IsResolved;
            }



            //frmIssue.BackgroundColor = issue.IsResolved ? Color.FromArgb("#3fe07f") : Color.FromArgb("#e6353e");
        }
        catch (Exception ex) { }
        finally { viewModel.IsBusy = false; }
    }

    async void Save_Issue_ClickedAsync(System.Object sender, System.EventArgs e)
    {
        try
        {
            if (App.firestoreService != null)
            {
                viewModel.IsBusy = true;

                // save image first
                var imgUrl = await App.firestoreService.UploadImageAsync(_mPhotoPath, true);

                var resolution = BuildSolution();
                resolution.ImageAttachment = imgUrl;
                await App.firestoreService.AddResolutionToIssue(Issue.IssueId, resolution);
                await DisplayAlert("Success", "Resolution submitted successfully!", "OK");
                await Navigation.PopModalAsync();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
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
            if (_isModal)
            { 
                await Navigation.PopModalAsync();
            }
            else
            { 
                await Navigation.PopAsync();
            }
            
        }
        catch (Exception ex)
        {
            Console.Write(ex);
        }
    }

    void AddSolution_Clicked(System.Object sender, System.EventArgs e)
    {
        if(PopupOverlay.IsVisible == false)
        {
            PopupOverlay.IsVisible = true;
            frmSolution.IsVisible = false;
            btnSubmitSolution.IsVisible = false;
        }
        else
        {
            PopupOverlay.IsVisible = false;
            frmSolution.IsVisible = true;
            btnSubmitSolution.IsVisible = true;
            _ = GetLocationAndAddress();
        }
    }

    // Fetch location and reverse geocode details
    private async Task GetLocationAndAddress()
    {
        try
        {  
            // Request location
            var location = await Geolocation.GetLastKnownLocationAsync();

            if (location != null)
            {
                currentLocation = location;
                // Update UI with latitude and longitude
                locationLabel.Text = $"LatLng: {Math.Round(location.Latitude,6)},{Math.Round(location.Longitude,6)}";

               

                // Reverse geocoding to get address
                var placemarks = await Geocoding.GetPlacemarksAsync(location);
                var placemark = placemarks?.FirstOrDefault();

                if (placemark != null)
                {
                    addressLabel.Text = $"{placemark.Thoroughfare}, {placemark.Locality}, {placemark.CountryName}";
                }

               
            }
            else
            {
                locationLabel.Text = "Unable to retrieve location";
            }
        }
        catch (FeatureNotSupportedException fnsEx)
        {
            // Handle not supported on device exception
            locationLabel.Text = "Location not supported on this device";
        }
        catch (PermissionException pEx)
        {
            // Handle permission exception
            locationLabel.Text = "Location permission not granted";
        }
        catch (Exception ex)
        {
            // Handle other exceptions
            locationLabel.Text = "Error retrieving location";
        }
        finally
        {
        }
    }
    public async void TakePhoto()
    {
        if (MediaPicker.Default.IsCaptureSupported)
        {
            FileResult photo = await MediaPicker.Default.CapturePhotoAsync();

            if (photo != null)
            {
                // save the file into local storage
                string localFilePath = System.IO.Path.Combine(FileSystem.CacheDirectory, photo.FileName);

                using Stream sourceStream = await photo.OpenReadAsync();
                using FileStream localFileStream = File.OpenWrite(localFilePath);

                await sourceStream.CopyToAsync(localFileStream);
                issuePhoto.Source = ImageSource.FromStream(() => sourceStream);
            }
        }
    }

    private async void OnTakePhotoClicked(object sender, EventArgs e)
    {
        try
        {
            var photo = await MediaPicker.Default.CapturePhotoAsync();
            if (photo != null)
            {
                var stream = await photo.OpenReadAsync();
                resolutionPhoto.Source = ImageSource.FromStream(() => stream);
                _hasPhoto = true;
                _mPhotoPath = photo.FullPath;
                CheckCanSubmit();
            }
        }
        catch (Exception ex) {
            Console.Write(ex);
        }
        
        

    }
    private void CheckCanSubmit()
    {
        btnSubmitSolution.IsEnabled = !string.IsNullOrEmpty(agenciesEntry.Text) &&
            !string.IsNullOrEmpty(descriptionEntry.Text) && _hasPhoto;
    }

    private Solution BuildSolution()
    {
        var resolution = new Solution {
            SolutionId = "",
            IssueId = 0,
            Description = descriptionEntry.Text,
            Latitude = Math.Round(currentLocation.Latitude, 6),
            Longitude = Math.Round(currentLocation.Longitude, 6),
            Location = addressLabel.Text,
            ImageAttachment = "painted_wall.jpg",
            ResolvedBy = App.User.FullName,
            ResolvedByEmail = App.User.Email,
            AgenciesInvoled = agenciesEntry.Text,
            Timestamp = DateTime.Now
        };
        return resolution;

    }
  
}
