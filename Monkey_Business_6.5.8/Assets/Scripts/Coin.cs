using UnityEngine;
using TMPro;

public class Coin : MonoBehaviour
{
    [SerializeField] private int pickupValue = 1;
    [SerializeField] private AudioClip coinPickSFX;

    bool wasCollected = false; 

    private void Start()
    {
    
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            if (wasCollected)
            {
                return;
            }

            wasCollected = true;

            FindFirstObjectByType<GameSession>().ProcessPlayerScore(pickupValue);


            
            AudioSource.PlayClipAtPoint(coinPickSFX, Camera.main.transform.position);

            Player player = collision.gameObject.GetComponent<Player>();
            player.coins += 1;
            Destroy(gameObject);
        }
    }
}
