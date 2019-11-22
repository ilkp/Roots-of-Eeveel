using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionDetection : MonoBehaviour
{

    public PlayerMovement player;
    public Enemy enemy;

    void OnTriggerEnter(Collider collider)
    {
        Debug.Log("Trigger");
        if (collider.CompareTag("Player"))
        {
            enemy._playerHit = true;
            enemy._hand.enabled = false;
            player.GetHurt();
        }
    }
}
