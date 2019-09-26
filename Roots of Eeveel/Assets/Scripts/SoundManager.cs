using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
	public static SoundManager Instance { get; private set; }
	public List<Enemy> enemies;

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
        foreach (Enemy enemy in Instance.enemies)
        {
            if ((enemy.transform.position - source).magnitude < 10.0f)
            {
                enemy.alert(source);
            }
        }
	}

}
