using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; 

public class UselessQuoteRandomiszer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI uselessTippTextField;
    [SerializeField] private string [] uselessTipps = new string[1];

    private int index; 
    // Start is called before the first frame update
    private void Awake()
    {
        index = Random.Range(0, uselessTipps.Length);
        uselessTippTextField.SetText(uselessTipps[index]);
    }


}
