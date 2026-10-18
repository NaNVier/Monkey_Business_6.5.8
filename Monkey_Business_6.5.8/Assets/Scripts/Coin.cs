using UnityEngine;
using TMPro;

public class Coin : MonoBehaviour
{
    [SerializeField] private AudioClip coinPickSFX;
    private TextMeshProUGUI coinText;

    bool wasCollected = false; 

    private void Start()
    {
        coinText = GameObject.FindWithTag("CoinText").GetComponent<TextMeshProUGUI>();
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
            
            AudioSource.PlayClipAtPoint(coinPickSFX, Camera.main.transform.position);

            Player player = collision.gameObject.GetComponent<Player>();
            player.coins += 1;
            coinText.text = player.coins.ToString();
            Destroy(gameObject);
        }
    }
}
