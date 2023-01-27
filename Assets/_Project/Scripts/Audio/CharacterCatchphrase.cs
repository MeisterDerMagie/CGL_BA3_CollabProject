using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class CharacterCatchphrase : MonoBehaviour
{
    [SerializeField] private float delay = 0.5f;
    [SerializeField] private EventReference characterChange;
    private FMOD.Studio.EventInstance instance;

    public void PlayCatchphrase()
    {
        if (Unity.Netcode.NetworkManager.Singleton.IsServer) return;

        StopAllCoroutines();

        StartCoroutine(Catchphrase());
    }

    IEnumerator Catchphrase()
    {
        yield return new WaitForSeconds(delay);

        instance.getPlaybackState(out PLAYBACK_STATE state);

        if (state == PLAYBACK_STATE.PLAYING) instance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);

        instance = RuntimeManager.CreateInstance(CharacterManager.Instance.GetCharacter(PlayerData.LocalPlayerData.CharacterId).catchPhrase);
        instance.start();
    }

    public void PlayCharacterChange()
    {
        RuntimeManager.PlayOneShot(characterChange);
    }
}
