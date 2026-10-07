using UnityEngine;

public class RisingWater : MonoBehaviour
{
    [SerializeField] public float riseSpeed = 1f;
    [SerializeField] public float maxHeight = 30f;
    private SpriteRenderer spriteRenderer;
    private BoxCollider2D boxCollider;
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        boxCollider = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if(transform.localScale.y < maxHeight)
        {
            Vector2 currentSize = transform.localScale;
            currentSize.y += riseSpeed * Time.deltaTime;
            transform.localScale = currentSize;
        }
    }
}
