﻿
using UnityEngine;


public class Winning : MonoBehaviour
{
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.SetGameOver(true);
        }
    }
}
