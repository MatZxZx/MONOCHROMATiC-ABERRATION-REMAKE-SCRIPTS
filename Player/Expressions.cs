using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Expressions : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private SkinnedMeshRenderer[] skinnedMeshRenderer;
    [SerializeField] private Material blurMaterial;

    void Awake()
    {
        
    }

    void Update()
    {
        SpeedBlur();
    }

    private void SpeedBlur()
    {
        blurMaterial.SetFloat("_BlurAmount", Mathf.Lerp(0, 0.01f, player.rb.velocity.magnitude / (player.maxSpeed * 12)));
    }
}
