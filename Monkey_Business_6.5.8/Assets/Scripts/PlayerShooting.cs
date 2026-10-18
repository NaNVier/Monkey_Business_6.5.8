using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    [SerializeField] private float attackCooldown;
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject[] Laser;
    private Player playerMovement;
    AudioSource audioSource;
    [SerializeField] AudioClip shoot;
    private float cooldownTimer = Mathf.Infinity;
    
    private void Awake()
    {
        playerMovement = GetComponent<Player>();
        audioSource = GetComponent<AudioSource>();
    }
    private void Update()
    {
        if(Input.GetMouseButton(0) && cooldownTimer > attackCooldown)
        {
            audioSource.PlayOneShot(shoot, 1f);
            Attack();
        }

        cooldownTimer += Time.deltaTime;
    }

    private void Attack()
    {
        cooldownTimer = 0;

        Laser[findLaser()].transform.position = firePoint.position;
        Laser[findLaser()].GetComponent<Laser>().SetDirection(Mathf.Sign(transform.localScale.x));
    }

    private int findLaser()
    {
        for (int i = 0; i < Laser.Length ; i++)
        {
            if (!Laser[i].activeInHierarchy)
            {
                return i;
            }
        }
        
        return 0;

    }
}
