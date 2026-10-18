using UnityEngine;

public class Health_Pickup : MonoBehaviour
{
    [SerializeField] private AudioClip healthPickSFX;
    [SerializeField] private float heal = 5f;

    bool wasCollected = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            if (wasCollected)
            {
                return;
            }

            wasCollected = true;

            collision.GetComponent<PlayerHealth>().HealDamage(heal);
            Destroy(gameObject);
        }
    }
}
