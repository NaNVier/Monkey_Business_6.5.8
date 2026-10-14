using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] public float maxEnemyHealth;
    private float currentEnemyHealth;

    SpriteRenderer spriteRenderer;
    private Color originalColor;
    void Awake()
    {
        currentEnemyHealth = maxEnemyHealth;

        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null) originalColor = spriteRenderer.color;
    }

    public void EnemyTakeDamage(float damageAmount)
    {
        currentEnemyHealth -= damageAmount;
        currentEnemyHealth -= Mathf.Clamp(currentEnemyHealth, 0, maxEnemyHealth);

        StartCoroutine(FlashDamageColor());

        if (currentEnemyHealth <= 0f)
        {
            Die();
        }
    }

    private System.Collections.IEnumerator FlashDamageColor()
    {
        if (spriteRenderer == null) yield break;

        spriteRenderer.color = Color.red; // Flash red on hit
        yield return new WaitForSeconds(0.15f);
        spriteRenderer.color = originalColor;
    }
    void Update()
    {
        
    }

    void Die()
    {
        Destroy(gameObject);
    }
}
