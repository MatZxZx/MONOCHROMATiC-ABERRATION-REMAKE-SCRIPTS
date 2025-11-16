using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillBoard : MonoBehaviour
{
    private Transform cameraTransform;
    void Start()
    {
        cameraTransform = Camera.main.transform;
    }
    void Update()
    {
        transform.LookAt(cameraTransform);
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
    }
}
