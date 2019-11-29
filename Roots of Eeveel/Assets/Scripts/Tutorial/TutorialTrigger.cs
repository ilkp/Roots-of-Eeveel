using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialTrigger : MonoBehaviour
{

    [SerializeField]
    private TutorialIndices tutorialIndex = TutorialIndices.Sneak;
    [SerializeField]
    private TutorialManager tutorialManager;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            tutorialManager.tutorials[(int)tutorialIndex].Active = true;
            gameObject.SetActive(false);
        }
    }
}
