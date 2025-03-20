using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ReadExcel : MonoBehaviour
{
    public TextAsset textAssetdata;

    public TMP_InputField Textinput;
    private string Text;

    public TextMeshProUGUI Directions;
    public TextMeshProUGUI Ingredients;
    public TextMeshProUGUI Name;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    
    // Update is called once per frame
    void Update()
    {
        Text = Textinput.text;
    }

    public void Search()
    {
        string[] data = textAssetdata.text.Split(new string[] { ";", "\n"}, System.StringSplitOptions.None);

        for (int i = 0; i<data.Length; i++)
        {
            if(Text. ToLower() == data[i]. ToLower())
            {
                Directions.text = data[i+4];
                Ingredients.text = data[i+3];
                Name.text = data[i+1];
            }
        }
    }
}
