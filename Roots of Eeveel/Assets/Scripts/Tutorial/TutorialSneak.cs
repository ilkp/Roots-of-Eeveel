using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialSneak : ITutorial
{
	public string HintText => throw new System.NotImplementedException();

	public Sprite HintSprite { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
	public bool Completed { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

	public bool CheckActivation()
	{
		throw new System.NotImplementedException();
	}

	public bool CheckCompletion()
	{
		throw new System.NotImplementedException();
	}
}
