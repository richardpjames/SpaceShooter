using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FXManager : MonoBehaviour
{
    public static FXManager Instance;

    [Header("Camera Shake")]
    [SerializeField] private float amplitude = 10f;
    [SerializeField] private float frequency = 10f;
    [SerializeField] private float duration = 0.25f;
    private CinemachineVirtualCamera _virtualCamera;
    private bool _shaking = false;
    [Header("Hit Stop")]
    [SerializeField] private float stopDuration = 0.1f;
    private bool _stopping = false;

    private void Awake()
    {
        // Ensure that this is the only instance
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void HitStop()
    {
        if (!_stopping)
        {
            StartCoroutine(StopCoroutine());
        }
    }

    private IEnumerator StopCoroutine()
    {
        // Set that we are already stopping
        _stopping = true;
        // Store the current timescale and set to zero
        float prevTimeScale = Time.timeScale;
        Time.timeScale = 0f;
        // Wait for the duration to allow for the stop - cannot use waitforseconds with timescale at zero
        float endTime = Time.realtimeSinceStartup + stopDuration;
        while (Time.realtimeSinceStartup < endTime)
        {
            yield return null;
        }
        // Reset everything
        Time.timeScale = prevTimeScale;
        _stopping = false;
    }

    public void ShakeCamera()
    {
        // Prevent multiple instances of the camera shaking
        if (!_shaking)
        {
            StartCoroutine(ShakeCoroutine());
        }
    }

    private IEnumerator ShakeCoroutine()
    {
        // Set that we are shaking
        _shaking = true;
        // Get the noise component from CineMachine
        _virtualCamera = Object.FindFirstObjectByType<CinemachineVirtualCamera>();
        CinemachineBasicMultiChannelPerlin noise = _virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        // Up the frequency and amplitude for the duration
        noise.m_AmplitudeGain = amplitude;
        noise.m_FrequencyGain = frequency;
        yield return new WaitForSeconds(duration);
        // Reset back to zero to steady the camera
        noise.m_AmplitudeGain = 0;
        noise.m_FrequencyGain = 0;
        // Set that we are no longer shaking
        _shaking = false;
        Camera.main.transform.rotation = Quaternion.identity;
    }
}
