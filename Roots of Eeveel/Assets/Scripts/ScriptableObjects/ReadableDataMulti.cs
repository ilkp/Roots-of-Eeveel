
using UnityEngine;
using TMPro;

[CreateAssetMenu(menuName = "Readables/Readable Multi")]
public class ReadableDataMulti : ScriptableObject
{
	public TMP_FontAsset font;
	public int fontSize;
	public Color fontColor;
	public TextAlignmentOptions alignment;
	public Sprite UISprite;
	public string[] UIText;
	public float TAnchorLeftMinX;
	public float TAnchorLeftMaxX; // in relation to 0.5f
	public float TAnchorRightMinX; // in relation to 0.5f
	public float TAnchorRightMaxX;
	public float TAnchorMinY;
	public float TAnchorMaxY;
}
