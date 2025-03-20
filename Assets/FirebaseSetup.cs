using Firebase;
using Firebase.Database;
using UnityEngine;

public class FirebaseSetup : MonoBehaviour
{
    public void Start()
    {
        //Check if Firebase is initialized
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
            var dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                // Initialize Firebase
                FirebaseApp app = FirebaseApp.DefaultInstance;

                // Get a reference to the database.
                DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;

                Debug.Log("Firebase initialized successfully!");
            }
            else
            {
                Debug.LogError(string.Format("Could not resolve all Firebase dependencies: {0}", dependencyStatus));
            }
        });
        // ... within your FirebaseSetup or another script ...

        DatabaseReference reference = FirebaseDatabase.DefaultInstance.GetReference("recipes");

        reference.GetValueAsync().ContinueWith(task => {
            if (task.IsFaulted)
            {
                Debug.LogError("Error getting value: " + task.Exception);
                return;
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                Debug.Log("Recipes: " + snapshot.Value);
            }
        });

    }
}
