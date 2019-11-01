using System;
using System.Collections;
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
    [SerializeField] private Camera playerCam;
    [SerializeField] private bool isZoomed;

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
        papyrus.enabled = !papyrus.enabled;
        reticule.enabled = !reticule.enabled;
        if (!papyrus.enabled && isZoomed)
        {
            SecondInteract();
        }
    }

    public void SecondInteract()
    {
        if (papyrus.enabled || isZoomed)
        {
            isZoomed = !isZoomed;
            StopAllCoroutines();
            StartCoroutine(Zoom());
        }
    }

    /// <summary>
    /// Is run when player looks away from the object
    /// </summary>
    public void Reset()
    {
        papyrus.enabled = false;
        if (isZoomed)
        {
            SecondInteract();
        }
    }

    /// <summary>
    /// Is run when player lets go of the object
    /// </summary>
    public void StopInteraction()
    {
        // Do nothing
    }

    IEnumerator Zoom()
    {
        float fov = (isZoomed ? 40f : 60f);
        while (playerCam.fieldOfView < fov || playerCam.fieldOfView > fov)
        {
            playerCam.fieldOfView = Mathf.Lerp(playerCam.fieldOfView, fov, 0.2f);
            yield return 0;
        }
    }
}
