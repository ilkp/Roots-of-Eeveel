using System;
using UnityEngine;
using TMPro;

public class Interactable_ReadableUI : MonoBehaviour, IInteractable
{
	[SerializeField] private ReadableData data;
	[SerializeField] private UnityEngine.UI.Image uiImage;
	[SerializeField] private TMP_Text uiText;
	private float imageHeight = 800f;

	public string ToolTip { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

	public event Action<IInteractable> OnInteract;

	public void Interact()
	{
		uiText.gameObject.SetActive(true);
		float widthMultiplier = imageHeight / data.UISprite.rect.height;
		uiText.rectTransform.anchorMin = new Vector2(data.TAnchorMinX, data.TAnchorMinY);
		uiText.rectTransform.anchorMax = new Vector2(data.TAnchorMaxX, data.TAnchorMaxY);
		uiImage.rectTransform.sizeDelta = new Vector2(data.UISprite.rect.width * widthMultiplier, imageHeight);
		uiImage.sprite = data.UISprite;
		uiText.text = data.UIText;
	}

	public void StopInteraction()
	{
		Debug.Log("stop intercation");
		uiText.gameObject.SetActive(false);
	}
}
