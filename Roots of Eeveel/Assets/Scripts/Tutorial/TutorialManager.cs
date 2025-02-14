﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI tutorialText;
    [SerializeField] private Image tutorialImage;

    [SerializeField] public Sprite[] sprites;

    public ITutorial[] tutorials;
    private ITutorial activeTutorial;
	private const float fadeOutMax = 0.5f;
	private bool fading = false;

    public void ResetTutorials()
    {
        foreach (ITutorial t in tutorials)
        {
            t.Completed = false;
        }
    }

    private void Start()
    {
        tutorials = new ITutorial[(int)TutorialIndices.Sneak + 1];
        tutorials[(int)TutorialIndices.Look] = new TutorialLook(this);
        tutorials[(int)TutorialIndices.Movement] = new TutorialMovement(this);
        tutorials[(int)TutorialIndices.Pickup] = new TutorialPickup(this);
        tutorials[(int)TutorialIndices.Sneak] = new TutorialSneak(this);
    }

    // Update is called once per frame
    void Update()
    {
		if (fading)
		{
			return;
		}

        if (activeTutorial != null)
        {
            if (activeTutorial.CheckCompletion())
            {
				fading = true;
				Coroutine fade = StartCoroutine(fadeOut());
			}
        }
        else
        {
            foreach (ITutorial tutorial in tutorials)
            {
                if (!tutorial.Completed && tutorial.Active)
                {
                    activeTutorial = tutorial;
                    tutorialText.enabled = true;
                    tutorialImage.enabled = true;
					tutorialImage.color = new Color(1, 1, 1, 1);
					tutorialText.faceColor = new Color(1, 1, 1, 1);
					tutorialText.text = tutorial.HintText;
                    tutorialImage.sprite = tutorial.HintSprite;
                    break;
                }
            }
        }
    }

	private IEnumerator fadeOut()
	{
		Color c = tutorialImage.color;
		float timer = 0;
		float a;
		while (timer < fadeOutMax)
		{
			a = 1f - timer / fadeOutMax;
			tutorialImage.color = new Color(c.r, c.g, c.b, a);
			tutorialText.faceColor = new Color(c.r, c.g, c.b, a);
			timer += Time.deltaTime;
			yield return null;
		}
		tutorialImage.color = new Color(c.r, c.g, c.b, 0);
		tutorialText.faceColor = new Color(c.r, c.g, c.b, 0);
		yield return new WaitForSeconds(0.5f);
		activeTutorial.Completed = true;
		tutorialText.enabled = false;
		tutorialImage.enabled = false;
		activeTutorial = null;
		fading = false;
	}
}

public enum TutorialIndices
{
    Look,
    Movement,
    Pickup,
    Sneak,
}