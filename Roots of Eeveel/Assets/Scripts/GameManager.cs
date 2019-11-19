using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System;
using UnityEngine.Rendering;
using UnityEngine.Experimental.Rendering.HDPipeline;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
	public AudioSettings audioSettings;

    public static GameManager Instance { get; private set; }
	public GameSettingsWrapper gameSettings { get; private set; }
    public GameObject endingScreenWin;
	public GameObject endingScreenLose;
	public bool Paused = true;

	[SerializeField] private Image loadingScreenBackground;
	[SerializeField] private Image loadingScreenLogo;

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
			case 0: // startup scene
				audioSettings.StopMenuMusic();
				loadGameSettings();
				applyGameSettings();
				StartCoroutine(LoadSceneAsync(1));
				break;
			case 1: // main menu scene
				audioSettings.StopMenuMusic();
				audioSettings.PlayMenuMusic();
				ButtonLinker.Instance.gameObject.SetActive(true);
				ButtonLinker.Instance.ToMain();
				break;
			case 2: // game scene
				applyBrightness();
				StartCoroutine(audioSettings.FadeMenuMusic());
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
		loadingScreenBackground.enabled = true;
		loadingScreenLogo.enabled = true;
		AsyncOperation operation = SceneManager.LoadSceneAsync(scene);
		while (!operation.isDone)
		{
			yield return null;
		}
		loadingScreenBackground.enabled = false;
		loadingScreenLogo.enabled = false;
	}

	private void loadGameSettings()
	{
		string path = Path.Combine(Application.dataPath, "settings.txt");
		string file;
		GameSettingsWrapper wrapper;
		try
		{
			file = File.ReadAllText(path);
			wrapper = JsonUtility.FromJson<GameSettingsWrapper>(file); // wrapper will be null if json can't be deserialized
			if (wrapper == null)
			{
				Debug.LogWarning("Failed to deserialize settings file");
			}
		}
		catch(FileNotFoundException)
		{
			Debug.LogWarning("Game settings file not found");
			wrapper = null;
		}
		if (wrapper == null)
		{
			wrapper = new GameSettingsWrapper();
			wrapper.setValues(Screen.width, Screen.height, Screen.currentResolution.refreshRate, (int)FullScreenMode.ExclusiveFullScreen, 0.5f, 0.3f, 0.3f);
		}
		gameSettings = wrapper;
	}

	public void saveGameSettings(Resolution resolution, FullScreenMode fullscreenMode, float brightness, float musicVolume, float soundsVolume)
	{
		string path = Path.Combine(Application.dataPath, "settings.txt");
		GameSettingsWrapper wrapper = new GameSettingsWrapper();
		wrapper.setValues(resolution.width, resolution.height, resolution.refreshRate, (int)fullscreenMode, brightness, musicVolume, soundsVolume);
		string file = JsonUtility.ToJson(wrapper);
		File.WriteAllText(path, file);
		gameSettings = wrapper;
	}

	public void applyGameSettings()
	{
		RenderSettings.ambientLight = new Color(gameSettings.Brightness, gameSettings.Brightness, gameSettings.Brightness, 1.0f);
		Screen.SetResolution(gameSettings.ResolutionX, gameSettings.ResolutionY, (FullScreenMode)gameSettings.FullscreenMode, gameSettings.RefreshRate);
		audioSettings.musicVolume = gameSettings.MusicVolume;
		audioSettings.soundsVolume = gameSettings.SoundsVolume;
		if (SceneManager.GetActiveScene().buildIndex == 2)
		{
			applyBrightness();
		}
	}

	public void applyBrightness()
	{
		Volume renderSettings = GameObject.FindObjectOfType<Volume>();
		IndirectLightingController ilc;
		renderSettings.profile.TryGet<IndirectLightingController>(out ilc);
		ilc.indirectDiffuseIntensity.value = gameSettings.Brightness;
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
