using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AudioSettings", menuName = "AudioSettings")]
public class AudioSettings : ScriptableObject
{
	[Header("Atmosphere", order = 0)] //----------------------------------------------------------------------------
	[FMODUnity.EventRef] [SerializeField] private string atmosphereMusic;
	private FMOD.Studio.EventInstance atmosphereInstance;

	[Header("Enemy", order = 1)] //---------------------------------------------------------------------------------
	[FMODUnity.EventRef] [SerializeField] private string enemyFootStep;
	private FMOD.Studio.EventInstance enemyFootStepInstance;
	[FMODUnity.EventRef] [SerializeField] private string enemyIdle;
	private FMOD.Studio.EventInstance enemyIdleInstance;

	[Header("Interactables", order = 2)] //-------------------------------------------------------------------------
	[FMODUnity.EventRef] [SerializeField] private string wrongKey;
	private FMOD.Studio.EventInstance wrongKeyInstance;

	[Header("Music", order = 3)] //---------------------------------------------------------------------------------
	[FMODUnity.EventRef] [SerializeField] private string menuMusic;
	[SerializeField] private float menuMusicFadeTime = 5f;
	private FMOD.Studio.EventInstance musicInstance;

	[Header("Player", order = 4)] //--------------------------------------------------------------------------------
	[FMODUnity.EventRef] [SerializeField] private string playerFootStep;
	private FMOD.Studio.EventInstance playerFootstepInstance;

	[Header("UI", order = 5)] //------------------------------------------------------------------------------------
	[FMODUnity.EventRef] [SerializeField] private string button;
	private FMOD.Studio.EventInstance buttonInstance;

}
