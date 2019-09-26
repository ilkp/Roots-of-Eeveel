using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Button sends OnInteract event every time it is pressed
/// </summary>
[RequireComponent(typeof(Collider))]
public class Interactable_Button : MonoBehaviour, IInteractable
{
	/// <summary>
	/// Delivers the activation info to subscribed classees
	/// </summary>
	public event Action<IInteractable> OnInteract;

	/// <summary>
	/// Whether the button should act like a button or switch/lever
	/// </summary>
	[Tooltip("Where the button should act like a button or switch/lever")]
	[SerializeField] private bool isToggle = false;

	/// <summary>
	/// Whether the button is currently active or not
	/// </summary>
	private bool isActive = false;

	private void Awake()
	{
		// Makes sure that the object is tagged as 'interactable', so that the player may interact with it correctly
		gameObject.tag = "Interactable";
	}

	public void Interact()
	{
		if (isToggle)
		{
			// Send activation event
			OnInteract(this);

			// Toggle material color to match isActive state
			if (isActive)
			{
				gameObject.GetComponent<Renderer>().material.color = Color.white;
			}
			else
			{
				gameObject.GetComponent<Renderer>().material.color = Color.red;
			}

			// Toggle isActive
			isActive = !isActive;
		}
		else
		{
			// Runs only ones
			if (!isActive)
			{
				isActive = true;
				OnInteract(this);
				gameObject.GetComponent<Renderer>().material.color = Color.red;
			}
		}
	}

	public void StopInteraction()
	{
		// Return to inactivecolor when the button is released, only if in toggle mode
		if (!isToggle)
		{
			gameObject.GetComponent<Renderer>().material.color = Color.white;
		}
	}
}
