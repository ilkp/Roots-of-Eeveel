using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEditor;

public class InteractableReadableMulti : MonoBehaviour, IInteractable
{
	public ReadableDataMulti readableData;
	private Image uiImage;
	private TextMeshProUGUI uiTextLeft;
	private TextMeshProUGUI uiTextRight;
	private Image arrowLeft;
	private Image arrowRight;
	private float arrowAlphaHigh = 0.5f;
	private float arrowAlphaLow = 0.05f;
	private float imageHeight = 800f;
	private float maxViewDist;
	private int currentPage = 0;
	private int maxPages;

	public string ToolTip { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

	public event Action<IInteractable> OnInteract;

	private void Start()
	{
		gameObject.tag = "Interactable";
		uiImage = GameObject.FindGameObjectWithTag("UIReadableImage").GetComponent<Image>();
		uiTextLeft = GameObject.FindGameObjectWithTag("UIReadableTextLeft").GetComponent<TextMeshProUGUI>();
		uiTextRight = GameObject.FindGameObjectWithTag("UIReadableTextRight").GetComponent<TextMeshProUGUI>();
		maxViewDist = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().getGrabDistance();
		arrowLeft = GameObject.FindGameObjectWithTag("ArrowLeft").GetComponent<Image>();
		arrowRight = GameObject.FindGameObjectWithTag("ArrowRight").GetComponent<Image>();
		maxPages = readableData.UIText.Length;
	}

	public void Interact()
	{
		if (uiImage.enabled || uiTextLeft.enabled || uiTextRight.enabled)
		{
			return;
		}
		uiImage.enabled = true;
		uiTextLeft.enabled = true;
		uiTextRight.enabled = true;
		arrowLeft.enabled = true;
		arrowRight.enabled = true;
		setArrowColors();
		float widthMultiplier = imageHeight / readableData.UISprite.rect.height;

		uiTextLeft.rectTransform.anchorMin = new Vector2(readableData.TAnchorLeftMinX, readableData.TAnchorMinY);
		uiTextLeft.rectTransform.anchorMax = new Vector2(0.5f - readableData.TAnchorLeftMaxX, readableData.TAnchorMaxY);
		uiTextLeft.font = readableData.font;
		uiTextLeft.fontSize = readableData.fontSize;
		uiTextLeft.color = readableData.fontColor;

		uiTextRight.rectTransform.anchorMin = new Vector2(0.5f + readableData.TAnchorRightMinX, readableData.TAnchorMinY);
		uiTextRight.rectTransform.anchorMax = new Vector2(readableData.TAnchorRightMaxX, readableData.TAnchorMaxY);
		uiTextRight.font = readableData.font;
		uiTextRight.fontSize = readableData.fontSize;
		uiTextRight.color = readableData.fontColor;

		uiImage.rectTransform.sizeDelta = new Vector2(readableData.UISprite.rect.width * widthMultiplier, imageHeight);
		uiImage.sprite = readableData.UISprite;

		uiTextLeft.text = readableData.UIText[currentPage].Replace("\\n", "\n");
		uiTextRight.text = readableData.UIText[currentPage + 1].Replace("\\n", "\n");

		StartCoroutine(hold());
	}

	public void StopInteraction()
	{

	}

	private IEnumerator hold()
	{
		yield return new WaitForSeconds(0.25f);
		Transform player = GameObject.FindGameObjectWithTag("Player").transform;
		while ((transform.position - player.position).magnitude < maxViewDist)
		{
			if (Input.GetButtonDown("Fire1"))
			{
				break;
			}
			else
			{
				yield return null;
			}
		}
		uiImage.enabled = false;
		uiTextLeft.enabled = false;
		uiTextRight.enabled = false;
		arrowRight.enabled = false;
		arrowLeft.enabled = false;
	}

	public void NextPage(int direction)
	{
		if (currentPage == 0 && direction != 1)
		{
			return;
		}
		if (currentPage == maxPages - 2 && direction != -1)
		{
			return;
		}
		currentPage += direction * 2;
		setArrowColors();
		uiTextLeft.text = readableData.UIText[currentPage].Replace("\\n", "\n");
		uiTextRight.text = readableData.UIText[currentPage + 1].Replace("\\n", "\n");
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.LeftArrow))
		{
			NextPage(-1);
		}
		if (Input.GetKeyDown(KeyCode.RightArrow))
		{
			NextPage(1);
		}
	}

	private void setArrowColors()
	{
		Color c = arrowLeft.color;
		if (currentPage == 0)
		{
			arrowLeft.color = new Color(c.r, c.g, c.b, arrowAlphaLow);
		}
		else
		{
			arrowLeft.color = new Color(c.r, c.g, c.b, arrowAlphaHigh);
		}

		c = arrowRight.color;
		if (currentPage == maxPages - 2)
		{
			arrowRight.color = new Color(c.r, c.g, c.b, arrowAlphaLow);
		}
		else
		{
			arrowRight.color = new Color(c.r, c.g, c.b, arrowAlphaHigh);
		}
	}
}