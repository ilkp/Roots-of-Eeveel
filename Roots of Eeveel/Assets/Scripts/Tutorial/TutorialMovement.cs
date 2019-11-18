
using UnityEditor;
using UnityEngine;

public class TutorialMovement : ITutorial
{
    public string HintText => "Move with";
    public bool Active { get; set; } = false;
    public Sprite HintSprite { get; set; }
    public bool Completed { get; set; } = false;

    public TutorialMovement(Sprite sprite)
    {
        HintSprite = sprite;
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
