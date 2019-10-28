using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AudioSettings", menuName = "AudioSettings")]
public class AudioSettings : ScriptableObject
{
	public float musicVolume;
	public float soundsVolume;

	#region Atmosphere
	[Header("Atmosphere", order = 0)] //----------------------------------------------------------------------------
	[FMODUnity.EventRef] [SerializeField] private string roomTone;
	private FMOD.Studio.EventInstance roomToneInstance;
	[FMODUnity.EventRef] [SerializeField] private string tensionMusic;
	[SerializeField] private string tensionParameterName;
	private FMOD.Studio.EventInstance tensionInstance;


	public void PlayRoomTone()
	{
		roomToneInstance = FMODUnity.RuntimeManager.CreateInstance(roomTone);
		roomToneInstance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(new Vector3(0, 0, 0)));
		roomToneInstance.start();
	}

	public void StopRoomTone()
	{
		roomToneInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
		roomToneInstance.release();
	}

	public void PlayRaisingTension(float progress)
	{
		tensionInstance = FMODUnity.RuntimeManager.CreateInstance(tensionMusic);
		tensionInstance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(new Vector3(0, 0, 0)));
		tensionInstance.setParameterByName(tensionParameterName, progress);
		tensionInstance.start();
	}

	public void StopRaisingTension()
	{
		tensionInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
		tensionInstance.release();
	}

	public void SetRisingTensionProgress(float progress)
	{
		tensionInstance.setParameterByName(tensionParameterName, progress);
	}
	#endregion

	#region Enemy
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
	#endregion

	#region Interactables
	[Header("Interactables", order = 2)] //-------------------------------------------------------------------------
	[FMODUnity.EventRef] [SerializeField] private string metalSound;
	[FMODUnity.EventRef] [SerializeField] private string woodSound;

	public IEnumerator PlayThrowableWood(GameObject go, Rigidbody rb)
	{
		Debug.Log("PlayThrowableWood");
		FMOD.Studio.EventInstance woodInstance = FMODUnity.RuntimeManager.CreateInstance(woodSound);
		woodInstance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(go, rb));
		woodInstance.start();

		while (true)
		{
			woodInstance.getPlaybackState(out FMOD.Studio.PLAYBACK_STATE state);

			if (state != FMOD.Studio.PLAYBACK_STATE.PLAYING)
			{
				woodInstance.release();
				break;
			}

			yield return null;
		}
	}

	public IEnumerator PlayThrowableMetal(GameObject go, Rigidbody rb)
	{
		FMOD.Studio.EventInstance metalInstance = FMODUnity.RuntimeManager.CreateInstance(metalSound);
		metalInstance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(go, rb));
		metalInstance.start();

		while (true)
		{
			metalInstance.getPlaybackState(out FMOD.Studio.PLAYBACK_STATE state);

			if (state != FMOD.Studio.PLAYBACK_STATE.PLAYING)
			{
				metalInstance.release();
				break;
			}

			yield return null;
		}
	}

	#endregion

	#region Music
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

		PlayRoomTone();
	}

	#endregion

	#region Player
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

	#endregion

	#region UI
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

	#endregion
}
