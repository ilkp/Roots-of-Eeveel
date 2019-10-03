using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToolTip : MonoBehaviour
{
	[SerializeField] private Text toolTip;
	[SerializeField] private float popupDistance = 2;
	[SerializeField] private float dotAngle = 360;
	[SerializeField] private GameObject head;
	private List<GameObject> interactables = new List<GameObject>();
	private GameObject target;

	private void Start()
	{
		foreach (GameObject go in GameObject.FindGameObjectsWithTag("Interactable"))
		{
			if (go.GetComponent<IInteractable>() != null)
			{
				interactables.Add(go);
			}
		}
	}

	private void Update()
	{
		float minDistance;
		Vector3 headForward = head.transform.forward.normalized;
		Vector3 headPosition = head.transform.position;

		for (int i = interactables.Count - 1; i >= 0; --i)
		{
			if (interactables[i] == null || !interactables[i].CompareTag("Interactable"))
			{
				interactables.RemoveAt(i);
			}
		}

		if (target != null)
		{
			float distanceToTarget = Vector3.Distance(headPosition, target.transform.position);

			float dot = Vector3.Dot (headForward, (target.transform.position - headPosition).normalized);
			float minDot = (dotAngle > 180)? -(dotAngle - 180) : 180 - dotAngle;
			dot *= 180;

			if (distanceToTarget < popupDistance && dot > minDot)
			{
				minDistance = distanceToTarget;
			}
			else
			{
				target = null;
				minDistance = Mathf.Infinity;
			}
		}
		else
		{
			minDistance = Mathf.Infinity;
		}


		foreach (GameObject go in interactables)
		{
			if (go == null)
			{
				// Fix later, what happens when objects is deleted?
				continue;
			}
			float distance = Vector3.Distance(headPosition, go.transform.position);
			float dot = Vector3.Dot (headForward, (go.transform.position - headPosition).normalized);
			float minDot = (dotAngle > 180)? -(dotAngle - 180) : 180 - dotAngle;
			dot *= 180;


			if (distance < popupDistance && distance < minDistance && dot > minDot)
			{
				minDistance = distance;

				target = go;
			}
		}



		if (target != null)
		{
			showPopup(true, target.GetComponent<IInteractable>().ToolTip);
		}
		else
		{
			showPopup(false);
		}
	}

	private void showPopup(bool isVisible, string toolTipText = null)
	{
		if (isVisible)
		{
			toolTip.text = toolTipText;
		}
		else
		{
			toolTip.text = null;
		}
	}

}
