using UnityEngine;
using UnityEngine.Windows.Speech;
using System.Collections.Generic;
using System.IO;

public class DictationEngine : MonoBehaviour
{
    protected DictationRecognizer dictationRecognizer;

    public static bool recording = false;

    private static List<string> records = new List<string>();
    public static int currentIndex = -1;

    private void Awake()
    {
        LoadRecords();
    }

    private void DictationRecognizer_OnDictationHypothesis(string text)
    {
        records[currentIndex] = text;
    }

    private void DictationRecognizer_OnDictationComplete(DictationCompletionCause completionCause)
    {
        switch (completionCause)
        {
            case DictationCompletionCause.TimeoutExceeded:
            case DictationCompletionCause.PauseLimitExceeded:
            case DictationCompletionCause.Canceled:
            case DictationCompletionCause.Complete:
                CloseDictationEngine();
                break;
            case DictationCompletionCause.UnknownError:
            case DictationCompletionCause.AudioQualityFailure:
            case DictationCompletionCause.MicrophoneUnavailable:
            case DictationCompletionCause.NetworkFailure:
                // Error
                CloseDictationEngine();
                break;
        }
    }

    private void DictationRecognizer_OnDictationResult(string text, ConfidenceLevel confidence)
    {
        Debug.Log("Dictation result: " + text);
        records[currentIndex] = text;
        SaveSystem.needsSave = true;
        CloseDictationEngine();
    }

    private void DictationRecognizer_OnDictationError(string error, int hresult)
    {
        Debug.Log("Dictation error: " + error);
    }

    private void OnApplicationQuit()
    {
        CloseDictationEngine();
        SaveRecords();
    }

    public void StartDictationEngine()
    {
        dictationRecognizer = new DictationRecognizer();
        dictationRecognizer.DictationHypothesis += DictationRecognizer_OnDictationHypothesis;
        dictationRecognizer.DictationResult += DictationRecognizer_OnDictationResult;
        dictationRecognizer.DictationComplete += DictationRecognizer_OnDictationComplete;
        dictationRecognizer.DictationError += DictationRecognizer_OnDictationError;
        dictationRecognizer.Start();

        recording = true;
        records.Add("");
        currentIndex++;
    }

    private void CloseDictationEngine()
    {
        if (dictationRecognizer != null)
        {
            dictationRecognizer.DictationHypothesis -= DictationRecognizer_OnDictationHypothesis;
            dictationRecognizer.DictationComplete -= DictationRecognizer_OnDictationComplete;
            dictationRecognizer.DictationResult -= DictationRecognizer_OnDictationResult;
            dictationRecognizer.DictationError -= DictationRecognizer_OnDictationError;
            if (dictationRecognizer.Status == SpeechSystemStatus.Running)
            {
                dictationRecognizer.Stop();
            }
            dictationRecognizer.Dispose();
            recording = false;
        }
    }

    public static string GetRecorded(int i)
    {
        if (i < 0) return "";
        return records[i];
    }

    private void SaveRecords(string fileName = "recordings.txt")
    {
        string path = "Assets/Resources/Saves/" + fileName;
        StreamWriter writer = new StreamWriter(path, false, System.Text.Encoding.UTF8);

        foreach (string line in records)
        {
            print(line);
            writer.WriteLine(line);
        }

        writer.Close();
    }

    private void LoadRecords(string fileName = "recordings.txt")
    {
        string path = "Assets/Resources/Saves/" + fileName;
        StreamReader reader = new StreamReader(path);

        string line;
        while ((line = reader.ReadLine()) != null)
        {
            records.Add(line);
            currentIndex++;
        }
    }
}