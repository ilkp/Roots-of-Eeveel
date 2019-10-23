
using UnityEngine;
using UnityEngine.UI;

public class MenuSlider : MonoBehaviour
{
	private Slider slider;

	private void Awake()
	{
		slider = GetComponentInChildren<Slider>();
		slider.minValue = 0.0f;
		slider.maxValue = 1.0f;
	}

	public float getAmount()
	{
		return slider.value;
	}
}
