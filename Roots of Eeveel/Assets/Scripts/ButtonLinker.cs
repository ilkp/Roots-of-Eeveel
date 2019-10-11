
using UnityEngine;

public class ButtonLinker : MonoBehaviour
{
	private bool inMainMenu = true;
	private bool active = true;
	public static ButtonLinker Instance;
	[SerializeField] private GameObject mainMenuCanvas;
	[SerializeField] private GameObject optionsCanvas;
	[SerializeField] private GameObject creditsCanvas;
	[SerializeField] private GameObject Background;


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
		Background.SetActive(false);
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
		mainMenuCanvas.SetActive(false);
		optionsCanvas.SetActive(false);
		creditsCanvas.SetActive(false);
	}

	public void ToMain()
	{
		ToMenu();
		Background.SetActive(true);
	}

	public void ToMenu()
	{
		Cursor.lockState = CursorLockMode.Confined;
		Cursor.visible = true;
		Time.timeScale = 0.0f;
		GameManager.Instance.Paused = true;
		mainMenuCanvas.SetActive(true);
		optionsCanvas.SetActive(false);
		creditsCanvas.SetActive(false);
	}

	public void ToOptions()
	{
		mainMenuCanvas.SetActive(false);
		optionsCanvas.SetActive(true);
		creditsCanvas.SetActive(false);
	}

	public void ToCredits()
	{
		mainMenuCanvas.SetActive(false);
		optionsCanvas.SetActive(false);
		creditsCanvas.SetActive(true);
	}
}
