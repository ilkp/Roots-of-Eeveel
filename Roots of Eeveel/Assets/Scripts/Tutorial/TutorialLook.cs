using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialLook : ITutorial
{
    public string HintText => throw new System.NotImplementedException();
    public bool Active { get; set; } = false;
    public Sprite HintSprite { get; set; }
    public bool Completed { get; set; }
    public TutorialManager Manager { get; set; }

    public TutorialLook(TutorialManager manager)
    {
        Manager = manager;
    }

    public bool CheckCompletion()
    {
        if (Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0)
        {
            Manager.tutorials[(int)TutorialIndices.Movement].Active = true;
            return true;
        }
        return false;
    }
}
