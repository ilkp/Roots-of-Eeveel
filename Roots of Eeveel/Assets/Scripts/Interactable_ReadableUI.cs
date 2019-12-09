using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class Interactable_ReadableUI : MonoBehaviour, IInteractable
{
    public ReadableData readableData;
    private Image uiImage;
    private TextMeshProUGUI uiText;
    private float imageHeight = 800f;
    private float maxViewDist;

    public string ToolTip { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public event Action<IInteractable> OnInteract;

    private void Start()
    {
        gameObject.tag = "Interactable";
        uiImage = GameObject.FindGameObjectWithTag("UIReadableImage").GetComponent<Image>();
        uiText = GameObject.FindGameObjectWithTag("UIReadableTextLeft").GetComponent<TextMeshProUGUI>();
        maxViewDist = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().getGrabDistance();
    }

    public void Interact()
    {
        if (uiImage.enabled || uiText.enabled)
        {
            return;
        }
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().allowMovement = false;
        uiImage.enabled = true;
        uiText.enabled = true;
        float widthMultiplier = imageHeight / readableData.UISprite.rect.height;
        uiText.rectTransform.anchorMin = new Vector2(readableData.TAnchorMinX, readableData.TAnchorMinY);
        uiText.rectTransform.anchorMax = new Vector2(readableData.TAnchorMaxX, readableData.TAnchorMaxY);
        uiText.font = readableData.font;
        uiText.fontSize = readableData.fontSize;
        uiText.alignment = readableData.alignment;
        uiText.color = readableData.fontColor;
        uiImage.rectTransform.sizeDelta = new Vector2(readableData.UISprite.rect.width * widthMultiplier, imageHeight);
        uiImage.sprite = readableData.UISprite;
        uiText.text = readableData.UIText.Replace("\\n", "\n");

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
            if (Input.GetKeyDown(Keybindings.Instance.interaction) || Input.GetKeyDown(Keybindings.Instance.altInteraction))
            {
                break;
            }
            else
            {
                yield return null;
            }
        }
        uiImage.enabled = false;
        uiText.enabled = false;

        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().allowMovement = true;
    }
}
