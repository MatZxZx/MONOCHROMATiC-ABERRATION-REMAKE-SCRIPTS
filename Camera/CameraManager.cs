using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraManager : MonoBehaviour
{
    [SerializeField] Camera baseCam;
    [SerializeField] private float zoomMin = 2.5f, zoomMax = 100, zoomSpeed = 2;
    CinemachineFreeLook cam;
    CinemachineInputProvider input;
    void Awake()
    {
        cam = GetComponent<CinemachineFreeLook>();
        // PlayerInput pi = GetComponentInParent<Player>().playerInput;
        // input.XYAxis = pi.
    }

    // Update is called once per frame
    void Update()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        baseCam.aspect = ((float) Screen.width) / Screen.height;
    }
    public void ZoomIn()
    {
        if (cam.m_Lens.FieldOfView > zoomMin)
        {
            cam.m_Lens.FieldOfView = Mathf.Lerp(cam.m_Lens.FieldOfView, cam.m_Lens.FieldOfView - zoomSpeed, 0.1f);
        }
    }
    public void ZoomOut()
    {
        if (cam.m_Lens.FieldOfView < zoomMax)
        {
            cam.m_Lens.FieldOfView = Mathf.Lerp(cam.m_Lens.FieldOfView, cam.m_Lens.FieldOfView + zoomSpeed, 0.1f);
        }
    }

    public void CameraReCenter()
    {
        cam.m_RecenterToTargetHeading.m_enabled = true;
        cam.m_RecenterToTargetHeading.RecenterNow();
        cam.m_RecenterToTargetHeading.m_enabled = false;
    }
}
