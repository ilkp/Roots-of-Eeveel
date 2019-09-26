using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
	/// <summary>
	/// The key to be used for interaction
	/// </summary>
	[Tooltip("The key to be used for interaction")]
	[SerializeField] private KeyCode interaction;

	/// <summary>
	/// Parent object of the camera
	/// </summary>
	[Tooltip("Parent object of the camera")]
	[SerializeField] private Transform head;

	/// <summary>
	/// Image to be drawn as reticle
	/// </summary>
	[Tooltip("Image to be drawn as reticle")]
	[SerializeField] private Image reticule;

	/// <summary>
	/// Color of the reticle when not on interactable object
	/// </summary>
	[Tooltip("Color of the reticle when not on interactable object")]
	[SerializeField] private Color colorReticleInactive = Color.white;

	/// <summary>
	/// Color of the reticle when on interactable object
	/// </summary>
	[Tooltip("Color of the reticle when on interactable object")]
	[SerializeField] private Color colorReticleActive = Color.red;

	private Rigidbody playerRB;
	/// <summary>
	/// The interactable object that the player is currently interacting with
	/// </summary>
	private GameObject interactable;
	/// <summary>
	/// The main camera
	/// </summary>
	private Camera cam;

	/// <summary>
	/// Float that determins how sensitive the camera movement is
	/// </summary>
	[Tooltip("Float that determins how sensitive the camera movement is")]
	[SerializeField] private float sensitivity;
	/// <summary>
	/// The movement speed of the player
	/// </summary>
	[Tooltip("The movement speed of the player")]
	[SerializeField] private float speed;
	/// <summary>
	/// Maximum angle that player may look up
	/// </summary>
	[Tooltip("Maximum angle that player may look up")]
	[SerializeField] private float maxY;
	/// <summary>
	/// Minimum angle that player may look up
	/// </summary>
	[Tooltip("Minimum angle that player may look up")]
	[SerializeField] private float minY;
	/// <summary>
	/// How far the player is able to interact with objects
	/// </summary>
	[Tooltip("How far the player is able to interact with objects")]
	[SerializeField] private float grabDistance;
    /// <summary>
    /// Player sneak speed modifier
    /// </summary>
    [Tooltip("Player sneak speed modifier")]
    [SerializeField] private float sneakSpeedModifier;
    /// <summary>
    /// Player run speed modifier
    /// </summary>
    [Tooltip("Player run speed modifier")]
    [SerializeField] private float runSpeedModifier;

    private float rotationX;
	private float rotationY;
    private bool sneaking;
    private bool running;


	// Start is called before the first frame update
	void Start()
	{
		// Get players rigidbody
		playerRB = GetComponent<Rigidbody>();
		// Set cursor invisible
		Cursor.visible = false;
		// Set cursor locked to the center of the game window
		Cursor.lockState = CursorLockMode.Locked;

		cam = Camera.main;
	}

	// Update is called once per frame
	void Update()
	{
		#region Movement
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

        // Check sneaking condition
        sneaking = Input.GetKey(KeyCode.LeftControl);
        // Check sneaking condition
        running = Input.GetKey(KeyCode.LeftShift) && !sneaking;
        #endregion

        #region Interaction
        // Camera raycast

        // Gives objects a chance to reset stuff if needed
        // Checks if left mouse button is released
        if (interactable && Input.GetKeyUp(interaction))
		{
			// Tells interactable object to run funtion 'Reset'
			interactable.SendMessage("StopInteraction", SendMessageOptions.DontRequireReceiver);

			// Clear any previous hit objects
			interactable = null;
		}

		// Check if object is visible to the character and close enough
		if (Physics.Raycast(head.transform.position, head.transform.forward, out RaycastHit hit, grabDistance) && hit.collider.CompareTag("Interactable"))
		{
			// Set reticule color to red
			reticule.color = colorReticleActive;

			// Check if player pressed the interaction button
			if (Input.GetKeyDown(interaction))
			{
				// Store hit object
				interactable = hit.transform.gameObject;
				// Call 'Interact' on the target
				interactable.SendMessage("Interact", SendMessageOptions.DontRequireReceiver);
			}
		}
		else
		{
			// Set reticule color to white
			reticule.color = colorReticleInactive;
		}
		#endregion

	}

	private void FixedUpdate()
	{
        // Move player accrding to the inputs
        float speedModifier = speed * (sneaking ? sneakSpeedModifier : 1.0f) * (running ? runSpeedModifier : 1.0f)* Time.deltaTime;

        playerRB.MovePosition(transform.position + (Vector3.Normalize(transform.forward * Input.GetAxisRaw("Vertical") + transform.right * Input.GetAxisRaw("Horizontal")) * speedModifier));
	}

    public void Die()
    {
        GameManager.Instance.SetGameOver();
    }
}
