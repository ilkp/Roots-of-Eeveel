
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }
    [SerializeField] private List<Enemy> enemies;
    // Array for enemy hearing ranges in different states
    [SerializeField] private float[] enemyHearingRanges;

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

    }

    public void LoadEnemies()
    {
        enemies.Clear();
        foreach (GameObject enemyObject in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            enemies.Add(enemyObject.GetComponent<Enemy>());
        }
    }

    public static void makeSound(Vector3 source, float amplitude, bool isPlayer)
    {
        foreach (Enemy enemy in Instance.enemies)
        {
            Enemy enemyAI = enemy.GetComponent<Enemy>();
            double distance = Vector3.Distance(source, enemy.transform.position);
            distance = Mathf.Clamp((float)(distance / Instance.enemyHearingRanges[(int)enemyAI.state]), 0, 1);
            double oomph = Mathf.Clamp(amplitude / 600, 0f, 1f);
            double alertness = 1 - (1 - distance);
            //Debug.Log("Oomph: " + oomph + "\nValppaus: " + alertness + "\nDistance to enemy: " + distance + "\nAmplitude: " + amplitude);

            if (alertness < oomph)
            {
                enemy.alert(source, isPlayer);
            }
        }
    }

}
