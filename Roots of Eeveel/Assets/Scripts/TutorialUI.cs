using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialUI : MonoBehaviour
{
	[SerializeField] private Text controlsText;
	[SerializeField] private float fadeDelay = 60;
	[SerializeField] private float fadeTime = 6;

	private void Start()
	{
		StartCoroutine(fadeText(controlsText, fadeDelay, fadeTime));
	}

	private IEnumerator fadeText(Text text, float fadeDelay, float fadeTime)
	{
		yield return new WaitForSeconds(fadeDelay);

		Color originalColor = text.color;

		for (float currentTime = 0.01f; currentTime < fadeTime; currentTime += Time.deltaTime)
		{
			text.color = Color.Lerp(originalColor, Color.clear, Mathf.Min(1, currentTime / fadeTime));
			yield return null;
		}

		text.gameObject.SetActive(false);
	}
}
