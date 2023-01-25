using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using System;
using System.Runtime.InteropServices;
using Unity.Netcode;

/// <summary>
/// communication with FMOD via this script about current beat + bar + other markers
/// play background music from this script in all rhythm stages of game (recording, playback etc)
/// </summary>

public class BackingTrack : MonoBehaviour
{
    [SerializeField] private EventReference track;
    private FMOD.Studio.EventInstance musicInstance;

    public TimelineInfo timelineInfo = null;
    private GCHandle timelineHandle;

    private FMOD.Studio.EVENT_CALLBACK beatCallback;

    public delegate void BeatEventDelegate();
    public event BeatEventDelegate beatUpdated;

    public delegate void BarEventDelegate();
    public event BarEventDelegate barUpdated;

    public int lastBeat = 0;
    public int lastBar = 0;

    public static BackingTrack Singleton;

    public DateTime startTime = DateTime.MinValue;
    public float startUnity = -1f;
    public float timeSinceStart => (float)(DateTime.Now - startTime).TotalSeconds;
    public float timeSinceStartUnity => Time.time - startUnity;

    [StructLayout(LayoutKind.Sequential)]
    public class TimelineInfo
    {
        public int currentBeat = 0;
        public int currentBar = 0;
    }

    private void Start()
    {
        timelineInfo = new TimelineInfo();
        beatCallback = new FMOD.Studio.EVENT_CALLBACK(BeatEventCallback);
        timelineHandle = GCHandle.Alloc(timelineInfo, GCHandleType.Pinned);
        musicInstance.setUserData(GCHandle.ToIntPtr(timelineHandle));
        musicInstance.setCallback(beatCallback, FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_BEAT);

        Singleton = this;
    }

    private void Update()
    {
        /*
        if (lastBeat != timelineInfo.currentBeat)
        {
            lastBeat = timelineInfo.currentBeat;

            if(beatUpdated != null)
                beatUpdated();
        }

        if (lastBar != timelineInfo.currentBar)
        {
            lastBar = timelineInfo.currentBar;

            if (barUpdated != null)
                barUpdated();
        }
        */
    }

    public void StartMusic()
    {
        if (NetworkManager.Singleton == null)
        {
            // start locally saved track
        }
        else
        {
            // start track saved in playerdata
        }
        musicInstance = RuntimeManager.CreateInstance(track);
        musicInstance.start();

        timelineInfo = new TimelineInfo();
        beatCallback = new FMOD.Studio.EVENT_CALLBACK(BeatEventCallback);
        timelineHandle = GCHandle.Alloc(timelineInfo, GCHandleType.Pinned);
        musicInstance.setUserData(GCHandle.ToIntPtr(timelineHandle));
        musicInstance.setCallback(beatCallback, FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_BEAT | FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_MARKER);
    }

    public void StopMusic()
    {
        musicInstance.setUserData(IntPtr.Zero);
        musicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        musicInstance.release();
    }

    private void OnDestroy()
    {
        musicInstance.setUserData(IntPtr.Zero);
        musicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        musicInstance.release();
        timelineHandle.Free();
    }

# if UNITY_EDITOR
    private void OnGUI()
    {
        if (timelineInfo == null) return;
        //GUILayout.Box($"Current Beat = {timelineInfo.currentBeat}, current Bar = {timelineInfo.currentBar}");
    }
#endif

    [AOT.MonoPInvokeCallback(typeof(FMOD.Studio.EVENT_CALLBACK))]
    static FMOD.RESULT BeatEventCallback(FMOD.Studio.EVENT_CALLBACK_TYPE type, IntPtr instancePtr, IntPtr parameterPtr)
    {
        FMOD.Studio.EventInstance instance = new FMOD.Studio.EventInstance(instancePtr);

        IntPtr timelineInfoPtr;
        FMOD.RESULT result = instance.getUserData(out timelineInfoPtr);

        if (result != FMOD.RESULT.OK)
            Debug.LogError("Timeline Callback error: " + result);
        else if (timelineInfoPtr != IntPtr.Zero)
        {
            GCHandle timelineHandle = GCHandle.FromIntPtr(timelineInfoPtr);
            TimelineInfo timelineInfo = (TimelineInfo)timelineHandle.Target;

            switch(type)
            {
                case FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_BEAT:
                    {
                        var parameter = (FMOD.Studio.TIMELINE_BEAT_PROPERTIES)Marshal.PtrToStructure(parameterPtr, typeof(FMOD.Studio.TIMELINE_BEAT_PROPERTIES));
                        timelineInfo.currentBeat = parameter.beat;
                        timelineInfo.currentBar = parameter.bar;

                        if (Singleton.startTime == DateTime.MinValue) Singleton.startTime = DateTime.Now;
                        if (Singleton.startUnity == -1) Singleton.startUnity = Time.time;

                        if (Singleton.lastBeat != timelineInfo.currentBeat)
                        {
                            Singleton.lastBeat = timelineInfo.currentBeat;

                            if (Singleton.beatUpdated != null)
                                Singleton.beatUpdated();
                        }

                        if (Singleton.lastBar != timelineInfo.currentBar)
                        {
                            Singleton.lastBar = timelineInfo.currentBar;

                            if (Singleton.barUpdated != null)
                                Singleton.barUpdated();
                        }
                    }
                    break;
            }
        }
        return FMOD.RESULT.OK;
    }
}
