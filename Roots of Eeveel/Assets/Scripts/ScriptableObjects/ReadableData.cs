
using UnityEngine;
using TMPro;

[CreateAssetMenu(menuName = "Readables/Readable Base")]
public class ReadableData : ScriptableObject
{
	public TMP_FontAsset font;
	public int fontSize;
	public Color fontColor;
	public TextAlignmentOptions alignment;
	public Sprite UISprite;
	public string UIText;
	public float TAnchorMinX;
	public float TAnchorMaxX;
	public float TAnchorMinY;
	public float TAnchorMaxY;
}

