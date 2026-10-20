using NUnit.Framework.Constraints;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    [Header("Move Settings")]
    [SerializeField] private float runSpeed = 5.0f;
    [SerializeField] private float runAcceleration = 30f;
    [SerializeField] private float runDeceleration = 40f;
    public InputActionAsset InputActions => inputActions;

    [Header("Jump Settings")]
    [SerializeField] private float jumpSpeed = 5.0f;
    [SerializeField][Range(0.1f, 1f)] private float jumpCutMultiplier = 0.5f;
    [SerializeField] private float coyoteTime = 0.1f;
    [SerializeField] private float jumpBufferTime = 0.1f;
    [SerializeField] private float ladderJumpTime = 0.15f;
    float jumpedOffLadderTimer;
    [SerializeField] private float fallGravityMultiplier = 2.0f;
    [SerializeField] private float climbSpeed = 5.0f;
    float gravityScaleAtStart;
    float lastGroundTime;
    float jumpBufferTimer;
    [SerializeField] private LayerMask groundLayer;
    LayerMask climbingLayer;

    [SerializeField] private Vector2 bounce = new Vector2(0f, 15f);
    [SerializeField] private InputActionAsset inputActions;
    InputAction moveAction;
    InputAction jumpAction;
    public bool JumpPressedThisFrame => jumpAction != null && jumpAction.WasPressedThisFrame();
    public bool SpecialMovementActive { get; private set; }
    public LayerMask GroundLayer => groundLayer.value != 0 ? groundLayer : LayerMask.GetMask("Ground");
    public Vector2 MoveInput { get; private set; } 

    Rigidbody2D playerCharacter;

    Animator playerAnimator;

    BoxCollider2D playerFeetCollider;

    CapsuleCollider2D playerBodyCollider;

    AudioSource audioSource;
    [SerializeField] AudioClip die;
    [SerializeField] AudioClip jumpSound;

    public int coins;

    bool isAlive = true;
    public bool IsAlive => IsAlive;
    [SerializeField] private Vector2 deathseq = new Vector2(25f, 25f);

    // Initializes its contents before the game begins
    void Awake()
    {
        playerCharacter = GetComponent<Rigidbody2D>();

        playerAnimator = GetComponentInChildren<Animator>();

        playerFeetCollider = GetComponent<BoxCollider2D>();

        playerBodyCollider = GetComponent<CapsuleCollider2D>();

        gravityScaleAtStart = playerCharacter.gravityScale;

        climbingLayer = LayerMask.GetMask("Climbing");
        
        InputActionMap playerMap = inputActions.FindActionMap("Player", true);

        moveAction = playerMap.FindAction("Move", true);

        jumpAction = playerMap.FindAction("Jump", true);

        audioSource = GetComponent<AudioSource>();

        playerCharacter.linearDamping = 0.0f;

        runAcceleration = Mathf.Max(runAcceleration, runSpeed * 6.0f);
        runDeceleration = Mathf.Max(runDeceleration, runSpeed * 8.0f);


        playerMap.Enable();

    }

    private void OnDestroy()
    {
        inputActions?.Disable();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (Checkpoint.savedPosition != Vector2.zero)
        {
            transform.position = Checkpoint.savedPosition;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!isAlive)
        {
            return;
        }

        MoveInput = moveAction.ReadValue<Vector2>();

        Run();
        FlipSprite();
        Jump();
        BetterGravity();
        Climb();
        Bounce();
    }

    public void SetSpecialMovementActive(bool active)
    {
        SpecialMovementActive = active;
    }

    private void Run()
    {
        float hMovement = MoveInput.x;

        float targetSpeed = MoveInput.x * runSpeed;

        float speedChange = Mathf.Abs(targetSpeed) > Mathf.Epsilon ? runAcceleration : runDeceleration;

        float newSpeed = Mathf.MoveTowards(playerCharacter.linearVelocity.x, targetSpeed, speedChange * Time.deltaTime);

        playerCharacter.linearVelocity = new Vector2(newSpeed, playerCharacter.linearVelocity.y);

        bool hSpeed = Mathf.Abs(playerCharacter.linearVelocity.x) > Mathf.Epsilon;

        playerAnimator.SetBool("run", hSpeed);
    }

    private void FlipSprite()
    {
        bool hMovement = Mathf.Abs(playerCharacter.linearVelocity.x) > Mathf.Epsilon;

        if(hMovement)
        {
            transform.localScale = new Vector2(Mathf.Sign(playerCharacter.linearVelocity.x), 1f);
        }
    }

    private void Jump()
    {

        if (jumpAction.WasReleasedThisFrame() && playerCharacter.linearVelocity.y > 0)
        {
            playerCharacter.linearVelocity = new Vector2(playerCharacter.linearVelocity.x, playerCharacter.linearVelocity.y * jumpCutMultiplier);
        }
        
        bool isGrounded = playerFeetCollider.IsTouchingLayers(GroundLayer) || playerFeetCollider.IsTouchingLayers(climbingLayer);

        if(isGrounded)
        {
            lastGroundTime = coyoteTime;
        }
        else
        {
            lastGroundTime -= Time.deltaTime;
        }

        if(JumpPressedThisFrame)
        {         
            jumpBufferTimer = jumpBufferTime;
        }
        else
        {
            jumpBufferTimer -= Time.deltaTime;
        }

        if(lastGroundTime <= 0 || jumpBufferTimer <= 0)
        {
            return; 
        }

        playerCharacter.linearVelocity = new Vector2(playerCharacter.linearVelocity.x, jumpSpeed);

        lastGroundTime = 0;

        jumpBufferTimer = 0;

        jumpedOffLadderTimer = ladderJumpTime;
        audioSource.PlayOneShot(jumpSound, 0.7f);
    }

    private void Bounce()
    {
        if (playerBodyCollider.IsTouchingLayers(LayerMask.GetMask("bouncepad")))
        {
            playerCharacter.linearVelocity = bounce;
        }
    }

    private void BetterGravity()
    {
        if (playerBodyCollider.IsTouchingLayers(climbingLayer))
        {
            return;
        }
        
        //Use stronger gravity when falling, then cap fall speed

        float gravityMultiplier = playerCharacter.linearVelocity.y < 0 ? fallGravityMultiplier : 1f;

        playerCharacter.gravityScale = gravityScaleAtStart * gravityMultiplier;

        if (playerCharacter.linearVelocity.y < -jumpSpeed * fallGravityMultiplier)
        {
            playerCharacter.linearVelocity = new Vector2(playerCharacter.linearVelocity.x, -jumpSpeed * fallGravityMultiplier);
        }
    }

    private void Climb()
    {
        jumpedOffLadderTimer -= Time.deltaTime;

        if (jumpedOffLadderTimer > 0 || !playerBodyCollider.IsTouchingLayers(climbingLayer))
        {
            playerAnimator.SetBool("climb", false);

            playerCharacter.gravityScale = gravityScaleAtStart;

            return;
        }

        float vMovement = MoveInput.y;

        Vector2 climbingVelocity = new Vector2(MoveInput.x * runSpeed, vMovement * climbSpeed);

        playerCharacter.linearVelocity = climbingVelocity;

        playerCharacter.gravityScale = 0f;

        bool vSpeed = Mathf.Abs(playerCharacter.linearVelocity.y) > Mathf.Epsilon;

        playerAnimator.SetBool("climb", vSpeed);
    }
    public void Die()
    {
        if (!isAlive)
        {
            return;
        }
        
        isAlive = false;

        playerAnimator.SetTrigger("die");

        playerCharacter.linearVelocity = deathseq;

        audioSource.PlayOneShot(die, 1.0f);

        FindFirstObjectByType<GameSession>().ProcessPlayerDeath();

    }
}

