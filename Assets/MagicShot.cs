using UnityEngine;

public class MagicShot : MonoBehaviour
{
    public float destroyTime = 5f; // The time after which the magic shot will be destroyed
    public float speed = 10f; // The speed at which the magic shot travels

    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.velocity = transform.forward * speed;

        Destroy(gameObject, destroyTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // If you want to do anything on collision, like dealing damage, add it here

        Destroy(gameObject); // Destroy the magic shot on collision
    }
}