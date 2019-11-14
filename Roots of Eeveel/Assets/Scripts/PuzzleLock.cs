using Puzzle;
using System;
using UnityEngine;

public class PuzzleLock : MonoBehaviour, IPuzzleCondition
{
	[SerializeField] private ReadableLock readableData;
	[SerializeField] private string identifier;
	public DemoPuzzleDoor door;
	private AudioSettings audioSettings;

	public string Identifier { get { return identifier; } set { identifier = value; } }
	public bool Solved { get; set; }
	
	public Transform keyPosition;
	private Interactable_Key _keySubscriber;

	public event ConditionMetEventHandler ConditionMet;
	public event ConditionUnmetEventHandler ConditionUnmet;

	private void Awake()
	{
		Solved = false;
		audioSettings = FindObjectOfType<GameManager>().audioSettings;
	}

	private void Start()
	{
		Interactable_ReadableUI r = GetComponent<Interactable_ReadableUI>();
		r.Font = readableData.font;
		r.FontSize = readableData.fontSize;
		r.FontColor = readableData.fontColor;
		r.UISprite = readableData.UISprite;
		r.Text = readableData.UIText;
		r.TAnchorMinX = readableData.TAnchorMinX;
		r.TAnchorMaxX = readableData.TAnchorMaxX;
		r.TAnchorMinY = readableData.TAnchorMinY;
		r.TAnchorMaxY = readableData.TAnchorMaxY;
	}

	public void OnConditionMet()
	{
		if (ConditionMet != null)
		{
			ConditionMet(this, EventArgs.Empty);
		}
	}

	public void OnConditionUnmet()
	{
		if (ConditionUnmet != null)
		{
			ConditionUnmet(this, EventArgs.Empty);
			audioSettings.PlayWrongKey(gameObject);
		}
	}

	public void Solve(Interactable_Key keySubscriber)
	{
		Solved = true;
		_keySubscriber = keySubscriber;
		ConditionUnmet += keySubscriber.OnConditionUnmet;
		GetComponent<Interactable_ReadableUI>().UISprite = readableData.UISpriteSolved;
		OnConditionMet();
	}

	public void Unsolve()
	{
		Solved = false;
		OnConditionUnmet();
		ConditionUnmet -= _keySubscriber.OnConditionUnmet;
		GetComponent<Interactable_ReadableUI>().UISprite = readableData.UISprite;
		_keySubscriber = null;
	}
}
