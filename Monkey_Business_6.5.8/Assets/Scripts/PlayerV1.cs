using System.Collections;
using UnityEngine;
using UnityEngine.UI;
// using UnityEngine.InputSystem;

public class PlayerV1 : MonoBehaviour
{

    //[SerializeField] private float runSpeed = 5.0f; 
    //
    //[SerializeField] private float runAcceleration = 30f;
    //
    //[SerializeField] private floar runDeceleration = 40f; 
    //
    //[SerializeField] private InputActionAsset inputActions;
    //
    //InputAction moveAction;
    //
    //public Vector2 MoveInput {get; private set;}
    //
    //Rigidbody2D playerCharacter; 
    //
    // Initializes its contents before the game begins 
    //void Awake()
    //{
    //    playerCharacter = GetComponent<Rigidbody2D>();
    //
    //    InputActionMap playerMap = inputActions.FindActionMap ("Player", true);
    //
    //    moveAction = playerMap.FindAction ("Move", true);
    //
    //    playerMap.Enable();
    //}

    public int coins;
    public int health = 100;
    public float moveSpeed = 5f;
    public float jumpForce = 5f;
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;
    public Image healthImage;

    [Header("Shooting")]
    public GameObject bulletPrefab;
    public float fireRate;

    [Header("Crouch")]
    public float crouchSpeed = 2f;
    public KeyCode crouchKeyP1 = KeyCode.S;

    private bool isCrouching;
    private CapsuleCollider2D capsuleCollider;
    private Vector2 originalColliderSize;

    private float fireTimer;

    private Rigidbody2D rb;
    private bool isGrounded;

    private Animator animator;

    private SpriteRenderer spriteRenderer;
    public int extraJumpsValue = 1;
    private int extraJumps;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        capsuleCollider = GetComponent<CapsuleCollider2D>();
        originalColliderSize = capsuleCollider.size;

        extraJumps = extraJumpsValue; 
    }

    
    void Update()
    {
        float moveInput = Input.GetAxis ("Horizontal");
        rb.linearVelocity = new Vector2 (moveInput * moveSpeed, rb.linearVelocityY);

        // MoveInput = moveAction.ReadValue<Vector2>();
        //
        //Run();
        
        if(rb.linearVelocity.x != 0)
        {
            if(rb.linearVelocity.x > 0)
            {
                spriteRenderer.flipX = false;
            }
            else
            {
                spriteRenderer.flipX = true;
            }
        }

        if (isGrounded)
        {
            extraJumps = extraJumpsValue;
        }

        if(Input.GetKeyDown (KeyCode.Space))
        {
            if (isGrounded)
            {
                rb.linearVelocity = new Vector2 (rb.linearVelocityX, jumpForce);
            }
            else if (extraJumps > 0)
            {
                rb.linearVelocity = new Vector2 (rb.linearVelocityX, jumpForce);
                extraJumps--;
            }

        }

        SetAnimation(moveInput);

        healthImage.fillAmount = health / 100f;

        Handleshooting();

        bool crouchInput = Input.GetKey(crouchKeyP1);

        if (crouchInput)
        {
            StartCrouch();
        }
        else
        {
            StopCrouch();
        }
    }

    //private void Run()
    //{
    //   float hMovement = MoveInput.x;
    //
    //   float targetSpeed = moveInput.x * runSpeed;
    //
    //   float speedChange;
    //
    //   if(Mathf.Abs(targetSpeed) > Mathf.Epsilon)
    //   {
    //      speedChange = runAcceleration; 
    //   }
    //   else
    //   {
    //      speedChange = runDeceleration;
    //   }
    //
    //   float newSpeed = Mathf.MoveTowards(playerCharacter.linearVelocity.x,targetSpeed, speedChange * Time.deltaTime);
    //
    //   playerCharacter.linearVelocity = new Vector2(newSpeed, playerCharacter,linearVelocity.y);
    //}

    private void FixedUpdate()
    {
        isGrounded = Physics2D.OverlapCircle (groundCheck.position, groundCheckRadius, groundLayer);

        float currentSpeed = isCrouching ? crouchSpeed : moveSpeed; 
    }

    private void SetAnimation (float moveInput)
    {
        if (isCrouching)
        {
            animator.Play("Player_crouch");
            return;
        }
        
        if(isGrounded)
        {
            if (moveInput == 0)
            {
                animator.Play ("Player-Idle");
            }
            else
            {
                animator.Play ("Player_run");
            }
        }
        else
        {
            if (rb.linearVelocityY > 0)
            {
                animator.Play ("Player_jump");
            }
            else
            {
                animator.Play ("Player_fall");
            }
        }
    }

    private void OnCollisionEnter2D (Collision2D collision)
    {
        if (collision.gameObject.tag == "Damage")
        {
            health -= 25;
            rb.linearVelocity = new Vector2(rb.linearVelocityX, jumpForce);
            StartCoroutine(Blinkred());

            if (health <= 0)
            {
                Die();
            }
        }
    }

    private IEnumerator Blinkred()
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = Color.white;
    }

    private void Die()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene ("SampleScene");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "PowerUp")
        {
            extraJumps = 2;
            Destroy(collision.gameObject);
        }
    }

    private void Handleshooting()
    {
        fireTimer -= Time.deltaTime;

        if(Input.GetMouseButton(0) && fireTimer <= 0f)
        {
            Shoot();

            fireTimer = fireRate;
        }
    }

    private void Shoot()
    {
        GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);

        Bullet bulletScript = bullet.GetComponent<Bullet>();

        if(spriteRenderer.flipX)
        {
            bulletScript.SetDirection(Vector2.left);
        }
        else
        {
            bulletScript.SetDirection(Vector2.right);
        }
    }

    void StartCrouch()
    {
        if (isCrouching) return;

        isCrouching = true;
        capsuleCollider.size = new Vector2(originalColliderSize.x, originalColliderSize.y * 0.5f);

    }

    void StopCrouch()
    {
        if (!isCrouching) return;

        isCrouching = false;

        capsuleCollider.size = originalColliderSize;

    }
}
