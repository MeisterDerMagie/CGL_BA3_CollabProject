using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using MEC;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using Wichtel.Animation;

public class MrSchubidu : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI speechbubbleTextField;
    [SerializeField, MultiLineProperty] private List<string> lines = new();
    [SerializeField, Min(0.01f)] private float textAnimationSpeed = 1f;
    [SerializeField] private RectTransform speechbubble;
    [SerializeField] private UnityEvent onSchubiduFinished;

    [Title("Animations")]
    [SerializeField] private AnimatorStateReference speechbubbleIn;
    [SerializeField] private AnimatorStateReference speechbubbleOut, speechbubbleNextLine, schubiduIdle, schubiduTalking;

    private int _currentLine = -1;
    private Tween _textAnimation;

    private bool _isAnimating;

    private void Start()
    {
        Next();
        speechbubbleIn.Play();
    }

    //either skip the currently running text animation, or if animation finished, show next line
    [Button, DisableInEditorMode]
    public void Next()
    {
        if (_isAnimating)
        {
            _textAnimation?.Kill();
            _isAnimating = false;
            speechbubbleTextField.maxVisibleCharacters = speechbubbleTextField.textInfo.characterCount;
            schubiduIdle.Play();
            return;
        }

        _currentLine++;
        if (_currentLine > lines.Count - 1)
        {
            Exit();
            return;
        }

        string text = lines[_currentLine];
        speechbubbleTextField.SetText(text);
        speechbubbleTextField.ForceMeshUpdate();
        int textCharacterCount = speechbubbleTextField.textInfo.characterCount;
        
        float animDuration = textCharacterCount / textAnimationSpeed * 0.05f;

        speechbubbleTextField.maxVisibleCharacters = 0;
        _textAnimation = DOTween.To(()=>speechbubbleTextField.maxVisibleCharacters, x=> speechbubbleTextField.maxVisibleCharacters = x, textCharacterCount, animDuration)
            .SetEase(Ease.Linear)
            .OnComplete(() =>
        {
            _isAnimating = false;
            schubiduIdle.Play();
        });

        speechbubbleNextLine.Play();
        schubiduTalking.Play();
        
        _isAnimating = true;
    }

    private void Exit()
    {
        schubiduIdle.Play();
        Timing.RunCoroutine(_AnimSpeechbubbleOut());
    }

    private IEnumerator<float> _AnimSpeechbubbleOut()
    {
        //anim out
        yield return Timing.WaitUntilDone(Timing.RunCoroutine(speechbubbleOut._Play()));
        
        //hide speechbubble
        speechbubble.gameObject.SetActive(false);
        
        //call finished event
        onSchubiduFinished.Invoke();
    }
}
