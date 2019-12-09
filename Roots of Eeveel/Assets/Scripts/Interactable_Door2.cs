using System;
using System.Collections;
using UnityEngine;

public class Interactable_Door2 : MonoBehaviour, IInteractable
{
    public float minAngle;
    public float maxAngle;
    public float currentAngle;
    public float grabDistance;
    private Transform playerCam;

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
        gameObject.tag = "Interactable";
        playerCam = GameObject.Find("Main Camera").GetComponent<Transform>();
    }

    public void Interact()
    {
        RaycastHit hit;
        Physics.Raycast(playerCam.position, playerCam.forward, out hit, 4f);
        grabDistance = Vector3.Distance(hit.transform.position, playerCam.position);
        StartCoroutine(Hold());
    }

    public void StopInteraction()
    {
        StopAllCoroutines();
    }

    private IEnumerator Hold()
    {
        Vector3 axis = Vector3.up;
        while ((Input.GetKey(Keybindings.Instance.interaction) || Input.GetKey(Keybindings.Instance.altInteraction)) && Input.GetAxisRaw("Vertical") == 0 && Input.GetAxisRaw("Horizontal") == 0)
        {
            transform.localEulerAngles = new Vector3(
                0,
                transform.localEulerAngles.y +
                    Vector3.Angle(transform.right,
                        new Vector3(playerCam.position.x + (playerCam.forward.x * grabDistance), 0, playerCam.position.z + (playerCam.forward.z * grabDistance))),
                0);
            yield return null;
        }
        StopInteraction();
    }
}
