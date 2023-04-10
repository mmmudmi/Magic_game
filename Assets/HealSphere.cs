using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealSphere : MonoBehaviour
{
    [SerializeField] private float healRate = 20f;
    [SerializeField] private float healInterval = 1f;
    private float nextHealTime;

    private void Start()
    {
        nextHealTime = Time.time;
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && Time.time >= nextHealTime)
        {
            Health playerHealth = other.GetComponent<Health>();
            if (playerHealth != null)
            {
                playerHealth.Heal((int)healRate);
                Debug.Log("healing");
                nextHealTime = Time.time + healInterval;
            }
        }
    }
}
