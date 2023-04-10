using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;



[RequireComponent(typeof(CharacterController))]

public class PlayerMovement : MonoBehaviour
{  
    [SerializeField] AudioClip missedSwordSFX;
    [SerializeField] AudioClip JumpSFX;
    [SerializeField] AudioClip OughSFX;
    [SerializeField] AudioClip ParticleSFX;
    private AudioSource walkingAudio;



    [Header("Magic Attack Settings")]
    [SerializeField] private string magicAttackTrigger = "MagicAttack";
    [SerializeField] private GameObject magicAttackEffectPrefab;
    private bool isMagicAttacking;
    [Header("Attack Settings")]
    [SerializeField] private string attackTrigger = "Attack";
    public bool isAttacking;

    [Header("Movement Settings")] [SerializeField]
    private float moveSpeed = 5f;

    [SerializeField] private float runSpeed = 8f;

    [SerializeField] private float walkRotateSpeed = 15f;
    [SerializeField] private float runRotateSpeed = 20f;
    [SerializeField] private float gravity = -9.81f;

    [SerializeField] private float jumpGravity = -30f; // New variable for jump gravity
    [SerializeField] private float groundDistance = 0f;
    [SerializeField] public LayerMask groundMask;

    [Header("Jump Settings")] [SerializeField]
    private float jumpForce = 9f;
private bool movementEnabled = true;

    [SerializeField] private float jumpAcceleration = 0.5f;
    [SerializeField] private float jumpTime = 0.65f;

    [Header("Roll Settings")] [SerializeField]
    private float rollSpeed = 8.5f;

    [SerializeField] private float rollAcceleration = 1f;
    [SerializeField] private float rollTime = 0.5f;
    [SerializeField] private float gravityTransitionTime = 0.8f; // New field for gravity transition time
    private Animator animator;
    private CharacterController characterController;
    private Vector3 movementDirection;
    private Vector2 moveInput;
    private bool isDead = false;

    private bool isRunning;
    private bool isJumping;
    private bool isRolling;
    private Transform cameraTransform;
    private Transform groundCheck;
    private float currentRollSpeed;
    private float currentJumpForce;
    private float jumpTimer;

    [Header("Roll Cooldown Settings")] [SerializeField]
    private float rollCooldown = 0.2f;
    
    private float rollCooldownTimer;
    private bool rollDirectionSet;
    private Vector3 rollDirection;
 


public void SetPlayerMovementEnabled(bool isEnabled)
{
    movementEnabled = isEnabled;
}
    private void Start()
    {   

        
        groundCheck = GameObject.FindGameObjectWithTag("Ground").transform;

        cameraTransform = GameObject.FindGameObjectWithTag("MainCamera").transform;
        animator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();
        rollCooldownTimer = 0f;
        currentRollSpeed = rollSpeed;
        currentJumpForce = jumpForce;
   
        
        walkingAudio = GameObject.Find("walkingAudio").GetComponent<AudioSource>();


    }
    

 public bool IsHoldingShield()
    {
        ShieldObject shield = GetComponentInChildren<ShieldObject>();

        
        return shield != null;
    }







    private void Update()
    {
        
        if (isDead) return;
        HandleJumping();
        HandleRolling();
        ApplyMovement();
        UpdateRollCooldownTimer();
        ApplyGravity();
        HandleAttacking();
        HandleMagicAttacking();
        UpdateAnimation();
        if (knockbackTimer > 0)
        {
            characterController.Move(knockbackVelocity * Time.deltaTime);
            knockbackTimer -= Time.deltaTime;
        }

        //walking audio
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector2 movement = new Vector2(horizontal, vertical).normalized;

        
    if (walkingAudio != null) // Add this check
    {
        if (movement.magnitude > 0)
        {
            if (!walkingAudio.isPlaying)
            {
                walkingAudio.pitch = Random.Range(0.6f, 1.4f);
                walkingAudio.Play();
            }
        }
        else
        {
            walkingAudio.Stop();
        }
    }
        
    }


