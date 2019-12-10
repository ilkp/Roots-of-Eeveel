using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(ConfigurableJoint))]
public class Interactable_Door : MonoBehaviour, IInteractable
{
    [SerializeField] private AudioSettings audioSettings;

    public int doorType; // 0 = small door, 1 = big door left, 2 = big door right
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
        hingeSettings();
        audioSettings = FindObjectOfType<GameManager>().audioSettings;
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.mass = 10.0f;
        rb.drag = 2f;
        gameObject.tag = "Interactable";
    }

    public void Interact()
    {
        audioSettings.PlayDoorInteract(this.gameObject);
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
        float mousey, mousex;
        float mouseModifier;
        while ((Input.GetKey(Keybindings.Instance.interaction) || Input.GetKey(Keybindings.Instance.altInteraction)) && Input.GetAxisRaw("Vertical") == 0 && Input.GetAxisRaw("Horizontal") == 0)
        {
            mousey = Input.GetAxis("Mouse Y");
            mousex = Input.GetAxis("Mouse X");
            mouseModifier = mousex + mousey * (doorPlayerAngleForward < 90 ? 1.0f : -1.0f);
            rb.AddForce(transform.forward * mouseModifier * 10);
            yield return null;
        }
        StopInteraction();
    }

    private void hingeSettings()
    {
        ConfigurableJoint joint = GetComponent<ConfigurableJoint>();
        gameObject.layer = LayerMask.NameToLayer("Door");
        joint.connectedBody.gameObject.layer = LayerMask.NameToLayer("Wall");

        joint.axis = new Vector3(0, 1, 0);
        SoftJointLimit limitHigh = joint.lowAngularXLimit;
        SoftJointLimit limitLow = joint.highAngularXLimit;
        switch (doorType)
        {
            case 0:
                limitHigh.limit = 90;
                limitLow.limit = 0;
                break;
            case 1:
                limitHigh.limit = 90;
                limitLow.limit = 0;
                break;
            case 2:
                limitHigh.limit = 0;
                limitLow.limit = -90;
                break;
            default:
                break;
        }

        joint.highAngularXLimit = limitHigh;
        joint.lowAngularXLimit = limitLow;

        joint.xMotion = ConfigurableJointMotion.Locked;
        joint.yMotion = ConfigurableJointMotion.Locked;
        joint.zMotion = ConfigurableJointMotion.Locked;
        joint.angularXMotion = ConfigurableJointMotion.Limited;
        joint.angularYMotion = ConfigurableJointMotion.Locked;
        joint.angularZMotion = ConfigurableJointMotion.Locked;
    }
}
