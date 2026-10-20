using UnityEngine;

public class VerticalScroll : MonoBehaviour
{
    [Tooltip("Game Units per Second")]
    [SerializeField] private float scrollSpeed = 0.2f;
    void Update()
    {
        transform.Translate(new Vector2 (0.0f, scrollSpeed * Time.deltaTime));
    }
}
