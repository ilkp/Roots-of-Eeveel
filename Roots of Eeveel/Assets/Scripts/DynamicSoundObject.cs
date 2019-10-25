using UnityEngine;

public enum SoundType
{
Metal, Wood
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
				StartCoroutine(audioSettings.PlayThrowableMetal(gameObject, rb));
				break;
			case SoundType.Wood:
				StartCoroutine(audioSettings.PlayThrowableWood(gameObject, rb));
				break;
			default:
				Debug.Log("SoundType not found: " + soundType);
				break;
		}
    }
}
