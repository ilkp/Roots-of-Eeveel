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
		SoftJointLimit limitLow = joint.lowAngularXLimit;
		SoftJointLimit limitHigh = joint.highAngularXLimit;
		limitLow.limit = -90.0f;
		limitHigh.limit = 0.0f;
		joint.lowAngularXLimit = limitLow;
		joint.highAngularXLimit = limitHigh;
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
        float doorPlayerAngleForward = Vector3.Angle(transform.right, GameObject.FindGameObjectWithTag("Player").transform.forward);
		float mousey;
		float mouseModifier;
        while(Input.GetButton("Fire1") && Input.GetAxisRaw("Vertical") == 0 && Input.GetAxisRaw("Horizontal") == 0)
        {
            mousey = Input.GetAxis("Mouse Y");
			mouseModifier = mousey * (doorPlayerAngleForward < 90 ? 1.0f : -1.0f);
			rb.AddForce(transform.right * mouseModifier * 10);
            yield return null;
        }
		StopInteraction();
	}
}
