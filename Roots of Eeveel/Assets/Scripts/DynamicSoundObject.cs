using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicSoundObject : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        Vector3 collisionForce = collision.impulse / Time.fixedDeltaTime;
        Debug.Log(collisionForce.magnitude);
        SoundManager.makeSound(transform.position, collisionForce.magnitude);
    }
}
