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

	private void OnEnable()
	{
		SceneManager.sceneLoaded += OnLevelFinishedLoaded;
	}

	private void OnDisable()
	{
		SceneManager.sceneLoaded -= OnLevelFinishedLoaded;
	}

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

	
	private void OnLevelFinishedLoaded(Scene scene, LoadSceneMode loadMode)
	{
		switch (scene.buildIndex)
		{
			case 0:
				StartCoroutine(LoadSceneAsync(1));
				break;
			case 1:
				//GameObject.FindGameObjectWithTag("s").GetComponent<Button>().onClick.
				break;
			default:
				break;

		}
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

	public void LoadScene(int scene)
	{
		StartCoroutine(LoadSceneAsync(scene));
	}

	private IEnumerator LoadSceneAsync(int scene)
	{
		AsyncOperation operation = SceneManager.LoadSceneAsync(scene);
		while (!operation.isDone)
		{
			yield return null;
		}
	}
}
