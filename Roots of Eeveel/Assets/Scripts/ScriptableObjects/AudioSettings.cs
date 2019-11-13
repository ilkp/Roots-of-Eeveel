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
	[FMODUnity.EventRef] [SerializeField] private string ceramicSound;
	[FMODUnity.EventRef] [SerializeField] private string glassSound;

	/// <summary>
	/// This is Coroutine
	/// </summary>
	/// <param name="go"></param>
	/// <param name="rb"></param>
	/// <returns></returns>
	public IEnumerator PlayThrowableWood(GameObject go, Rigidbody rb, float amplitude)
	{
		//Debug.Log("PlayThrowableWood");
		FMOD.Studio.EventInstance woodInstance = FMODUnity.RuntimeManager.CreateInstance(woodSound);
		woodInstance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(go, rb));
		woodInstance.setParameterByName("impact_strenght", Mathf.Clamp(amplitude / 600, 0f, 1f));
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

	/// <summary>
	/// This is Coroutine
	/// </summary>
	/// <param name="go"></param>
	/// <param name="rb"></param>
	/// <returns></returns>
	public IEnumerator PlayThrowableMetal(GameObject go, Rigidbody rb, float amplitude)
	{
		FMOD.Studio.EventInstance metalInstance = FMODUnity.RuntimeManager.CreateInstance(metalSound);
		metalInstance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(go, rb));
		metalInstance.setParameterByName("impact_strenght", Mathf.Clamp(amplitude / 600, 0f, 1f));
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

	/// <summary>
	/// This is Coroutine
	/// </summary>
	/// <param name="go"></param>
	/// <param name="rb"></param>
	/// <returns></returns>
	public IEnumerator PlayThrowableCeramic(GameObject go, Rigidbody rb, float amplitude)
	{
		//Debug.Log("PlayThrowableWood");
		FMOD.Studio.EventInstance ceramicInstance = FMODUnity.RuntimeManager.CreateInstance(ceramicSound);
		ceramicInstance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(go, rb));
		ceramicInstance.setParameterByName("impact_strenght", Mathf.Clamp(amplitude / 600, 0f, 1f));
		ceramicInstance.start();

		while (true)
		{
			ceramicInstance.getPlaybackState(out FMOD.Studio.PLAYBACK_STATE state);

			if (state != FMOD.Studio.PLAYBACK_STATE.PLAYING)
			{
				ceramicInstance.release();
				break;
			}

			yield return null;
		}
	}

	/// <summary>
	/// This is Coroutine
	/// </summary>
	/// <param name="go"></param>
	/// <param name="rb"></param>
	/// <returns></returns>
	public IEnumerator PlayThrowableGlass(GameObject go, Rigidbody rb, float amplitude)
	{
		//Debug.Log("PlayThrowableWood");
		FMOD.Studio.EventInstance glassInstance = FMODUnity.RuntimeManager.CreateInstance(glassSound);
		glassInstance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(go, rb));
		glassInstance.setParameterByName("impact_strenght", Mathf.Clamp(amplitude / 600, 0f, 1f));
		glassInstance.start();

		while (true)
		{
			glassInstance.getPlaybackState(out FMOD.Studio.PLAYBACK_STATE state);

			if (state != FMOD.Studio.PLAYBACK_STATE.PLAYING)
			{
				glassInstance.release();
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

	/// <summary>
	/// This is Coroutine
	/// </summary>
	/// <returns></returns>
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
	[FMODUnity.EventRef] [SerializeField] private string playerTakeDamageLow;
	[FMODUnity.EventRef] [SerializeField] private string playerTakeDamageHigh;
	[FMODUnity.EventRef] [SerializeField] private string playerHPHeartbeatSlow;
	[FMODUnity.EventRef] [SerializeField] private string playerHPHeartbeatFast;
	[FMODUnity.EventRef] [SerializeField] private string playerHPRoots;
	private FMOD.Studio.EventInstance playerFootStepInstance;
	private FMOD.Studio.EventInstance playerDamageLowInstance;
	private FMOD.Studio.EventInstance playerDamageHighInstance;
	private FMOD.Studio.EventInstance playerHPHeartbeatFastInstance;
	private FMOD.Studio.EventInstance playerHPHeartbeatSlowInstance;
	private FMOD.Studio.EventInstance playerHPRootsInstance;

	//public void PlayPlayerFootStep(Transform transform, bool isRun, bool isSneak)
	public void PlayPlayerFootStep(bool isRun, bool isSneak)
	{
		if (!playerFootStepInstance.isValid())
		{
			playerFootStepInstance = FMODUnity.RuntimeManager.CreateInstance(playerFootStep);
		}

		playerFootStepInstance.setParameterByName("player_sneak", (isSneak ? 1 : 0));
		playerFootStepInstance.setParameterByName("player_run", (isRun ? 1 : 0));

		//playerFootStepInstance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(transform));
		playerFootStepInstance.start();
	}

	public void PlayPlayerDamageLow()
	{
		if (!playerDamageLowInstance.isValid())
		{
			playerDamageLowInstance = FMODUnity.RuntimeManager.CreateInstance(playerTakeDamageLow);
		}

		playerDamageLowInstance.start();
	}

	public void PlayPlayerDamageHigh()
	{
		if (!playerDamageHighInstance.isValid())
		{
			playerDamageHighInstance = FMODUnity.RuntimeManager.CreateInstance(playerTakeDamageHigh);
		}

		playerDamageHighInstance.start();
	}

	public void PlayPlayerHPRoots()
	{
		Debug.Log("PlayerHPRoots");	
		if (!playerHPRootsInstance.isValid())
		{
			playerHPRootsInstance = FMODUnity.RuntimeManager.CreateInstance(playerHPRoots);
		}

		playerHPRootsInstance.start();
	}

	public void PlayPlayerHPHeartbeatSlow()
	{
		Debug.Log("PlayerHPHeartbeatSlow");
		if (!playerHPHeartbeatSlowInstance.isValid())
		{
			playerHPHeartbeatSlowInstance = FMODUnity.RuntimeManager.CreateInstance(playerHPHeartbeatSlow);
		}

		playerHPHeartbeatSlowInstance.start();
	}

	public void PlayPlayerHPHeartbeatFast()
	{
		Debug.Log("PlayerHPHeartbeatFast");
		if (!playerHPHeartbeatFastInstance.isValid())
		{
			playerHPHeartbeatFastInstance = FMODUnity.RuntimeManager.CreateInstance(playerHPHeartbeatFast);
		}

		playerHPHeartbeatFastInstance.start();
	}

	public void StopPlayerHPHeartbeatFast()
	{
		playerHPHeartbeatFastInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
	}

	public void StopPlayerHPHeartbeatSlow()
	{
		playerHPHeartbeatSlowInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
	}

	#endregion

	#region UI
	[Header("UI", order = 5)] //------------------------------------------------------------------------------------
	[FMODUnity.EventRef] [SerializeField] private string button;
	[FMODUnity.EventRef] [SerializeField] private string startButton;

	/// <summary>
	/// This is Coroutine
	/// </summary>
	/// <returns></returns>
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

	public void PlayUIStartGameButton()
	{
		FMOD.Studio.EventInstance startButtonInstance = FMODUnity.RuntimeManager.CreateInstance(startButton);

		startButtonInstance.start();
	}

	#endregion
}
