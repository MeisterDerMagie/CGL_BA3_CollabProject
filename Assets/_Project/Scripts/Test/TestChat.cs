//(c) copyright by Martin M. Klöckener
using System;
using Sirenix.OdinInspector;
using UnityEngine;

public class TestChat : MonoBehaviour
{
    [Button]
    private void SaySomething(string text)
    {
        GetComponent<TEST_NetworkVariables>().testString.Value = text;
    }
}