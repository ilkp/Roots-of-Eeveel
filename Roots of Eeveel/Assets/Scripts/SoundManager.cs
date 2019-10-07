
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
		/*enemies.Clear();
		foreach(GameObject enemyObject in GameObject.FindGameObjectsWithTag("Enemy"))
		{
			enemies.Add(enemyObject.GetComponent<Enemy>());
		}*/
	}

	public static void makeSound(Vector3 source, float amplitude)
	{
        foreach (Enemy enemy in Instance.enemies)
        {
            Enemy enemyAI = enemy.GetComponent<Enemy>();
            double distance = Vector3.Distance(source, enemy.transform.position);
            double oomph = Mathf.Clamp(amplitude / 600, 0f, 1f) / (distance / 40f);
            double valppaus = enemyAI._alertness <= 0 ? 1 : Mathf.Clamp(1 / (float)enemyAI._alertness, 0f, 1f);
            Debug.Log("Oomph: " + oomph + "\nValppaus: " + valppaus + "\nDistance to enemy: " + distance + "\nAmplitude: " + amplitude);

            if ((valppaus / 2) < oomph)
            {
                enemyAI._alertness += (oomph / 2);
            }
            
            if (valppaus < oomph)
            {
                enemy.alert(source);
            }
        }
	}

}
