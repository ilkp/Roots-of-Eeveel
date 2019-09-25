using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
	[SerializeField] private KeyCode interaction;

	[SerializeField] private Rigidbody playerRB;
	[SerializeField] private Transform head;
	[SerializeField] private Image reticule;
	private GameObject interactable;
	private Camera camera;

	[SerializeField] private float sensitivity;
	[SerializeField] private float speed;
	[SerializeField] private float maxY;
	[SerializeField] private float minY;
	[SerializeField] private float grabDistance;
	private float rotationX;
	private float rotationY;


	// Start is called before the first frame update
    void Start()
	{
		// Set cursor invisible
		Cursor.visible = false;
		// Set cursor locked to the center of the game window
		Cursor.lockState = CursorLockMode.Locked;

		camera = Camera.main;
	}

    // Update is called once per frame
    void Update()
    {
		// Viewpoint rotation

		// Calculate horizontal rotation
		rotationX = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * sensitivity;
		
		// Calculate vertical rotation
		rotationY += Input.GetAxis("Mouse Y") * sensitivity;
		rotationY = Mathf.Clamp(rotationY, minY, maxY);

		// Rotate character for the horizontal rotation (camera is following the character)
		transform.localEulerAngles = new Vector3(0, rotationX, transform.rotation.eulerAngles.z);
		// Rotate only the camera in vertical rotation (so that the character model doesn't tilt)
		head.transform.localEulerAngles = (new Vector3(-rotationY, head.transform.localEulerAngles.y, 0));



		// Camera raycast

		// Gives objects a chance to reset stuff if needed
		// Checks if left mouse button is released
		if (interactable && Input.GetKeyUp(interaction))
		{
			// Tells interactable object to run funtion 'Reset'
			//interactable.SendMessage("Reset", SendMessageOptions.DontRequireReceiver);
			
			// Clear any previous hit objects
			interactable = null;
		}

		// Check if there is interactable object under the reticle
		if (Physics.Raycast(camera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0)), out RaycastHit hit) && hit.collider.CompareTag("Interactable"))
		{
			// Check if object is visible to the character and close enough
			if (Physics.Raycast(head.transform.position, hit.point - head.transform.position, out hit, grabDistance))
			{
				// Set reticule color to red
				reticule.color = Color.red;

				// Check if player pressed the interaction button
				if (Input.GetKeyDown(interaction))
				{
					// Store hit object
					interactable = hit.transform.gameObject;
					// Set reticule color to blue
					reticule.color = Color.blue;
				}
			}
			else
			{
				// Set reticule color to yellow
				reticule.color = Color.yellow;
			}
		}
		else
		{
			// Set reticule color to white
			reticule.color = Color.white;
		}


	}

	private void FixedUpdate()
	{
		playerRB.MovePosition(transform.position + (Vector3.Normalize(transform.forward * Input.GetAxisRaw("Vertical") + transform.right * Input.GetAxisRaw("Horizontal")) * speed * Time.deltaTime));
	}
}
