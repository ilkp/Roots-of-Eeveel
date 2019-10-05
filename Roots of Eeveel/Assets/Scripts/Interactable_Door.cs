using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(ConfigurableJoint))]
public class Interactable_Door : MonoBehaviour, IInteractable
{
    public event Action<IInteractable> OnInteract;
    private Rigidbody rb;

	// The audio instance that playes the actual sounds and sound to be played
	private FMOD.Studio.EventInstance puzzleCompleteSoundInstance;
	[FMODUnity.EventRef] [SerializeField] private string puzzleCompleteSound;

	// Gameobjects that hold the triggers and dictionary that holds their completion state
	[SerializeField] private List<GameObject> triggers;
	private Dictionary<IInteractable, bool> triggerState = new Dictionary<IInteractable, bool>();
	
	private string toolTip = "Hold down leftMouseButton to interact.";
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

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        gameObject.tag = "Interactable";
		ConfigurableJoint joint = GetComponent<ConfigurableJoint>();
		SoftJointLimit limitLow = joint.lowAngularXLimit;
		SoftJointLimit limitHigh = joint.highAngularXLimit;
		limitLow.limit = -90.0f;
		limitHigh.limit = 0.0f;
		joint.lowAngularXLimit = limitLow;
		joint.highAngularXLimit = limitHigh;
		joint.xMotion = ConfigurableJointMotion.Locked;
		joint.yMotion = ConfigurableJointMotion.Locked;
		joint.zMotion = ConfigurableJointMotion.Locked;
		joint.angularXMotion = triggers.Count > 0 ? ConfigurableJointMotion.Locked : ConfigurableJointMotion.Limited;
		joint.angularYMotion = ConfigurableJointMotion.Locked;
		joint.angularZMotion = ConfigurableJointMotion.Locked;

		// Create the instance with given audiofile. only one instance, so only one sound at a time, if need for multiple, make more instances.
		puzzleCompleteSoundInstance = FMODUnity.RuntimeManager.CreateInstance(puzzleCompleteSound);
		// Set the audio to be played from objects location, with RBs data, for some added effects?
		puzzleCompleteSoundInstance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));

		// List all triggers to dictionary, and makes sure they are valid
		foreach (GameObject trigger in triggers)
		{
			IInteractable interactable = trigger.GetComponent<IInteractable>();
			if (interactable != null)
			{
				triggerState.Add(interactable, false);
				interactable.OnInteract += this.trigger;
			}
			else
			{
				throw new Exception("Trigger missing 'IInteractable', like Interactable_Button-script");
			}
		}
	}

	private void trigger(IInteractable interactable)
	{
		// Flip triggering interactables state
		triggerState[interactable] = !triggerState[interactable];
		// Check if we are open. Release door hinge if we are.
		if (CheckAllTriggers())
		{
			puzzleCompleteSoundInstance.start();
			GetComponent<ConfigurableJoint>().angularXMotion = ConfigurableJointMotion.Limited;
		}
	}

	private bool CheckAllTriggers()
	{
		foreach (var pair in triggerState)
		{
			if (!pair.Value)
			{
				return false;
			}
		}
		return true;
	}

	public void Interact()
    {
        StartCoroutine(Hold());
    }

    public void StopInteraction()
    {
		GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().allowRotation = true;
		StopAllCoroutines();
    }

    private IEnumerator Hold()
    {
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().allowRotation = false;
        float doorPlayerAngleForward = Vector3.Angle(transform.right, GameObject.FindGameObjectWithTag("Player").transform.forward);
		float doorPlayerAngleSideways = Vector3.Angle(transform.forward, GameObject.FindGameObjectWithTag("Player").transform.forward);
		float mousey;
		float mousex;
		float mouseModifier;
        while(Input.GetButton("Fire1") && Input.GetAxisRaw("Vertical") == 0 && Input.GetAxisRaw("Horizontal") == 0)
        {
            mousey = Input.GetAxis("Mouse Y");
			mousex = Input.GetAxis("Mouse X");
			mouseModifier = mousey * (doorPlayerAngleForward < 90 ? 1.0f : -1.0f) + mousex * (doorPlayerAngleSideways < 90 ? 1.0f : -1.0f);
			rb.AddForce(transform.right * mouseModifier * 10);
            yield return null;
        }
		StopInteraction();
	}
}
