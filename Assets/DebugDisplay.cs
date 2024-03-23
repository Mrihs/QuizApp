using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DebugDisplay : MonoBehaviour
{
    public TMP_Text debugText; // Referenz zum Textfeld in der UI

    void OnEnable()
    {
        // Registriere die Methode OnLogMessageReceived als Listener für Debug-Nachrichten
        Application.logMessageReceived += OnLogMessageReceived;
    }

    void OnDisable()
    {
        // Entferne die Methode OnLogMessageReceived als Listener für Debug-Nachrichten
        Application.logMessageReceived -= OnLogMessageReceived;
    }

    void OnLogMessageReceived(string logMessage, string stackTrace, LogType logType)
    {
        // Überprüfen, ob das Textfeld vorhanden ist
        if (debugText != null)
        {
            // Abhängig vom Log-Typ die Nachricht entsprechend formatieren
            string formattedLog = "[" + logType.ToString() + "] " + logMessage + "\n";

            // Anzeigen der Debug-Nachricht im Textfeld
            debugText.text += formattedLog; // Anhängen an den vorhandenen Text
        }
    }
}
