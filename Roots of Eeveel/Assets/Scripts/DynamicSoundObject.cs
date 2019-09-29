
using UnityEngine;

public class DynamicSoundObject : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        Vector3 collisionForce = collision.impulse / Time.fixedDeltaTime;
        SoundManager.makeSound(transform.position, collisionForce.magnitude);
    }
}
