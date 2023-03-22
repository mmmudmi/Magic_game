using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

public class PlayerZoomAndShoot : MonoBehaviour
{
    public CinemachineVirtualCamera defaultCamera;
    public CinemachineVirtualCamera zoomedCamera;
    public ParticleSystem chargeParticles;
    public GameObject magicShotPrefab;
    public Transform magicShotSpawnPoint;
    public GameObject crosshair;

    private bool isZooming = false;
    private float chargeTime = 0f;

    private void Start()
    {
        zoomedCamera.Priority = 0;
        crosshair.SetActive(false);
        chargeParticles.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (isZooming)
        {
            Debug.Log("Zooming and charging...");
            defaultCamera.Priority = 0;
            zoomedCamera.Priority = 1;
            chargeTime += Time.deltaTime;
            ParticleSystem.MainModule mainModule = chargeParticles.main;
            mainModule.startSize = Mathf.Lerp(1f, 1.5f, chargeTime);
        }
        else
        {
            defaultCamera.Priority = 1;
            zoomedCamera.Priority = 0;
            chargeTime = 0f;
            ParticleSystem.MainModule mainModule = chargeParticles.main;
            mainModule.startSize = Mathf.Lerp(1.5f, 1f, chargeTime);
        }
    }

    public void OnZoom(InputValue value)
    {
        isZooming = value.isPressed;
        Debug.Log("Zoom state: " + isZooming);
        crosshair.SetActive(isZooming);
        chargeParticles.gameObject.SetActive(isZooming);
    }

    public void OnShoot()
    {
        if (isZooming)
        {
            GameObject magicShot = Instantiate(magicShotPrefab, magicShotSpawnPoint.position, magicShotSpawnPoint.rotation);
            // Set the velocity, force or any other properties of the magic shot here
            isZooming = false;
            crosshair.SetActive(false);
            chargeParticles.gameObject.SetActive(false);
        }
    }
}
