
using UnityEngine;

public class EndGame : MonoBehaviour
{
	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.tag == "Player")
		{
			GameManager.Instance.SetGameOver(true);
			gameObject.GetComponent<BoxCollider>().enabled = false;
		}
	}
}
