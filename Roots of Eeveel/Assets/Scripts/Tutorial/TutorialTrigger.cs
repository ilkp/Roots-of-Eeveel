using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialTrigger : MonoBehaviour
{

    [SerializeField]
    private TutorialIndices tutorialIndex;
    [SerializeField]
    private TutorialManager tutorialManager;
    private TutorialSneak sneek;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ((TutorialSneak)tutorialManager.tutorials[(int)tutorialIndex]).trigger = true;
            Destroy(gameObject);
        }
    }
}
