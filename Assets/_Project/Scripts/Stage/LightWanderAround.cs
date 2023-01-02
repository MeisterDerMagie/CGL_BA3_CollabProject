using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using MEC;
using UnityEngine;
using Wichtel;
using Random = UnityEngine.Random;

public class LightWanderAround : MonoBehaviour
{
    [SerializeField] private Transform lightTarget;
    [SerializeField] private Transform point1, point2;
    [SerializeField] private Range<float> durationRange;
    [SerializeField] private Range<float> idleTime;
    [SerializeField] private Ease ease;

    private Transform startPoint;
    
    private void Start()
    {
        //set random position at start
        startPoint = Random.Range(0, 2) == 0 ? point1 : point2;
        
        lightTarget.position = startPoint.position;
        
        //start wandering around
        Timing.RunCoroutine(_Wander().CancelWith(gameObject));
    }

    private IEnumerator<float> _Wander()
    {
        Transform lastPoint = startPoint;
        
        while (true)
        {
            Transform nextPoint = lastPoint == point1 ? point2 : point1;
            float duration = Random.Range(durationRange.minimum, durationRange.maximum);
            bool tweenKilled = false;

            lightTarget.DOMove(nextPoint.position, duration).SetEase(ease).OnKill(()=> tweenKilled = true);

            lastPoint = nextPoint;

            yield return Timing.WaitUntilTrue(()=> tweenKilled);

            yield return Timing.WaitForSeconds(Random.Range(idleTime.minimum, idleTime.maximum));
        }
    }
}
