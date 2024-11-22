using EcoWatch.Models;
using EcoWatch.ViewModels;

namespace EcoWatch.Views;

public partial class ReportIssuePage : ContentPage
{
    private Location currentLocation; // Store the current location
    private bool _hasPhoto = false;
    private string _mPhotoPath = null;
    private ReportIssueViewModel viewModel;
    public ReportIssuePage()
	{
		InitializeComponent();
        BindingContext = viewModel = new ReportIssueViewModel();
        _ = GetLocationAndAddress();

    }

    public async void TakePhoto()
    {
        if (MediaPicker.Default.IsCaptureSupported)
        {
            FileResult photo = await MediaPicker.Default.CapturePhotoAsync();

            if (photo != null)
            { 
                // save the file into local storage
                string localFilePath = Path.Combine(FileSystem.CacheDirectory, photo.FileName);
                _mPhotoPath = localFilePath;
                using Stream sourceStream = await photo.OpenReadAsync();
                using FileStream localFileStream = File.OpenWrite(localFilePath);

                await sourceStream.CopyToAsync(localFileStream);
                issuePhoto.Source = ImageSource.FromStream(() => sourceStream);
                
            }
        }
    }

    private async void OnTakePhotoClicked(object sender, EventArgs e)
    {
        var photo = await MediaPicker.Default.CapturePhotoAsync();
        if (photo != null)
        {
            var stream = await photo.OpenReadAsync();
            issuePhoto.Source = ImageSource.FromStream(() => stream);
            _hasPhoto = true;
            _mPhotoPath = photo.FullPath;
            CheckCanSubmit();
        }
    }

    private async void OnSubmitReportClicked(object sender, EventArgs e)
    {
        try
        {
            if (currentLocation != null)
            {
                viewModel.IsBusy = true;
                // save image first
                var imgUrl = await App.firestoreService.UploadImageAsync(_mPhotoPath, false);

                // Save issue to the database
                var issue = BuildIssue();
                issue.ImageAttachment = imgUrl;
                await App.firestoreService.SaveIssueToFirestoreAsync(issue);
                await DisplayAlert("Success", "Issue reported successfully!", "OK");
                await Navigation.PopModalAsync();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
        finally
        {
            viewModel.IsBusy = false;
        }

    }
    private Issue BuildIssue()
    {
       var issue =  new Issue
        { 
            Title = issueEntry.Text,
            Description = descriptionEntry.Text,
            Latitude = Math.Round( currentLocation.Latitude,6),
            Longitude = Math.Round(currentLocation.Longitude,6),
            Location = addressLabel.Text,
            ImageAttachment = "litter_park.jpg",
            SubmittedBy = App.User.FullName,
            SubmittedByEmail = App.User.Email,
            Timestamp = DateTime.Now,
            IsResolved = false
        };
        return issue;

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
                locationLabel.Text = $"LatLng: {Math.Round(location.Latitude, 6)},{Math.Round(location.Longitude, 6)}";



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

    private void CheckCanSubmit()
    {
        btnSubmitIssue.IsEnabled = !string.IsNullOrEmpty(issueEntry.Text) &&
            !string.IsNullOrEmpty(descriptionEntry.Text) && _hasPhoto;

    }

    void entry_TextChanged(System.Object sender, Microsoft.Maui.Controls.TextChangedEventArgs e)
    {
        CheckCanSubmit();
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
