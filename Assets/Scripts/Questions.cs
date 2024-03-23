using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class Questions : MonoBehaviour
{
    [Header("Player Fields")]
    [Tooltip("The Text for the Players' Name")]
    public TMP_Text playerNameText;
    [Tooltip("The Text for the Players' Score")]
    public TMP_Text playerScoreText;
    [Tooltip("The Text for the Players' Lobby")]
    public TMP_Text playerLobbyText;

    [Header("Question Fields")]
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
    [Tooltip("The Default Button Color")]
    public Color defaultButtonColor;
    [Tooltip("Buttons' color for correct answer")]
    public Color correctButtonColor;
    [Tooltip("Buttons' color after wrong answer given")]
    public Color wrongButtonColor;
    [Tooltip("Buttons' text Color")]
    public Color buttonTextColor;


    [Header("Questions")]
    [Tooltip("The CSV-File with the Questions")]
    public TextAsset csvFile;


    [HideInInspector]
    public string playerName; // A Variable for the Players' Name
    [HideInInspector]
    public int playerScore; // A Variable for the Players' Score
    [HideInInspector]
    public string lobbyName; // A Variable for the Players' Lobby



    // Structure for the Question-Data
    public struct QuestionData
    {
        public string question;
        public string option1;
        public string option2;
        public string option3;
        public string option4;
        public int correct;
    }
    public List<QuestionData> questionDataList = new List<QuestionData>();

    private int currentQuestionIndex = 0; // A Variable for the current Question-Index


    public void Start()
    {
        playerScore = 0;

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

        // Lese das CSV-Asset und fülle die Frage-Datenliste
        LoadCSV(csvFile.text);




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

    }


    public void Update()
    {
        //Assign Variables to the names
        playerNameText.text = playerName;
        playerScoreText.text = playerScore.ToString();
        playerLobbyText.text = lobbyName;
    }








    ///////////////////////// Button Functions /////////////////////////
    public void BackButtonPressed()
    {
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
            Debug.Log("Correct Answer!");
            if (buttonID == 1) { startButtonAColor = correctButtonColor; }
            if (buttonID == 2) { startButtonBColor = correctButtonColor; }
            if (buttonID == 3) { startButtonCColor = correctButtonColor; }
            if (buttonID == 4) { startButtonDColor = correctButtonColor; }
        }

        if (buttonID != questionDataList[currentQuestionIndex].correct)
        {
            Debug.Log("Wrong Answer!");
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

            // Aktualisieren der Farbeinstellungen der Buttons
            buttonA.GetComponent<Image>().color = newButtonAColor;
            buttonB.GetComponent<Image>().color = newButtonBColor;
            buttonC.GetComponent<Image>().color = newButtonCColor;
            buttonD.GetComponent<Image>().color = newButtonDColor;

            questionText.color = newTextColor;
            buttonTextA.color = newTextColor;
            buttonTextB.color = newTextColor;
            buttonTextC.color = newTextColor;
            buttonTextD.color = newTextColor;

            yield return null;
        }

        // Sicherstellen, dass die Farbe am Ende des Fades auf vollständig transparent gesetzt wird
        buttonA.GetComponent<Image>().color = new Color(startButtonAColor.r, startButtonAColor.g, startButtonAColor.b, 0f);
        buttonB.GetComponent<Image>().color = new Color(startButtonBColor.r, startButtonBColor.g, startButtonBColor.b, 0f);
        buttonC.GetComponent<Image>().color = new Color(startButtonCColor.r, startButtonCColor.g, startButtonCColor.b, 0f);
        buttonD.GetComponent<Image>().color = new Color(startButtonDColor.r, startButtonDColor.g, startButtonDColor.b, 0f);
        questionText.color = new Color(startTextColor.r, startTextColor.g, startTextColor.b, 0f);
        buttonTextA.color =  new Color(startTextColor.r, startTextColor.g, startTextColor.b, 0f);
        buttonTextB.color = new Color(startTextColor.r, startTextColor.g, startTextColor.b, 0f);
        buttonTextC.color = new Color(startTextColor.r, startTextColor.g, startTextColor.b, 0f);
        buttonTextD.color = new Color(startTextColor.r, startTextColor.g, startTextColor.b, 0f);

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

            // Aktualisieren der Farbeinstellungen der Buttons
            buttonA.GetComponent<Image>().color = newButtonColor;
            buttonB.GetComponent<Image>().color = newButtonColor;
            buttonC.GetComponent<Image>().color = newButtonColor;
            buttonD.GetComponent<Image>().color = newButtonColor;

            questionText.color = newTextColor;
            buttonTextA.color = newTextColor;
            buttonTextB.color = newTextColor;
            buttonTextC.color = newTextColor;
            buttonTextD.color = newTextColor;

            yield return null;
        }

        // Sicherstellen, dass die Farbe am Ende des Fades auf vollständig undurchsichtig gesetzt wird
        buttonA.GetComponent<Image>().color = new Color(defaultButtonColor.r, defaultButtonColor.g, defaultButtonColor.b, 1f);
        buttonB.GetComponent<Image>().color = new Color(defaultButtonColor.r, defaultButtonColor.g, defaultButtonColor.b, 1f);
        buttonC.GetComponent<Image>().color = new Color(defaultButtonColor.r, defaultButtonColor.g, defaultButtonColor.b, 1f);
        buttonD.GetComponent<Image>().color = new Color(defaultButtonColor.r, defaultButtonColor.g, defaultButtonColor.b, 1f);


        questionText.color = new Color(buttonTextColor.r, buttonTextColor.g, buttonTextColor.b, 1f);
        buttonTextA.color = new Color(buttonTextColor.r, buttonTextColor.g, buttonTextColor.b, 1f);
        buttonTextB.color = new Color(buttonTextColor.r, buttonTextColor.g, buttonTextColor.b, 1f);
        buttonTextC.color = new Color(buttonTextColor.r, buttonTextColor.g, buttonTextColor.b, 1f);
        buttonTextD.color = new Color(buttonTextColor.r, buttonTextColor.g, buttonTextColor.b, 1f);

    }





    ///////////////////////// Load CSV File /////////////////////////
    void LoadCSV(string fileName)
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
            if (fields.Length == 6)
            {
                QuestionData question = new QuestionData();
                question.question = fields[0];
                List<string> options = new List<string> { fields[1], fields[2], fields[3], fields[4] };
                // Zufällige Umplatzierung der Optionen
                Shuffle(options);
                question.option1 = options[0];
                question.option2 = options[1];
                question.option3 = options[2];
                question.option4 = options[3];
                int.TryParse(fields[5], out question.correct);
                // Anpassen des 'correct'-Werts basierend auf der neuen Position der ersten Option
                question.correct = options.IndexOf(fields[1])+1;
                questionDataList.Add(question);



                Debug.Log("Loaded question: " + question.question +
          " Option1: " + question.option1 +
          " Option2: " + question.option2 +
          " Option3: " + question.option3 +
          " Option4: " + question.option4 +
          " Correct: " + question.correct);
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
}