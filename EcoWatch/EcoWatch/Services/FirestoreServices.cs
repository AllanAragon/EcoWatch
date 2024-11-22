#if ANDROID
using Android.Graphics;
using Android.Media;
#endif

using EcoWatch.Models;
using Firebase.Auth; 
using Firebase.Storage;
using Google.Cloud.Firestore;
using Google.Cloud.Firestore.V1;
using Newtonsoft.Json;
using SkiaSharp;
using Path = System.IO.Path;
using Stream = System.IO.Stream;
using User = EcoWatch.Models.User;

namespace EcoWatch.Services;

public class FirestoreServices
{
    private FirestoreDb _db;
    private readonly string Collection_Issues = "--";
    private readonly string Collection_Users = "--";
    private readonly string Project_ID = "--";
    private Dictionary<string, string> dictDocumentValues = new Dictionary<string, string>();
    private readonly string Storage_URL = "--";
    private readonly string WebAPIKey = "--";
    public FirestoreServices()
    {
        try
        {
            // Path to the service account key JSON file
            //string pathToCredentials = Path.Combine(Environment.CurrentDirectory, "/serviceAccount.json");
            //string pathToCredentials = Path.Combine(AppContext.BaseDirectory, "serviceAccount.json");

            //Console.WriteLine($"Resolved Path: {pathToCredentials}");

            // Set the GOOGLE_APPLICATION_CREDENTIALS environment variable
            //Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", pathToCredentials);
            //if (DeviceInfo.Platform == DevicePlatform.Android)
            //{
            //SetFirebaseCredentialsAsync();
            //}
            //SetFirebaseCredentialsAsync();
            // Initialize Firestore using the project ID
            //_db = FirestoreDb.Create(Project_ID);
            //Console.WriteLine("Firestore database initialized successfully.");
            SetupFirestore();
        }
        catch (Exception ex)
        {

            Console.WriteLine(ex.ToString());
        }

    }

    private async void SetupFirestore()
    {
        if (_db == null)
        {
            var stream = await FileSystem.OpenAppPackageFileAsync("serviceAccount.json");
            var reader = new StreamReader(stream);
            var contents = reader.ReadToEnd();

            _db = new FirestoreDbBuilder() { JsonCredentials = contents, ProjectId = Project_ID }.Build();
        }
    }

    private async void SetFirebaseCredentialsAsync()
    {
        var localPath = System.IO.Path.Combine(FileSystem.CacheDirectory, "serviceAccount.json");

        using var json = await FileSystem.OpenAppPackageFileAsync("serviceAccount.json");
        using var dest = File.Create(localPath);
        await json.CopyToAsync(dest);

        Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", localPath);
        dest.Close();
    }


    private string GetDocumentId(string issueID)
    {
        string docID = string.Empty;
        try
        {
            if (dictDocumentValues.TryGetValue(issueID, out docID))
            {
                // Successfully retrieved the value
                Console.WriteLine($"value fetched");
            }
        }
        catch (Exception ex) { }
        return docID;
    }
    public async Task<List<Issue>> GetIssuesAsync()
    {
        List<Issue> issues = new List<Issue>();
        var documentMapped = new Dictionary<string, string>();
        CollectionReference issuesRef = _db.Collection(Collection_Issues);
        QuerySnapshot snapshot = await issuesRef.GetSnapshotAsync();

        foreach (DocumentSnapshot doc in snapshot.Documents)
        {
            if (doc.Exists)
            {
                Issue issue = doc.ConvertTo<Issue>();
                issues.Add(issue);
                documentMapped.Add(issue.IssueId, doc.Id);
            }
        }
        dictDocumentValues = documentMapped;
        return issues;
    }

    public async Task SaveIssueToFirestoreAsync(Issue issue)
    {
        CollectionReference issuesCollection = _db.Collection(Collection_Issues);

        // Generate a GUID for IssueId if not already set
        if (string.IsNullOrEmpty(issue.IssueId))
        {
            issue.IssueId = Guid.NewGuid().ToString();
        }

        // Convert Issue to dictionary
        Dictionary<string, object> issueData = new Dictionary<string, object>
        {
            { "IssueId", issue.IssueId },
            { "Title", issue.Title },
            { "Description", issue.Description },
            { "Latitude", issue.Latitude },
            { "Longitude", issue.Longitude },
            { "Location", issue.Location },
            { "ImageAttachment", issue.ImageAttachment },
            { "SubmittedBy", issue.SubmittedBy },
            { "SubmittedByEmail", issue.SubmittedByEmail },
            { "Timestamp", issue.Timestamp.ToUniversalTime() },
            { "IsResolved", issue.IsResolved }
        };
        // Save the issue document
        DocumentReference docRef = await issuesCollection.AddAsync(issueData);
        Console.WriteLine($"Issue saved with ID: {docRef.Id}");
    }

