using System;
using System.Collections;
using UnityEngine;

public class Interactable_Door2 : MonoBehaviour, IInteractable
{
    public bool locked;
    public float unlockMinAngle;
    public float unlockMaxAngle;
    public float unlockAngle;
    private float minAngle;
    private float maxAngle;
    private float grabDistance;
    private Transform playerCam;
    private Vector3 grabPosition;
    private Vector3 pivotPosition;

    public event Action<IInteractable> OnInteract;
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

    // Start is called before the first frame update
    void Start()
    {
        if (!locked)
        {
            Unlock();
        }
        gameObject.tag = "Interactable";
        playerCam = GameObject.Find("Main Camera").GetComponent<Transform>();
    }

    public void Interact()
    {
        int n = Vector3.Dot(playerCam.position, transform.forward) > 0 ? 1 : -1;
        grabDistance = Vector3.Dot((transform.position - playerCam.position), transform.forward * n)
                        / Vector3.Dot(playerCam.forward, transform.forward * n);
        StartCoroutine(Hold());
    }

    public void StopInteraction()
    {
        Debug.Log("Boop");
        StopAllCoroutines();
    }

    private IEnumerator Hold()
    {
        Vector3 axis = Vector3.up;
        int multiplier;
        float angle = 0;
        while ((Input.GetKey(Keybindings.Instance.interaction) || Input.GetKey(Keybindings.Instance.altInteraction)))
        {
            grabPosition = playerCam.position + (playerCam.forward * grabDistance);
            pivotPosition = transform.position;
            grabPosition.y = 0;
            pivotPosition.y = 0;

            multiplier = Vector3.Dot(transform.right, grabPosition - pivotPosition) > 0 ? 1 : -1;

            angle = Vector3.Angle(
                transform.right * multiplier,
                grabPosition - pivotPosition);

            if (Vector3.Dot(transform.forward * multiplier, grabPosition - pivotPosition) > 0)
            {
                angle *= -1;
            }

            float newAngle = (transform.localEulerAngles.y > 180 ? transform.localEulerAngles.y - 360 : transform.localEulerAngles.y) + angle;
            if (newAngle >= minAngle && newAngle <= maxAngle)
            {
                transform.localEulerAngles = new Vector3(
                0,
                newAngle,
                0);
            }


            yield return null;
        }
        StopInteraction();
    }

    public void Unlock()
    {
        locked = false;
        minAngle = unlockMinAngle;
        maxAngle = unlockMaxAngle;
        transform.localEulerAngles = new Vector3(
                0,
                unlockAngle,
                0);
    }
}
