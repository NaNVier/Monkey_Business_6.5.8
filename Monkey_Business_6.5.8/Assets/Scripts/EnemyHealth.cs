using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] public float maxEnemyHealth;
    private float currentEnemyHealth;
    void Awake()
    {
        currentEnemyHealth = maxEnemyHealth;
    }

    public void EnemyTakeDamage(float _damage)
    {
        currentEnemyHealth -= Mathf.Clamp(currentEnemyHealth - _damage, 0, maxEnemyHealth);

        if (currentEnemyHealth > 0)
        {
            return;
        }
        else
        {
            Die();
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    void Die()
    {
        Destroy(gameObject);
    }
}
