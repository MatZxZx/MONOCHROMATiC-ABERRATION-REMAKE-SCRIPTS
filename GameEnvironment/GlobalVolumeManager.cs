using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class GlobalVolumeManager : MonoBehaviour
{
    [SerializeField] private Volume volume;
    [SerializeField] private VolumeProfile postProcessProfile;
    [SerializeField] private bool disable;

    [Header("Overrides")]
    [SerializeField] private Bloom _bloom;
    [SerializeField] private MotionBlur _motionBlur;
    [SerializeField] private WhiteBalance _whiteBalance;
    [SerializeField] private ChromaticAberration _chromaticAberration;
    [SerializeField] private Vignette _vignette;


    void Start()
    {
        volume = GetComponent<Volume>();

        volume.profile.TryGet(out _bloom);
        volume.profile.TryGet(out _motionBlur);
        volume.profile.TryGet(out _whiteBalance);
        volume.profile.TryGet(out _chromaticAberration);
        volume.profile.TryGet(out _vignette);
    }

    void Update()
    {

    }

    public void SetMotionBlur(float intensity, float clamp) {
        _motionBlur.intensity.value = intensity;
        _motionBlur.clamp.value = clamp;
    }
}
