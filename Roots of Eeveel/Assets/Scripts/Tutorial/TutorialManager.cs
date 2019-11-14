using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
	[SerializeField] private TextMeshProUGUI tutorialText;
	[SerializeField] private Image tutorialImage;

	[SerializeField] private Sprite wasdSprite;

	private List<ITutorial> tutorials;
	private ITutorial activeTutorial;

	public void ResetTutorials()
	{
		foreach (ITutorial t in tutorials)
		{
			t.Completed = false;
		}
	}

	private void Start()
	{
		tutorials = new List<ITutorial>() { new TutorialMovement(wasdSprite) };
	}

	// Update is called once per frame
	void Update()
    {
		if (activeTutorial != null)
		{
			if (activeTutorial.CheckCompletion())
			{
				activeTutorial.Completed = true;
				tutorialText.enabled = false;
				tutorialImage.enabled = false;
				activeTutorial = null;
			}
		}
		else
		{
			foreach (ITutorial tutorial in tutorials)
			{
				if (!tutorial.Completed && tutorial.CheckActivation())
				{
					activeTutorial = tutorial;
					tutorialText.enabled = true;
					tutorialImage.enabled = true;
					tutorialText.text = tutorial.HintText;
					tutorialImage.sprite = tutorial.HintSprite;
					break;
				}
			}
		}
    }
}
