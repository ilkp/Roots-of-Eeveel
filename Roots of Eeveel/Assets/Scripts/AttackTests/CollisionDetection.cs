using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionDetection : MonoBehaviour
{

    private BoxCollider hand;
    public PlayerMovement player;
    public Enemy enemy;

    void Awake()
    {
        hand = GetComponent<BoxCollider>();
        hand.enabled = false;
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Player"))
        {
            hand.enabled = false;
            enemy._playerHit = true;
            player.GetHurt();
        }
    }
}
