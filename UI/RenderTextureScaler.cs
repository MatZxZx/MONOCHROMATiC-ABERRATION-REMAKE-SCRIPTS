using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RenderTextureScaler : MonoBehaviour
{
    [SerializeField] private RawImage r;
    [SerializeField] private int quality = 10;

    void Awake()
    {
        r = GetComponent<RawImage>();
    }

    void Update()
    {
        // r.texture.height = Display.main.systemHeight;
        // r.texture.width = Display.main.systemWidth;
    }
}
