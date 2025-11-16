using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    CinemachineFreeLook cam;
    void Awake()
    {
        cam = GetComponent<CinemachineFreeLook>();

    }

    // Update is called once per frame
    void Update()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void CameraReCenter()
    {
        cam.m_RecenterToTargetHeading.RecenterNow();
    }
}
