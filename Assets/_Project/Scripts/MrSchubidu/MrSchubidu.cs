using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using FMOD.Studio;
using FMODUnity;
using MEC;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using Wichtel.Animation;
using STOP_MODE = FMOD.Studio.STOP_MODE;

public class MrSchubidu : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI speechbubbleTextField;
    [SerializeField] private List<DialogLine> dialogLines = new();
    [SerializeField, Min(0.01f)] private float textAnimationSpeed = 1f;
    [SerializeField] private UnityEvent onSchubiduFinished;

    [Title("Animations")]
    [SerializeField] private AnimatorStateReference schubiduIn;
    [SerializeField] private AnimatorStateReference schubiduOut, speechbubbleNextLine, schubiduIdle, schubiduTalking;

    private int _currentLine = -1;
    private Tween _textAnimation;
    private EventInstance _soundEventInstance;
    private CoroutineHandle _delayedNext;

    private bool _isAnimating;
    private bool _wasKilled;

    private void Start()
    {
        Timing.RunCoroutine(_AnimSchubiduIn().CancelWith(gameObject));
    }

    //either skip the currently running text animation, or if animation finished, show next line
    // ReSharper disable Unity.PerformanceAnalysis
    [Button, DisableInEditorMode]
    public void Next(bool forceNextLine = false)
    {
        if (_wasKilled)
        {
            Debug.LogWarning("This MrSchubidu has been killed. Don't call Next() on him.", this);
            return;
        }
        
        //if the typewriter animation is playing, skip it and instead show the whole text immediately
        if (!forceNextLine && _isAnimating)
        {
            _textAnimation?.Kill();
            _isAnimating = false;
            speechbubbleTextField.maxVisibleCharacters = speechbubbleTextField.textInfo.characterCount;
            schubiduIdle.Play();
            return;
        }

        //hide the bubble if we reached the end
        _currentLine++;
        if (_currentLine > dialogLines.Count - 1)
        {
            Kill();
            return;
        }

        //get line
        DialogLine line = dialogLines[_currentLine];
        
        //set text
        string text = line.text;
        speechbubbleTextField.SetText(text);
        speechbubbleTextField.ForceMeshUpdate();
        
        //type writer animation
        _textAnimation?.Kill();
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

        //play animations
        speechbubbleNextLine.Play();
        schubiduTalking.Play();

        //stop previous sound in case it's still playing
        _soundEventInstance.stop(STOP_MODE.ALLOWFADEOUT);
        _soundEventInstance.release();

        //play sound
        if (!line.soundEvent.IsNull)
        {
            _soundEventInstance = RuntimeManager.CreateInstance(line.soundEvent);
            _soundEventInstance.start();
            _soundEventInstance.release();
        }
        
        //if a duration was specified, automatically show the next line after that time
        Timing.KillCoroutines(_delayedNext);
        if (line.duration > 0f)
        {
            _delayedNext = Timing.RunCoroutine(_NextDelayed(line.duration).CancelWith(gameObject));
        }

        _isAnimating = true;
    }

    private IEnumerator<float> _NextDelayed(float delay)
    {
        yield return Timing.WaitForSeconds(delay);
        Next(true);
    }

    private IEnumerator<float> _AnimSchubiduIn()
    {
        //play in-animation
        yield return Timing.WaitUntilDone(Timing.RunCoroutine(schubiduIn._Play().CancelWith(gameObject)));
        
        //then start the text by calling Next()
        Next();
    }
    
    private IEnumerator<float> _AnimSchubiduOutAndDestroy()
    {
        //anim out
        yield return Timing.WaitUntilDone(Timing.RunCoroutine(schubiduOut._Play().CancelWith(gameObject)));

        //call finished event
        onSchubiduFinished.Invoke();
        
        //destroy game object
        Destroy(gameObject);
    }

    public void SetLines(List<DialogLine> lines)
    {
        dialogLines = lines;
        _currentLine = -1;
    }

    public void SetLinesAndShowNextLine(List<DialogLine> lines)
    {
        SetLines(lines);
        Next(true);
    }
    
    [Button]
    private void TEST_SetLinesAndRestart()
    {
        var lines = new List<DialogLine>()
        {
            new DialogLine("1"),
            new DialogLine("2", 4f),
            new DialogLine("3", 2f),
        };
        
        SetLinesAndShowNextLine(lines);
    }

    //stop schubidu and anim out
    [Button]
    public void Kill()
    {
        _wasKilled = true;
        
        //stop text animation and delayed Next
        Timing.KillCoroutines(_delayedNext);
        _textAnimation?.Kill();
        
        //stop sound
        _soundEventInstance.stop(STOP_MODE.ALLOWFADEOUT);
        _soundEventInstance.release();
        
        //play out animations
        schubiduIdle.Play();
        Timing.RunCoroutine(_AnimSchubiduOutAndDestroy().CancelWith(gameObject));
    }

    /// <summary> Kill schubidu without out-animation </summary>
    [Button]
    public void KillImmediate()
    {
        //stop text animation and delayed Next
        Timing.KillCoroutines(_delayedNext);
        _textAnimation?.Kill();
        
        //stop sound
        _soundEventInstance.stop(STOP_MODE.ALLOWFADEOUT);
        _soundEventInstance.release();

        //destroy object
        Destroy(gameObject);
    }
}

[Serializable]
public struct DialogLine
{
    [Multiline]
    public string text;
    
    [Title("Optional"), FoldoutGroup("Optional"), InfoBox("Set duration to 0 if the player needs to click to advance.")]
    public float duration;
    [FoldoutGroup("Optional")]
    public EventReference soundEvent;
    
    //constructors
    public DialogLine(string text, float duration = 0f, EventReference soundEvent = new())
    {
        this.text = text;
        this.duration = duration;
        this.soundEvent = soundEvent;
    }
}
