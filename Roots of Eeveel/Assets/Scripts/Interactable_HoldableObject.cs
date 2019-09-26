using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Interactable_HoldableObject : MonoBehaviour, IInteractable
{
	public event Action<IInteractable> OnInteract;

	private GameObject player;
	private Transform head;
	private Rigidbody rb;
	private float pullForce = 10;
	//private Vector3 velocity;
	//private Vector3 lastFramePosition;
	private Vector3 destination;

	private void Start()
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
		StartCoroutine("Hold");
	}

	public void StopInteraction()
	{
		rb.useGravity = true;
		StopAllCoroutines();
	}

	IEnumerator Hold()
	{
		while (true)
		{
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
			destination = head.position + head.forward * 2;

			rb.velocity = ((destination - transform.position) * pullForce);
			rb.angularVelocity *= 0.9f * Time.fixedDeltaTime;

			yield return null;
		}
	}
}
