

using UnityEngine;

public interface ITutorial
{
    string HintText { get; }
    Sprite HintSprite { get; set; }
    bool Active { get; set; }
    bool Completed { get; set; }
    bool CheckCompletion();
}