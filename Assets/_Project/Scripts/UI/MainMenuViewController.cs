using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class MainMenuViewController : SerializedMonoBehaviour
{
    [SerializeField] private View mainView, createGameView, joinGameView, settingsView, creditsView;
    private Vector2Int _currentPos;
    
    private void Start()
    {
        //disable all views except the MainView
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }
        
        ShowView(mainView);
    }

    public void ShowView(View view)
    {
        //enable view
        view.gameObject.SetActive(true);

        //set position relative to the currentPos
        
        
        //animate position

        //disable old view
    }
}
