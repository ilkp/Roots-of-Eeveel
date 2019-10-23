using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System;

public class GameManager : MonoBehaviour
{
	// The audio instance that playes the actual sounds
	private FMOD.Studio.EventInstance audioInstance = new FMOD.Studio.EventInstance();
	// The audio to be played
	[FMODUnity.EventRef] [SerializeField] private string menuMusic;
	// Fade time of the menu music
	[SerializeField] private float musicFadeTime = 5f;

	public static GameManager Instance { get; private set; }
    public GameObject endingScreenWin;
	public GameObject endingScreenLose;
	public bool Paused = true;

	private void OnEnable()
	{
		audioInstance = FMODUnity.RuntimeManager.CreateInstance(menuMusic);
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
			case 0: // startup scene
				audioInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
				StartCoroutine(LoadSceneAsync(1));
				break;
			case 1: // main menu scene
				audioInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
				ButtonLinker.Instance.gameObject.SetActive(true);
				ButtonLinker.Instance.ToMain();
				audioInstance.start(); // Start playing the menu tune
				break;
			case 2: // game scene
				StartCoroutine(FadeMusic(musicFadeTime));
				SoundManager.Instance.LoadEnemies();
				ButtonLinker.Instance.ToGame();
				endingScreenWin = GameObject.FindGameObjectWithTag("GameOverWin");
				endingScreenWin.gameObject.SetActive(false);
				endingScreenLose = GameObject.FindGameObjectWithTag("GameOverLose");
				endingScreenLose.gameObject.SetActive(false);
				break;
			default:
				break;

		}
	}

	private IEnumerator FadeMusic(float fadeTime)
	{
		float counter = 0f;
		audioInstance.getVolume(out float startVolume);

		while (counter < fadeTime)
		{
			counter += Time.deltaTime;
			audioInstance.setVolume(Mathf.Lerp(startVolume, 0f, (counter / fadeTime)));

			yield return null;
		}

		audioInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
	}

	public void SetGameOver(bool winStatus)
    {
		GameObject gameOverGroup = winStatus ? endingScreenWin : endingScreenLose;
		gameOverGroup.SetActive(true);
        Time.timeScale = 0;
        StartCoroutine(GameOverFadeIn(gameOverGroup.GetComponent<CanvasGroup>()));
	}

    private IEnumerator GameOverFadeIn(CanvasGroup gameOverGroup)
    {
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
