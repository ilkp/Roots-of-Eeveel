using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class DynamicSoundObject : MonoBehaviour
{
	// The audio instance that playes the actual sounds
	private FMOD.Studio.EventInstance soundInstance;
	// The audio to be played
	[FMODUnity.EventRef] [SerializeField] private string sound;
	private Rigidbody rb;


	private void Awake()
	{
		rb = GetComponent<Rigidbody>();
		// Create the instance with given audiofile. only one instance, so only one sound at a time, if need for multiple, make more instances.
		soundInstance = FMODUnity.RuntimeManager.CreateInstance(sound);
	}

	private void OnCollisionEnter(Collision collision)
    {
        Vector3 collisionForce = collision.impulse / Time.fixedDeltaTime;
        SoundManager.makeSound(transform.position, collisionForce.magnitude);

		// Set the audio to be played from objects location, with RBs data, for some added effects?
		soundInstance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject, rb));
		// Play The Fucking Sound Already!
		soundInstance.start();
    }
}
