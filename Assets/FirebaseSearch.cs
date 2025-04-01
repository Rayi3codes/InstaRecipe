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
    public TMP_Text recipeNameText;       // For displaying the recipe name
    public TMP_Text recipeIngredientsText; // For displaying the ingredients
    public TMP_Text recipeDirectionsText;  // For displaying the directions
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
        // Clear previous results
        recipeNameText.text = "";
        recipeIngredientsText.text = "";
        recipeDirectionsText.text = "";
        recipeImage.sprite = null;

        if (string.IsNullOrEmpty(recipeName))
        {
            recipeNameText.text = "Please enter a recipe name.";
            return;
        }

        // Use the root reference since recipes are at the root of the JSON structure
        FirebaseDatabase.DefaultInstance
            .GetReference("")
            .GetValueAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted)
                {
                    recipeNameText.text = "Error fetching data!";
                }
                else if (task.IsCompleted)
                {
                    DataSnapshot snapshot = task.Result;
                    bool found = false;

                    foreach (var child in snapshot.Children)
                    {
                        if (child.Child("recipe_name").Exists && child.Child("recipe_name").Value != null)
                        {
                            string name = child.Child("recipe_name").Value.ToString();

                            if (name.ToLower().Contains(recipeName.ToLower()))
                            {
                                found = true;
                                string ingredients = child.Child("ingredients").Value.ToString();
                                string directions = child.Child("cooking_directions").Value.ToString();
                                string imageUrl = child.Child("image_url").Value.ToString();

                                // Set the separate text fields
                                recipeNameText.text = name;
                                recipeIngredientsText.text = ingredients;
                                recipeDirectionsText.text = directions;

                                StartCoroutine(LoadImage(imageUrl));
                                break;
                            }
                        }
                    }

                    if (!found)
                    {
                        recipeNameText.text = "Recipe not found!";
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
