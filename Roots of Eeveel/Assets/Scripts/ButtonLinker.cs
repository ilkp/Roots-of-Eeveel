
using UnityEngine;
using UnityEngine.UI;

public class ButtonLinker : MonoBehaviour
{
	private bool inMainMenu = true;
	private bool active = true;
	public static ButtonLinker Instance;
	[SerializeField] private GameObject mainMenuGroup;
	[SerializeField] private GameObject optionsGroup;
	[SerializeField] private GameObject creditsGroup;
	[SerializeField] private GameObject backgroud;

	[SerializeField] private Slider brightnessSlider;
	[SerializeField] private Slider musicVolumeSlider;
	[SerializeField] private Slider soundVolumeSlider;

	private void Awake()
	{
		if (Instance != null)
		{
			Destroy(gameObject);
		}
		Instance = this;
		DontDestroyOnLoad(this);
		gameObject.SetActive(false);
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape) && !inMainMenu)
		{
			if (active)
			{
				ToGame();
			}
			else
			{
				ToMenu();
			}
			active = !active;
		}
	}

	public void NewGame()
	{
		GameManager.Instance.LoadScene(2);
		backgroud.SetActive(false);
		inMainMenu = false;
		active = false;
		ToGame();
	}

	public void QuitGame()
	{
		GameManager.Instance.QuitGame();
	}

	public void ToGame()
	{
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
		Time.timeScale = 1.0f;
		GameManager.Instance.Paused = false;
		mainMenuGroup.SetActive(false);
		optionsGroup.SetActive(false);
		creditsGroup.SetActive(false);
	}

	public void ToMain()
	{
		ToMenu();
		backgroud.SetActive(true);
		inMainMenu = true;
	}

	public void ToMenu()
	{
		Cursor.lockState = CursorLockMode.Confined;
		Cursor.visible = true;
		Time.timeScale = 0.0f;
		GameManager.Instance.Paused = true;
		mainMenuGroup.SetActive(true);
		optionsGroup.SetActive(false);
		creditsGroup.SetActive(false);
	}

	public void ToOptions()
	{
		mainMenuGroup.SetActive(false);
		optionsGroup.SetActive(true);
		creditsGroup.SetActive(false);
	}

	public void ToCredits()
	{
		mainMenuGroup.SetActive(false);
		optionsGroup.SetActive(false);
		creditsGroup.SetActive(true);
	}

	public void saveSettings()
	{
		GameManager.Instance.saveGameSettings(
			Screen.currentResolution,
			Screen.fullScreenMode,
			brightnessSlider.value,
			musicVolumeSlider.value,
			soundVolumeSlider.value);
		GameManager.Instance.applyGameSettings();
	}
}
