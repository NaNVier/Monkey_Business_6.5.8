using UnityEngine;

public class Coin : MonoBehaviour
{
    [SerializeField] private AudioClip coinPickSFX;

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
            
            AudioSource.PlayClipAtPoint(coinPickSFX, Camera.main.transform.position);

            Player player = collision.gameObject.GetComponent<Player>();
            player.coins += 1;
            Destroy(gameObject);
        }
    }
}
