
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

    [SerializeField] private Image reticuleImage;
    /// <summary>
    /// Image to be drawn as reticle
    /// </summary>
    [Tooltip("Image to be drawn as reticle")]
    [SerializeField] private Sprite reticuleDefault;
    [SerializeField] private Sprite reticuleRead;
    [SerializeField] private Sprite reticuleHold;

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
    public float sensitivity;
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

    private ToolTip toolTip;
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
    public bool allowMovement = true;
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
        toolTip = GetComponent<ToolTip>();
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



        #region Interaction

        if (interactable)
        {
            // Gives objects a chance to reset stuff if needed
            // Checks if left mouse button is released
            if (Input.GetKeyUp(Keybindings.Instance.interaction) || Input.GetKeyUp(Keybindings.Instance.altInteraction))
            {
                // Tells interactable object to run funtion 'Reset'
                interactable.SendMessage("StopInteraction", SendMessageOptions.DontRequireReceiver);
            }
            // Secondary interact for secondary needs
            if (Input.GetKeyDown(Keybindings.Instance.secondaryInteraction) || Input.GetKeyDown(Keybindings.Instance.altSecondaryInteraction))
            {
                interactable.SendMessage("SecondInteract", SendMessageOptions.DontRequireReceiver);
                if (interactable.GetComponent<Interactable_HoldableObject>() != null)
                {
                    interactable = null;
                    holdingItem = false;
                }
            }
        }

        // Check if object is visible to the character and close enough
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out RaycastHit hit, grabDistance) && hit.collider.CompareTag("Interactable"))
        {
            // Highlight the object
            if (lastHit != null && lastHit != hit.collider)
            {
                lastHit.GetComponent<Renderer>().material.SetFloat("_OnOff", 0f);
            }
            hit.collider.GetComponent<Renderer>().material.SetFloat("_OnOff", 1f);
            lastHit = hit.collider;

            // Change reticule
            if (hit.collider.GetComponent<Interactable_ReadableUI>() != null ||
                hit.collider.GetComponent<InteractableReadableMulti>() != null)
            {
                toolTip.showPopup(true, null, toolTip._leftClick);
                reticuleImage.sprite = reticuleRead;
                reticuleImage.rectTransform.sizeDelta = new Vector2(reticuleRead.rect.width, reticuleRead.rect.height);
            }
            else if (hit.collider.GetComponent<Interactable_HoldableObject>() != null ||
                    hit.collider.GetComponent<Interactable_Key>() != null ||
                    hit.collider.GetComponent<Interactable_Door>() != null ||
                    hit.collider.GetComponent<Interactable_Door2>() != null)
            {
                toolTip.showPopup(true, "Hold", toolTip._leftClick);
                reticuleImage.sprite = reticuleHold;
                reticuleImage.rectTransform.sizeDelta = new Vector2(reticuleRead.rect.width, reticuleRead.rect.height);
            }

            if (Input.GetKey(Keybindings.Instance.interaction) || Input.GetKey(Keybindings.Instance.altInteraction))
            {
                reticuleImage.enabled = false;
            }
            else
            {
                reticuleImage.enabled = true;
            }
            if (interactable && interactable.GetComponent<Interactable_HoldableObject>() != null)
            {
                toolTip.showPopup(true, "Throw", toolTip._rightClick);
            }

            // Check if player pressed the interaction button
            if (Input.GetKeyDown(Keybindings.Instance.interaction) || Input.GetKeyDown(Keybindings.Instance.altInteraction))
            {
                // Store hit object
                interactable = hit.transform.gameObject;
                // Call 'Interact' on the target
                interactable.SendMessage("Interact", SendMessageOptions.DontRequireReceiver);
                holdingItem = true;
                if (interactable.GetComponent<Rigidbody>() != null)
                {
                    holdSpeedModifier = Mathf.Clamp(1f / Mathf.Pow(interactable.GetComponent<Rigidbody>().mass, 0.33f), 0.5f, 1f);
                }
                else
                {
                    holdSpeedModifier = 1f;
                }
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

            // Reset reticule
            reticuleImage.sprite = reticuleDefault;
            reticuleImage.rectTransform.sizeDelta = new Vector2(reticuleDefault.rect.width, reticuleDefault.rect.height);
            if (!reticuleImage.enabled)
            {
                reticuleImage.enabled = true;
            }

            // Reset whatever is being done
            if (interactable && !(Input.GetKey(Keybindings.Instance.interaction) || Input.GetKey(Keybindings.Instance.altInteraction)))
            {
                interactable.SendMessage("Reset", SendMessageOptions.DontRequireReceiver);
                holdingItem = false;
                interactable = null;
            }
            if (!interactable)
            {
                toolTip.showPopup(false);
            }
        }
        #endregion

    }

    private void FixedUpdate()
    {
        // Move player accrding to the inputs
        if (allowMovement)
        {
            #region Movement
            // Viewpoint rotation

            if (allowRotation && allowMovement)
            {
                // Calculate horizontal rotation
                rotationX = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * sensitivity * Time.fixedDeltaTime;

                // Calculate vertical rotation
                rotationY += Input.GetAxis("Mouse Y") * sensitivity * Time.fixedDeltaTime;
                rotationY = Mathf.Clamp(rotationY, minY, maxY);

                // Rotate character for the horizontal rotation (camera is following the character)
                transform.localEulerAngles = new Vector3(0, rotationX, transform.rotation.eulerAngles.z);
                // Rotate only the camera in vertical rotation (so that the character model doesn't tilt)
                head.transform.localEulerAngles = (new Vector3(-rotationY, head.transform.localEulerAngles.y, 0));
            }

            // Check sneaking condition
            sneaking = Input.GetKey(Keybindings.Instance.crouch) || Input.GetKey(Keybindings.Instance.altCrouch);
            // Check sneaking condition
            //  DELETE THIS WHENEVER CONVENIENT !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            running = Input.GetKey(KeyCode.LeftShift) && !sneaking;

            if (sneaking)
            {
                //audioSettings.PlaySneakReverb();
                cam.transform.position = Vector3.Lerp(cam.transform.position, head.transform.position + cameraSneakDisplacement, 0.3f);
            }
            else
            {
                //audioSettings.StopSneakReverb();
                cam.transform.position = Vector3.Lerp(cam.transform.position, head.transform.position, 0.3f);
            }

            if (footstepSoundTimer > footStepMaxTime)
            {
                audioSettings.PlayPlayerFootStep(running, sneaking);

                footstepSoundTimer = 0.0f;
                SoundManager.makeSound(gameObject.transform.position, sneaking ? 75.0f : 175.0f, true);
            }

            // Actual movement
            float speedModifier = (sneaking ? sneakSpeedModifier : 1.0f) * (running ? runSpeedModifier : 1.0f) * (holdingItem ? holdSpeedModifier : 1.0f) * Time.deltaTime;
            Vector3 direction = transform.forward * Keybindings.Instance.vertical.GetAxis() + transform.right * Keybindings.Instance.horizontal.GetAxis();
            playerRB.MovePosition(transform.position + (Vector3.Normalize(direction) * speed * speedModifier));
            if (direction.magnitude > 0)
            {
                footstepSoundTimer += speedModifier;
            }
        }
        #endregion

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
        audioSettings.StopPlayerHPHeartbeat();
        GameManager.Instance.SetGameOver(false);
        foreach (GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            enemy.GetComponent<Enemy>().state = Enemy.State.Dormant;
        }
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
                audioSettings.PlayPlayerHPHeartbeat(39f);
                break;
            case HealthState.Lower:
                audioSettings.PlayPlayerHPHeartbeat(21f);
                break;
            case HealthState.Lowest:
                audioSettings.PlayPlayerHPHeartbeat(2f);
                break;
            default:
                Debug.Log("UpdateHealtAudio - HP out of bounds!");
                break;
        }
    }
}
