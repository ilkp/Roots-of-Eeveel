using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Interactable_HoldableObject : MonoBehaviour, IInteractable
{
	[SerializeField] private string toolTip = "Hold down leftMouseButton to hold the object in hand.\nRightMouseButton to throw the object.";
	public string ToolTip
	{
		get
		{
			return toolTip;
		}
		set
		{
			toolTip = value;
		}
	}

	public event Action<IInteractable> OnInteract;

	private GameObject player;
	private Transform head;
	private Rigidbody rb;
	[SerializeField] private float pullForce = 10;
	[SerializeField] private float throwForce = 10;
	//private Vector3 velocity;
	//private Vector3 lastFramePosition;
	private Vector3 destination;

	private void Awake()
	{
		player = FindObjectOfType<PlayerMovement>().gameObject;
		head = player.transform.GetChild(0).transform;
		rb = GetComponent<Rigidbody>();
		gameObject.tag = "Interactable";
	}

	public void Interact()
	{
		rb.useGravity = false;
		//lastFramePosition = transform.position;
		StartCoroutine(Hold());
	}

	public void StopInteraction()
	{
		rb.useGravity = true;
		StopAllCoroutines();
	}

	IEnumerator Hold()
	{
		float holdDistance = Vector3.Distance(head.transform.position, transform.position);
		Vector3 offSet = transform.position - (head.position + head.forward * holdDistance);


		while (true)
		{
			float zoom = Input.GetAxis("Mouse ScrollWheel");
			holdDistance = Mathf.Clamp(holdDistance + zoom, 1.5f, 4);

			if (Input.GetKeyDown(KeyCode.Mouse1))
			{
				rb.AddForce(head.forward * throwForce, ForceMode.Impulse);
				rb.useGravity = true;

				break;
			}
			
			//velocity = (transform.position - lastFramePosition) / (Time.deltaTime * 2);
			//lastFramePosition = transform.position;

			Ray ray = new Ray(transform.position, head.position - transform.position);
			RaycastHit[] hits = Physics.RaycastAll(ray, Vector3.Distance(transform.position, head.position));

			for (int i = 0; i < hits.Length; i++)
			{
				if (hits[i].transform != null)
				{
					if (hits[i].transform != transform)
					{
						//destination = hits[i].point;
						break;
					}
				}
				else
				{
					//destination = player.transform.position + player.transform.forward * 10;
					break;
				}
			}
			destination = head.position + offSet + head.forward * holdDistance;

			rb.velocity = ((destination - transform.position) * pullForce);
			rb.angularVelocity *= 0.9f * Time.fixedDeltaTime;

			yield return null;
		}
	}
}
