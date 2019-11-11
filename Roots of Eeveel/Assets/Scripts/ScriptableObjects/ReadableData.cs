
using UnityEngine;

[CreateAssetMenu(fileName = "New Readable", menuName = "Readables")]
public class ReadableData : ScriptableObject
{
	public Sprite UISprite;
	public string UIText;
	public float TAnchorMinX;
	public float TAnchorMaxX;
	public float TAnchorMinY;
	public float TAnchorMaxY;
}
