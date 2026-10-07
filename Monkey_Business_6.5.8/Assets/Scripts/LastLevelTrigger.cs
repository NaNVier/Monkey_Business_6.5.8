using UnityEngine;

public class LastLevelTrigger : MonoBehaviour
{
    public string lastLevelName;
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            LoadLastLevel();
        }
    }

    private void LoadLastLevel()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(lastLevelName);
        Time.timeScale = 1;
    }
}
