using Puzzle;
using System;
using UnityEngine;

public class PuzzleLock : MonoBehaviour, IPuzzleCondition
{
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
		gameObject.tag = identifier;
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
		Debug.Log("lock solved");
		Solved = true;
		_keySubscriber = keySubscriber;
		ConditionUnmet += keySubscriber.OnConditionUnmet;
		OnConditionMet();
	}

	public void Unsolve()
	{
		Debug.Log("Lock unsolved");
		Solved = false;
		OnConditionUnmet();
		ConditionUnmet -= _keySubscriber.OnConditionUnmet;
		_keySubscriber = null;
	}
}
