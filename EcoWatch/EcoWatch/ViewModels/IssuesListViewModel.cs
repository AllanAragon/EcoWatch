using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using EcoWatch.Models;
using EcoWatch.Views;

namespace EcoWatch.ViewModels
{
	public  class IssuesListViewModel : ViewModelBase
	{

        public Command<Issue> EditCommand { get; }
        public Command<Issue> DeleteCommand { get; }
        public IssuesListViewModel()
		{
            _ = FetchIssuesAsync();
            EditCommand = new Command<Issue>(OnEditIssue);
            DeleteCommand = new Command<Issue>(OnDeleteIssue);
        }
        private async void OnEditIssue(Issue issue)
        { 
            // Logic to edit the issue
            Console.WriteLine($"Editing issue: {issue.Title}");
        }

        private void OnDeleteIssue(Issue issue)
        {
            // Logic to delete the issue
            Console.WriteLine($"Deleting issue: {issue.Title}");
        }

        public void ApplySearch(string searchQuery)
        {
            try
            {
                FilteredIssues = new ObservableCollection<Issue>(FilteredIssues.Where(i => i.Title.ToLower().Contains(searchQuery)).ToList());
            }
            catch (Exception ex) { }
        }

        public void ApplyFilter(string status, string searchQuery)
        {
            try
            {
                if (status == "open")
                {
                    FilteredIssues = new ObservableCollection<Issue>(Issues.Where(i => !i.IsResolved).ToList());
                }
                else if (status == "resolve")
                {
                    FilteredIssues = new ObservableCollection<Issue>(Issues.Where(i => i.IsResolved).ToList());
                }
                else if (status == "my_report")
                {
                    var myEmail = App.User.Email;
                    FilteredIssues = new ObservableCollection<Issue>(Issues.Where(i => i.SubmittedByEmail == myEmail).ToList());
                }
                else
                {
                    FilteredIssues = Issues;
                }

                if (!string.IsNullOrEmpty(searchQuery))
                    FilteredIssues = new ObservableCollection<Issue>(FilteredIssues.Where(i => i.Title.ToLower().Contains(searchQuery)).ToList());

            }
            catch (Exception e) { }

        }


	}
}

