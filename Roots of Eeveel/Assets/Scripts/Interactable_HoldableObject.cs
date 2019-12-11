using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Interactable_HoldableObject : MonoBehaviour, IInteractable
{
    [SerializeField] private string toolTip = "Hold down leftMouseButton to hold the object in hand.\nRightMouseButton to throw the object.";
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

    public event Action<IInteractable> OnInteract;
    private bool insidePlayer;

    private GameObject player;
    private Transform head;
    private Rigidbody rb;
    [SerializeField] private float pullForce = 10;
    [SerializeField] private float throwForce = 10;
    private Vector3 destination;

    private void Awake()
    {
        player = FindObjectOfType<PlayerMovement>().gameObject;
        head = player.GetComponentInChildren<Camera>().transform;
        rb = GetComponent<Rigidbody>();
        gameObject.tag = "Interactable";
        rb.Sleep();
        insidePlayer = false;
    }

    public void Interact()
    {
        rb.useGravity = false;
        StartCoroutine(Hold());
    }

    public void StopInteraction()
    {
        if (!insidePlayer)
        {
            gameObject.layer = 0;
        }
        rb.useGravity = true;
        StopAllCoroutines();
    }

    IEnumerator Hold()
    {
        float holdDistance = Vector3.Distance(head.transform.position, transform.position);
        Vector3 offSet = transform.position - (head.position + head.forward * holdDistance);
        gameObject.layer = 11;

        while (true)
        {
            float zoom = Input.GetAxis("Mouse ScrollWheel");
            holdDistance = Mathf.Clamp(holdDistance + zoom, 1f, 4f);

            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                rb.AddForce(head.forward * throwForce, ForceMode.Impulse);
                rb.useGravity = true;

                break;
            }

            destination = head.position + offSet + head.forward * holdDistance;
            rb.velocity = ((destination - transform.position) * pullForce / (rb.mass < 1 ? rb.mass : rb.mass / 2));
            rb.angularVelocity *= 0.9f * Time.fixedDeltaTime;

            yield return null;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            insidePlayer = true;
            gameObject.layer = 11;
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            insidePlayer = false;
            gameObject.layer = 0;
        }
    }
}