    public async Task AddResolutionToIssue(string issueId, Solution resolution)
    {
        // get documentID
        var documentID = GetDocumentId(issueId);

        // Reference to the document you want to update
        DocumentReference docRef = _db.Collection(Collection_Issues).Document(documentID);
        // Generate a GUID for SolutionId if not already set
        if (string.IsNullOrEmpty(resolution.SolutionId))
        {
            resolution.SolutionId = Guid.NewGuid().ToString();
        }
        // Update fields in Firestore
        await docRef.UpdateAsync(new Dictionary<string, object>
        {
            { "IsResolved", true },
            { "Resolution", new Dictionary<string, object>
                {
                    { "SolutionId", resolution.SolutionId },
                    { "IssueId", resolution.IssueId },
                    { "Description", resolution.Description },
                    { "Latitude", resolution.Latitude },
                    { "Longitude", resolution.Longitude },
                    { "Location", resolution.Location },
                    { "ImageAttachment", resolution.ImageAttachment },
                    { "ResolvedBy", resolution.ResolvedBy },
                    { "ResolvedByEmail", resolution.ResolvedByEmail },
                    { "AgenciesInvoled", resolution.AgenciesInvoled },
                    { "Timestamp", Timestamp.FromDateTime(resolution.Timestamp.ToUniversalTime()) }
                }
            }
        });
        Console.WriteLine("Issue updated with resolution.");
    }

    public async Task<string> UploadImageAsync(string localFilePath, bool isResolved)
    {
        try
        {
            var sSubFolder = !isResolved ? "Issues" : "Solutions";
            // Create Firebase Storage instance
            var storage = new FirebaseStorage(Storage_URL);
            var fileName = $"{Path.GetFileName(localFilePath)}"; // Organize by issue ID

            var origStream = File.OpenRead(localFilePath);
            //var oL = origStream.Length;
            int orientation = 0; 
            var resizedStream = RIS(origStream, orientation);
           // var rL = resizedStream.Length;
            // Upload file 
            var uploadTask = storage.Child(sSubFolder).Child(fileName).PutAsync(resizedStream);

            // Get the download URL
            var downloadUrl = await uploadTask;
            Console.WriteLine("File uploaded successfully. URL: " + downloadUrl);

            return downloadUrl; // Return the URL for saving in Firestore
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error uploading image: " + ex.Message);
            return "";
        }
    }

    public async Task<string> RegisterAsync(string email, string password, string fullName)
    {
        string response = "";

        try
        {
            var authProvider = new FirebaseAuthProvider(new FirebaseConfig(WebAPIKey));

            var auth = await authProvider.CreateUserWithEmailAndPasswordAsync(email, password);
            string token = auth.FirebaseToken;
            if (token != null)
            {
                // Save the user's full name in Firestore
                var userDocument = _db.Collection(Collection_Users).Document(auth.User.LocalId);
                // Convert Issue to dictionary
                Dictionary<string, object> userData = new Dictionary<string, object>
                    {
                        { "FullName", fullName },
                        { "Email", email },
                        { "UserId", auth.User.LocalId },
                        { "IsSolutionProvider", false },
                        { "CreatedAt", DateTime.UtcNow }
                    };
                await userDocument.SetAsync(userData);
                response = "Successfully registered user.";
            }
            else
            {
                response = "Unable to register user. Please try again";
            }
        }
        catch (FirebaseAuthException ex)
        {
            response = ex.Reason switch
            {
                AuthErrorReason.EmailExists => "This email is already registered.",
                AuthErrorReason.InvalidEmailAddress => "Invalid email format.",
                AuthErrorReason.WeakPassword => "Password must be stronger.",
                _ => "Registration failed. Try again."
            };
        }
        catch (Exception ex)
        {
            response = "Unable to register user. Please try again";
        }

        return response;
    }

    public async Task<string> LoginUserAsync(string email, string password)
    {
        var response = string.Empty;
        try
        {
            var authProvider = new FirebaseAuthProvider(new FirebaseConfig(WebAPIKey));
            var auth = await authProvider.SignInWithEmailAndPasswordAsync(email, password);
            if (auth != null)
            {
                var content = await auth.GetFreshAuthAsync();
                var serializedContent = JsonConvert.SerializeObject(content);
                Preferences.Set("FreshFirebaseToken", serializedContent);

                response = "Login successful!";

                var user = auth.User;
                DocumentReference docRef = _db.Collection(Collection_Users).Document(user.LocalId);
                DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

                if (snapshot.Exists)
                {
                    // Deserialize document into your user model
                    App.User = snapshot.ConvertTo<User>();
                }
            }
            else
            {
                response = "Error: Unable to login. Please try again";
            }
        }
        catch (Exception e)
        {
            Console.Write(e.ToString());
        }
        return response;
    }

    private static Stream RIS(Stream stream, int reorient)
    {
        try
        {
            // decode to bitmap
#if ANDROID
            Bitmap bitmap = BitmapFactory.DecodeStream(stream);

            // re-orient bitmap
            //bitmap = OrientBitmap(bitmap, reorient);

            /**NEW IMPLEMENTATION**/

            int bWidth = bitmap.Width / 3;
            int bHeight = bitmap.Height / 3;

            Bitmap resizeBitmap = Bitmap.CreateScaledBitmap(bitmap, bWidth, bHeight, false);

            // save image as PNG on a stream
            MemoryStream outStream = new MemoryStream();
            resizeBitmap.Compress(Bitmap.CompressFormat.Jpeg, 25, outStream);

            // rewind stream for re-reading
            outStream.Position = 0;
            return outStream;
#endif
        }
        catch (Exception ex) {
            return null;
        }
        return null;
    }
  }
