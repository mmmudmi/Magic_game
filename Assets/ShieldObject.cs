using UnityEngine;

public class ShieldObject : MonoBehaviour
{
    public float knockbackReductionPercentage = 50f;
    public int damageReductionAmount = 5;
    [SerializeField] private float pickUpRange = 3f;
    private PlayerMovement playerMovement;
    private Animator animator;
    private GameObject playerObject;
    private MonsterAI monster;
    private GameSession gameSession;
    private Transform player;
    
    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        
        playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            playerMovement = playerObject.GetComponent<PlayerMovement>();
            animator = playerObject.GetComponent<Animator>();
        }
        else
        {
            Debug.LogError("No GameObject with the 'Player' tag found in the scene.");
        }
        
        GameObject monsterObject = GameObject.FindGameObjectWithTag("Boss");
        if (monsterObject != null)
        {
            monster = monsterObject.GetComponent<MonsterAI>();
        }
        else
        {
            Debug.LogError("No GameObject with the 'Monster' tag found in the scene.");
        }

        gameSession = GameSession.Instance;
        if (gameSession == null)
        {
            Debug.LogError("No GameSession object found in the scene.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PickUpShield();
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (!gameSession.IsEquippedShield && Vector3.Distance(player.position, transform.position) <= pickUpRange)
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                PickUpShield();
            }
        }
        if (Input.GetMouseButton(1) && playerMovement.IsHoldingShield())
        {
            Debug.Log("ActivateShield");
            ActivateShield();
            playerMovement.SetPlayerMovementEnabled(false);
        }
        else
        {
            DeactivateShield();
            playerMovement.SetPlayerMovementEnabled(true);
        }
    }

    private void PickUpShield()
    {
        Debug.Log("PICK SHIELD!!!");
        Transform shieldHolder = FindShieldHolder(player, "ShieldHolder");

        if (shieldHolder != null)
        {
            gameSession.IsEquippedShield = true;
            AttachShieldToPlayer(shieldHolder);
        }
    }

    private Transform FindShieldHolder(Transform parentTransform, string shieldHolderName)
    {
        foreach (Transform child in parentTransform)
        {
            if (child.name == shieldHolderName)
            {
                return child;
            }
            else
            {
                Transform foundChild = FindShieldHolder(child, shieldHolderName);
                if (foundChild != null)
                {
                    return foundChild;
                }
            }
        }
        return null;
    }

    private void AttachShieldToPlayer(Transform shieldHolder)
    {
        Debug.Log("ShieldHolder found!");
        transform.SetParent(shieldHolder);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        GetComponent<Collider>().enabled = true;
        GetComponent<Rigidbody>().isKinematic = true;
    }

    private void ActivateShield()
    {
        animator.SetBool("holdShield", true);
        // Implement logic to activate shield visuals and effects
    }

    private void DeactivateShield()
    {
        if (animator != null)
        {
            animator.SetBool("holdShield", false);
        }
        // Implement logic to deactivate shield visuals and effects
    }

    public Vector3 ReduceKnockback(Vector3 originalKnockback)
    {
        return originalKnockback * (1 - knockbackReductionPercentage / 100f);
    }

      public int ReduceDamage(int originalDamage)
    {
        return Mathf.Max(0, originalDamage - damageReductionAmount);
    }
}
