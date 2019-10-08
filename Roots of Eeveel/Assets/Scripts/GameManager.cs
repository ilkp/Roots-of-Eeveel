using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public Canvas endingScreen;
    public Button resetButton;

    private void Awake()
    {
        Time.timeScale = 1;
        if (Instance != null)
        {
            Destroy(this);
        }
        Instance = this;
        DontDestroyOnLoad(this);
    }

    public void SetGameOver()
    {
        endingScreen.gameObject.SetActive(true);
        Time.timeScale = 0;
        StartCoroutine(GameOverFadeIn());
    }

    private IEnumerator GameOverFadeIn()
    {
        CanvasGroup gameOverGroup = endingScreen.GetComponent<CanvasGroup>();
        while (gameOverGroup.alpha < 1f)
        {
            gameOverGroup.alpha += 0.01f;
            yield return 0;
        }

        yield return new WaitForSecondsRealtime(2);
        resetButton.gameObject.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void ResetGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
