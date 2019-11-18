using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialSneak : ITutorial
{
    public string HintText => "Sneak";
    public bool Active { get; set; } = false;
    public Sprite HintSprite { get; set; }
    public bool Completed { get; set; }
    public TutorialManager Manager { get; set; }

    public TutorialSneak(TutorialManager manager)
    {
        Manager = manager;
    }

    public bool CheckCompletion()
    {
        throw new System.NotImplementedException();
    }
}
