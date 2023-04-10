using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using Cinemachine;

public class MonsterAI : MonoBehaviour
{   
    private AudioSource walkingAudio;

    [SerializeField] AudioClip hitSFX;
    [SerializeField] AudioClip roarSFX;
    [SerializeField] AudioClip collideSFX;
[SerializeField] private CinemachineVirtualCamera virtualCamera;
    private PlayerMovement playerController;
    ///change every level
   
    // if there are 3 levels...
    //[SerializeField] float RunAwayHealthLimit2 = 500f;
    //[SerializeField] float RunAwayHealthLimit3 = 1000f;
    public float damageCooldown = 1f;
    private float lastDamageTime = -Mathf.Infinity;
    public bool BehindExitDoor = false;

    [SerializeField] public LayerMask groundMask;
    public bool isAttacking = false;
    private Rigidbody playerRb;


 	private bool warpActivated = false;

	public bool isEscape = false;
    public Animator animator;
    public bool isDead = false;
    public float lastDidSomething = 0;
    public float pauseTime = 3f;

    public UnityEngine.AI.NavMeshAgent agent;

    public Transform player;
    public Transform MonsWarp;
    public Vector3 MonsWarpLocation;

    public LayerMask whatIsGround, whatIsPlayer;
    private bool isAlreadyRunAway = false;




    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;
    
    //Attacking
    public float timeBetweenAttacks;
    bool alreadyAttacked;
    //public GameObject projectile;
    private bool playerFirstDetected = false;

    //States
    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;

    //check movement
    private float prev_movement;    
   
    private Health health;
    private void Awake()
    {

        health = GetComponent<Health>();

       
        

        player = GameObject.Find("mainCharacter").transform;
        playerController = player.GetComponent<PlayerMovement>();


        MonsWarp = GameObject.FindGameObjectWithTag("Warp").transform; 
		MonsWarp.gameObject.SetActive(false);


        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
      
        animator.SetBool("isWalking",false);
        animator.SetBool("isRunning",false);
        //animator.SetBool("isDead",false);

        playerRb = player.GetComponent<Rigidbody>();
        prev_movement = transform.position.x;

        walkingAudio = GameObject.Find("MwalkingAudio").GetComponent<AudioSource>();

    }

    // private void Die()
    // {
    // animator.SetBool("isDead",true);
    // yield return new WaitForSeconds(6);
    // Destroy(gameObject);
    // }

    private void ApplyKnockback(Vector3 knockbackForce)
    {
        player.GetComponent<PlayerMovement>().ApplyKnockback(knockbackForce);
    }
  	private void ActivateWarp() 
{
    Debug.Log("ACTIVATE!!");
    Health bossHealth = GetComponent<Health>();
    if (bossHealth != null && bossHealth.RunAwayHealthLimitReached())
    {
        MonsWarp.gameObject.SetActive(true);

        MonsWarp = GameObject.FindGameObjectWithTag("Warp").transform; 

        // Set warpActivated to true to avoid reactivating the warp multiple times
        warpActivated = true;

        // Activate the warp here (e.g., enable the warp GameObject, trigger an animation, etc.)
        // Assuming the Warp GameObject is initially disabled, you can enable it like this:
        MonsWarpLocation = new Vector3(MonsWarp.position.x, MonsWarp.position.y, MonsWarp.position.z);
    }
}
private float originalSpeed;
private bool isSlowedDown;
private float slowDownEndTime;
private void SwitchToPlayerTarget()
{
    if (virtualCamera != null)
    {
        virtualCamera.LookAt = player;
    }
}
public void ApplySlowDown(float slowDownFactor, float duration)
{
    if (!isSlowedDown)
    {
        originalSpeed = agent.speed;
        isSlowedDown = true;
    }

    agent.speed = originalSpeed * slowDownFactor;
    slowDownEndTime = Time.time + duration;
}



    private void Update()
    {

        
       if (isDead) { SceneManager.LoadScene(0); } //win scene
    if (SceneManager.GetActiveScene().name == "Scene3")
    {
        if (!warpActivated && health.RunAwayHealthLimitReached())
        {
            ActivateWarp();
            isEscape = true;
            runAway();
        }
    }
    if (Time.time < lastDidSomething + pauseTime) return;
    if (isSlowedDown && Time.time >= slowDownEndTime)
{
    agent.speed = originalSpeed;
    isSlowedDown = false;
}
    if (SceneManager.GetActiveScene().name == "Scene2")
    {
        MonsWarp.gameObject.SetActive(false);
    }
	    if (!isEscape) {
    
        //Check for sight and attack range
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        if (playerInSightRange && !playerFirstDetected)
            {
                sightRange = sightRange*2; // set the new sight range value
                playerFirstDetected = true; // set the flag to true to avoid changing the sight range again
            }


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
            else {
                playerInSightRange = false; 
                playerInAttackRange = false;
            }
        }
      
    }

