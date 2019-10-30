
using System;

namespace Puzzle
{
	public delegate void PuzzleSolvedEventHandler(object source, EventArgs args);
	public delegate void PuzzleUnsolvedEventHandler(object source, EventArgs args);

	public interface IPuzzleCondition
	{
		string Identifier { get; set; }
		bool Solved { get; set; }
		event PuzzleSolvedEventHandler PuzzleSolved;
		event PuzzleUnsolvedEventHandler PuzzleUnsolved;
		void OnPuzzleSolved();
		void OnPuzzleUnsolved();
		void Solve(Interactable_Key keySubscriber);
		void Unsolve();
	}
}