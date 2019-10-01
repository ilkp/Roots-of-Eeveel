
using UnityEngine;

[RequireComponent(typeof(Collider), typeof(Rigidbody))]
public class StaticSoundObject : MonoBehaviour
{
	// The audio instance that playes the actual sounds
	private FMOD.Studio.EventInstance soundInstance;
	// The audio to be played
	[FMODUnity.EventRef] [SerializeField] private string sound;

	private void Awake()
	{
		GetComponent<Collider>().isTrigger = true;
		// Create the instance with given audiofile. only one instance, so only one sound at a time, if need for multiple, make more instances.
		soundInstance = FMODUnity.RuntimeManager.CreateInstance(sound);
		// Set the audio to be played from objects location, with RBs data, for some added effects?
		soundInstance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			SoundManager.makeSound(transform.position, 100);
			// Play The Fucking Sound Already!
			soundInstance.start();
		}
	}
}