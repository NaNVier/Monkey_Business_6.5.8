using System.Collections;
using UnityEngine;
using UnityEngine.UI;
// using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{

    //[SerializeField] private InputActionAsset inputActions;
    //InputAction moveAction;

    // Initializes its contents before the game begins 
    //void Awake()
    //{
    //    InputActionMap playerMap = inputActions.FindActionMap("Player", true);

    //    moveAction = playerMap.FindAction("Move", true);

    //    playerMap.Enable();
    //}

    public int health = 100;
    public float moveSpeed = 5f;
    public float jumpForce = 5f;
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;
    public Image healthImage;

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

        extraJumps = extraJumpsValue; 
    }

    
    void Update()
    {
        float moveInput = Input.GetAxis("Horizontal");
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocityY);

        if(isGrounded)
        {
            extraJumps = extraJumpsValue;
        }

        if(Input.GetKeyDown(KeyCode.Space))
        {
            if(isGrounded)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocityX, jumpForce);
            }
            else if(extraJumps > 0)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocityX, jumpForce);
                extraJumps--;
            }

        }

        SetAnimation(moveInput);

        healthImage.fillAmount = health / 100f;
    }

    private void FixedUpdate()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    private void SetAnimation(float moveInput)
    {
        if(isGrounded)
        {
            if(moveInput == 0)
            {
                animator.Play("Player-Idle");
            }
            else
            {
                animator.Play("Player_run");
            }
        }
        else
        {
            if(rb.linearVelocityY > 0)
            {
                animator.Play("Player_jump");
            }
            else
            {
                animator.Play("Player_fall");
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Damage")
        {
            health -= 25;
            rb.linearVelocity = new Vector2(rb.linearVelocityX, jumpForce);
            StartCoroutine(Blinkred());

            if(health <= 0)
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
        UnityEngine.SceneManagement.SceneManager.LoadScene("SampleScene");
    }
}
