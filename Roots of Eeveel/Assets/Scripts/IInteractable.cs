using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Interface for defining common aspects of all interactable objects
/// </summary>
public interface IInteractable
{
	/// <summary>
	/// ToolTip is shown on the screen when player is near the object and Should describe how the objects is used.
	/// </summary>
	string ToolTip
	{
		get; set;
	}
	
	/// <summary>
	/// Takes care of delivering interaction info to other classees
	/// </summary>
	event Action<IInteractable> OnInteract;
	
	/// <summary>
	/// Is run when player tries to interact with the object
	/// </summary>
	void Interact();

	/// <summary>
	/// Is run when player lets go of the object
	/// </summary>
	void StopInteraction();
}
