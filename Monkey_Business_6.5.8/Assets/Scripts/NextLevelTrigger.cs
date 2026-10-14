using UnityEngine;

public class NextLevelTrigger : MonoBehaviour
{
    public string nextLevelName;
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            LoadNextLevel();
        }
    }

    private void LoadNextLevel()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(nextLevelName);
        Time.timeScale = 1;
    }
}
