using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Potion : MonoBehaviour
{
    [SerializeField] private AudioClip boostSFX;
    [SerializeField] private float value = 20f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Health playerHealth = other.GetComponent<Health>();
            if (playerHealth != null)
            {
                playerHealth.Heal((int)value);
            }

            AudioSource.PlayClipAtPoint(boostSFX, transform.position, 1.0f);
            Destroy(gameObject);
        }
    }
}
