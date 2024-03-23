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
    [Header("Player Fields")]
    [Tooltip("The Text for the Players' Name")]
    public TMP_Text playerNameText;
    [Tooltip("The Text for the Players' Score")]
    public TMP_Text playerScoreText;
    [Tooltip("The Text for the Opponents' Name")]
    public TMP_Text opponentNameText;
    [Tooltip("The Text for the Opponents' Score")]
    public TMP_Text opponentScoreText;
    [Tooltip("The Text for the Players' Lobby")]
    public TMP_Text playerLobbyText;
    [Tooltip("The Text for the Category")]
    public TMP_Text categoryText;
    [Tooltip("The Text for the Questions' Number")]
    public TMP_Text questionCounter;

    [Header("Question Fields")]
    [Tooltip("The Pannel for the Questions")]
    public GameObject questionPannel;
    [Tooltip("The Text for the current Question")]
    public TMP_Text questionText;
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

    [Tooltip("A Text-Efelemt, which displays the log infos")]
    public TMP_Text debugText;


    [Header("Questions")]
    [Tooltip("The CSV-File with the Questions")]
    public TextAsset csvFile;


    [HideInInspector]
    public string playerName; // A Variable for the Players' Name
    [HideInInspector]
    public int playerScore; // A Variable for the Players' Score
    [HideInInspector]
    public string Category; // A Variable for the selected Category
    [HideInInspector]
    public string lobbyName; // A Variable for the Players' Lobby
    [HideInInspector]
    public string questionNumberText; // The Text for the questionCounter
    public int questionNumber = 1; // A Variable for the question Count

    private string lobbyCodeName;
    private string opponentName;
    private int opponentScore;

    // Structure for the Question-Data
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

    private int currentQuestionIndex = 0; // A Variable for the current Question-Index

    private List<Player> playersInRoom = new List<Player>(); // Liste der Spieler im Raum



    public void Start()
    {
        playerScore = 0;
        questionNumber = 1;

        // Load Players' Name from PlayerPrefs if the key exists
        if (PlayerPrefs.HasKey("PlayerName"))
            {playerName = PlayerPrefs.GetString("PlayerName");}
        else //Name Players' Name if no key is defined in the PlayerPrefs
            {playerName = "Unknown Player";}

        // Load Players' Lobby Name from PlayerPrefs if the key exists
        if (PlayerPrefs.HasKey("LobbyName"))
            {lobbyName = PlayerPrefs.GetString("LobbyName");}
        else //Name Players' Lobby if no key is defined in the PlayerPrefs
            {lobbyName = "Unknown Lobby";}

        // Load Players' Lobby Name from PlayerPrefs if the key exists
        if (PlayerPrefs.HasKey("Category"))
        { Category = PlayerPrefs.GetString("Category"); }
        else //Name Players' Lobby if no key is defined in the PlayerPrefs
        { Category = "Test"; }

        //Create the CodeName for the Lobby
        lobbyCodeName = lobbyName + Category;

        // Connect to Photon server
        PhotonNetwork.ConnectUsingSettings();

        // Lese das CSV-Asset und fülle die Frage-Datenliste
        LoadCSV(csvFile.text, Category);



        ////Set the Question
        if (questionDataList.Count > 0)
        {
            questionText.text = questionDataList[currentQuestionIndex].question;
            buttonTextA.text = questionDataList[currentQuestionIndex].option1;
            buttonTextB.text = questionDataList[currentQuestionIndex].option2;
            buttonTextC.text = questionDataList[currentQuestionIndex].option3;
            buttonTextD.text = questionDataList[currentQuestionIndex].option4;
        }
        else
        { Debug.LogError("No questions available!"); } //Create an Error-Message when No Question is available


        buttonA.GetComponent<Image>().color = defaultButtonColor;
        buttonB.GetComponent<Image>().color = defaultButtonColor;
        buttonC.GetComponent<Image>().color = defaultButtonColor;
        buttonD.GetComponent<Image>().color = defaultButtonColor;

        questionText.color = buttonTextColor;
        buttonTextA.color = buttonTextColor;
        buttonTextB.color = buttonTextColor;
        buttonTextC.color = buttonTextColor;
        buttonTextD.color = buttonTextColor;
        questionPannel.GetComponent<Image>().color = defaultQuestionPanelColor;
    }


    public void Update()
    {
        // Aktualisiere die Liste der Spieler im Raum
        UpdatePlayersInRoom();

        // Durchlaufe die Liste der Spieler und gib eine Benachrichtigung für jeden Spieler aus
        foreach (Player player in playersInRoom)
        {
            Debug.Log("Player in room: "+ player.NickName);
            // Hier kannst du weitere Aktionen für jeden Spieler ausführen, wenn nötig
        }

        //Debug.Log("Photon Network Connected: " + PhotonNetwork.IsConnected);
        //Debug.Log("Photon InRoom: " + PhotonNetwork.InRoom);
        //Assign Variables to the names
        playerNameText.text = playerName;
        playerScoreText.text = playerScore.ToString();
        playerLobbyText.text = lobbyName;
        categoryText.text = Category;

        opponentNameText.text = opponentName;


        questionCounter.text = "Frage " + questionNumber.ToString();
    }








