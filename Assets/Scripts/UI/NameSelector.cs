using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
public class NameSelector : MonoBehaviour
{
    [SerializeField] private TMP_InputField nameField;
    [SerializeField] private Button connectButton;
    [SerializeField] private int minNameLength = 1;
    [SerializeField] private int maxNameLength = 12; // same check for length should be done on server side as well as client

    public const string playerNameKey = "playerName";
    void Start()
    {
        // let dedicated servers skip this scene
        if(SystemInfo.graphicsDeviceType == GraphicsDeviceType.Null)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            return;
        }


        // if player has played before load up previous name, by loading name form playerprefs
        nameField.text = PlayerPrefs.GetString(playerNameKey, string.Empty);
        HandleNameChanged();
       
        
    }

    // if length in range enable connect button otherwise disable it
    public void HandleNameChanged()
    {
        connectButton.interactable =
            nameField.text.Length >= minNameLength &&
            nameField.text.Length <= maxNameLength;
    }

    // called when click connect set the player prefs, save name and go to next scene.  better to do dynamically
    public void Connect()
    {
        PlayerPrefs.SetString(playerNameKey, nameField.text);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);  
    }
}
