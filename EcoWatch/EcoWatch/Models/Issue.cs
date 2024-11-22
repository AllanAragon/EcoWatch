using System;
using Google.Cloud.Firestore;

namespace EcoWatch.Models
{
    [FirestoreData]
    public class Issue
    {
        [FirestoreProperty]
        public string IssueId { get; set; }
        [FirestoreProperty]
        public string Title { get; set; }
        [FirestoreProperty]
        public string Description { get; set; }
        [FirestoreProperty]
        public double Latitude { get; set; }
        [FirestoreProperty]
        public double Longitude { get; set; }
        [FirestoreProperty]
        public string Location { get; set; }
        [FirestoreProperty]
        public string ImageAttachment { get; set; }
        [FirestoreProperty]
        public string SubmittedBy { get; set; }
        [FirestoreProperty]
        public string SubmittedByEmail { get; set; }
        [FirestoreProperty]
        public DateTime Timestamp { get; set; }
        [FirestoreProperty]
        public bool IsResolved { get; set; }
        [FirestoreProperty]
        public Solution Resolution { get; set; }

        public string NarrativeSummary
        {
            get
            {
                var reportDetails = $"On {Timestamp.ToString("MMMM dd, yyyy")}, an issue titled \"{Title}\" was reported by {SubmittedBy}. The issue, described as \"{Description}\", occurred at {Location} (LatLng: {Latitude}, {Longitude}).";

                //if (!string.IsNullOrEmpty(ImageAttachment))
                //{
                //    reportDetails += $" An image was attached to the report: {ImageAttachment}.";
                //}

                if (IsResolved && Resolution != null)
                {
                    var resolutionDetails = $"\n\n The issue was resolved on {Resolution.Timestamp.ToString("MMMM dd, yyyy")} by {Resolution.ResolvedBy}. The resolution involved \"{Resolution.Description}\" at {Resolution.Location} (LatLng: {Resolution.Latitude}, {Resolution.Longitude}).";

                    if (!string.IsNullOrEmpty(Resolution.AgenciesInvoled))
                    {
                        resolutionDetails += $" Agencies involved included: {Resolution.AgenciesInvoled}.";
                    }

                    //if (!string.IsNullOrEmpty(Resolution.ImageAttachment))
                    //{
                    //    resolutionDetails += $" A resolution image was provided: {Resolution.ImageAttachment}.";
                    //}

                    return reportDetails + resolutionDetails;
                }

                return reportDetails + " The issue is currently unresolved.";
            }
        }
    }
}

