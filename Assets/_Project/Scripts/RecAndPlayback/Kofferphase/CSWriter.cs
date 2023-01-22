using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class CSWriter : MonoBehaviour
{
    string fileName = "";

    public List<float> timelineTimestamps = new List<float>();
    public List<float> timerTimestamps = new List<float>();

    [System.Serializable]
    public class LatencyTestEighth
    {
        public float targetTime;
        public float actualTime;

        public LatencyTestEighth(float target, float actual)
        {
            targetTime = target;
            actualTime = actual;
        }
    }

    public List<LatencyTestEighth> testEighths;

    void Start()
    {
        fileName = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "/LatencyTest.csv";
        testEighths = new List<LatencyTestEighth>();
    }

    public void AddNewNote(float target, float actual)
    {
        LatencyTestEighth note = new LatencyTestEighth(target, actual);

        testEighths.Add(note);
    }

    public void WriteFile()
    {
        TextWriter tw = new StreamWriter(fileName, true);
        
        for (int i = 0; i < timelineTimestamps.Count; i++)
        {
            string line = string.Empty;
            line += i < timerTimestamps.Count ? timerTimestamps[i] + ";" : ";";
            line += i < timelineTimestamps.Count ? timelineTimestamps[i] : "";
                
            tw.WriteLine(line);
        }
        tw.Close();
        Debug.Log("Saved File To Desktop");
    }
}