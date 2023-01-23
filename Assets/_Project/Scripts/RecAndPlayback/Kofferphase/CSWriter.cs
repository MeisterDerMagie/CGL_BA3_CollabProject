using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class CSWriter : MonoBehaviour
{
    string fileName = "";

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
    [SerializeField] string _fileName = "LatencyTest";

    void Start()
    {
        fileName = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "/" + _fileName + ".csv";
        testEighths = new List<LatencyTestEighth>();
    }

    public void AddNewNote(float target, float actual)
    {
        LatencyTestEighth note = new LatencyTestEighth(target, actual);

        testEighths.Add(note);
    }

    public void WriteFile()
    {
        if (testEighths.Count > 0)
        {
            /*
            TextWriter tw = new StreamWriter(fileName, false);
            tw.WriteLine("Target Time, Actual Time");
            tw.Close();
            */

            TextWriter tw = new StreamWriter(fileName, true);
            for (int i = 0; i < testEighths.Count; i++)
            {
                tw.WriteLine(testEighths[i].targetTime.ToString() + ";" + testEighths[i].actualTime.ToString());
            }
            tw.Close();
        }
        Debug.Log("Saved File To Desktop");
    }
}
