using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;  
using UnityEngine.UI;
using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using UnityEngine.Networking;
using System.Linq;

public class FirebaseSearch : MonoBehaviour
{
    public TMP_InputField searchInput;  
    public Button searchButton;
    public TMP_Text resultText;       
    public Image recipeImage;

    private DatabaseReference dbReference;

    void Start()
    {
        // Initialize Firebase
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.Result == DependencyStatus.Available)
            {
                FirebaseApp app = FirebaseApp.DefaultInstance;
                FirebaseDatabase database = FirebaseDatabase.GetInstance("https://instarecipietesting-default-rtdb.europe-west1.firebasedatabase.app/");
                dbReference = database.RootReference;

                searchButton.onClick.AddListener(() => SearchRecipe(searchInput.text));
            }
            else
            {
                Debug.LogError("Could not resolve all Firebase dependencies.");
            }
        });
    }

    void SearchRecipe(string recipeName)
    {
        if (string.IsNullOrEmpty(recipeName))
        {
            resultText.text = "Please enter a recipe name.";
            return;
        }

        FirebaseDatabase.DefaultInstance
            .GetReference("recipes")  // Ensure this matches your Firebase database structure
            .GetValueAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted)
                {
                    resultText.text = "Error fetching data!";
                }
                else if (task.IsCompleted)
                {
                    DataSnapshot snapshot = task.Result;
                    bool found = false;

                    foreach (var child in snapshot.Children)
                    {
                        string name = child.Child("recipe_name").Value.ToString();

                        if (name.ToLower().Contains(recipeName.ToLower()))
                        {
                            found = true;
                            string ingredients = child.Child("ingredients").Value.ToString();
                            string directions = child.Child("cooking_directions").Value.ToString();
                            string imageUrl = child.Child("image_url").Value.ToString();

                            resultText.text = $"<b>Recipe:</b> {name}\n\n<b>Ingredients:</b>\n{ingredients}\n\n<b>Directions:</b>\n{directions}";

                            StartCoroutine(LoadImage(imageUrl));
                            break;
                        }
                    }

                    if (!found)
                    {
                        resultText.text = "Recipe not found!";
                    }
                }
            });
    }

    IEnumerator LoadImage(string url)
    {
        using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(url))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Texture2D texture = DownloadHandlerTexture.GetContent(request);
                recipeImage.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            }
            else
            {
                Debug.LogError("Failed to load image: " + request.error);
            }
        }
    }
}
