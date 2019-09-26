using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public Canvas gameOverScreen;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
        }
        Instance = this;
        DontDestroyOnLoad(this);
    }

    public void SetGameOver()
    {
        gameOverScreen.gameObject.SetActive(true);
    }
}
