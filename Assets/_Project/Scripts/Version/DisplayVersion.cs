using System.Collections;
using System.Collections.Generic;
using PlanetaGameLabo.UnityGitVersion;
using TMPro;
using UnityEngine;

public class DisplayVersion : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textField;

    private void Start()
    {
        textField.SetText(GitVersion.version.versionString);
    }
}
