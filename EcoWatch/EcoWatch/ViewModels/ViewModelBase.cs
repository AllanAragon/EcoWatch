using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using EcoWatch.Models;

namespace EcoWatch.ViewModels
{
    public partial class ViewModelBase : ObservableObject
    {
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsNotBusy))]
        bool isBusy;

        [ObservableProperty]
        public ObservableCollection<Issue> issues;

        [ObservableProperty]
        public ObservableCollection<Issue> filteredIssues;


        [ObservableProperty]
        bool enableAddingSolution;

        [ObservableProperty]
        string openIssues;

        [ObservableProperty]
        string resolvedIssues;

        [ObservableProperty]
        string allIssues;
        

        public bool IsNotBusy => !IsBusy;

        public ViewModelBase()
        {
            Issues = new ObservableCollection<Issue>();

        }

        public async Task FetchIssuesAsync()
        {
            try
            {
                if (App.firestoreService != null)
                {
                    var issues = await App.firestoreService.GetIssuesAsync();
                    if (issues?.Count() > 0)
                    {
                        Issues = new ObservableCollection<Issue>(issues);
                        FilteredIssues = new ObservableCollection<Issue>(issues);
                    }

                }
                calculateSummary();
            }
            catch (Exception ex) {
                Console.WriteLine(ex);
            }
        }

        private void calculateSummary()
        {
            try
            {
                int countAll = Issues.Count();
                int countResolved = Issues.Where(x => x.IsResolved).Count();
                int countOpen = countAll - countResolved;

                OpenIssues = $"{countOpen} Open";
                ResolvedIssues = $"{countResolved} Resolved";
                AllIssues = $"{countAll} Total";
            }
            catch (Exception ex) { }
        }

        public void GetMockData()
        {
            var mockIssues = new List<Issue>
{
    new Issue { IssueId = "", Title = "Litter in Park", Description = "Scattered trash in Luneta Park", Latitude = 14.5823, Longitude = 120.9784, Location = "Luneta Park, Manila", ImageAttachment = "litter_park.jpg", SubmittedBy = "Alice Santos", SubmittedByEmail = "alice@example.com", Timestamp = DateTime.Now.AddDays(-10), IsResolved = false },

    new Issue { IssueId = "", Title = "Broken Street Light", Description = "Street light out on Ayala Ave", Latitude = 14.5546, Longitude = 121.0188, Location = "Ayala Ave, Makati", ImageAttachment = "street_light.jpg", SubmittedBy = "Bob Reyes", SubmittedByEmail = "bob@example.com", Timestamp = DateTime.Now.AddDays(-8), IsResolved = false },

    new Issue { IssueId = "", Title = "Graffiti on Wall", Description = "Vandalism on City Hall walls", Latitude = 10.3157, Longitude = 123.8854, Location = "Cebu City Hall, Cebu", ImageAttachment = "graffiti.jpg", SubmittedBy = "Charlie Cruz", SubmittedByEmail = "charlie@example.com", Timestamp = DateTime.Now.AddDays(-15), IsResolved = true,
        Resolution = new Solution { SolutionId = "", IssueId = 3, Description = "Painted over graffiti", Latitude = 10.3157, Longitude = 123.8854, Location = "Cebu City Hall, Cebu", ImageAttachment = "painted_wall.jpg", ResolvedBy = "City Maintenance", ResolvedByEmail = "maintenance@citycebu.gov", AgenciesInvoled = "City Maintenance Dept", Timestamp = DateTime.Now.AddDays(-12) }
    },

    new Issue { IssueId = "", Title = "Illegal Dumping", Description = "Large trash pile near the river", Latitude = 14.6751, Longitude = 121.0437, Location = "Marikina River, Marikina", ImageAttachment = "dumping.jpg", SubmittedBy = "Diana Flores", SubmittedByEmail = "diana@example.com", Timestamp = DateTime.Now.AddDays(-5), IsResolved = false },

    new Issue { IssueId = "", Title = "Flooded Road", Description = "Water accumulation on main street", Latitude = 14.5995, Longitude = 120.9842, Location = "Espana Blvd, Manila", ImageAttachment = "flooded_road.jpg", SubmittedBy = "Ethan Delos Santos", SubmittedByEmail = "ethan@example.com", Timestamp = DateTime.Now.AddDays(-3), IsResolved = true,
        Resolution = new Solution { SolutionId = "", IssueId = 5, Description = "Road cleared, drainage fixed", Latitude = 14.5995, Longitude = 120.9842, Location = "Espana Blvd, Manila", ImageAttachment = "cleared_road.jpg", ResolvedBy = "City Public Works", ResolvedByEmail = "publicworks@manilacity.gov", AgenciesInvoled = "City Public Works", Timestamp = DateTime.Now.AddDays(-1) }
    },

    new Issue { IssueId = "", Title = "Damaged Playground", Description = "Broken swings at local park", Latitude = 13.4066, Longitude = 123.3860, Location = "Peñaranda Park, Legazpi", ImageAttachment = "damaged_playground.jpg", SubmittedBy = "Fiona Mateo", SubmittedByEmail = "fiona@example.com", Timestamp = DateTime.Now.AddDays(-20), IsResolved = false },

    new Issue { IssueId = "", Title = "Overflowing Garbage Bins", Description = "Garbage bins are full at bus stop", Latitude = 14.6760, Longitude = 120.9897, Location = "Quezon Ave Station, QC", ImageAttachment = "overflowing_bins.jpg", SubmittedBy = "George Dela Cruz", SubmittedByEmail = "george@example.com", Timestamp = DateTime.Now.AddDays(-2), IsResolved = false },

    new Issue { IssueId = "", Title = "Leaking Fire Hydrant", Description = "Hydrant leaking near Boni Ave", Latitude = 14.5832, Longitude = 121.0569, Location = "Boni Ave, Mandaluyong", ImageAttachment = "leaking_hydrant.jpg", SubmittedBy = "Hannah Bautista", SubmittedByEmail = "hannah@example.com", Timestamp = DateTime.Now.AddDays(-1), IsResolved = false },

    new Issue { IssueId = "", Title = "Noise Pollution", Description = "Construction noise at night", Latitude = 14.5995, Longitude = 120.9842, Location = "Malate, Manila", ImageAttachment = "noise_pollution.jpg", SubmittedBy = "Ian Soriano", SubmittedByEmail = "ian@example.com", Timestamp = DateTime.Now.AddDays(-7), IsResolved = true,
        Resolution = new Solution { SolutionId = "", IssueId = 9, Description = "Enforced noise regulations", Latitude = 14.5995, Longitude = 120.9842, Location = "Malate, Manila", ImageAttachment = "noise_solution.jpg", ResolvedBy = "City Council", ResolvedByEmail = "council@manilacity.gov", AgenciesInvoled = "City Council", Timestamp = DateTime.Now.AddDays(-5) }
    },

    new Issue { IssueId = "", Title = "Potholes on Road", Description = "Potholes causing traffic issues", Latitude = 14.5764, Longitude = 120.9866, Location = "EDSA, Pasay City", ImageAttachment = "potholes.jpg", SubmittedBy = "Jane Lopez", SubmittedByEmail = "jane@example.com", Timestamp = DateTime.Now.AddDays(-10), IsResolved = false },

    new Issue { IssueId = "", Title = "Abandoned Vehicle", Description = "Car left on residential street", Latitude = 14.6614, Longitude = 121.0273, Location = "Banawe St, QC", ImageAttachment = "abandoned_vehicle.jpg", SubmittedBy = "Kevin Ramos", SubmittedByEmail = "kevin@example.com", Timestamp = DateTime.Now.AddDays(-14), IsResolved = false },

    new Issue { IssueId = "", Title = "Fallen Tree", Description = "Tree blocking road", Latitude = 13.6196, Longitude = 123.2061, Location = "Camaligan, Camarines Sur", ImageAttachment = "fallen_tree.jpg", SubmittedBy = "Lucy Villanueva", SubmittedByEmail = "lucy@example.com", Timestamp = DateTime.Now.AddDays(-12), IsResolved = true,
        Resolution = new Solution { SolutionId = "", IssueId = 12, Description = "Tree removed", Latitude = 13.6196, Longitude = 123.2061, Location = "Camaligan, Camarines Sur", ImageAttachment = "tree_removed.jpg", ResolvedBy = "Parks Department", ResolvedByEmail = "parks@camarinessur.gov", AgenciesInvoled = "Parks Dept", Timestamp = DateTime.Now.AddDays(-9) }
    },

    // 8 more issues with similar structure
};

            Issues = new ObservableCollection<Issue>(mockIssues);

        }

    }
}

