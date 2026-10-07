using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private float startingHealth = 100;
    [SerializeField] private Vector2 knockback = new Vector2(25f, 25f);

    private Rigidbody2D playerCharacter;
    private SpriteRenderer spriteRenderer;
    private CapsuleCollider2D playerBodyCollider;
    private BoxCollider2D playerFeetCollider;
    private Restart loadCurrent;
    public float currentHealth { get; private set; }

    [Header("Invincibility")]
    [SerializeField] public float iFrameDuration = 1f;
    private bool isInvin;

    void Awake()
    {
        currentHealth = startingHealth;
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerBodyCollider = GetComponent<CapsuleCollider2D>();
        playerFeetCollider = GetComponent<BoxCollider2D>();
        playerCharacter = GetComponent<Rigidbody2D>();
        loadCurrent = GetComponent<Restart>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(float _damage)
    {
        currentHealth = Mathf.Clamp(currentHealth - _damage, 0, startingHealth);

        if (isInvin)
        {
            return;
        }

        if (currentHealth > 0)
        {
            StartCoroutine(Blinkred());
            playerCharacter.linearVelocity = knockback;
            StartCoroutine(InvincibilityFrames());
        }
        else
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        }
    }

    private IEnumerator Blinkred()
    {
        spriteRenderer.color = new Color(Color.red.r, Color.red.g, Color.red.g, spriteRenderer.color.a);
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = new Color(Color.white.r, Color.white.g, Color.white.g, spriteRenderer.color.a);
    }

    private IEnumerator InvincibilityFrames()
    {
        isInvin = true;

        float elapsed = 0f;

        while(elapsed < iFrameDuration)
        {
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, .03f);
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 1);
            yield return new WaitForSeconds(0.1f);

            elapsed += 0.2f;
        }

        spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 1);
        isInvin = false;
    }

}
