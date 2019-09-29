
using UnityEngine;

[RequireComponent(typeof(AudioSource), typeof(Collider))]
public class StaticSoundObject : MonoBehaviour
{
	private AudioSource audioSource;

	private void Awake()
	{
		audioSource = GetComponent<AudioSource>();
		GetComponent<Collider>().isTrigger = true;
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			SoundManager.makeSound(transform.position, 100);
			audioSource.Play();
		}
	}
}