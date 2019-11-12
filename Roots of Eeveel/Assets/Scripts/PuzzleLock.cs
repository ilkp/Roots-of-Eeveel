using Puzzle;
using System;
using UnityEngine;

public class PuzzleLock : MonoBehaviour, IPuzzleCondition
{
	[SerializeField] private ReadableData unsolvedData;
	[SerializeField] private ReadableData solvedData;
	[SerializeField] private string identifier;
	public DemoPuzzleDoor door;

	public string Identifier { get { return identifier; } set { identifier = value; } }
	public bool Solved { get; set; }
	
	public Transform keyPosition;
	private Interactable_Key _keySubscriber;

	public event ConditionMetEventHandler ConditionMet;
	public event ConditionUnmetEventHandler ConditionUnmet;

	private void Awake()
	{
		Solved = false;
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
		}
	}

	public void Solve(Interactable_Key keySubscriber)
	{
		Solved = true;
		_keySubscriber = keySubscriber;
		ConditionUnmet += keySubscriber.OnConditionUnmet;
		GetComponent<Interactable_ReadableUI>().data = solvedData;
		OnConditionMet();
	}

	public void Unsolve()
	{
		Solved = false;
		OnConditionUnmet();
		ConditionUnmet -= _keySubscriber.OnConditionUnmet;
		GetComponent<Interactable_ReadableUI>().data = unsolvedData;
		_keySubscriber = null;
	}
}
