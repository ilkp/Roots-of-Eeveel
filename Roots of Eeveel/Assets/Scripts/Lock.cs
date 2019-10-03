using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lock : MonoBehaviour, IInteractable
{
	[SerializeField] public GameObject key;
	[ContextMenuItem("SetKeyRotation", "SetKeyRot")]
	public Vector3 keyRot;
	private void SetKeyRot()
	{
		keyRot = key.transform.eulerAngles;
	}
	[ContextMenuItem("SetKeyPosition", "SetKeyPos")]
	public Vector3 keyPos;
	private void SetKeyPos()
	{
		keyPos = key.transform.position;
	}
	[SerializeField] private string toolTip = "Hmm...\nSeems to be lock.\n:-D";
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

	private void Awake()
	{
		if (key == null)
		{
			throw new MissingReferenceException("Key not set!");
		}
		else if(key.GetComponent<Interactable_Key>() == null)
		{
			throw new MissingComponentException("Key doesn't contain 'key' script!");
		}
	}

	public event Action<IInteractable> OnInteract;

	public void Open()
	{
		if(OnInteract != null)
			OnInteract(this);
	}

	public void Interact()
	{
		throw new NotImplementedException();
	}

	public void StopInteraction()
	{
		throw new NotImplementedException();
	}
}
