
using UnityEngine;

public class ButtonLinker : MonoBehaviour
{
	public void NewGame()
	{
		GameManager.Instance.LoadScene(2);
	}
}
