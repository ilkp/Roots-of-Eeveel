
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class DynamicSoundObject : MonoBehaviour
{
	private AudioSource audioSource;

	private void Awake()
	{
		audioSource = GetComponent<AudioSource>();
	}

	private void OnCollisionEnter(Collision collision)
    {
        Vector3 collisionForce = collision.impulse / Time.fixedDeltaTime;
        SoundManager.makeSound(transform.position, collisionForce.magnitude);
		audioSource.Play();
    }
}
