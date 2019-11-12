
using UnityEngine;
using TMPro;

[CreateAssetMenu(fileName = "New Readable", menuName = "Readables")]
public class ReadableData : ScriptableObject
{
	public TMP_FontAsset font;
	public int fontSize;
	public Color fontColor;
	public Sprite UISprite;
	public string UIText;
	public float TAnchorMinX;
	public float TAnchorMaxX;
	public float TAnchorMinY;
	public float TAnchorMaxY;
}
