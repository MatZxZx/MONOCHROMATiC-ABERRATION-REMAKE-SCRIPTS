using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeInManager : MonoBehaviour
{
    public Transform reference;
    public Material map;
    public float rate = .1f;
    private float counter = 0;

    void Update()
    {
        counter += 1 * Time.deltaTime;
        if(counter > rate)
        {
            map.SetVector("_Position", reference.position);
        }
    }
}
