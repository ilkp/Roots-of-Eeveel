using System.Collections;
using UnityEngine;

public class ScriptedScare_01 : MonoBehaviour
{
	private bool triggered = false;
	private float elapsedTime = 0.0f;
	private float speed = 8.0f;
	[SerializeField] private GameObject capsule;

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.tag == "Player" && !triggered)
		{
			Debug.Log("collision");
			triggered = true;
			StartCoroutine(trigger());
		}
	}

	private IEnumerator trigger()
	{
		while (elapsedTime < 1.0f)
		{
			capsule.transform.Translate(Vector3.forward * speed * Time.deltaTime);
			elapsedTime += Time.deltaTime;
			yield return null;
		}
		capsule.SetActive(false);
	}
}
