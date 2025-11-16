using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventsHandler : MonoBehaviour
{
    public ParticleSystem[] p;
    public int emitAmount = 4;
    public void SFX(string sfxName)
    {
        SFXManager.PlayOneShot(sfxName);
    }
    public void Particle(int index)
    {
        p[index].Emit(4);
    }
}
