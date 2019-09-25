using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
	private static SoundManager Instance;
	private List<Enemy> enemies;

	void Awake()
	{
		if (Instance != null)
		{
			Destroy(gameObject);
		}
		Instance = this;
		DontDestroyOnLoad(gameObject);
	}


	public static void makeSound(Vector3 source, float amplitude)
	{

	}
}
