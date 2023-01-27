using System;
using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
using MEC;
using UnityEngine;
using STOP_MODE = FMOD.Studio.STOP_MODE;

public class PersistentAudioManager : MonoBehaviour
{

    [SerializeField] private EventReference mainTheme;
    [SerializeField] private EventReference ambience;

    [SerializeField] private float durationFadeInMainTheme, durationFadeOutMainTheme;
    [SerializeField] private float durationFadeInAmbience, durationFadeOutAmbience;

    private Bus _mainThemeBus;
    private Bus _ambienceBus;

    private float _defaultVolumeMainTheme;
    private float _defaultVolumeAmbience;

    private EventInstance _mainThemeInstance;
    private EventInstance _ambienceInstance;

    //start stop
    public void StartMainTheme()
    {
        _mainThemeInstance.getPlaybackState(out PLAYBACK_STATE playbackState);

        if (playbackState != PLAYBACK_STATE.PLAYING)
        {
            _mainThemeInstance.start();
            _mainThemeBus.setVolume(0f);
        }
        
        Timing.RunCoroutine(_Fade(_mainThemeBus, 0.25f, _defaultVolumeMainTheme));
    }

    public void StopMainTheme() => _mainThemeInstance.stop(STOP_MODE.IMMEDIATE);
    public void StartAmbience()
    {
        _ambienceInstance.getPlaybackState(out PLAYBACK_STATE playbackState);
        
        if (playbackState != PLAYBACK_STATE.PLAYING)
        {
            _ambienceInstance.start();
            _ambienceBus.setVolume(0f);
        }
        
        FadeInAmbience();
    }
    public void StopAmbience() => _ambienceInstance.stop(STOP_MODE.IMMEDIATE);

    //fade in / out
    public void FadeInMainTheme() => Timing.RunCoroutine(_Fade(_mainThemeBus, durationFadeInMainTheme, _defaultVolumeMainTheme));
    public void FadeOutMainTheme() => Timing.RunCoroutine(_Fade(_mainThemeBus, durationFadeOutMainTheme, 0f));

    public void FadeInAmbience() => Timing.RunCoroutine(_Fade(_ambienceBus, durationFadeInAmbience, _defaultVolumeAmbience));
    public void FadeOutAmbience() => Timing.RunCoroutine(_Fade(_ambienceBus, durationFadeOutAmbience, 0f));

    private IEnumerator<float> _Fade(Bus bus, float duration, float targetVol)
    {
        float currentTime = 0f;
        bus.getVolume(out float start);

        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            bus.setVolume(Mathf.Lerp(start, targetVol, currentTime / duration));
            yield return Timing.WaitForOneFrame;
        }

        bus.setVolume(targetVol);
    }
    
    private void Start()
    {
        _mainThemeInstance = RuntimeManager.CreateInstance(mainTheme);
        _ambienceInstance = RuntimeManager.CreateInstance(ambience);

        _mainThemeBus = RuntimeManager.GetBus("bus:/" + "Music/MainTheme");
        _ambienceBus =  RuntimeManager.GetBus("bus:/" + "Music/Ambience");
        
        _mainThemeBus.getVolume(out _defaultVolumeMainTheme);
        _ambienceBus.getVolume(out _defaultVolumeAmbience);
    }

    private void OnDestroy()
    {
        _mainThemeInstance.release();
        _ambienceInstance.release();
    }

    #region Singleton
    private static PersistentAudioManager singleton;
    public static PersistentAudioManager Singleton
        => singleton == null ? FindObjectOfType<PersistentAudioManager>() : singleton;

    public void Awake()
    {
        if (singleton == null)
        {
            singleton = this;
            DontDestroyOnLoad(this);
        }
        else
            Destroy(gameObject);
    }
    #endregion
}