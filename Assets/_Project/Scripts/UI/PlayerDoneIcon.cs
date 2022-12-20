using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using Sirenix.OdinInspector;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PlayerDoneIcon : NetworkBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private RectTransform content;
    [SerializeField] private float notDonePosition, donePosition;
    [SerializeField] private float animDuration = 0.5f;

    [SerializeField] private UnityEvent onPlayerDone, onPlayerNotDone;
    
    private NetworkVariable<bool> _isDone = new NetworkVariable<bool>();
    private NetworkVariable<uint> _assignedCharacterId = new NetworkVariable<uint>();

    private TweenerCore<Vector2, Vector2, VectorOptions> _tween;

    public override void OnNetworkSpawn()
    {
        _isDone.OnValueChanged += OnDoneStateChanged;
        _assignedCharacterId.OnValueChanged += OnAssignedCharacterChanged;
    }

    //Setters
    public void SetCharacter(uint characterId)
    {
        bool isValidCharacterId = CharacterManager.Instance.IsValidCharacterId(characterId);
        if (!isValidCharacterId) return;
        
        _assignedCharacterId.Value = characterId;
    }

    public void SetDoneState(bool isDone)
    {
        _isDone.Value = isDone;
    }
    
    //Changed events
    private void OnAssignedCharacterChanged(uint previousvalue, uint newvalue)
    {
        if (previousvalue == newvalue) return;

        Character assignedCharacter = CharacterManager.Instance.GetCharacter(newvalue);

        iconImage.sprite = assignedCharacter.characterIcon;
    }

    private void OnDoneStateChanged(bool previousvalue, bool newvalue)
    {
        if (previousvalue == newvalue) return;
        
        if(newvalue) onPlayerDone.Invoke();
        else onPlayerNotDone.Invoke();
        
        PlayAnimation();
    }
    
    private void PlayAnimation()
    {
        if (!_isDone.Value)
        {
            _tween?.Kill();
            _tween = content.DOAnchorPosY(notDonePosition, animDuration).SetEase(Ease.InOutCubic);
        }
        else
        {
            _tween?.Kill();
            content.DOAnchorPosY(donePosition, animDuration).SetEase(Ease.InOutCubic);
        }
    }
}