    private void runAway() //run away at the end of each level
    {
        
    if (!isAlreadyRunAway)
    {
        Debug.Log("Go away");
        agent.speed = 10;
        animator.SetTrigger("escape");
        // head to MonsWarp
        agent.SetDestination(MonsWarp.position);
        isAlreadyRunAway = true;
        SwitchToPlayerTarget(); // Add this line to switch the camera target when the monster escapes
    }
    if (!walkingAudio.isPlaying)
    {
        walkingAudio.pitch = Random.Range(0.5f, 1f) * 1.5f;
        walkingAudio.Play();
    }
}

  



    private void Patroling()
    {

        if (isDead|| warpActivated) {return;}
        else if (prev_movement == transform.position.x+ transform.position.z) {
             animator.SetBool("isWalking",false);
             walkingAudio.Stop();
        } else {
            animator.SetBool("isWalking",true);

            if (!walkingAudio.isPlaying)
            {
                walkingAudio.pitch = Random.Range(0.5f, 1f);
                walkingAudio.Play();
            }

        }
        prev_movement = transform.position.x+ transform.position.z ;
        agent.speed = 2;
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
        animator.SetBool("isWalking", false);
	
   		if (isDead || warpActivated) {return;} // Add the warpActivated check here.
        else if (prev_movement == transform.position.x+ transform.position.z) {
            //animator.SetBool("isRunning",false);
            animator.SetBool("isWalking",false);
            walkingAudio.Stop();

        } else {
            animator.SetBool("isWalking",true);
            
            //animator.SetBool("isRunning",true);

            if (!walkingAudio.isPlaying)
            {
                walkingAudio.pitch = Random.Range(0.5f, 1f);
                walkingAudio.Play();
            }
        }

        prev_movement = transform.position.x+ transform.position.z;
 // Set the chasing speed
   		agent.speed = 4;
        
        agent.SetDestination(player.position);

        lastDidSomething  = Time.time; 
        
    }

    private void AttackPlayer()
    {
         if (isDead || warpActivated) {return;} // Add the warpActivated check here.
		
        else if (prev_movement == transform.position.x + transform.position.z) {
            //animator.SetBool("isRunning",false);
            animator.SetBool("isWalking",false);
            walkingAudio.Stop();
        } else {
            animator.SetBool("isWalking",true);
            //animator.SetBool("isRunning",true);
            if (!walkingAudio.isPlaying)
            {
                walkingAudio.pitch = Random.Range(0.5f, 1f);
                walkingAudio.Play();
            }
        }
        prev_movement = transform.position.x+ transform.position.z;

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

 
    IEnumerator hurt_audio()
    {
        AudioSource.PlayClipAtPoint(hitSFX, transform.position,1.2f); 
        yield return new WaitForSeconds(1);
        AudioSource.PlayClipAtPoint(roarSFX, transform.position,1.5f); 
    }
  private ShieldObject shield;
    private void AttackRandomly()
{
    int randomNumber = Random.Range(0, 2);
    Vector3 knockbackDirection = (player


.position - transform.position).normalized;
Vector3 knockbackForce;
int damageAmount;



if (randomNumber == 0)
{
    animator.SetTrigger("punch");
    knockbackForce = new Vector3(knockbackDirection.x * 50f, 50f, knockbackDirection.z * 50f);
    damageAmount = 10; // Set the damage amount for punch
    AudioSource.PlayClipAtPoint(collideSFX, transform.position,1.6f); 
}
else
{
    animator.SetTrigger("pound");
    knockbackForce = new Vector3(knockbackDirection.x * 50f, 50f, knockbackDirection.z * 50f);
    damageAmount = 20; // Set the damage amount for pound
    AudioSource.PlayClipAtPoint(collideSFX, transform.position,1.6f); 
}
 



    if (playerController.IsHoldingShield() && Input.GetMouseButton(1))
    {
         GameObject shieldObject = GameObject.FindGameObjectWithTag("Shield");
        shield = shieldObject.GetComponent<ShieldObject>();
         


       
        Debug.Log("FCK");
        knockbackForce = shield.ReduceKnockback(knockbackForce);
        damageAmount = shield.ReduceDamage(damageAmount);
    }

    StartCoroutine(WaitForImpactAndApplyKnockback(0.6f, knockbackForce, damageAmount));

}   
private IEnumerator WaitForImpactAndApplyKnockback(float waitTime, Vector3 knockbackForce, int damageAmount)
{
    Health playerHealth;
    yield return new WaitForSeconds(waitTime);
    ApplyKnockback(knockbackForce);
    playerHealth = player.GetComponent<Health>();
    playerHealth.TakeDamage(damageAmount);

}

}