    private void HandleJumping()
    {
        if (!isJumping || !characterController.isGrounded) return;

        if (jumpTimer <= jumpTime)
        {
            movementDirection.y = currentJumpForce;
            jumpTimer += Time.deltaTime;
            currentJumpForce += jumpAcceleration * Time.deltaTime;
        }
        else
        {
            isJumping = false;
            jumpTimer = 0f;
            currentJumpForce = jumpForce;
        }
    }
    private void UpdateRollCooldownTimer()
    {
        if (rollCooldownTimer > 0f)
        {
            rollCooldownTimer -= Time.deltaTime;
        }
    }
    
    private void HandleRolling()
    {
        if (!isRolling || rollCooldownTimer > 0f) return;

        if (!rollDirectionSet)
        {
            rollDirection = moveInput.magnitude > 0
                ? new Vector3(moveInput.x, 0f, moveInput.y).normalized
                : transform.forward;

            // Take camera orientation into account
            if (cameraTransform != null)
            {
                rollDirection = cameraTransform.TransformDirection(rollDirection);
                rollDirection.y = 0f;
                rollDirection.Normalize();
            }

            Quaternion targetRotation = Quaternion.LookRotation(rollDirection);
            transform.rotation = targetRotation;
            rollDirectionSet = true;
        }

        characterController.Move(rollDirection * currentRollSpeed * Time.deltaTime);
        currentRollSpeed += rollAcceleration * Time.deltaTime;

        rollTime -= Time.deltaTime;
        if (rollTime <= 0f)
        {
            isRolling = false;
            rollTime = 0.5f;
            currentRollSpeed = rollSpeed;
            rollDirection = Vector3.zero;
            rollDirectionSet = false;
        }
    }


