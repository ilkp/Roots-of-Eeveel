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

	public event PuzzleSolvedEventHandler PuzzleSolved;
	public event PuzzleUnsolvedEventHandler PuzzleUnsolved;

	private void Awake()
	{
		Solved = false;
		gameObject.tag = identifier;
	}

	public void OnPuzzleSolved()
	{
		if (PuzzleSolved != null)
		{
			PuzzleSolved(this, EventArgs.Empty);
		}
	}

	public void OnPuzzleUnsolved()
	{
		if (PuzzleUnsolved != null)
		{
			PuzzleUnsolved(this, EventArgs.Empty);
		}
	}

	public void Solve(Interactable_Key keySubscriber)
	{
		Solved = true;
		_keySubscriber = keySubscriber;
		PuzzleUnsolved += keySubscriber.OnPuzzleUnsolved;
		OnPuzzleSolved();
	}

	public void Unsolve()
	{
		Solved = false;
		OnPuzzleUnsolved();
		PuzzleUnsolved -= _keySubscriber.OnPuzzleUnsolved;
		_keySubscriber = null;
	}
}
