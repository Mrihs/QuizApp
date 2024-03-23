using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class QuestionLoader : MonoBehaviour
{
    [Header("Questions")]
    [Tooltip("The CSV-File with the Questions")]
    public TextAsset csvFile;

    [Header("Dropdown-Field")]
    [Tooltip("Dropdown Object displaying the Categories")]
    public TMP_Dropdown dropdown;


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



    // Start is called before the first frame update
    void Start()
    {
        LoadCSV(csvFile.text);
        FillDropdownWithCategories();
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
            if (fields.Length == 7)
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
                question.correct = options.IndexOf(fields[2]) + 1;
                questionDataList.Add(question);


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




















    void FillDropdownWithCategories()
    {
        // Extract unique categories from questionDataList
        HashSet<string> categories = new HashSet<string>();
        foreach (QuestionData data in questionDataList)
        {
            categories.Add(data.category);
        }

        // Clear existing options and add unique categories to the dropdown
        dropdown.ClearOptions();
        dropdown.AddOptions(new List<string>(categories));
    }


}
