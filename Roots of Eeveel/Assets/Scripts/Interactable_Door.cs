using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Interactable_Door : MonoBehaviour, IInteractable
{
    public event Action<IInteractable> OnInteract;
    private Rigidbody rb;

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
        StopAllCoroutines();
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().allowRotation = true;
    }

    IEnumerator Hold()
    {
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().allowRotation = false;
        float doorPlayerAngle = Vector3.Angle(transform.right, GameObject.FindGameObjectWithTag("Player").transform.forward);
        float  mousey;
        while(Input.GetButton("Fire1"))
        {
            mousey = Input.GetAxis("Mouse Y");

            rb.AddForce(transform.right * (doorPlayerAngle < 90 ? 1.0f : -1.0f) * mousey * 10);
            yield return null;
        }
    }
}
