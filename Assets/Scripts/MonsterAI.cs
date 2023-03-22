using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterAI : MonoBehaviour
{
    ///change every level
    [SerializeField] float RunAwayHealthLimit = 1000f;
    public float damageCooldown = 1f;
    private float lastDamageTime = -Mathf.Infinity;
    public bool BehindExitDoor = false;

    [SerializeField] public LayerMask groundMask;
    public bool isAttacking = false;
    private Rigidbody playerRb;
    


    public Animator animator;
    public bool isDead = false;
    public float lastDidSomething = 0;
    public float pauseTime = 3f;

    public UnityEngine.AI.NavMeshAgent agent;

    public Transform player;

    public LayerMask whatIsGround, whatIsPlayer;

    public float health;

    //Patroling
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;

    //Attacking
    public float timeBetweenAttacks;
    bool alreadyAttacked;
    //public GameObject projectile;

    //States
    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;

    //check movement
    private float prev_movement;

    private void Awake()
    {
        player = GameObject.Find("mainCharacter").transform;
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        animator.SetBool("die",false);
        animator.SetBool("isWalking",false);
        animator.SetBool("isRunning",false);
        playerRb = player.GetComponent<Rigidbody>();
        prev_movement = transform.position.x;

    }
    private void ApplyKnockback(Vector3 knockbackForce)
    {
        player.GetComponent<PlayerMovement>().ApplyKnockback(knockbackForce);
    }

    private void Update()
    {
        if (isDead) {return;}
        if (health <= RunAwayHealthLimit) {runAway();}
        if (Time.time < lastDidSomething + pauseTime) return; 
        //Check for sight and attack range
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);


        if (!playerInSightRange && !playerInAttackRange)  Patroling();
        if (playerInSightRange && !playerInAttackRange)
        {
            ChasePlayer();
        }
        else if (playerInSightRange && playerInAttackRange)
        {
            if (!isAttacking)
            {
                isAttacking = true;
                AttackPlayer();
            }
        }
        else
        {
            isAttacking = false;
        }

      
    }

    private void runAway() //run away at the end of each level
    {
        animator.SetBool("isRunning",true);
        //not yet complimented on where to head to 
            //agent.SetDestination(ExitToNextLevel.position);
        
    }


    private void Patroling()
    {

        if (isDead) {return;}
        else if (prev_movement == transform.position.x) {
             animator.SetBool("isWalking",false);
        } else {
            animator.SetBool("isWalking",true);
        }
        prev_movement = transform.position.x ;
        
        if (!walkPointSet) SearchWalkPoint();

        if (walkPointSet) agent.SetDestination(walkPoint);

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        //Walkpoint reached
        if (distanceToWalkPoint.magnitude < 1f) walkPointSet = false;

        lastDidSomething  = Time.time; 
        
    }

    private void SearchWalkPoint()
    {
        //Calculate random point in range
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
            walkPointSet = true;
    }

    private void ChasePlayer()
    {
        if (isDead) {return;}
        else if (prev_movement == transform.position.x) {
             animator.SetBool("isWalking",false);
        } else {
            animator.SetBool("isWalking",true);
        }
        prev_movement = transform.position.x;
        
        agent.SetDestination(player.position);

        lastDidSomething  = Time.time; 
        
    }

    private void AttackPlayer()
    {
        if (isDead) {return;}
        else if (prev_movement == transform.position.x) {
            animator.SetBool("isWalking",false);
        } else {
            animator.SetBool("isWalking",true);
        }
        prev_movement = transform.position.x;

        // Disable movement and collider during attack
        agent.isStopped = true;

        transform.LookAt(player);
   
        if (!alreadyAttacked)
        {
            ///Attack code here
            AttackRandomly();
            ///End of attack code

            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
        lastDidSomething  = Time.time; 
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
        isAttacking = false;
        agent.isStopped = false;
 
    }

    public void TakeDamage(int damage)
    {
        if (isDead) { return; }

        // Check if the damage cooldown has passed
        if (Time.time >= lastDamageTime + damageCooldown)
        {
            health -= damage;
            lastDamageTime = Time.time;

            if (health <= 0) isDead = true;
            animator.SetBool("die", true);
        }
    }

    private void AttackRandomly()
    {
        int randomNumber = Random.Range(0, 2);
        Vector3 knockbackDirection = (player.position - transform.position).normalized;
        Vector3 knockbackForce;

        if (randomNumber == 0)
        {
            animator.SetTrigger("punch");
            knockbackForce = new Vector3(knockbackDirection.x * 100f, 100f, knockbackDirection.z *100f);
        }
        else
        {
            animator.SetTrigger("pound");
            knockbackForce = new Vector3(knockbackDirection.x * 100f, 100f, knockbackDirection.z * 100f);
        }

        // Wait for the attack animation to reach the point of impact before applying knockback
        StartCoroutine(WaitForImpactAndApplyKnockback(0.5f, knockbackForce));
    }
    private IEnumerator WaitForImpactAndApplyKnockback(float waitTime, Vector3 knockbackForce)
    {
        yield return new WaitForSeconds(waitTime);
        ApplyKnockback(knockbackForce);
    }


}