    private void ApplyMovement()
    {
        if (isRolling || !movementEnabled) return;
       

        if (characterController.isGrounded && movementDirection.y < 0)
        {
            movementDirection.y = -2f;
        }

        Vector3 moveDirection = new Vector3(moveInput.x, 0f, moveInput.y).normalized;

        // Take camera orientation into account
        if (cameraTransform != null)
        {
            moveDirection = cameraTransform.TransformDirection(moveDirection);
            moveDirection.y = 0f;
            moveDirection.Normalize();
        }

        if (moveDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            float rotationSpeed = isRunning ? runRotateSpeed : walkRotateSpeed;
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        float movementSpeed = isRunning ? runSpeed : moveSpeed;
        characterController.Move(moveDirection * movementSpeed * Time.deltaTime);
    }

    private void ApplyGravity()
    {
     
        if (characterController.isGrounded && movementDirection.y < 0)
        {
            movementDirection.y = -2f;
        }

        if (isJumping)
        {
            // Custom jump gravity for a few seconds
            if (jumpTimer < jumpTime)
            {
                movementDirection.y += jumpGravity * Time.deltaTime;
            }
            else
            {
                // Gradually transition back to normal gravity
                float t = (jumpTimer - jumpTime) / gravityTransitionTime; // Calculate the transition progress
                float currentGravity = Mathf.Lerp(jumpGravity, gravity, t); // Interpolate between jumpGravity and gravity
                movementDirection.y += currentGravity * Time.deltaTime;
            }

            jumpTimer += Time.deltaTime;
            currentJumpForce += jumpAcceleration * Time.deltaTime;
        }
        else
        {
            movementDirection.y += gravity * Time.deltaTime;
        }

        characterController.Move(movementDirection * Time.deltaTime);

        if (characterController.isGrounded)
        {
            isJumping = false;
            currentRollSpeed = rollSpeed;
            jumpTimer = 0f; // Reset jump timer when grounded
            currentJumpForce = jumpForce; // Reset jump force when grounded
        }
    }




    private void UpdateAnimation()
    {
        bool isMoving = moveInput.magnitude > 0;
        bool notRollingOrJumping = !isRolling && !isJumping;

        animator.SetBool("IsWalking", isMoving && !isRunning && notRollingOrJumping);
        animator.SetBool("IsRunning", isMoving && isRunning && notRollingOrJumping);
        animator.SetBool("IsIdle", !isMoving && notRollingOrJumping);
        animator.SetBool("IsJumping", isJumping && !isRolling);
        animator.SetBool("IsRolling", isRolling && !isJumping);

        if (!Keyboard.current.leftShiftKey.isPressed && isRunning && isMoving)
        {
            isRunning = false;
        }
    }
    private IEnumerator ResetAttack()
    {
        yield return new WaitForSeconds(0.5f); // Adjust the duration to match your attack animation length
        isAttacking = false;
    }
  
    private IEnumerator ResetMagicAttack()
    {
        yield return new WaitForSeconds(0.5f); // Adjust the duration to match your magic attack animation length
        isMagicAttacking = false;
    }
    private void HandleAttacking()
    {
        if (isAttacking || isJumping || isRolling) return;

        // Removed animator.SetTrigger(attackTrigger);
        // Removed StartCoroutine(ResetAttack());
    }

    private void HandleMagicAttacking()
    {
        if (isMagicAttacking || isJumping || isRolling) return;
    }
   
    public void OnAttack(InputValue value)
    {
        if (value.isPressed && !isAttacking && !isJumping && !isRolling)
        {
            StartCoroutine(PerformAttack());
            StartCoroutine(ResetAttack());
        }
    }

    private IEnumerator PerformAttack()
    {
        Debug.Log("ATTACK");
        isAttacking = true;
        // Perform your attack animation or logic here
        // ...
        animator.SetTrigger(attackTrigger);

        // Set the duration of the attack
        yield return new WaitForSeconds(0.5f);
        AudioSource.PlayClipAtPoint(missedSwordSFX, transform.position,2.5f);

        isAttacking = false;
    }





    public void OnMagicAttack(InputValue value)
    {
        if (value.isPressed && !isMagicAttacking && !isJumping && !isRolling)
        {
            isMagicAttacking = true;
            AudioSource.PlayClipAtPoint(ParticleSFX, transform.position,1.6f);
            animator.SetTrigger(magicAttackTrigger);

            // Instantiate the magic attack effect
            Vector3 effectPosition = transform.position + transform.forward * 10f; // Adjust the position and offset as needed
            GameObject magicAttackEffectInstance = Instantiate(magicAttackEffectPrefab, effectPosition, transform.rotation);

            // Play the Particle System and destroy it after a certain duration
            ParticleSystem magicAttackEffect = magicAttackEffectInstance.GetComponent<ParticleSystem>();
            magicAttackEffect.Play();
            Destroy(magicAttackEffectInstance, magicAttackEffect.main.duration);

            StartCoroutine(ResetMagicAttack());
        }
    }

    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    public void OnRun(InputValue value)
    {
        isRunning = value.isPressed; 
    }

    public void OnJump(InputValue value)
    {
        if (value.isPressed && characterController.isGrounded && !isRolling)
        {
            isJumping = true;
            AudioSource.PlayClipAtPoint(JumpSFX, transform.position,4f);
        }
    }

    public void OnRoll(InputValue value)
    {
        if (value.isPressed && !isRolling && !isJumping && rollCooldownTimer <= 0f && !isAttacking && !isMagicAttacking)
        {
            isRolling = true;
            rollCooldownTimer = rollCooldown;
            AudioSource.PlayClipAtPoint(JumpSFX, transform.position,4f);
        }
    }
    private Vector3 knockbackVelocity;
    private float knockbackTimer = 0f;
    public void ApplyKnockback(Vector3 knockbackForce)
    {
        knockbackVelocity = knockbackForce;
        knockbackTimer = 0.2f; // Adjust the duration of the knockback effect
    }
    
}
