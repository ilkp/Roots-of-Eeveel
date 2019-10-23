using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AudioSettings", menuName = "AudioSettings")]
public class AudioSettings : ScriptableObject
{
	[Header("Atmosphere", order = 0)] //----------------------------------------------------------------------------
	[FMODUnity.EventRef] [SerializeField] private string atmosphereMusic;
	private FMOD.Studio.EventInstance atmosphereInstance;

	public void PlayAtmosphere()
	{
		atmosphereInstance = FMODUnity.RuntimeManager.CreateInstance(atmosphereMusic);
		atmosphereInstance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(new Vector3(0, 0, 0)));
		atmosphereInstance.start();
	}



	[Header("Enemy", order = 1)] //---------------------------------------------------------------------------------
	[FMODUnity.EventRef] [SerializeField] private string enemyFootStep;
	[FMODUnity.EventRef] [SerializeField] private string enemyIdle;
	private FMOD.Studio.EventInstance enemyIdleInstance;

	private Dictionary<GameObject, FMOD.Studio.EventInstance> EnemySounds = new Dictionary<GameObject, FMOD.Studio.EventInstance>();

	public void PlayEnemyFootStep(GameObject go)
	{
		FMOD.Studio.EventInstance enemyFootStepInstance = FMODUnity.RuntimeManager.CreateInstance(enemyFootStep);
		enemyFootStepInstance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(go));
		enemyFootStepInstance.start();

		EnemySounds.Add(go, enemyFootStepInstance);
	}

	public void PlayEnemyIdle(GameObject go)
	{
		FMOD.Studio.EventInstance enemyIdleInstance = FMODUnity.RuntimeManager.CreateInstance(enemyIdle);
		enemyIdleInstance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(go));
		enemyIdleInstance.start();

		EnemySounds.Add(go, enemyIdleInstance);
	}

	public void StopEnemySound(GameObject go)
	{
		if (!EnemySounds.ContainsKey(go))
			return;
		EnemySounds[go].stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
		EnemySounds[go].release();
		EnemySounds.Remove(go);
	}


	[Header("Interactables", order = 2)] //-------------------------------------------------------------------------
	[FMODUnity.EventRef] [SerializeField] private string wrongKey;
	private FMOD.Studio.EventInstance wrongKeyInstance;



	[Header("Music", order = 3)] //---------------------------------------------------------------------------------
	[FMODUnity.EventRef] [SerializeField] private string menuMusic;
	[SerializeField] private float menuMusicFadeTime = 5f;
	private FMOD.Studio.EventInstance musicInstance;

	public void PlayMenuMusic()
	{
		if (!musicInstance.isValid())
		{
			musicInstance = FMODUnity.RuntimeManager.CreateInstance(menuMusic);
		}

		musicInstance.start();
	}

	public void StopMenuMusic()
	{
		musicInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
	}

	public IEnumerator FadeMenuMusic()
	{
		float counter = 0f;
		musicInstance.getVolume(out float startVolume);

		while (counter < menuMusicFadeTime)
		{
			counter += Time.deltaTime;
			musicInstance.setVolume(Mathf.Lerp(startVolume, 0f, (counter / menuMusicFadeTime)));

			yield return null;
		}

		musicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);

		PlayAtmosphere();
	}



	[Header("Player", order = 4)] //--------------------------------------------------------------------------------
	[FMODUnity.EventRef] [SerializeField] private string playerFootStep;
	private FMOD.Studio.EventInstance playerFootstepInstance;

	public void PlayPlayerFootStep(Transform transform)
	{
		if (!playerFootstepInstance.isValid())
		{

			playerFootstepInstance = FMODUnity.RuntimeManager.CreateInstance(playerFootStep);
		}

		playerFootstepInstance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(transform));
		playerFootstepInstance.start();
	}



	[Header("UI", order = 5)] //------------------------------------------------------------------------------------
	[FMODUnity.EventRef] [SerializeField] private string button;

	public IEnumerator PlayUIButton()
	{
		FMOD.Studio.EventInstance buttonInstance = FMODUnity.RuntimeManager.CreateInstance(button);

		buttonInstance.start();

		while (true)
		{
			buttonInstance.getPlaybackState(out FMOD.Studio.PLAYBACK_STATE state);

			if (state != FMOD.Studio.PLAYBACK_STATE.PLAYING)
			{
				buttonInstance.release();
				break;
			}

			yield return null;
		}
	}

}
