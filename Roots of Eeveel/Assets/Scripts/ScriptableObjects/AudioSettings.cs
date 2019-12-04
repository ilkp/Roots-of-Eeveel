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
	[FMODUnity.EventRef] [SerializeField] private string raisingTension;
	private int raisingTensionProgress = 0;
	private const string tensionParameter = "Locks open"; // 0/1/2/3/4/5
	private const string monsterChaseParameter = "monster chase"; //0/1
	private FMOD.Studio.EventInstance tensionInstance;

	public void StopAll()
	{
		StopRoomTone();
		StopEnemySoundAll();
		StopMenuMusic();
		StopPlayerHPHeartbeat();
		StopRaisingTension();
	}

	public void PlayRoomTone()
	{
		roomToneInstance = FMODUnity.RuntimeManager.CreateInstance(roomTone);
		roomToneInstance.start();
	}

	public void StopRoomTone()
	{
		roomToneInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
		roomToneInstance.release();
	}

	public void PlayRaisingTension(float progress)
	{
		raisingTensionProgress = (int)progress;
		Debug.Log("RaisingTensionStarted:" + raisingTensionProgress);
		tensionInstance = FMODUnity.RuntimeManager.CreateInstance(raisingTension);
		tensionInstance.setParameterByName(tensionParameter, progress);
		tensionInstance.setParameterByName(monsterChaseParameter, 0);
		tensionInstance.start();
	}

	public void StopRaisingTension()
	{
		tensionInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
		tensionInstance.release();
	}

	public void IncreaseRisingTensionProgress()
	{
		if (!tensionInstance.isValid()) PlayRaisingTension(0);

		raisingTensionProgress++;
		Debug.Log("RaisingTension:" + raisingTensionProgress);
		tensionInstance.setParameterByName(tensionParameter, raisingTensionProgress);
	}

	public void DecreaseRisingTensionProgress()
	{
		--raisingTensionProgress;

		Debug.Log("RaisingTension:" + raisingTensionProgress);
		tensionInstance.setParameterByName(tensionParameter, raisingTensionProgress);
	}

	public void SetRisingTensionMonsterChase(bool chase)
	{
		tensionInstance.setParameterByName(monsterChaseParameter, (chase ? 1 : 0));
		Debug.Log("RaisingTensionmonsterChase:" + chase);
	}

	#endregion

	#region Enemy
	[Header("Enemy", order = 1)] //---------------------------------------------------------------------------------
	[FMODUnity.EventRef] [SerializeField] private string enemyFootStep;
	[FMODUnity.EventRef] [SerializeField] private string enemyIdle;
	[FMODUnity.EventRef] [SerializeField] private string enemyNoticeSound;

	private Dictionary<GameObject, FMOD.Studio.EventInstance> EnemySounds = new Dictionary<GameObject, FMOD.Studio.EventInstance>();

	public IEnumerator PlayEnemyNoticeSound()
	{
		FMOD.Studio.EventInstance enemyNoticeInstance = FMODUnity.RuntimeManager.CreateInstance(enemyNoticeSound);
		enemyNoticeInstance.start();

		while (true)
		{
			enemyNoticeInstance.getPlaybackState(out FMOD.Studio.PLAYBACK_STATE state);

			if (state != FMOD.Studio.PLAYBACK_STATE.PLAYING)
			{
				enemyNoticeInstance.release();
				break;
			}

			yield return null;
		}
	}

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

	private List<GameObject> ChasingEnemys = new List<GameObject>();

	public void AddEnemyToChase(GameObject go)
	{
		ChasingEnemys.Add(go);

		SetRisingTensionMonsterChase(true);

	}

	public void RemoveEnemyFromChase(GameObject go)
	{
		ChasingEnemys.Remove(go);

		if (ChasingEnemys.Count == 0)
		{
			SetRisingTensionMonsterChase(false);
		}
	}

	public void StopEnemySound(GameObject go)
	{
		if (!EnemySounds.ContainsKey(go))
			return;
		EnemySounds[go].stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
		EnemySounds[go].release();
		EnemySounds.Remove(go);
	}

	public void StopEnemySoundAll()
	{
		foreach(GameObject go in EnemySounds.Keys)
		{
			EnemySounds[go].stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
			EnemySounds[go].release();
			EnemySounds.Remove(go);
		}
	}

	#endregion

	#region Interactables
	[Header("Interactables", order = 2)] //-------------------------------------------------------------------------
	[FMODUnity.EventRef] [SerializeField] private string metalSound;
	[FMODUnity.EventRef] [SerializeField] private string woodSound;
	[FMODUnity.EventRef] [SerializeField] private string ceramicSound;
	[FMODUnity.EventRef] [SerializeField] private string glassSound;
	[FMODUnity.EventRef] [SerializeField] private string keyPickupSound;
	[FMODUnity.EventRef] [SerializeField] private string keyThrowSound;
	[FMODUnity.EventRef] [SerializeField] private string keyInsertSound;
	[FMODUnity.EventRef] [SerializeField] private string keyWrongLockSound;
	[FMODUnity.EventRef] [SerializeField] private string doorInteractSound;
	private FMOD.Studio.EventInstance keyPickupInstance;
	private FMOD.Studio.EventInstance keyInsertInstance;
	[FMODUnity.EventRef] [SerializeField] private string letterPickupSound;
	[FMODUnity.EventRef] [SerializeField] private string letterPutdownSound;
	[FMODUnity.EventRef] [SerializeField] private string bookPickupSound;
	[FMODUnity.EventRef] [SerializeField] private string bookPutdownSound;

	/// <summary>
	/// This is Coroutine
	/// </summary>
	/// <param name="go"></param>
	/// <param name="rb"></param>
	/// <returns></returns>
	public IEnumerator PlayBookPickup()
	{
		FMOD.Studio.EventInstance bookPickupInstance = FMODUnity.RuntimeManager.CreateInstance(bookPickupSound);
		bookPickupInstance.start();

		while (true)
		{
			bookPickupInstance.getPlaybackState(out FMOD.Studio.PLAYBACK_STATE state);

			if (state != FMOD.Studio.PLAYBACK_STATE.PLAYING)
			{
				bookPickupInstance.release();
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
	public IEnumerator PlayBookPutdown()
	{
		FMOD.Studio.EventInstance bookPutdownInstance = FMODUnity.RuntimeManager.CreateInstance(bookPutdownSound);
		bookPutdownInstance.start();

		while (true)
		{
			bookPutdownInstance.getPlaybackState(out FMOD.Studio.PLAYBACK_STATE state);

			if (state != FMOD.Studio.PLAYBACK_STATE.PLAYING)
			{
				bookPutdownInstance.release();
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
	public IEnumerator PlayLetterPutdown()
	{
		FMOD.Studio.EventInstance letterPutdownInstance = FMODUnity.RuntimeManager.CreateInstance(letterPutdownSound);
		letterPutdownInstance.start();

		while (true)
		{
			letterPutdownInstance.getPlaybackState(out FMOD.Studio.PLAYBACK_STATE state);

			if (state != FMOD.Studio.PLAYBACK_STATE.PLAYING)
			{
				letterPutdownInstance.release();
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
	public IEnumerator PlayLetterPickup()
	{
		FMOD.Studio.EventInstance letterPickupInstance = FMODUnity.RuntimeManager.CreateInstance(letterPickupSound);
		letterPickupInstance.start();

		while (true)
		{
			letterPickupInstance.getPlaybackState(out FMOD.Studio.PLAYBACK_STATE state);

			if (state != FMOD.Studio.PLAYBACK_STATE.PLAYING)
			{
				letterPickupInstance.release();
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
	public IEnumerator PlayWrongKey(GameObject go)
	{
		FMOD.Studio.EventInstance wrongInstance = FMODUnity.RuntimeManager.CreateInstance(keyWrongLockSound);
		wrongInstance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(go));
		wrongInstance.start();

		while (true)
		{
			wrongInstance.getPlaybackState(out FMOD.Studio.PLAYBACK_STATE state);

			if (state != FMOD.Studio.PLAYBACK_STATE.PLAYING)
			{
				wrongInstance.release();
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
	public IEnumerator PlayDoorInteract(GameObject go)
	{
		FMOD.Studio.EventInstance doorInteractInstance = FMODUnity.RuntimeManager.CreateInstance(doorInteractSound);
		doorInteractInstance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(go));
		doorInteractInstance.start();

		while (true)
		{
			doorInteractInstance.getPlaybackState(out FMOD.Studio.PLAYBACK_STATE state);

			if (state != FMOD.Studio.PLAYBACK_STATE.PLAYING)
			{
				doorInteractInstance.release();
				break;
			}

			yield return null;
		}
	}

	public void PlayKeyPickup()
	{
		if (!keyPickupInstance.isValid())
		{
			keyPickupInstance = FMODUnity.RuntimeManager.CreateInstance(keyPickupSound);
		}

		keyPickupInstance.start();
	}

	public void PlayKeyInsert(GameObject go)
	{
		if (!keyInsertInstance.isValid())
		{
			keyInsertInstance = FMODUnity.RuntimeManager.CreateInstance(keyInsertSound);
		}

		keyInsertInstance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(go));
		keyInsertInstance.start();
	}

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
	public IEnumerator PlayThrowableKey(GameObject go, Rigidbody rb, float amplitude)
	{
		//Debug.Log("PlayThrowableWood");
		FMOD.Studio.EventInstance keyThrowInstance = FMODUnity.RuntimeManager.CreateInstance(keyThrowSound);
		keyThrowInstance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(go, rb));
		keyThrowInstance.setParameterByName("impact_strenght", Mathf.Clamp(amplitude / 600, 0f, 1f));
		keyThrowInstance.start();

		while (true)
		{
			keyThrowInstance.getPlaybackState(out FMOD.Studio.PLAYBACK_STATE state);

			if (state != FMOD.Studio.PLAYBACK_STATE.PLAYING)
			{
				keyThrowInstance.release();
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
	[FMODUnity.EventRef] [SerializeField] private string playerHPHeartbeat;
	[FMODUnity.EventRef] [SerializeField] private string playerHPRoots;
	private FMOD.Studio.EventInstance playerFootStepInstance;
	private FMOD.Studio.EventInstance playerDamageLowInstance;
	private FMOD.Studio.EventInstance playerDamageHighInstance;
	private FMOD.Studio.EventInstance playerHPHeartbeatInstance;
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
		//Debug.Log("PlayerHPRoots");	
		if (!playerHPRootsInstance.isValid())
		{
			playerHPRootsInstance = FMODUnity.RuntimeManager.CreateInstance(playerHPRoots);
		}

		playerHPRootsInstance.start();
	}

	/// <summary>
	/// Hp 0-100
	/// </summary>
	/// <param name="hp"></param>
	public void PlayPlayerHPHeartbeat(float hp)
	{
		Debug.Log("PlayerHPHeartbeat Start");
		if (!playerHPHeartbeatInstance.isValid())
		{
			playerHPHeartbeatInstance = FMODUnity.RuntimeManager.CreateInstance(playerHPHeartbeat);
		}

		playerHPHeartbeatInstance.setParameterByName("Player HP", hp);
		playerHPHeartbeatInstance.start();
	}

	public void StopPlayerHPHeartbeat()
	{
		Debug.Log("PlayerHPHeartbeat Stop");
		playerHPHeartbeatInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
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
