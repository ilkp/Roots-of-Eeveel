using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialPickup : ITutorial
{
    public string HintText => "Pickup";
    public bool Active { get; set; } = false;
    public Sprite HintSprite { get; set; }
    public bool Completed { get; set; }
    public TutorialManager Manager { get; set; }
    private PlayerMovement player;

    public TutorialPickup(TutorialManager manager)
    {
        Manager = manager;
        HintSprite = Manager.sprites[(int)TutorialIndices.Pickup];
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
    }

    public bool CheckCompletion()
    {
        if (player.interactable)
        {
            if (player.interactable.name == "Key")
            {
                return true;
            }
        }
        return false;
    }
}
