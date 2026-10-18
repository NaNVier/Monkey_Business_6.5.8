using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private float startingHealth = 100;
    [SerializeField] private Vector2 knockback = new Vector2(5f, 5f);

    [Header("Sound Effects")]
    [SerializeField] AudioClip hit;
    AudioSource audioSource;

    private Rigidbody2D playerCharacter;
    private SpriteRenderer spriteRenderer;
    public float currentHealth { get; private set; }

    private Player playerDeath;

    [Header("Invincibility")]
    [SerializeField] public float iFrameDuration = 1f;
    private bool isInvin;
    bool hasCollided = false;

    void Awake()
    {
        currentHealth = startingHealth;
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerCharacter = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        playerDeath = GetComponent<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y < -10)
        {
            playerDeath.Die();
        }
    }

    public void HealDamage(float _heal)
    {
        currentHealth = Mathf.Clamp(currentHealth + _heal, 0, startingHealth);

        if(currentHealth < 10)
        {
            return;
        }
    }

    public void TakeDamage(float damage)
    {
        if (hasCollided == false)
        {
            currentHealth = Mathf.Clamp(currentHealth - damage, 0, startingHealth);

            if (isInvin)
            {
                return;
            }

            if (currentHealth > 0)
            {
                audioSource.PlayOneShot(hit, 1.0f);
                StartCoroutine(Blinkred());
                playerCharacter.linearVelocity = knockback;
                StartCoroutine(InvincibilityFrames());
            }

            if (currentHealth <= 0 || Mathf.Approximately(currentHealth, 0f))
            {
                playerDeath.Die();
            }
        }
        else
        {
            return;
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

        hasCollided = true;

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
        hasCollided = false;
    }
}
