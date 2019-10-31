using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Interface for defining common aspects of all interactable objects
/// </summary>
public class Interactable_Readable : MonoBehaviour, IInteractable
{

    /// <summary>
    /// Papyrus object attached to player's camera
    /// </summary>
    [Tooltip("Papyrus object attached to the player's camera.")]
    [SerializeField] private MeshRenderer papyrus;
    [SerializeField] private Image reticule;

    [SerializeField] private string toolTip = "Take a closer look";

    /// <summary>
    /// ToolTip is shown on the screen when player is near the object and Should describe how the objects is used.
    /// </summary>
    public string ToolTip
    {
        get
        {
            return toolTip;
        }
        set
        {
            toolTip = value;
        }
    }

    /// <summary>
    /// Takes care of delivering interaction info to other classees
    /// </summary>
    public event Action<IInteractable> OnInteract;

    /// <summary>
    /// Is run when player tries to interact with the object
    /// </summary>
    public void Interact()
    {
        // Show papyrus
        //Time.timeScale = 0;
        papyrus.enabled = true;
        reticule.enabled = false;
    }

    /// <summary>
    /// Is run when player lets go of the object
    /// </summary>
    public void StopInteraction()
    {
        // Disable papyrus
        papyrus.enabled = false;
        reticule.enabled = true;
        //Time.timeScale = 1;
    }
}
