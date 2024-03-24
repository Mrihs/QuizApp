using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;


public class Questions : MonoBehaviourPunCallbacks
{
    ///////////////////////// Input Fields /////////////////////////
    ////////// Player Infos //////////
    [Header("Player Fields")]
    [Tooltip("The Text for the Players' Name")]
    public TMP_Text playerNameText;
    [Tooltip("The Text for the Players' Score")]
    public TMP_Text playerScoreText;
    [Tooltip("The Text for the Opponents' Name")]
    public TMP_Text opponentNameText;
    [Tooltip("The Text for the Opponents' Score")]
    public TMP_Text opponentScoreText;


    ////////// Lobby Infos //////////
    [Header("Lobby Information")]
    [Tooltip("The Text for the Players' Lobby")]
    public TMP_Text playerLobbyText;
    [Tooltip("The Text for the Category")]
    public TMP_Text categoryText;


    ////////// Question Objects //////////
    [Header("Question Objects")]
    [Tooltip("The Pannel for the Questions")]
    public GameObject questionPannel;
    [Tooltip("The Text for the current Question")]
    public TMP_Text questionText;
    [Tooltip("The Text for the Questions' Number")]
    public TMP_Text questionCounter;


    ////////// Question Fields //////////
    [Header("Question Fields")]
    [Tooltip("The Text for Button A")]
    public TMP_Text buttonTextA;
    [Tooltip("The Text for Button B")]
    public TMP_Text buttonTextB;
    [Tooltip("The Text for Button C")]
    public TMP_Text buttonTextC;
    [Tooltip("The Text for Button D")]
    public TMP_Text buttonTextD;
    [Tooltip("The Button Element for Button A")]
    public GameObject buttonA;
    [Tooltip("The Button Element for Button B")]
    public GameObject buttonB;
    [Tooltip("The Button Element for Button C")]
    public GameObject buttonC;
    [Tooltip("The Button Element for Button D")]
    public GameObject buttonD;


    ////////// Animation Settings //////////
    [Header("Animation Settings")]
    [Tooltip("The Duration of the Fading-Animation")]
    public float FadeDuration = 0.5f;


    ////////// Buttons //////////
    [Header("Button Colors")]
    [Tooltip("The Default Color of the Question Panel")]
    public Color defaultQuestionPanelColor;
    [Tooltip("The Default Button Color")]
    public Color defaultButtonColor;
    [Tooltip("Buttons' color for correct answer")]
    public Color correctButtonColor;
    [Tooltip("Buttons' color after wrong answer given")]
    public Color wrongButtonColor;
    [Tooltip("Buttons' text Color")]
    public Color buttonTextColor;


    ////////// Questions //////////
    [Header("Questions")]
    [Tooltip("The CSV-File with the Questions")]
    public TextAsset csvFile;


    ////////// Debug Infos //////////
    [Header("Debugs")]
    [Tooltip("A Text-Efelemt, which displays the log infos")]
    public TMP_Text debugText;





    ////////// Question Data //////////
    public struct QuestionData
    {
        public string category;
        public string question;
        public string option1;
        public string option2;
        public string option3;
        public string option4;
        public int correct;
    }
    public List<QuestionData> questionDataList = new List<QuestionData>();





    //////////////////// Private Variables ////////////////////
    // A Variable for the Players' Name
    private string playerName;
    // A Variable for the Players' Score
    private int playerScore;
    // A Variable for the selected Category
    private string Category;
    // The internal code of the lobby
    private string lobbyCodeName;
    // A variable for the opponents' name
    private string opponentName;
    // A variable for the score of the opponenent
    private int opponentScore;
    // A Variable for the Players' Lobby
    private string lobbyName;
    // The Text for the questionCounter
    private string questionNumberText;
    // A Variable for the question Count
    private int questionNumber = 1;
    // A Variable for the current Question-Index
    private int currentQuestionIndex = 0;
    // List of Players in a room
    private List<Player> playersInRoom = new List<Player>();














