
using System;

namespace Puzzle
{
	public delegate void ConditionMetEventHandler(object source, EventArgs args);
	public delegate void ConditionUnmetEventHandler(object source, EventArgs args);

	public interface IPuzzleCondition
	{
		string Identifier { get; set; }
		bool Solved { get; set; }
		event ConditionMetEventHandler ConditionMet;
		event ConditionUnmetEventHandler ConditionUnmet;
		void OnConditionMet();
		void OnConditionUnmet();
		void Solve(Interactable_Key keySubscriber);
		void Unsolve();
	}
}