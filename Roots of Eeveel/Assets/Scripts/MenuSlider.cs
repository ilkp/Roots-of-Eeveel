
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MenuSlider : MonoBehaviour
{
	private Slider slider;
	private TMP_InputField inputField;
	string inputFieldPrevValue;

	private void Awake()
	{
		slider = GetComponentInChildren<Slider>();
		inputField = GetComponentInChildren<TMP_InputField>();
		inputField.text = slider.value.ToString();
		inputFieldPrevValue = slider.value.ToString();
	}

	public float getAmount()
	{
		return slider.value;
	}

	public void sliderValueChanged()
	{
		slider.value = Mathf.Round(slider.value * 100f) / 100f;
		inputField.text = slider.value.ToString();
	}

	public void inputValueChanged()
	{
		float newValue;
		if (inputField.text.Length == 0 || inputField.text == null)
		{
			inputFieldPrevValue = "";
			slider.value = 0;
			return;
		}
		else if (!float.TryParse(inputField.text, out newValue))
		{
			inputField.text = inputFieldPrevValue;
			return;
		}
		inputFieldPrevValue = inputField.text;
		slider.value = newValue;
	}
}
