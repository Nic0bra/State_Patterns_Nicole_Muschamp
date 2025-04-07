using Cinemachine;
using UnityEngine;
using UnityEngine.Events;

public class ShakeCamera : MonoBehaviour
{
    //Reference camera
    [SerializeField] CinemachineVirtualCamera vcam;

    //Initialize variables
    [SerializeField] float shakeStrength = 2f;
    [SerializeField] float shakeDuration = 0.2f;

    bool isShaking = false;
    private void Update()
    {
        //Check the isShaking flag
        if (isShaking)
        {
            StartShake();
        }
    }

    public void EnableShake()
    {
        isShaking = true;
    }
    //Start Shake function
    public void StartShake()
    {
        //Not working properly
        Debug.Log("Start Shake called");
        //Get the camera
        var noise = vcam?.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        //Make sure noise is active
        if (noise != null)
        {
            //Start Shake after duration stop it
            noise.m_AmplitudeGain = shakeStrength;
            //Not working properly
            Debug.Log($"Shake applied with {shakeStrength} strength and {shakeDuration} duration");
            Invoke(nameof(StopShake), shakeDuration);
        }
        //Problems with the shake so we debuggin
        else { Debug.Log("Noise component missing on vcam"); }
    }

    //End Shake function
    public void StopShake()
    {   //Get the camera
        var noise = vcam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        //Set shake to 0 to stop shake
        if (noise != null)
        {
            noise.m_AmplitudeGain = 0;
            isShaking = false;
        }
    }
}
