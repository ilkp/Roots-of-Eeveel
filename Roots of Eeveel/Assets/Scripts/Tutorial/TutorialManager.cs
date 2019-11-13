using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
	[SerializeField] private TextMeshProUGUI tutorialText;
	[SerializeField] private Image tutorialImage;

	private List<ITutorial> tutorials;
	private ITutorial activeTutorial;

	private void Start()
	{
		tutorials = new List<ITutorial>() { new TutorialMovement() };
	}

	// Update is called once per frame
	void Update()
    {
		if (activeTutorial != null)
		{
			if (activeTutorial.CheckCompletion())
			{
				tutorials.Remove(activeTutorial);
				activeTutorial = null;
			}
		}
		else
		{
			foreach (ITutorial tutorial in tutorials)
			{
				if (tutorial.CheckActivation())
				{
					activeTutorial = tutorial;
					tutorialText.text = tutorial.HintText;
					tutorialImage.sprite = tutorial.HintSprite;
					break;
				}
			}
		}
    }
}
