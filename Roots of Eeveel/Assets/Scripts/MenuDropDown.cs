
using UnityEngine;
using TMPro;

public class MenuDropDown : MonoBehaviour
{
	[SerializeField] private TMP_Dropdown dropDown;

	private void Awake()
	{
		dropDown = GetComponentInChildren<TMP_Dropdown>();
		dropDown.options.Clear();
		for (int i = 0; i < Screen.resolutions.Length; i++)
		{
			dropDown.options.Add(new TMP_Dropdown.OptionData(Screen.resolutions[i].ToString()));
		}
	}
}
