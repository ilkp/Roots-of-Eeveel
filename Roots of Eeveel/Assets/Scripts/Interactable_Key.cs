using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Collider), typeof(Rigidbody))]
public class Interactable_Key : MonoBehaviour, IInteractable
{
	enum State
	{
		free, inLock
	}

	private State state = State.free;
	private Rigidbody rb;
	private Transform head;
	private PlayerMovement pm;
	private Vector3 destination;

	[SerializeField] private string lockIdentifier;
	[SerializeField] private float throwForce = 10;
	[SerializeField] private float pullForce = 10;
	[SerializeField] private float minHoldDistance = 1.5f;
	[SerializeField] private float maxHoldDistance = 4;
	[SerializeField] private float rotationSpeed = 1;
	[SerializeField] private string toolTip = "Seems important...\nHold LeftMouseButton to keep in hand.\nHold RightMouseButton to inspect closer.\nMouseWheel to get object closer/farther";
	public string ToolTip
	{
		get
		{
			return toolTip;
		} set
		{
			toolTip = value;
		}
	}

	public event Action<IInteractable> OnInteract;

	private void Awake()
	{
		gameObject.tag = "Interactable";
		rb = GetComponent<Rigidbody>();
		pm = FindObjectOfType<PlayerMovement>();
		head = pm.transform.GetChild(0).transform;
	}

	public void Interact()
	{
		if (state == State.free)
		{
			rb.useGravity = false;

			StopAllCoroutines();
			StartCoroutine("Hold");
		}
	}

	public void StopInteraction()
	{
		if (state == State.free)
		{
			rb.useGravity = true;
			pm.allowRotation = true;
			rb.freezeRotation = false;

			StopCoroutine("Hold");
		}
	}

	IEnumerator Hold()
	{
		float currentHoldDistance = Vector3.Distance(head.transform.position, transform.position);

		while (true)
		{
			float zoom = Input.GetAxis("Mouse ScrollWheel");
			currentHoldDistance = Mathf.Clamp(currentHoldDistance + zoom, 1.5f, 4);

			if (Input.GetKey(KeyCode.Mouse1))
			{
				pm.allowRotation = false;
				rb.freezeRotation = true;

				Vector2 rotation = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxis("Mouse Y"));

				transform.Rotate(rotation * rotationSpeed);

			}
			else
			{
				pm.allowRotation = true;
				rb.freezeRotation = false;
			}

			Ray ray = new Ray(transform.position, head.position - transform.position);
			RaycastHit[] hits = Physics.RaycastAll(ray, Vector3.Distance(transform.position, head.position));

			for (int i = 0; i < hits.Length; i++)
			{
				if (hits[i].transform != null)
				{
					if (hits[i].transform != transform)
					{
						break;
					}
				}
				else
				{
					break;
				}
			}
			destination = head.position + head.forward * currentHoldDistance;

			rb.velocity = ((destination - transform.position) * pullForce);
			rb.angularVelocity *= 0.9f * Time.fixedDeltaTime;

			yield return null;
		}
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.tag == "Lock" && collision.gameObject.GetComponent<PuzzleLock>().Identifier == lockIdentifier)
		{
			PuzzleLock targetLock = collision.gameObject.GetComponent<PuzzleLock>();
			if (targetLock.Solved)
			{
				return;
			}

			StartCoroutine(LerpToPlace(targetLock.keyPosition.position, targetLock.keyPosition.rotation.eulerAngles, targetLock));
			pm.allowRotation = true;
			gameObject.tag = "Untagged";
			StopCoroutine("Hold");
		}
	}

	public void OnConditionUnmet(object sender, System.EventArgs args)
	{
		rb.isKinematic = false;
		rb.useGravity = true;
		transform.Translate(new Vector3(0, 0, -1.0f));
		rb.AddForce(-Vector3.forward * Random.Range(1, 5), ForceMode.Impulse);
		rb.AddTorque(new Vector3(Random.Range(0, 2), Random.Range(0, 2), Random.Range(0, 2)), ForceMode.Impulse);
		StartCoroutine(reTag());
	}

	private IEnumerator reTag()
	{
		yield return new WaitForSeconds(2);
		gameObject.tag = "Interactable";
	}

	IEnumerator LerpToPlace(Vector3 targetPos, Vector3 targetRot, PuzzleLock targetLock)
	{
		rb.isKinematic = true;
		float time = 0;
		Vector3 startPos = transform.position;
		Vector3 startRot = transform.eulerAngles;
		startRot = new Vector3((startRot.x > 180) ? startRot.x - 360 : startRot.x, (startRot.y > 180) ? startRot.y - 360 : startRot.y, (startRot.z > 180) ? startRot.z - 360 : startRot.z);

		while (time != 1)
		{
			time = Mathf.Clamp(time + Time.deltaTime, 0, 1);

			transform.position = Vector3.Lerp(startPos, targetPos, time);
			transform.eulerAngles = Vector3.Lerp(startRot, targetRot, time);

			yield return null;
		}
		targetLock.Solve(this);
		targetLock.door.checkLocks();
	}
}