///////////////////////// Button Functions /////////////////////////
public void BackButtonPressed()
    {
        // Leave Photon room
        PhotonNetwork.LeaveRoom();

        //Load Scene Number 0
        SceneManager.LoadScene(0);
    }


    public void buttonPressedA()
    {
        StartCoroutine(FadeOutButtons(1, 1));
    }


    public void buttonPressedB()
    {
        StartCoroutine(FadeOutButtons(1, 2));
    }


    public void buttonPressedC()
    {
        StartCoroutine(FadeOutButtons(1, 3));
    }


    public void buttonPressedD()
    {
        StartCoroutine(FadeOutButtons(1, 4));
    }


    void AnswerGiven(int givenAnswer)
    {
        questionNumber += 1;

        if (givenAnswer == questionDataList[currentQuestionIndex].correct)
        {
            playerScore += 10;
        }

        currentQuestionIndex += 1;
        // Überprüfen, ob currentQuestionIndex außerhalb des gültigen Bereichs liegt
        if (currentQuestionIndex >= questionDataList.Count)
        {
            currentQuestionIndex = 0; // Zurücksetzen auf 0, wenn außerhalb des Bereichs
        }
        questionText.text = questionDataList[currentQuestionIndex].question;
        buttonTextA.text = questionDataList[currentQuestionIndex].option1;
        buttonTextB.text = questionDataList[currentQuestionIndex].option2;
        buttonTextC.text = questionDataList[currentQuestionIndex].option3;
        buttonTextD.text = questionDataList[currentQuestionIndex].option4;
    }





    IEnumerator FadeOutButtons(float duration, int buttonID)
    {
        Color startButtonAColor = defaultButtonColor;
        Color startButtonBColor = defaultButtonColor;
        Color startButtonCColor = defaultButtonColor;
        Color startButtonDColor = defaultButtonColor;

        Color startTextColor = buttonTextA.color;
        float startTime = Time.time;


        if (buttonID == questionDataList[currentQuestionIndex].correct)
        {
            //Debug.Log("Correct Answer!");
            if (buttonID == 1) { startButtonAColor = correctButtonColor; }
            if (buttonID == 2) { startButtonBColor = correctButtonColor; }
            if (buttonID == 3) { startButtonCColor = correctButtonColor; }
            if (buttonID == 4) { startButtonDColor = correctButtonColor; }
        }

        if (buttonID != questionDataList[currentQuestionIndex].correct)
        {
            //Debug.Log("Wrong Answer!");
            if (buttonID == 1) { startButtonAColor = wrongButtonColor; }
            if (buttonID == 2) { startButtonBColor = wrongButtonColor; }
            if (buttonID == 3) { startButtonCColor = wrongButtonColor; }
            if (buttonID == 4) { startButtonDColor = wrongButtonColor; }

            if (questionDataList[currentQuestionIndex].correct == 1) { startButtonAColor = correctButtonColor; }
            if (questionDataList[currentQuestionIndex].correct == 2) { startButtonBColor = correctButtonColor; }
            if (questionDataList[currentQuestionIndex].correct == 3) { startButtonCColor = correctButtonColor; }
            if (questionDataList[currentQuestionIndex].correct == 4) { startButtonDColor = correctButtonColor; }
        }


        while (Time.time - startTime < duration)
        {
            float t = (Time.time - startTime) / duration;
            // Direkte Verwendung des Alpha-Werts aus Lerp
            Color newTextColor = new Color(startTextColor.r, startTextColor.g, startTextColor.b, Mathf.Lerp(startTextColor.a, 0f, t));
            Color newButtonAColor = new Color(startButtonAColor.r, startButtonAColor.g, startButtonAColor.b, Mathf.Lerp(startButtonAColor.a, 0f, t));
            Color newButtonBColor = new Color(startButtonBColor.r, startButtonBColor.g, startButtonBColor.b, Mathf.Lerp(startButtonBColor.a, 0f, t));
            Color newButtonCColor = new Color(startButtonCColor.r, startButtonCColor.g, startButtonCColor.b, Mathf.Lerp(startButtonCColor.a, 0f, t));
            Color newButtonDColor = new Color(startButtonDColor.r, startButtonDColor.g, startButtonDColor.b, Mathf.Lerp(startButtonDColor.a, 0f, t));

            Color newQuestionPannelColor = new Color(defaultQuestionPanelColor.r, defaultQuestionPanelColor.g, defaultQuestionPanelColor.b, Mathf.Lerp(defaultQuestionPanelColor.a, 0f, t));


            // Aktualisieren der Farbeinstellungen der Buttons
            questionPannel.GetComponent<Image>().color = newQuestionPannelColor;
            buttonA.GetComponent<Image>().color = newButtonAColor;
            buttonB.GetComponent<Image>().color = newButtonBColor;
            buttonC.GetComponent<Image>().color = newButtonCColor;
            buttonD.GetComponent<Image>().color = newButtonDColor;

            questionText.color = newTextColor;
            buttonTextA.color = newTextColor;
            buttonTextB.color = newTextColor;
            buttonTextC.color = newTextColor;
            buttonTextD.color = newTextColor;
            questionCounter.color = newTextColor;

            yield return null;
        }

        // Sicherstellen, dass die Farbe am Ende des Fades auf vollständig transparent gesetzt wird
        questionPannel.GetComponent<Image>().color = new Color(defaultQuestionPanelColor.r, defaultQuestionPanelColor.g, defaultQuestionPanelColor.b, 0f);
        buttonA.GetComponent<Image>().color = new Color(startButtonAColor.r, startButtonAColor.g, startButtonAColor.b, 0f);
        buttonB.GetComponent<Image>().color = new Color(startButtonBColor.r, startButtonBColor.g, startButtonBColor.b, 0f);
        buttonC.GetComponent<Image>().color = new Color(startButtonCColor.r, startButtonCColor.g, startButtonCColor.b, 0f);
        buttonD.GetComponent<Image>().color = new Color(startButtonDColor.r, startButtonDColor.g, startButtonDColor.b, 0f);
        questionText.color = new Color(startTextColor.r, startTextColor.g, startTextColor.b, 0f);
        buttonTextA.color =  new Color(startTextColor.r, startTextColor.g, startTextColor.b, 0f);
        buttonTextB.color = new Color(startTextColor.r, startTextColor.g, startTextColor.b, 0f);
        buttonTextC.color = new Color(startTextColor.r, startTextColor.g, startTextColor.b, 0f);
        buttonTextD.color = new Color(startTextColor.r, startTextColor.g, startTextColor.b, 0f);
        questionCounter.color = new Color(startTextColor.r, startTextColor.g, startTextColor.b, 0f);

        AnswerGiven(buttonID);

        StartCoroutine(FadeInButtons(1));
    }


    IEnumerator FadeInButtons(int dur)
    {
        Color startButtonColor = defaultButtonColor;
        Color startTextColor = buttonTextA.color;
        float startTime = Time.time;

        while (Time.time - startTime < dur)
        {
            float t = (Time.time - startTime) / dur;
            // Direkte Verwendung des Alpha-Werts aus Lerp
            Color newButtonColor = new Color(startButtonColor.r, startButtonColor.g, startButtonColor.b, Mathf.Lerp(0f, 1f, t));
            Color newTextColor = new Color(buttonTextColor.r, buttonTextColor.g, buttonTextColor.b, Mathf.Lerp(0f, 1f, t));
            Color newQuestionPannelColor = new Color(defaultQuestionPanelColor.r, defaultQuestionPanelColor.g, defaultQuestionPanelColor.b, Mathf.Lerp(0f, 1f, t));            

            // Aktualisieren der Farbeinstellungen der Buttons
            questionPannel.GetComponent<Image>().color = newQuestionPannelColor;
            buttonA.GetComponent<Image>().color = newButtonColor;
            buttonB.GetComponent<Image>().color = newButtonColor;
            buttonC.GetComponent<Image>().color = newButtonColor;
            buttonD.GetComponent<Image>().color = newButtonColor;

            questionText.color = newTextColor;
            buttonTextA.color = newTextColor;
            buttonTextB.color = newTextColor;
            buttonTextC.color = newTextColor;
            buttonTextD.color = newTextColor;
            questionCounter.color = newTextColor;

            yield return null;
        }

        // Sicherstellen, dass die Farbe am Ende des Fades auf vollständig undurchsichtig gesetzt wird
        questionPannel.GetComponent<Image>().color = new Color(defaultQuestionPanelColor.r, defaultQuestionPanelColor.g, defaultQuestionPanelColor.b, 1f);

        buttonA.GetComponent<Image>().color = new Color(defaultButtonColor.r, defaultButtonColor.g, defaultButtonColor.b, 1f);
        buttonB.GetComponent<Image>().color = new Color(defaultButtonColor.r, defaultButtonColor.g, defaultButtonColor.b, 1f);
        buttonC.GetComponent<Image>().color = new Color(defaultButtonColor.r, defaultButtonColor.g, defaultButtonColor.b, 1f);
        buttonD.GetComponent<Image>().color = new Color(defaultButtonColor.r, defaultButtonColor.g, defaultButtonColor.b, 1f);


        questionText.color = new Color(buttonTextColor.r, buttonTextColor.g, buttonTextColor.b, 1f);
        buttonTextA.color = new Color(buttonTextColor.r, buttonTextColor.g, buttonTextColor.b, 1f);
        buttonTextB.color = new Color(buttonTextColor.r, buttonTextColor.g, buttonTextColor.b, 1f);
        buttonTextC.color = new Color(buttonTextColor.r, buttonTextColor.g, buttonTextColor.b, 1f);
        buttonTextD.color = new Color(buttonTextColor.r, buttonTextColor.g, buttonTextColor.b, 1f);
        questionCounter.color = new Color(buttonTextColor.r, buttonTextColor.g, buttonTextColor.b, 1f);
    }





    ///////////////////////// Load CSV File /////////////////////////
    void LoadCSV(string fileName, string Category)
    {
        string[] lines = fileName.Split('\n');
        bool isFirstLine = true; // Flag to skip the first line
        foreach (string line in lines)
        {
            if (isFirstLine)
            {
                isFirstLine = false;
                continue; // Skip the first line
            }
            string[] fields = line.Split(';');
            if (fields.Length == 7)
            {
                if(fields[0] == Category)
                { 
                    QuestionData question = new QuestionData();
                    question.category = fields[0];
                    question.question = fields[1];
                    List<string> options = new List<string> { fields[2], fields[3], fields[4], fields[5] };
                    // Zufällige Umplatzierung der Optionen
                    Shuffle(options);
                    question.option1 = options[0];
                    question.option2 = options[1];
                    question.option3 = options[2];
                    question.option4 = options[3];
                    int.TryParse(fields[6], out question.correct);
                    // Anpassen des 'correct'-Werts basierend auf der neuen Position der ersten Option
                    question.correct = options.IndexOf(fields[2])+1;
                    questionDataList.Add(question);
                }

            }
            else
            {
                Debug.LogError("Invalid line in CSV: " + line);
            }
        }
    }


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








    //public override void OnConnected()
    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();

        Debug.Log("Connected to Photon ");

        PhotonNetwork.NickName = playerName;

        // Check if lobby with the lobbyCodeName already exists
        PhotonNetwork.JoinOrCreateRoom(lobbyCodeName, new Photon.Realtime.RoomOptions { MaxPlayers = 2 }, null);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined lobby: " + PhotonNetwork.CurrentRoom.Name);
        // Implement logic for what to do when joined lobby

        // Rufe die Liste der Spieler im Raum ab
        Player[] players = PhotonNetwork.PlayerList;

        // Durchlaufe die Liste der Spieler und suche nach dem anderen Spieler
        foreach (Player player in players)
        {
            // Überprüfe, ob es sich bei dem Spieler nicht um den lokalen Spieler handelt
            if (!player.IsLocal)
            {
                // Weise den Namen des anderen Spielers der opponentName-Variable zu
                opponentName = player.NickName;
                opponentNameText.text = opponentName;

                // Abrufen des Spielstands des anderen Spielers
                object scoreObject;
                if (player.CustomProperties.TryGetValue("playerScore", out scoreObject))
                {
                    opponentScore = (int)scoreObject;
                    opponentScoreText.text = opponentScore.ToString();
                }

                break; // Beende die Schleife, sobald der andere Spieler gefunden wurde
            }
        }
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log("Failed to join lobby: " + message);
        // Implement logic for what to do if joining lobby fails
    }

    public override void OnLeftRoom()
    {
        // After leaving the room, load Scene Number 0
        SceneManager.LoadScene(0);
    }


    public void OnFailedToConnectToPhoton(object parameters)
    {
        Debug.Log("OnFailedToConnectToPhoton. StatusCode: " + parameters + " ServerAddress: " + PhotonNetwork.ServerAddress);
    }



    public void UpdateOpponentScore(int newOpponentScore)
    {
        opponentScore = newOpponentScore;
        opponentScoreText.text = opponentScore.ToString();
    }



    public void UpdatePlayersInRoom()
    {
        playersInRoom.Clear(); // Lösche die aktuelle Liste

        // Fülle die Liste mit den aktuellen Spielern im Raum
        playersInRoom.AddRange(PhotonNetwork.PlayerList.Where(player => !player.IsLocal));
    }



    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log("A new player has entered the room.");

        // Überprüfe, ob es sich um den neuen Spieler handelt
        if (!newPlayer.IsLocal)
        {
            // Weise den Namen des neuen Spielers der opponentName-Variable zu
            opponentName = newPlayer.NickName;
            opponentNameText.text = opponentName;

            // Hier kannst du weitere Aktualisierungen für den neuen Spieler durchführen, wenn nötig
        }
    }

}