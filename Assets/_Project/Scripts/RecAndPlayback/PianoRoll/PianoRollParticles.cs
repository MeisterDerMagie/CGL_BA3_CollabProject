using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PianoRollParticles : MonoBehaviour
{
    [SerializeField] GameObject glow;
    [SerializeField] ParticleSystem particle;

    public void TurnOnParticle (bool active)
    {
        glow.SetActive(active);
        if (active)
        {
            particle.Play();
        }
        else
        {
            particle.Stop();
        }
    }
}