    ///////////////////////// At Start of Scene /////////////////////////
    public void Start()
    {
        //////////////////// Player Preferences ////////////////////
        // Load Players' Name from PlayerPrefs if the key exists
        if (PlayerPrefs.HasKey("PlayerName"))
        { playerName = PlayerPrefs.GetString("PlayerName"); }
        else //Name Players' Name if no key is defined in the PlayerPrefs
        { playerName = "Unknown Player"; }

        // Load Players' Lobby Name from PlayerPrefs if the key exists
        if (PlayerPrefs.HasKey("LobbyName"))
        { lobbyName = PlayerPrefs.GetString("LobbyName"); }
        else //Name Players' Lobby if no key is defined in the PlayerPrefs
        { lobbyName = "Unknown Lobby"; }

        // Load Players' Lobby Name from PlayerPrefs if the key exists
        if (PlayerPrefs.HasKey("Category"))
        { Category = PlayerPrefs.GetString("Category"); }
        else //Name Players' Lobby if no key is defined in the PlayerPrefs
        { Category = "Test"; }





        //////////////////// Define Values ////////////////////
        // Set the Playerscore to zero
        playerScore = 0;
        // Set the Question-Number to 1
        questionNumber = 1;

        //Create the CodeName for the Lobby
        lobbyCodeName = lobbyName + Category;

        // Connect to Photon server
        PhotonNetwork.ConnectUsingSettings();





        //////////////////// Get Questions ////////////////////
        // Read the CSV-File and append the data to the question-list
        LoadCSV(csvFile.text, Category);





        //////////////////// Prepare first Question ////////////////////
        ////////// Define texts for first questions and answers //////////
        if (questionDataList.Count > 0)
        {
            questionText.text = questionDataList[currentQuestionIndex].question;
            buttonTextA.text = questionDataList[currentQuestionIndex].option1;
            buttonTextB.text = questionDataList[currentQuestionIndex].option2;
            buttonTextC.text = questionDataList[currentQuestionIndex].option3;
            buttonTextD.text = questionDataList[currentQuestionIndex].option4;
        }
        else //Create a Log-Error if no Question is available
        { Debug.LogError("No questions available!"); }




        ////////// Define Colors for Text and Buttons //////////
        ///// Color for Buttons
        buttonA.GetComponent<Image>().color = defaultButtonColor;
        buttonB.GetComponent<Image>().color = defaultButtonColor;
        buttonC.GetComponent<Image>().color = defaultButtonColor;
        buttonD.GetComponent<Image>().color = defaultButtonColor;

        ///// Color for Text
        questionText.color = buttonTextColor;
        buttonTextA.color = buttonTextColor;
        buttonTextB.color = buttonTextColor;
        buttonTextC.color = buttonTextColor;
        buttonTextD.color = buttonTextColor;
        questionPannel.GetComponent<Image>().color = defaultQuestionPanelColor;
    }














    ///////////////////////// At every Frame /////////////////////////
    public void Update()
    {
        ////////// Photon //////////
        // Update the list of players in the lobby
        UpdatePlayersInRoom();

        // For every player in the lobby
        foreach (Player player in playersInRoom)
        {
            // Create a Log with the players Nickname
            Debug.Log("Player in room: "+ player.NickName);
        }

        // Create a Debug for the PhotonNetwork Connection
        //Debug.Log("Photon Network Connected: " + PhotonNetwork.IsConnected);
        // Create a Debug for the Lobby-Connection
        //Debug.Log("Photon InRoom: " + PhotonNetwork.InRoom);





        ////////// Update Variables //////////
        // Update the text of playerNameText
        playerNameText.text = playerName;
        // Update the text of playerScoreText
        playerScoreText.text = playerScore.ToString();
        // Update the text of playerLobbyText
        playerLobbyText.text = lobbyName;
        // Update the text of categoryText
        categoryText.text = Category;
        // Update the text for the Question-Counter
        questionCounter.text = "Frage " + questionNumber.ToString();


        // Update the text of Opoponents name
        opponentNameText.text = opponentName;

    }













    ///////////////////////// Button Functions /////////////////////////
    ////////// If the Back-Button is pressedd //////////
    public void BackButtonPressed()
    {
        // Leave the Photon room
        PhotonNetwork.LeaveRoom();

        //Load Scene Number 0
        // SceneManager.LoadScene(0);
    }




