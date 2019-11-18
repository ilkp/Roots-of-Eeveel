using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialLook : ITutorial
{
    public string HintText => throw new System.NotImplementedException();
    public bool Active { get; set; } = false;
    public Sprite HintSprite { get; set; }
    public bool Completed { get; set; }

    public bool CheckCompletion()
    {
        if (Completed)
        {

        }
        return Completed;
    }
}
