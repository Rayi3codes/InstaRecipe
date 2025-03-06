using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    private void Awake()
    {
        CreateInstance();
    }

    private void CreateInstance()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject); // Prevent duplicate UIManager instances
        }
    }

    public void LoadLoginScene()
    {
        SceneManager.LoadScene("LOGIN"); // Replace with your actual scene name
    }

    public void LoadRegistrationScene()
    {
        SceneManager.LoadScene("REGISTER"); // Replace with your actual scene name
    }
}
