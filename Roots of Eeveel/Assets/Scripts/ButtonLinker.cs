
using UnityEngine;

public class ButtonLinker : MonoBehaviour
{
	[SerializeField] private GameObject mainMenuCanvas;
	[SerializeField] private GameObject optionsCanvas;
	[SerializeField] private GameObject creditsCanvas;

	public void NewGame()
	{
		GameManager.Instance.LoadScene(2);
	}

	public void QuitGame()
	{
		GameManager.Instance.QuitGame();
	}

	public void ToMain()
	{
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
