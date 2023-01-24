using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class CSWriterKoffer : MonoBehaviour
{
    string fileName = "";

    [System.Serializable]
    public class LatencyTestNote
    {
        public float targetDate;
        public float targetUnity;
        public float isDate;
        public float isUnity;
        public string scoreDate;
        public string scoreUnity;

        public LatencyTestNote(float _targetDate, float _targetUnity, float _isDate, float _isUnity, string _scoreDate, string _scoreUnity)
        {
            targetDate = _targetDate;
            targetUnity = _targetUnity;
            isDate = _isDate;
            isUnity = _isUnity;
            scoreDate = _scoreDate;
            scoreUnity = _scoreUnity;
        }
    }

    public List<LatencyTestNote> testNotes;
    [SerializeField] string _fileName = "LatencyKofferphase";
    public List<LatencyTestNote> testFmod;
    string fileFmodName = "LatencyKofferphaseFmod";

    void Start()
    {
        fileName = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "/" + _fileName + ".csv";
        testNotes = new List<LatencyTestNote>();
        testFmod = new List<LatencyTestNote>();
    }

    public void AddNewNote(float _targetDate, float _targetUnity, float _isDate, float _isUnity, string _scoreDate, string _scoreUnity)
    {
        LatencyTestNote note = new LatencyTestNote(_targetDate, _targetUnity, _isDate, _isUnity, _scoreDate, _scoreUnity);

        testNotes.Add(note);
    }

    public void AddNewFmod(float _targetDate, float _targetUnity, float _isDate, float _isUnity)
    {
        LatencyTestNote note = new LatencyTestNote(_targetDate, _targetUnity, _isDate, _isUnity, "", "");
        testFmod.Add(note);
    }

    private void OnDestroy()
    {
        WriteFile();
        WriteFmodFile();
    }

    public void WriteFile()
    {
        if (testNotes.Count > 0)
        {
            TextWriter tw = new StreamWriter(fileName, true);
            tw.WriteLine("Target Time Date; Target Time Unity; Is Time Input Date; Is Time Input Unity; Score Date; Score Unity");
            tw.Close();

            tw = new StreamWriter(fileName, true);
            for (int i = 0; i < testNotes.Count; i++)
            {
                tw.WriteLine(testNotes[i].targetDate.ToString() + ";" + testNotes[i].targetUnity.ToString() + ";" +
                    testNotes[i].isDate.ToString() + ";" + testNotes[i].isUnity.ToString() + ";" +
                    testNotes[i].scoreDate.ToString() + ";" + testNotes[i].scoreUnity.ToString());
            }
            tw.Close();
        }
        Debug.Log("Saved File To Desktop");
    }

    public void WriteFmodFile()
    {
        if (testFmod.Count > 0)
        {
            TextWriter tw = new StreamWriter(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "/" + fileFmodName + ".csv", true);
            tw.WriteLine("Target Time Date; Target Time Unity; Is Time Fmod Date; Is Time Fmod Unity");
            tw.Close();

            tw = new StreamWriter(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "/" + fileFmodName + ".csv", true);
            for (int i = 0; i <testFmod.Count; i++)
            {
                tw.WriteLine(testFmod[i].targetDate.ToString() + ";" + testFmod[i].targetUnity.ToString() + ";" +
                    testFmod[i].isDate.ToString() + ";" + testFmod[i].isUnity.ToString());
            }
            tw.Close();
        }
    }

    public void WriteTestBars(List<float> testEighths)
    {
        TextWriter tw = new StreamWriter(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "/" + "TestBars" + ".csv", true);
        for (int i = 0; i < testEighths.Count; i++)
        {
            tw.WriteLine(testEighths[i].ToString());
        }
        tw.Close();
    }
}
