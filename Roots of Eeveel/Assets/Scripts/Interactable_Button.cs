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
	[Tooltip("Where the button should act like a button(False) or switch/lever(True)")]
	[SerializeField] private bool isToggle = false;

	/// <summary>
	/// Whether the button is currently active or not
	/// </summary>
	private bool isActive = false;
	private Animator animator;

	private void Awake()
	{
		// Makes sure that the object is tagged as 'interactable', so that the player may interact with it correctly
		gameObject.tag = "Interactable";

		animator = GetComponent<Animator>();
		if (animator == null)
		{
			animator = GetComponentInChildren<Animator>();
		}

		animator.SetBool("IsPressed", isActive);
		animator.SetBool("IsToggle", isToggle);
	}

	public void Interact()
	{
		if (isToggle)
		{
			// Toggle isActive
			isActive = !isActive;
			// Toggle animator state to match isActive state
			animator.SetBool("IsPressed", isActive);

			StartCoroutine(DelayedOnInteract(0.25f));

		}
		else
		{
			// Runs only ones
			if (!isActive)
			{
				isActive = true;
				// Toggle animator state to match isActive state
				animator.SetBool("IsPressed", isActive);

				StartCoroutine(DelayedOnInteract(0.25f));

			}
		}
	}

	private IEnumerator DelayedOnInteract(float delay)
	{
		yield return new WaitForSeconds(delay);

		OnInteract(this);
	}

	public void StopInteraction()
	{

	}
}