    ////////// If Button A is pressed //////////
    public void buttonPressedA()
    { // Start the Coroutine FadeOutButtons with Answer-Number 1
        StartCoroutine(FadeOutButtons(FadeDuration, 1));
    }

    ////////// If Button B is pressed //////////
    public void buttonPressedB()
    { // Start the Coroutine FadeOutButtons with Answer-Number 2
        StartCoroutine(FadeOutButtons(FadeDuration, 2));
    }

    ////////// If Button C is pressed //////////
    public void buttonPressedC()
    { // Start the Coroutine FadeOutButtons with Answer-Number 3
        StartCoroutine(FadeOutButtons(FadeDuration, 3));
    }

    ////////// If Button D is pressed //////////
    public void buttonPressedD()
    { // Start the Coroutine FadeOutButtons with Answer-Number 4
        StartCoroutine(FadeOutButtons(FadeDuration, 4));
    }













    ///////////////////////// When Answer is given /////////////////////////
    void AnswerGiven(int givenAnswer)
    {
        // Increase the Number of Questions
        questionNumber += 1;

        // If the current answer was corret
        if (givenAnswer == questionDataList[currentQuestionIndex].correct)
        { playerScore += 10; } // Add 10 points to the current score


        // Increase the Index-Number for the current Question 
        currentQuestionIndex += 1;
        // If the Index-Number is higher than the number of question
        if (currentQuestionIndex >= questionDataList.Count)
        { //reset the Index-Number to 0
            currentQuestionIndex = 0;
        }





        ////////// Replace Text //////////
        // Replace the text of the question
        questionText.text = questionDataList[currentQuestionIndex].question;
        // Replace the text of the answer-buttons
        buttonTextA.text = questionDataList[currentQuestionIndex].option1;
        buttonTextB.text = questionDataList[currentQuestionIndex].option2;
        buttonTextC.text = questionDataList[currentQuestionIndex].option3;
        buttonTextD.text = questionDataList[currentQuestionIndex].option4;
    }













