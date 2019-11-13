

using UnityEngine;

public interface ITutorial
{
	string HintText { get; }
	Sprite HintSprite { get; set; }
	bool Completed { get; set; }
	bool CheckCompletion();
	bool CheckActivation();
}