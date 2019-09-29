
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
	public static SoundManager Instance { get; private set; }
	[SerializeField] private List<Enemy> enemies;

	void Awake()
	{
		if (Instance != null)
		{
			Destroy(gameObject);
		}
		Instance = this;
		DontDestroyOnLoad(gameObject);
	}

	private void Start()
	{
		enemies.Clear();
		foreach(GameObject enemyObject in GameObject.FindGameObjectsWithTag("Enemy"))
		{
			enemies.Add(enemyObject.GetComponent<Enemy>());
		}
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
