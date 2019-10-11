using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public Canvas endingScreen;
    public Button resetButton;
	public bool Paused = true;

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
				ButtonLinker.Instance.gameObject.SetActive(true);
				ButtonLinker.Instance.ToMain();
				break;
			case 2:
				SoundManager.Instance.LoadEnemies();
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
		LoadScene(1);
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

	private GameSettingsWrapper loadGameSettings(string fileName)
	{
		string path = Path.Combine(Application.dataPath, fileName);
		Debug.Log(path);
		string file;
		GameSettingsWrapper wrapper;
		try
		{
			file = File.ReadAllText(path);
		}
		catch (FileNotFoundException)
		{
			throw new FileNotFoundException("Game settings file not found");
		}

		try
		{
			wrapper = JsonUtility.FromJson<GameSettingsWrapper>(file);
		}
		catch (ArgumentException)
		{
			throw new ArgumentException("Game settings file invalid");
		}
		return wrapper;
	}

	private void saveGameSettings(string fileName)
	{
		string path = Path.Combine(Application.dataPath, fileName);
		GameSettingsWrapper wrapper = new GameSettingsWrapper();
		wrapper.brightness = 1.0f;
		string file = JsonUtility.ToJson(wrapper);
		File.WriteAllText(path, file);
	}

	public void QuitGame()
	{
		#if UNITY_EDITOR
			UnityEditor.EditorApplication.isPlaying = false;
		#else
			Application.Quit();
		#endif
	}
}