    ///////////////////////// Fade Buttons out /////////////////////////
    IEnumerator FadeOutButtons(float duration, int buttonID)
    {
        ////////// Define start Colors //////////
        // Define start Colors for the buttons
        Color startButtonAColor = defaultButtonColor;
        Color startButtonBColor = defaultButtonColor;
        Color startButtonCColor = defaultButtonColor;
        Color startButtonDColor = defaultButtonColor;

        // Define the start Colors for the text
        Color startTextColor = buttonTextA.color;




        ////////// Define the value for the timer
        float startTime = Time.time;




        ////////// If a correct answer is given //////////
        if (buttonID == questionDataList[currentQuestionIndex].correct)
        {
            // Create a debug-Log for the correct answer
            //Debug.Log("Correct Answer!");

            // Change the Color of the respective button
            if (buttonID == 1) { startButtonAColor = correctButtonColor; }
            if (buttonID == 2) { startButtonBColor = correctButtonColor; }
            if (buttonID == 3) { startButtonCColor = correctButtonColor; }
            if (buttonID == 4) { startButtonDColor = correctButtonColor; }
        }




        ////////// If a wrong answer is given //////////
        if (buttonID != questionDataList[currentQuestionIndex].correct)
        {
            // Create a debug-Log for the wrong answer
            //Debug.Log("Wrong Answer!");

            // Change the Color of the respective button for the wrong answer
            if (buttonID == 1) { startButtonAColor = wrongButtonColor; }
            if (buttonID == 2) { startButtonBColor = wrongButtonColor; }
            if (buttonID == 3) { startButtonCColor = wrongButtonColor; }
            if (buttonID == 4) { startButtonDColor = wrongButtonColor; }

            // Change the Color of the button for the correct answer
            if (questionDataList[currentQuestionIndex].correct == 1) { startButtonAColor = correctButtonColor; }
            if (questionDataList[currentQuestionIndex].correct == 2) { startButtonBColor = correctButtonColor; }
            if (questionDataList[currentQuestionIndex].correct == 3) { startButtonCColor = correctButtonColor; }
            if (questionDataList[currentQuestionIndex].correct == 4) { startButtonDColor = correctButtonColor; }
        }




        ////////// During the animation //////////
        while (Time.time - startTime < duration)
        {
            // calculate the time
            float t = (Time.time - startTime) / duration;

            // Adjust the colors
            Color newTextColor = new Color(startTextColor.r, startTextColor.g, startTextColor.b, Mathf.Lerp(startTextColor.a, 0f, t));
            Color newButtonAColor = new Color(startButtonAColor.r, startButtonAColor.g, startButtonAColor.b, Mathf.Lerp(startButtonAColor.a, 0f, t));
            Color newButtonBColor = new Color(startButtonBColor.r, startButtonBColor.g, startButtonBColor.b, Mathf.Lerp(startButtonBColor.a, 0f, t));
            Color newButtonCColor = new Color(startButtonCColor.r, startButtonCColor.g, startButtonCColor.b, Mathf.Lerp(startButtonCColor.a, 0f, t));
            Color newButtonDColor = new Color(startButtonDColor.r, startButtonDColor.g, startButtonDColor.b, Mathf.Lerp(startButtonDColor.a, 0f, t));
            Color newQuestionPannelColor = new Color(defaultQuestionPanelColor.r, defaultQuestionPanelColor.g, defaultQuestionPanelColor.b, Mathf.Lerp(defaultQuestionPanelColor.a, 0f, t));


            // Update the Colors
            questionPannel.GetComponent<Image>().color = newQuestionPannelColor; // Update Panel-Colors
            buttonA.GetComponent<Image>().color = newButtonAColor; // Update Button-Colors
            buttonB.GetComponent<Image>().color = newButtonBColor; // Update Button-Colors
            buttonC.GetComponent<Image>().color = newButtonCColor; // Update Button-Colors
            buttonD.GetComponent<Image>().color = newButtonDColor; // Update Button-Colors
            questionText.color = newTextColor; // Update Text-Colors
            buttonTextA.color = newTextColor; // Update Text-Colors
            buttonTextB.color = newTextColor; // Update Text-Colors
            buttonTextC.color = newTextColor; // Update Text-Colors
            buttonTextD.color = newTextColor; // Update Text-Colors
            questionCounter.color = newTextColor; // Update Text-Colors


            // Wait a moment
            yield return null;
        }




        ////////// Replace the final colors //////////
        questionPannel.GetComponent<Image>().color = new Color(defaultQuestionPanelColor.r, defaultQuestionPanelColor.g, defaultQuestionPanelColor.b, 0f); // Update Panel-Colors
        buttonA.GetComponent<Image>().color = new Color(startButtonAColor.r, startButtonAColor.g, startButtonAColor.b, 0f); // Update Button-Colors
        buttonB.GetComponent<Image>().color = new Color(startButtonBColor.r, startButtonBColor.g, startButtonBColor.b, 0f); // Update Button-Colors
        buttonC.GetComponent<Image>().color = new Color(startButtonCColor.r, startButtonCColor.g, startButtonCColor.b, 0f); // Update Button-Colors
        buttonD.GetComponent<Image>().color = new Color(startButtonDColor.r, startButtonDColor.g, startButtonDColor.b, 0f); // Update Button-Colors
        questionText.color = new Color(startTextColor.r, startTextColor.g, startTextColor.b, 0f); // Update Text-Colors
        buttonTextA.color =  new Color(startTextColor.r, startTextColor.g, startTextColor.b, 0f); // Update Text-Colors
        buttonTextB.color = new Color(startTextColor.r, startTextColor.g, startTextColor.b, 0f); // Update Text-Colors
        buttonTextC.color = new Color(startTextColor.r, startTextColor.g, startTextColor.b, 0f); // Update Text-Colors
        buttonTextD.color = new Color(startTextColor.r, startTextColor.g, startTextColor.b, 0f); // Update Text-Colors
        questionCounter.color = new Color(startTextColor.r, startTextColor.g, startTextColor.b, 0f); // Update Text-Colors




        ////////// Start other functions //////////
        //Start function "AnswerGiver" with the button ID
        AnswerGiven(buttonID);

        //Start function "FadeInButtons"
        StartCoroutine(FadeInButtons(FadeDuration));
    }













