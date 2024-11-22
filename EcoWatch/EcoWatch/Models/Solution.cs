using System;
using Google.Cloud.Firestore;

namespace EcoWatch.Models
{   [FirestoreData]
	public class Solution
	{
        [FirestoreProperty]
        public string SolutionId { get; set; }
        [FirestoreProperty]
        public long IssueId { get; set; }
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
        public string ResolvedBy { get; set; }
        [FirestoreProperty]
        public string ResolvedByEmail { get; set; }
        [FirestoreProperty]
        public string AgenciesInvoled { get; set; }
        [FirestoreProperty]
        public DateTime Timestamp { get; set; }
    }
}

