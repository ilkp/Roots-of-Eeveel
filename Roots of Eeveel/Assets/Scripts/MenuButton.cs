
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class MenuButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
	private void Awake()
	{
		TextMeshProUGUI text = GetComponentInChildren<TextMeshProUGUI>();
		Material mat = text.fontMaterial;
		mat.EnableKeyword("GLOW_ON");
		mat.SetColor(ShaderUtilities.ID_GlowColor, Color.green);
		mat.SetFloat(ShaderUtilities.ID_GlowPower, 0.0f);
		mat.SetFloat(ShaderUtilities.ID_GlowOffset, -0.3f);
		mat.SetFloat(ShaderUtilities.ID_GlowOuter, 0.7f);
		mat.SetFloat("_GlowInner", 0.1f);
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		StartCoroutine(FindObjectOfType<GameManager>().audioSettings.PlayUIButton());
		GetComponentInChildren<TextMeshProUGUI>().fontSharedMaterial.SetFloat(ShaderUtilities.ID_GlowPower, 0.8f);
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		GetComponentInChildren<TextMeshProUGUI>().fontSharedMaterial.SetFloat(ShaderUtilities.ID_GlowPower, 0.0f);
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		GetComponentInChildren<TextMeshProUGUI>().fontSharedMaterial.SetFloat(ShaderUtilities.ID_GlowPower, 0.0f);
	}
}