    ///////////////////////// Fade Buttons in /////////////////////////
    IEnumerator FadeInButtons(float dur)
    {
        ////////// Define start Colors //////////
        // Define start Colors for the buttons
        Color startButtonColor = defaultButtonColor;
        // Define start Colors for the text
        Color startTextColor = buttonTextA.color;




        ////////// Define the value for the timer
        float startTime = Time.time;




        ////////// During the animation //////////
        while (Time.time - startTime < dur)
        {
            //Calculate the time
            float t = (Time.time - startTime) / dur;

            // Adjust the colors
            Color newButtonColor = new Color(startButtonColor.r, startButtonColor.g, startButtonColor.b, Mathf.Lerp(0f, 1f, t));
            Color newTextColor = new Color(buttonTextColor.r, buttonTextColor.g, buttonTextColor.b, Mathf.Lerp(0f, 1f, t));
            Color newQuestionPannelColor = new Color(defaultQuestionPanelColor.r, defaultQuestionPanelColor.g, defaultQuestionPanelColor.b, Mathf.Lerp(0f, 1f, t));


            // Update the Colors
            questionPannel.GetComponent<Image>().color = newQuestionPannelColor; // Update Questionpannel-Color
            buttonA.GetComponent<Image>().color = newButtonColor; // Update Button-Color
            buttonB.GetComponent<Image>().color = newButtonColor; // Update Button-Color
            buttonC.GetComponent<Image>().color = newButtonColor; // Update Button-Color
            buttonD.GetComponent<Image>().color = newButtonColor; // Update Button-Color
            questionText.color = newTextColor; // Update Text-Colors
            buttonTextA.color = newTextColor; // Update Text-Colors
            buttonTextB.color = newTextColor; // Update Text-Colors
            buttonTextC.color = newTextColor; // Update Text-Colors
            buttonTextD.color = newTextColor; // Update Text-Colors
            questionCounter.color = newTextColor; // Update Text-Colors

            // Wait a moment
            yield return null;
        }




        ////////// Replace the final colors //////////
        questionPannel.GetComponent<Image>().color = new Color(defaultQuestionPanelColor.r, defaultQuestionPanelColor.g, defaultQuestionPanelColor.b, 1f); // Update Questionpannel-Colors
        buttonA.GetComponent<Image>().color = new Color(defaultButtonColor.r, defaultButtonColor.g, defaultButtonColor.b, 1f); // Update Button-Colors
        buttonB.GetComponent<Image>().color = new Color(defaultButtonColor.r, defaultButtonColor.g, defaultButtonColor.b, 1f); // Update Button-Colors
        buttonC.GetComponent<Image>().color = new Color(defaultButtonColor.r, defaultButtonColor.g, defaultButtonColor.b, 1f); // Update Button-Colors
        buttonD.GetComponent<Image>().color = new Color(defaultButtonColor.r, defaultButtonColor.g, defaultButtonColor.b, 1f); // Update Button-Colors
        questionText.color = new Color(buttonTextColor.r, buttonTextColor.g, buttonTextColor.b, 1f); // Update Text-Colors
        buttonTextA.color = new Color(buttonTextColor.r, buttonTextColor.g, buttonTextColor.b, 1f); // Update Text-Colors
        buttonTextB.color = new Color(buttonTextColor.r, buttonTextColor.g, buttonTextColor.b, 1f); // Update Text-Colors
        buttonTextC.color = new Color(buttonTextColor.r, buttonTextColor.g, buttonTextColor.b, 1f); // Update Text-Colors
        buttonTextD.color = new Color(buttonTextColor.r, buttonTextColor.g, buttonTextColor.b, 1f); // Update Text-Colors
        questionCounter.color = new Color(buttonTextColor.r, buttonTextColor.g, buttonTextColor.b, 1f); // Update Text-Colors
    }













