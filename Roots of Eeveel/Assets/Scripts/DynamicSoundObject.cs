using UnityEngine;

public enum SoundType
{
Metal, Wood, Glass, Ceramic, Key
}

[RequireComponent(typeof(Rigidbody))]
public class DynamicSoundObject : MonoBehaviour
{
	private Rigidbody rb;
	private AudioSettings audioSettings;
	public SoundType soundType;

	private void Awake()
	{
		rb = GetComponent<Rigidbody>();
		audioSettings = FindObjectOfType<GameManager>().audioSettings;
	}

	private void OnCollisionEnter(Collision collision)
    {
        Vector3 collisionForce = collision.impulse / Time.fixedDeltaTime;
        SoundManager.makeSound(transform.position, collisionForce.magnitude, false);

		switch (soundType)
		{
			case SoundType.Metal:
				StartCoroutine(audioSettings.PlayThrowableMetal(gameObject, rb, collisionForce.magnitude));
				break;
			case SoundType.Wood:
				StartCoroutine(audioSettings.PlayThrowableWood(gameObject, rb, collisionForce.magnitude));
				break;
			case SoundType.Glass:
				StartCoroutine(audioSettings.PlayThrowableGlass(gameObject, rb, collisionForce.magnitude));
				break;
			case SoundType.Ceramic:
				StartCoroutine(audioSettings.PlayThrowableCeramic(gameObject, rb, collisionForce.magnitude));
				break;
			case SoundType.Key:
				StartCoroutine(audioSettings.PlayThrowableKey(gameObject, rb, collisionForce.magnitude));
				break;
			default:
				Debug.Log("SoundType not found: " + soundType);
				break;
		}
    }
}
