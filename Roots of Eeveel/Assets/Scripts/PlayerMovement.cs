
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private AudioSettings audioSettings;

    /// <summary>
    /// The key to be used for interaction
    /// </summary>
    [Tooltip("The key to be used for interaction")]
    [SerializeField] private KeyCode interaction;

    /// <summary>
    /// The key to be used for secondary interaction
    /// </summary>
    [Tooltip("The key to be used for secondary interaction")]
    [SerializeField] private KeyCode secondaryInteraction;

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
    [HideInInspector]
    public GameObject interactable;
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
    /// <summary>
    /// Player speed modifier when holding an object
    /// </summary>
    [Tooltip("Player speed modifier when holding an object")]
    [SerializeField] private float holdSpeedModifier;
    private bool holdingItem;

    /// <summary>
    /// UI container for health image
    /// </summary>
    [Tooltip("UI container for health image")]
    [SerializeField] private Image hpIndicator;

    /// <summary>
    /// HP Indicator images
    /// </summary>
    [Tooltip("Container for HP Indicator images")]
    [SerializeField] private Sprite[] hpIndicators;

    /// <summary>
    /// HP regeneration time
    /// </summary>
    [Tooltip("HP regeneration time")]
    [SerializeField] private float hpRegen;
    private float regenCounter;

    private GameObject[] highlightables;
    [SerializeField] private float viewlength = 3f;

    private Collider lastHit = null; // for turning off the highlight from GameObject that is no longer in focus

	private Vector3 cameraSneakDisplacement = new Vector3(0, -0.2f, 0);

    enum HealthState
    {
        Healthy,
        Low,
        Lower,
        Lowest,
    }

    /// <summary>
    /// Heath stage
    /// </summary>
    [Tooltip("Health State")]
    [SerializeField] private HealthState hp;



    private float rotationX;
    private float rotationY;
    public bool allowRotation = true;
    private bool sneaking;
    private bool running;
    private float footstepSoundTimer = 0.0f;
    private const float footStepMaxTime = 0.5f;

	public float getGrabDistance()
	{
		return grabDistance;
	}

    // Start is called before the first frame update
    void Start()
    {
        holdingItem = false;
        // Get players rigidbody
        playerRB = GetComponent<Rigidbody>();
        // Set cursor invisible
        Cursor.visible = false;
        // Set cursor locked to the center of the game window
        Cursor.lockState = CursorLockMode.Locked;

        cam = Camera.main;
        allowRotation = true;

        hp = HealthState.Healthy;
        hpIndicator.sprite = hpIndicators[(int)hp];
        highlightables = GameObject.FindGameObjectsWithTag("Interactable");
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.Paused)
        {
            return;
        }

        #region Movement
        // Viewpoint rotation

        if (allowRotation)
        {
            // Calculate horizontal rotation
            rotationX = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * sensitivity;

            // Calculate vertical rotation
            rotationY += Input.GetAxis("Mouse Y") * sensitivity;
            rotationY = Mathf.Clamp(rotationY, minY, maxY);

            // Rotate character for the horizontal rotation (camera is following the character)
            transform.localEulerAngles = new Vector3(0, rotationX, transform.rotation.eulerAngles.z);
            // Rotate only the camera in vertical rotation (so that the character model doesn't tilt)
            head.transform.localEulerAngles = (new Vector3(-rotationY, head.transform.localEulerAngles.y, 0));
        }

        // Check sneaking condition
        sneaking = Input.GetKey(KeyCode.LeftControl);
        // Check sneaking condition
        running = Input.GetKey(KeyCode.LeftShift) && !sneaking;

		if (sneaking)
		{
			cam.transform.position = Vector3.Lerp(cam.transform.position, head.transform.position + cameraSneakDisplacement, 0.3f);
		}
		else
		{
			cam.transform.position = Vector3.Lerp(cam.transform.position, head.transform.position, 0.3f);
		}

        if (footstepSoundTimer > footStepMaxTime)
        {
            audioSettings.PlayPlayerFootStep(running, sneaking);

            footstepSoundTimer = 0.0f;
            SoundManager.makeSound(gameObject.transform.position, sneaking ? 75.0f : 175.0f, true);
        }
        #endregion

        #region Interaction

        // Gives objects a chance to reset stuff if needed
        // Checks if left mouse button is released
        if (interactable && Input.GetKeyUp(interaction))
        {
            // Tells interactable object to run funtion 'Reset'
            interactable.SendMessage("StopInteraction", SendMessageOptions.DontRequireReceiver);

            // Clear any previous hit objects
            //interactable = null;
        }

        // Secondary interact for secondary needs
        if (interactable && Input.GetKeyDown(secondaryInteraction))
        {
            interactable.SendMessage("SecondInteract", SendMessageOptions.DontRequireReceiver);
        }

        // Check if object is visible to the character and close enough
        if (Physics.Raycast(head.transform.position, head.transform.forward, out RaycastHit hit, grabDistance) && hit.collider.CompareTag("Interactable"))
        {
            // Highlight the object
            if (lastHit != null && lastHit != hit.collider)
            {
                lastHit.GetComponent<Renderer>().material.SetFloat("_OnOff", 0f);
            }
            hit.collider.GetComponent<Renderer>().material.SetFloat("_OnOff", 1f);
            lastHit = hit.collider;

            GetComponent<ToolTip>().showPopup(true);

            // Check if player pressed the interaction button
            if (Input.GetKeyDown(interaction))
            {
                // Store hit object
                interactable = hit.transform.gameObject;
                // Call 'Interact' on the target
                interactable.SendMessage("Interact", SendMessageOptions.DontRequireReceiver);
                holdingItem = true;
            }
        }
        else
        {
            // Remove highlight from last object
            if (lastHit)
            {
                lastHit.GetComponent<Renderer>().material.SetFloat("_OnOff", 0f);
                lastHit = null;
            }

            // Reset whatever is being done
            if (interactable && !Input.GetKey(interaction))
            {
                interactable.SendMessage("Reset", SendMessageOptions.DontRequireReceiver);
                holdingItem = false;
                interactable = null;
            }
            // Disable reticule
            GetComponent<ToolTip>().showPopup(false);
        }
        #endregion

    }

    private void FixedUpdate()
    {
        // Move player accrding to the inputs
        float speedModifier = (sneaking ? sneakSpeedModifier : 1.0f) * (running ? runSpeedModifier : 1.0f) * (holdingItem ? holdSpeedModifier : 1.0f) * Time.deltaTime;
        Vector3 direction = transform.forward * Input.GetAxisRaw("Vertical") + transform.right * Input.GetAxisRaw("Horizontal");
        playerRB.MovePosition(transform.position + (Vector3.Normalize(direction) * speed * speedModifier));
        if (direction.magnitude > 0)
        {
            footstepSoundTimer += speedModifier;
        }
        if (hp > HealthState.Healthy)
        {
            if (regenCounter <= 0)
            {
                hp--;
                hpIndicator.sprite = hpIndicators[(int)hp];
				UpdateHealthAudio();
				if (hp > HealthState.Healthy)
                {
                    regenCounter = hpRegen;
				}
            }
            else
            {
                regenCounter -= Time.fixedDeltaTime;
            }
        }
    }

    public void Die()
    {
        GameManager.Instance.SetGameOver(false);
    }

    public void GetHurt()
    {
        if (hp < HealthState.Lowest)
        {
            hp++;
            hpIndicator.sprite = hpIndicators[(int)hp];
            regenCounter = hpRegen;
			//sound things
			if (Random.value < 0.5f)
			{		
				audioSettings.PlayPlayerDamageLow();
			}
			else
			{
				audioSettings.PlayPlayerDamageHigh();
			}

			UpdateHealthAudio();

		}
        else
        {
            Die();
        }
    }

	private void UpdateHealthAudio()
	{
		audioSettings.PlayPlayerHPRoots();

		switch (hp)
		{
			case HealthState.Healthy:
				audioSettings.StopPlayerHPHeartbeat();
				break;
			case HealthState.Low:
				audioSettings.PlayPlayerHPHeartbeat(75f);
				break;
			case HealthState.Lower:
				audioSettings.PlayPlayerHPHeartbeat(50f);
				break;
			case HealthState.Lowest:
				audioSettings.PlayPlayerHPHeartbeat(25f);
				break;
			default:
				Debug.Log("UpdateHealtAudio - HP out of bounds!");
				break;
		}
	}

    /*
    public void playFootstep()
    {
        footStepSource.Play();
    }*/
}
