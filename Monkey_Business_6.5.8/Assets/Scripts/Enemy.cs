using UnityEngine;
using UnityEngine.Rendering;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float speed = 1.0f;
    [SerializeField] private float damage;

    Rigidbody2D enemyCharacter;
    public Transform[] points;
    private int i;
    private SpriteRenderer spriteRenderer;
    private BoxCollider2D boxCollider;
    private Transform player;
    private bool isCharging;

    // Charge
    public bool canCharge = false;
    [SerializeField] public float chargeSpeed;
    [SerializeField] public float sightDistance;
    [SerializeField] public float loseDistance;
    public LayerMask playerLayer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        enemyCharacter = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
    }

    void Update()
    {
        if (canCharge && DetectPlayer())
        {
            isCharging = true;
        }

        if (isCharging)
        {
            ChargePlayer();
        }
        else
        {
            Patrol();
        }
    }

    void Patrol()
    {
        if(Vector2.Distance(transform.position, points[i].position) < 0.25f)
        {
            i++;
            if(i == points.Length)
            {
                i = 0;
            }
        }

        transform.position = Vector2.MoveTowards(transform.position, points[i].position, speed * Time.deltaTime);

        spriteRenderer.flipX = (transform.position.x - points[i].position.x) > 0f;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        transform.localScale = new Vector2(-(Mathf.Sign(enemyCharacter.linearVelocity.x)), 1.0f);
        
        if(collision.tag == "Player")
        {
            collision.GetComponent<PlayerHealth>().TakeDamage(damage);
        }

        if(collision.tag == "Laser")
        {
            Die();
        }
    }

    bool DetectPlayer()
    {
        Vector2 direction = spriteRenderer.flipX ? Vector2.left : Vector2.right;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, sightDistance, playerLayer);

        if(hit.collider != null)
        {
            player = hit.transform;
            return true;
        }

        return false;
    }

    void ChargePlayer()
    {
        if(player == null)
        {
            isCharging = false;
            return;
        }

        float distance = Vector2.Distance(transform.position, player.position);

        if(distance > loseDistance)
        {
            isCharging = false;
            player = null;
            return;
        }

        Vector2 target = new Vector2(player.position.x, transform.position.y);

        transform.position = Vector2.MoveTowards(transform.position, target, chargeSpeed * Time.deltaTime);

        spriteRenderer.flipX = (transform.position.x - player.position.x) < 0f;
    }

    bool IsFacingRight()
    {
        return transform.localScale.x > 0;
    }

    void Die()
    {
        Destroy(gameObject);
    }
}
