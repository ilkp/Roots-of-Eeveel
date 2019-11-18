
using UnityEditor;
using UnityEngine;

public class TutorialMovement : ITutorial
{
    public string HintText => "Move";
    public bool Active { get; set; } = false;
    public Sprite HintSprite { get; set; }
    public bool Completed { get; set; } = false;
    public TutorialManager Manager { get; set; }

    public TutorialMovement(TutorialManager manager)
    {
        Manager = manager;
    }

    public bool CheckCompletion()
    {
        if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
        {
            return true;
        }
        return false;
    }
}
