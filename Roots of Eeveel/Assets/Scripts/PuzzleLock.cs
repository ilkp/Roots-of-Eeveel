using Puzzle;
using System;
using UnityEngine;

public class PuzzleLock : MonoBehaviour, IPuzzleCondition
{
	[SerializeField] private Sprite lockEmpty;
	[SerializeField] private Sprite lockSolved;
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
		audioSettings.IncreaseRisingTensionProgress();
		_keySubscriber = keySubscriber;
		ConditionUnmet += keySubscriber.OnConditionUnmet;
		GetComponent<Interactable_ReadableUI>().readableData.UISprite = lockSolved;
		OnConditionMet();
	}

	public void Unsolve()
	{
		Solved = false;
		audioSettings.DecreaseRisingTensionProgress();
		OnConditionUnmet();
		ConditionUnmet -= _keySubscriber.OnConditionUnmet;
		GetComponent<Interactable_ReadableUI>().readableData.UISprite = lockEmpty;
		_keySubscriber = null;
	}
}
