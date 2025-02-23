using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraShake : MonoBehaviour
{
    private CinemachineVirtualCamera CinemachineVirtualCamera;

    private CinemachineBasicMultiChannelPerlin _cbmcp;
    void Awake()
    {
        // Suscribirse al evento de la cámara
        CinemachineVirtualCamera = GetComponent<CinemachineVirtualCamera>();
    }

    void Start()
    {
        _cbmcp = CinemachineVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

        _cbmcp.m_AmplitudeGain = 0;
    }

    public void ShakeCamera(float intensity, float time)
    {
        _cbmcp = CinemachineVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        _cbmcp.m_AmplitudeGain = intensity;
        StartCoroutine(StopShake(time));
    }

    IEnumerator StopShake(float time)
    {
        yield return new WaitForSeconds(time);
        _cbmcp = CinemachineVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        _cbmcp.m_AmplitudeGain = 0;
    }
}
