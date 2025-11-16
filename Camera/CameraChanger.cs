using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.Splines;

public class CameraChanger : MonoBehaviour
{

    public bool ThreeDimensional = true;
    public bool TwoDimensional = false;
    public CinemachineVirtualCameraBase cam;
    [SerializeField] private int[] dollyPriorities;

    void Awake()
    {
        cam = transform.parent.GetComponentInChildren<CinemachineVirtualCameraBase>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Player player = other.GetComponent<Player>();
            player.ChangeDimension = !player.ChangeDimension;
            if (ThreeDimensional)
            {
                if (player.ChangeDimension)
                {
                    Set2D(player);
                }
                else
                {
                    Set3D(player);
                }
            }
            else if(TwoDimensional)
            {
                if (!player.ChangeDimension)
                {
                    Set3D(player);
                }
                else
                {
                    Set2D(player);
                }
            }
        }
    }

    private void Set3D(Player p)
    {
        cam.Priority = dollyPriorities[0];
    }
    private void Set2D(Player p)
    {
        SplineContainer spline = transform.parent.GetComponentInChildren<SplineContainer>();
        cam.Priority = dollyPriorities[1];
        SetDollyCamera(p.transform);
        p.spline2D = spline;
    }

    private void SetDollyCamera(Transform t)
    {
        cam.LookAt = t;
        cam.Follow = t;
    }
}
