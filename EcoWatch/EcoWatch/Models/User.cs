using System;
using Google.Cloud.Firestore;

namespace EcoWatch.Models
{
    [FirestoreData]
    public class User
    {
        [FirestoreProperty]
        public string UserId { get; set; }

        [FirestoreProperty]
        public string Email { get; set; }

        [FirestoreProperty]
        public string FullName { get; set; }

        [FirestoreProperty]
        public bool IsSolutionProvider { get; set; }

        [FirestoreProperty]
        public DateTime CreatedAt { get; set; }
    }
}

