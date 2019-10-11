using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Winning : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject endScreen;
    public Button button;
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.endingScreen = endScreen;
            GameManager.Instance.resetButton = button;
            GameManager.Instance.SetGameOver();
        }
    }
}
