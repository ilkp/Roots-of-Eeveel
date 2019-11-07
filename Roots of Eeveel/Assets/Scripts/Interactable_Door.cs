using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(ConfigurableJoint))]
public class Interactable_Door : MonoBehaviour, IInteractable
{
    public event Action<IInteractable> OnInteract;
    private Rigidbody rb;
	
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

	private void Awake()
	{
		ConfigurableJoint joint = GetComponent<ConfigurableJoint>();
		gameObject.layer = LayerMask.NameToLayer("Door");
		joint.connectedBody.gameObject.layer = LayerMask.NameToLayer("Wall");
		joint.axis = new Vector3(0, 1, 0);
		SoftJointLimit limitHigh = joint.lowAngularXLimit;
		SoftJointLimit limitLow = joint.highAngularXLimit;
		limitHigh.limit = 90;
		limitLow.limit = 0;
		joint.highAngularXLimit = limitHigh;
		joint.lowAngularXLimit = limitLow;

		//limit.limit = 90.0f;
		//joint.angularYLimit = limit;
		joint.xMotion = ConfigurableJointMotion.Locked;
		joint.yMotion = ConfigurableJointMotion.Locked;
		joint.zMotion = ConfigurableJointMotion.Locked;
		joint.angularXMotion = ConfigurableJointMotion.Limited;
		joint.angularYMotion = ConfigurableJointMotion.Locked;
		joint.angularZMotion = ConfigurableJointMotion.Locked;
	}

	private void Start()
    {
        rb = GetComponent<Rigidbody>();
		rb.mass = 10.0f;
        gameObject.tag = "Interactable";
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
        float doorPlayerAngleForward = Vector3.Angle(transform.forward, GameObject.FindGameObjectWithTag("Player").transform.forward);
		float mousey;
		float mouseModifier;
        while(Input.GetButton("Fire1") && Input.GetAxisRaw("Vertical") == 0 && Input.GetAxisRaw("Horizontal") == 0)
        {
            mousey = Input.GetAxis("Mouse Y");
			mouseModifier = mousey * (doorPlayerAngleForward < 90 ? 1.0f : -1.0f);
			rb.AddForce(transform.forward * mouseModifier * 10);
            yield return null;
        }
		StopInteraction();
	}
}
