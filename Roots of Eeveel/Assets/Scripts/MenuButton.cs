
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class MenuButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
	private void Awake()
	{
		TMP_Text text = GetComponentInChildren<TMP_Text>();
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
		GetComponentInChildren<TMP_Text>().fontSharedMaterial.SetFloat(ShaderUtilities.ID_GlowPower, 0.8f);
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		GetComponentInChildren<TMP_Text>().fontSharedMaterial.SetFloat(ShaderUtilities.ID_GlowPower, 0.0f);
	}
}
