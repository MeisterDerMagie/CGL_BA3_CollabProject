//(c) copyright by Martin M. Klöckener
using System;
using Sirenix.OdinInspector;
using UnityEngine;

public class TestChat : MonoBehaviour
{
    [Button]
    private void SaySomething(string text)
    {
        GetComponent<PlayerData>().testString.Value = text;
    }
}