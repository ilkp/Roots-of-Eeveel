using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Puzzle_Door : MonoBehaviour, IInteractable
{
	/// <summary>
	/// Yes, doors may be used as triggers...
	/// </summary>
	public event Action<IInteractable> OnInteract;

	[Tooltip("All the objects that must be activated before the door may be interacted with.\nObjects Must contain script that inherits 'IInteractable'")]
	[SerializeField] private List<GameObject> triggers;

	/// <summary>
	/// Dictionary that hold all the triggers and their states
	/// </summary>
	private Dictionary<IInteractable, bool> interactables = new Dictionary<IInteractable, bool>();

	/// <summary>
	/// Is the door open or not :D
	/// </summary>
	private bool isOpen = false;

	/// <summary>
	/// Angle that the door closes to
	/// </summary>
	[Tooltip("Angle that the door closes to")]
	[ContextMenuItem("Current Angle", "SetClosedAngle")]
	[SerializeField] private float closedAngle = 0;

	private void SetClosedAngle()
	{
		closedAngle = transform.localEulerAngles.y;
		if (closedAngle > 180f)
			closedAngle -= 360f;
	}

	/// <summary>
	/// Angle that the door opens to when puzzle is solved
	/// </summary>
	[Tooltip("Angle that the door opens to when puzzle is solved")]
	[ContextMenuItem("Current Angle", "SetSolvedAngle")]
	[SerializeField] private float solvedAngle = 10;

	private void SetSolvedAngle()
	{
		solvedAngle = transform.localEulerAngles.y;
		if (solvedAngle > 180f)
			solvedAngle -= 360f;
	}

	/// <summary>
	/// Angle that the door opens to
	/// </summary>
	[Tooltip("Angle that the door opens to")]
	[ContextMenuItem("Current Angle", "SetOpenAngle")]
	[SerializeField] private float openAngle = 90;

	private void SetOpenAngle()
	{
		openAngle = transform.localEulerAngles.y;
		if (openAngle > 180f)
			openAngle -= 360f;
	}

	/// <summary>
	/// How fast the door should open
	/// </summary>
	[Tooltip("doors opening speed in degrees per second")]
	[SerializeField] private float openingSpeed = 90;

	private bool isSolved = false;

	private void Start()
	{
		// Makes sure that the tag is correct
		gameObject.tag = "Interactable";

		// List all triggers to dictionary, and makes sure they are valid
		foreach (GameObject trigger in triggers)
		{
			IInteractable interactable = trigger.GetComponent<IInteractable>();

			if (interactable != null)
			{
				interactables.Add(interactable, false);
				interactable.OnInteract += this.Triggered;
			}
			else
			{
				throw new System.Exception("Trigger missing 'IInteractable', like Interactable_Button-script");
			}
		}
	}

	/// <summary>
	/// Should be called by triggered event
	/// </summary>
	/// <param name="interactable"></param>
	private void Triggered(IInteractable interactable)
	{
		// Flip triggering interactables state
		interactables[interactable] = !interactables[interactable];
		// Check if we are open
		if (!isSolved && CheckAllTriggers())
		{
			isSolved = true;
			isOpen = false;
			StopAllCoroutines();
			StartCoroutine("LerpRotation", solvedAngle);
		}
		else if(isSolved && !CheckAllTriggers())
		{
			isSolved = false;
			isOpen = true;
			StopAllCoroutines();
			StartCoroutine("LerpRotation", closedAngle);
		}
	}

	/// <summary>
	/// Checks if all triggers are open
	/// </summary>
	/// <returns></returns>
	private bool CheckAllTriggers()
	{
		foreach (var pair in interactables)
		{
			if (!pair.Value)
			{
				return false;
			}
		}

		return true;
	}

	/// <summary>
	/// Is called when player tries to interact with the door
	/// </summary>
	public void Interact()
	{
		//OnInteract(this);
		//Debug.Log("Door interacted with");

		// Allow interaction only if all triggers are open
		if (CheckAllTriggers())
		{
			isOpen = !isOpen;
			// Stop previous coroutines anad run new
			StopAllCoroutines();
			StartCoroutine("LerpRotation", isOpen ? openAngle : closedAngle);
		}

	}

	public void StopInteraction()
	{
		// it's kind of lonely in here :/
	}

	/// <summary>
	/// Coroutine that turns the doors parent, for pivot correction
	/// </summary>
	/// <param name="targetAngle"></param>
	/// <returns></returns>
	IEnumerator LerpRotation(float targetAngle)
	{

		float startAngle = transform.localEulerAngles.y;

		if (startAngle > 180)
			startAngle -= 360;

		float angleDifference = targetAngle - startAngle;

		int increase = (angleDifference < 0)? 1 : -1;

		Debug.Log("Angle Difference: " + angleDifference + ", target angle: " + targetAngle + ", start angle: " + startAngle);

		while (angleDifference != 0)
		{
			angleDifference += openingSpeed * Time.deltaTime * increase;

			if ((increase > 0 && angleDifference > 0) || (increase < 0 && angleDifference < 0))
			{
				angleDifference = 0;
			}

			transform.localEulerAngles = new Vector3(transform.eulerAngles.x, targetAngle - angleDifference, transform.eulerAngles.z);

			yield return null;
		}

		// Make the rotation presice, for a good measure and OCD :)
		transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, targetAngle, transform.localEulerAngles.z);
	}

}
