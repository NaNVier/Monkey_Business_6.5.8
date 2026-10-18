using UnityEngine;

public class WaterTrigger : MonoBehaviour
{
    [SerializeField] private float damage = 10;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            collision.GetComponent<PlayerHealth>().TakeDamage(damage);
        }
    }
}
