using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialTrigger : MonoBehaviour
{

    [SerializeField]
    private TutorialIndices tutorialIndex;
    [SerializeField]
    private TutorialManager tutorialManager;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //tutorialManager.ResetTutorials[tutorialIndex];
            Destroy(gameObject);
        }
    }
}
