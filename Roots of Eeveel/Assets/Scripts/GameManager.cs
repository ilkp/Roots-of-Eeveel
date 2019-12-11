using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System;
using UnityEngine.Rendering;
using UnityEngine.Experimental.Rendering.HDPipeline;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
	public AudioSettings audioSettings;

    public static GameManager Instance { get; private set; }
	public GameSettingsWrapper gameSettings { get; private set; }
	public bool Paused = true;

	[SerializeField] private Image loadingScreenBackground;
	[SerializeField] private Image loadingScreenLogo;
	[SerializeField] private Image fillImage;
	private const float fillFull = 0.75f;
	private const float fillChange = 0.5f;

	private Image gameOverImage;
	private TMP_Text gameOverText;
	private TMP_Text creditsTextLeft;
	private TMP_Text creditsTextCenter;
	private TMP_Text creditsTextRight;

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
				gameOverImage = GameObject.FindGameObjectWithTag("GameOverImage").GetComponent<Image>();
				gameOverText = GameObject.FindGameObjectWithTag("GameOverText").GetComponent<TMP_Text>();
				creditsTextLeft = GameObject.FindGameObjectWithTag("Credits").GetComponent<TMP_Text>();
				creditsTextCenter = GameObject.FindGameObjectWithTag("CreditsCenter").GetComponent<TMP_Text>();
				creditsTextRight = GameObject.FindGameObjectWithTag("CreditsRight").GetComponent<TMP_Text>();
				applyBrightness();
				SoundManager.Instance.LoadEnemies();
				ButtonLinker.Instance.ToGame();
				break;
			default:
				break;

		}
	}

	public void SetGameOver(bool winStatus)
    {
		audioSettings.StopAllSounds();
		gameOverImage.enabled = true;
		gameOverText.enabled = true;
		if (winStatus)
		{
			gameOverText.text = "Thank you for playing!";
		}
		else
		{
			gameOverText.text = "Game over\nYou Lose!";
		}
		StartCoroutine(GameOverFadeIn(winStatus));

	}

    private IEnumerator GameOverFadeIn(bool winStatus)
    {
		float fadeTime = 3f;
		float timer = 0f;
		float creditsTimer = 20f;
        while (timer < fadeTime)
        {
			gameOverImage.color = new Color(0f, 0f, 0f, timer / fadeTime);
			gameOverText.alpha = Mathf.Pow(timer / fadeTime, 3);
			timer += Time.deltaTime;
            yield return 0;
        }
		gameOverImage.color = new Color(0f, 0f, 0f, 1f);
		gameOverText.alpha = 1f;
		yield return new WaitForSecondsRealtime(2);
		if (winStatus)
		{
			creditsTextLeft.enabled = true;
			creditsTextRight.enabled = true;
			creditsTextCenter.enabled = true;
			timer = 0f;
			float fadeAmount;
			while (timer < fadeTime)
			{
				fadeAmount = Mathf.Pow(timer / fadeTime, 3);
				gameOverText.alpha = Mathf.Pow(1 - timer / fadeTime, 3);
				creditsTextLeft.alpha = fadeAmount;
				creditsTextRight.alpha = fadeAmount;
				creditsTextCenter.alpha = fadeAmount;
				timer += Time.deltaTime;
				yield return 0;
			}
			creditsTextLeft.alpha = 1f;
			creditsTextRight.alpha = 1f;
			creditsTextCenter.alpha = 1f;
			timer = 0f;
			while (timer < creditsTimer)
			{
				Debug.Log(timer);
				timer += Time.deltaTime;
				if (Input.anyKeyDown)
				{
					break;
				}
				yield return null;
			}
		}
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
		fillImage.enabled = true;
		fillImage.transform.localScale = new Vector3(1, fillFull, 1);
		AsyncOperation operation = SceneManager.LoadSceneAsync(scene);
		operation.allowSceneActivation = false;
		float cutSceneTimer = 0f;
		const float cutSceneMaxTime = 9f;

		switch (scene)
		{
			case 2:
				StartCoroutine(audioSettings.FadeMenuMusic());
				StartCoroutine(audioSettings.PlayLoreStart());
				break;
			default:
				break;
		}

		while (!operation.isDone)
		{
			if (scene == 2)
			{
				cutSceneTimer += Time.deltaTime;
				if (cutSceneTimer > cutSceneMaxTime && operation.progress >= 0.9f)
				{
					operation.allowSceneActivation = true;
				}
			}
			else if (operation.progress >= 0.9f)
			{
				operation.allowSceneActivation = true;
			}
			fillImage.transform.localScale = new Vector3(1, fillFull - fillChange * (operation.progress / 0.9f), 1);
			yield return null;
		}
		loadingScreenBackground.enabled = false;
		loadingScreenLogo.enabled = false;
		fillImage.enabled = false;
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
			wrapper.setValues(
				Screen.width,
				Screen.height,
				Screen.currentResolution.refreshRate,
				(int)FullScreenMode.ExclusiveFullScreen,
				0.3f,
				0.3f,
				0.3f,
				0.3f,
				0.3f);
		}
		gameSettings = wrapper;
		ButtonLinker.Instance.initSliders(gameSettings);
	}

	public void saveGameSettings(
		Resolution resolution,
		FullScreenMode fullscreenMode,
		float brightness,
		float musicVolume,
		float soundsVolume,
		float atmosphereVolume,
		float voiceVolume)
	{
		string path = Path.Combine(Application.dataPath, "settings.txt");
		GameSettingsWrapper wrapper = new GameSettingsWrapper();
		wrapper.setValues(
			resolution.width,
			resolution.height,
			resolution.refreshRate,
			(int)fullscreenMode,
			brightness,
			musicVolume,
			soundsVolume,
			atmosphereVolume,
			voiceVolume);
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
