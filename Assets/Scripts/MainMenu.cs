using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Tooltip("The Input Field displaying the Players' Name")]
    public TMP_InputField PlayerNameInput;
    [Tooltip("The Input Field displaying the Players's Lobby")]
    public TMP_InputField LobbyNameInput;

    //Private Variable for the Players' Name
    private string PlayerName;
    //Private Variable for the Players' Lobby
    private string LobbyName;


    public void Start()
    {
        // Load Players' Name from PlayerPrefs if the key exists
        if (PlayerPrefs.HasKey("PlayerName"))
        {PlayerNameInput.text = PlayerPrefs.GetString("PlayerName"); }

        // Load Players' Lobby Name from PlayerPrefs if the key exists
        if (PlayerPrefs.HasKey("LobbyName"))
        {LobbyNameInput.text = PlayerPrefs.GetString("LobbyName"); }
    }

    // Function StartButtonPressed
    public void StartButtonPressed()
    {
        //Assing the Test of PlayerNameInput to the Variable PlayerName
        PlayerName = PlayerNameInput.text;
        //Assing the Test of LobbyNameInput to the Variable LobbyName
        LobbyName = LobbyNameInput.text;

        // Save the Players' Name in the PlayerPrefs
        PlayerPrefs.SetString("PlayerName", PlayerName);
        // Save the Players' Lobby in the PlayerPrefs
        PlayerPrefs.SetString("LobbyName", LobbyName);
        // Save the PlayerPrefs
        PlayerPrefs.Save();

        // Load Scene Number 1
        SceneManager.LoadScene(1);
    }
}
