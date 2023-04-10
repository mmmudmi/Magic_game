using UnityEngine;

public class SwordCollider : MonoBehaviour
{
    [SerializeField] private int damage = 10;
    [SerializeField] private float damageCooldown = 0.5f;

    private Health bossHealth;
    private float timeSinceLastDamage;
    private PlayerMovement playerMovement; // Add this to get the player script reference

    private void Start()
    {
        bossHealth = GameObject.FindGameObjectWithTag("Boss").GetComponent<Health>();
        timeSinceLastDamage = damageCooldown;
        playerMovement = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>(); // Get the player script reference
    }

    private void Update()
    {
        timeSinceLastDamage += Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Collide!");
        // Check if the player is holding a sword and if the player isAttacking before dealing damage
        if (other.CompareTag("Boss") && playerMovement != null && playerMovement.isAttacking)
        {
            Debug.Log("HIT!");
            bossHealth.TakeDamage(damage);
            timeSinceLastDamage = 0f;
        }
    }
}
