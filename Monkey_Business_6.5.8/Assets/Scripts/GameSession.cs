using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UIElements;

public class GameSession : MonoBehaviour
{
    [SerializeField] int playerLives = 3;

    [SerializeField] int playerScore = 0;

    [SerializeField] float deathDelay = 2.0f;

    [SerializeField] private TextMeshProUGUI lives;

    [SerializeField] private TextMeshProUGUI score;

    private void Awake()
    {
        int numGameSessions = FindObjectsByType<GameSession>().Length;

        if (numGameSessions > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }
    }

    private void Start()
    {
        RefreshUI();
    }

    private void RefreshUI()
    {
        lives.text = playerLives.ToString();
        score.text = playerScore.ToString();
    }

    public void ProcessPlayerDeath()
    {
        StartCoroutine(DeathSequence());
    }

    public void ProcessPlayerScore(int coins)
    {
        playerScore += coins;

        RefreshUI();
    }

    IEnumerator DeathSequence()
    {
        yield return new WaitForSecondsRealtime(deathDelay);

        if (playerLives > 1)
        {
            SubtractLife();
        }
        else
        {
            ResetGameSession();
        }
    }

    private void ResetGameSession()
    {
        SceneManager.LoadScene(0);

        Destroy(gameObject);
    }

    private void SubtractLife()
    {
        playerLives--;

        RefreshUI();

        var currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

        SceneManager.LoadScene(currentSceneIndex);
    }


}
