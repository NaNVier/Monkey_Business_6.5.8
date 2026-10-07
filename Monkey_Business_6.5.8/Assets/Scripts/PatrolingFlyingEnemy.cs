using UnityEngine;
using UnityEngine.AI;

public class PatrolingFlyingEnemy : MonoBehaviour
{
    [SerializeField] private float damage;
    [SerializeField] private float speed = 1.0f;
    public float detectionRange = 6;
    public float updateRate = 0.2f;

    public Transform[] points;
    private int i;
    private SpriteRenderer spriteRenderer;
    private Transform player;
    private NavMeshAgent agent;
    private float nextUpdateTime;
    
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;


        player = GameObject.FindWithTag("Player").transform;
    }

    
    void Update()
    {
        float distance = Vector2.Distance(transform.position, player.position);

        if(distance <= detectionRange)
        {
            if(Time.time >= nextUpdateTime)
            {
                agent.SetDestination(player.position);
                nextUpdateTime = Time.time + updateRate;
            }
        }
        else
        {
            agent.ResetPath();
            Patrol();
        }
    }

    void Patrol()
    {
        if (Vector2.Distance(transform.position, points[i].position) < 0.25f)
        {
            i++;
            if (i == points.Length)
            {
                i = 0;
            }
        }

        transform.position = Vector2.MoveTowards(transform.position, points[i].position, speed * Time.deltaTime);

        spriteRenderer.flipX = (transform.position.x - points[i].position.x) > 0f;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            collision.GetComponent<PlayerHealth>().TakeDamage(damage);
        }

        if (collision.gameObject.tag == "Laser")
        {
            Destroy(transform.parent.gameObject);
        }
    }
}
