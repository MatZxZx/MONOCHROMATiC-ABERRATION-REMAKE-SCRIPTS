using UnityEngine;
using Cinemachine;
public class CameraFocus : MonoBehaviour
{
    public CinemachineBrain Brain;
    public ICinemachineCamera CamA;
    public ICinemachineCamera CamB;
    void Start()
    {
        CamA= GetComponent<ICinemachineCamera>();
        CamB = GetComponent<ICinemachineCamera>();
        //Override parameters
        int layer = 1; //The layer to override
        //int priority = 1; //The priority of the override
        float weight = 1f; //The weight of the new camera
        float blendTime = 0f; //The time it takes to blend between cameras
        Brain.SetCameraOverride(layer, CamA, CamB, weight, blendTime);
    }
}