    ///////////////////////// Load CSV File /////////////////////////
    void LoadCSV(string fileName, string Category)
    {
        ////////// Define Variables //////////
        // Create a variable for the lines
        string[] lines = fileName.Split('\n');

        // Create a variable to mark the first line
        bool isFirstLine = true;




        ////////// For every line //////////
        foreach (string line in lines)
        {
            // if the variable for the first line is true (line)
            if (isFirstLine)
            {
                isFirstLine = false; // Set the variable for the first line to false
                continue; // Skip the first line
            }
            // Split the CSV into fields by a semicolomn
            string[] fields = line.Split(';');

            // Check if the CSV has 7 columns
            if (fields.Length == 7)
            {
                // Only look at questions from the selected Category
                if(fields[0] == Category)
                {
                    // Create a new QuestionData
                    QuestionData question = new QuestionData();

                    // Save the Category based on the value in the first cell
                    question.category = fields[0];

                    // Save the Question based on the value in the second cell
                    question.question = fields[1];

                    // Create a new list with the answer-options
                    List<string> options = new List<string> { fields[2], fields[3], fields[4], fields[5] };

                    // Shuffle the answer-Options
                    Shuffle(options);

                    // Save the Answer-Options
                    question.option1 = options[0];
                    question.option2 = options[1];
                    question.option3 = options[2];
                    question.option4 = options[3];

                    // Parse the integer-value for the correct answer
                    int.TryParse(fields[6], out question.correct);

                    // Adjust the correc-value based on the new position of the first option
                    question.correct = options.IndexOf(fields[2])+1;

                    // Add the new question to the questionDataList
                    questionDataList.Add(question);
                }

            }
            // Create an error-message if the CSV has not the correct amount of columns
            else { Debug.LogError("Invalid line in CSV: " + line); }
        }
    }




    ////////// Function to shuffle values //////////
    void Shuffle<T>(IList<T> list)
    {
        System.Random rng = new System.Random();
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }













    ///////////////////////// Photon-Functions /////////////////////////

    ////////// When Connected to the Master //////////
    public override void OnConnectedToMaster()
    {
        // run the base function on connected to master
        base.OnConnectedToMaster();

        // Create a log
        Debug.Log("Connected to Photon");

        // Save the playerName as Nickname in the PhotonNetwork
        PhotonNetwork.NickName = playerName;

        // Check if lobby with the lobbyCodeName already exists and either join or create it
        PhotonNetwork.JoinOrCreateRoom(lobbyCodeName, new Photon.Realtime.RoomOptions { MaxPlayers = 2 }, null);
    }





    ////////// When joined to a room //////////
    public override void OnJoinedRoom()
    {
        // Create a log including the Name of the lobby
        Debug.Log("Joined lobby: " + PhotonNetwork.CurrentRoom.Name);

        // Save the List of players in the network
        Player[] players = PhotonNetwork.PlayerList;

        // For every player in the players-list
        foreach (Player player in players)
        {
            // Look for the other player (the one who is not local0
            if (!player.IsLocal)
            {
                // Save the opponents Nickname as variable
                opponentName = player.NickName;
                //opponentNameText.text = opponentName;

                // finish the loop when another player is found
                break;
            }
        }
    }





    ////////// When connection to Photon failed //////////
    public void OnFailedToConnectToPhoton(object parameters)
    {
        // Create a log including the message from Photon
        Debug.Log("OnFailedToConnectToPhoton. StatusCode: " + parameters + " ServerAddress: " + PhotonNetwork.ServerAddress);
    }





    ////////// When Joining to a room failed //////////
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        // Create a log including the message from Photon
        Debug.Log("Failed to join lobby: " + message);
    }





    ////////// When Leaving a room //////////
    public override void OnLeftRoom()
    {
        // After leaving the room, load Scene Number 0
        SceneManager.LoadScene(0);
    }





    ////////// Update the players in the room //////////
    public void UpdatePlayersInRoom()
    {
        // Clear the current list of players in the room
        playersInRoom.Clear();

        // Recreate the list of Players in the room
        playersInRoom.AddRange(PhotonNetwork.PlayerList.Where(player => !player.IsLocal));
    }





    ////////// When a Player entered the room //////////
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        // Create a log
        Debug.Log("A new player has entered the room.");

        // Check if the new player is not the local player
        if (!newPlayer.IsLocal)
        {
            // Save the nickname of the other player as opponentname
            opponentName = newPlayer.NickName;
            //opponentNameText.text = opponentName;
        }
    }
}