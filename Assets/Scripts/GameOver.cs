using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    public GameObject gameOverScreen;
    public GameObject winScreen;
    public float fadeDuration = 2f;
    private CanvasGroup canvasGroup;
    private CanvasGroup winCanvasGroup;

    public int timesDied = 0;

    [SerializeField]
    private TextMeshProUGUI timesDiedText;

    void Start()
    {
        gameOverScreen.SetActive(false);
        winScreen.SetActive(false);

        canvasGroup = gameOverScreen.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameOverScreen.AddComponent<CanvasGroup>();
        }
        winCanvasGroup = winScreen.GetComponent<CanvasGroup>();
        if (winCanvasGroup == null)
        {
            winCanvasGroup = winScreen.AddComponent<CanvasGroup>();
        }
        canvasGroup.alpha = 0f;
    }

    public void ShowGameOverScreen()
    {
        gameOverScreen.SetActive(true);
        StartCoroutine(FadeInCoroutine());
    }

    public void ShowWinScreen()
    {
        timesDiedText.text = "Times you died: " + timesDied.ToString();
        winScreen.SetActive(true);
        StartCoroutine(FadeInWinCoroutine());
        canvasGroup.alpha = 1f;
    }

    IEnumerator FadeInCoroutine()
    {
        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, timer / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = 1f;
    }

    IEnumerator FadeInWinCoroutine()
    {
        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            winCanvasGroup.alpha = Mathf.Lerp(0f, 1f, timer / fadeDuration);
            yield return null;
        }

        winCanvasGroup.alpha = 1f;
    }

    public void Respawn()
    {
        gameOverScreen.SetActive(false);
        CheckpointManager.Instance.RespawnPlayer();
    }

    public void GoToMainMenu()
    {
        gameOverScreen.SetActive(false);
        winScreen.SetActive(false);
        SceneManager.LoadScene(0);
    }
}
