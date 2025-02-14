﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AudioSettings", menuName = "AudioSettings")]
public class AudioSettings : ScriptableObject
{

	public void StopAllSounds()
	{
		StopRoomTone();
		StopEnemySoundAll();
		StopMenuMusic();
		StopPlayerHPHeartbeat();
		StopRaisingTension();
		FMODUnity.RuntimeManager.GetBus("bus:/").stopAllEvents(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
	}

	#region VolumeControls
	public float musicVolume;
	private FMOD.Studio.Bus musicBus;
	public float soundsVolume;
	private FMOD.Studio.Bus soundBus;
	public float atmosphereVolume;
	private FMOD.Studio.Bus atmosphereBus;
	public float voiceVolume;
	private FMOD.Studio.Bus voiceBus;

	public void SetMusicVolume(float volume)
	{
		if (!musicBus.isValid())
		{
			musicBus = FMODUnity.RuntimeManager.GetBus("bus:/MU");
		}

		musicBus.setVolume(volume);
	}

	public void SetSoundVolume(float volume)
	{
		if (!soundBus.isValid())
		{
			soundBus = FMODUnity.RuntimeManager.GetBus("bus:/SX");
		}

		soundBus.setVolume(volume);
	}

	public void SetAtmosphereVolume(float volume)
	{
		if (!atmosphereBus.isValid())
		{
			atmosphereBus = FMODUnity.RuntimeManager.GetBus("bus:/AT");
		}

		atmosphereBus.setVolume(volume);
	}

	public void SetVoiceVolume(float volume)
	{
		if (!voiceBus.isValid())
		{
			voiceBus = FMODUnity.RuntimeManager.GetBus("bus:/VO");
		}

		voiceBus.setVolume(volume);
	}
	#endregion

	#region Atmosphere
	[Header("Atmosphere", order = 0)] //----------------------------------------------------------------------------
	[FMODUnity.EventRef] [SerializeField] private string roomTone;
	[FMODUnity.EventRef] [SerializeField] public string roomMechanics;
	[FMODUnity.EventRef] [SerializeField] public string roomMechanics1;
	[FMODUnity.EventRef] [SerializeField] public string roomMechanics2;
	private FMOD.Studio.EventInstance roomToneInstance;
	[FMODUnity.EventRef] [SerializeField] private string raisingTension;
	private int raisingTensionProgress = 0;
	private const string tensionParameter = "Locks open"; // 0/1/2/3/4/5
	private const string monsterChaseParameter = "monster chase"; //0/1
	private FMOD.Studio.EventInstance tensionInstance;

	public void PlayRoomTone()
	{
		roomToneInstance = FMODUnity.RuntimeManager.CreateInstance(roomTone);
		roomToneInstance.setVolume(0.5f);
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
	[FMODUnity.EventRef] [SerializeField] private string enemyStateSound;
	[FMODUnity.EventRef] [SerializeField] private string enemyAttackLungeSound;
	[FMODUnity.EventRef] [SerializeField] private string enemyAttachSlashSound;
	[FMODUnity.EventRef] [SerializeField] private string enemySXAttachSlashSound;
	private string enemyStateParameter = "Monster state";

	private Dictionary<GameObject, FMOD.Studio.EventInstance> EnemySounds = new Dictionary<GameObject, FMOD.Studio.EventInstance>();

	private Dictionary<GameObject, FMOD.Studio.EventInstance> EnemyStateSounds = new Dictionary<GameObject, FMOD.Studio.EventInstance>();

	public void PlayEnemyState(GameObject go)
	{
		FMOD.Studio.EventInstance enemyStateInstance = FMODUnity.RuntimeManager.CreateInstance(enemyStateSound);
		EnemyStateSounds.Add(go, enemyStateInstance);
		enemyStateInstance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(go));
		enemyStateInstance.setParameterByName(enemyStateParameter, 0);
		enemyStateInstance.start();
	}

	public void SetEnemyState(GameObject go, int value)
	{
		EnemyStateSounds[go].setParameterByName(enemyStateParameter, value);
	}

	public IEnumerator PlayEnemyAttackSound(GameObject go)
	{
		FMOD.Studio.EventInstance enemyAttackInstance = FMODUnity.RuntimeManager.CreateInstance((Random.value > 0.5f) ? enemyAttackLungeSound : enemyAttachSlashSound);
		FMOD.Studio.EventInstance enemySXAttackInstance = FMODUnity.RuntimeManager.CreateInstance(enemySXAttachSlashSound);
		enemySXAttackInstance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(go));
		enemyAttackInstance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(go));
		enemySXAttackInstance.start();
		enemyAttackInstance.start();
		Debug.Log("EnemyAttackSoundPlayed");
		while (true)
		{
			enemyAttackInstance.getPlaybackState(out FMOD.Studio.PLAYBACK_STATE state);

			if (state != FMOD.Studio.PLAYBACK_STATE.PLAYING)
			{
				enemyAttackInstance.release();
				enemySXAttackInstance.release();
				break;
			}

			yield return null;
		}
	}

	public IEnumerator PlayEnemyNoticeSound()
	{
		FMOD.Studio.EventInstance enemyNoticeInstance = FMODUnity.RuntimeManager.CreateInstance(enemyNoticeSound);
		enemyNoticeInstance.setVolume(0.5f);
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

	public IEnumerator PlayEnemyFootStep(GameObject go)
	{
		FMOD.Studio.EventInstance enemyFootStepInstance = FMODUnity.RuntimeManager.CreateInstance(enemyFootStep);
		enemyFootStepInstance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(go));
		enemyFootStepInstance.start();
		//Debug.Log("EnemyFootStepPlay");
		while (true)
		{
			enemyFootStepInstance.getPlaybackState(out FMOD.Studio.PLAYBACK_STATE state);

			if (state != FMOD.Studio.PLAYBACK_STATE.PLAYING)
			{
				enemyFootStepInstance.release();
				break;
			}

			yield return null;
		}
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
		GameObject[] enemyGOs = new GameObject[EnemySounds.Keys.Count];
		int j = 0;

		foreach (GameObject enemy in EnemySounds.Keys)
		{
			enemyGOs[j] = enemy;
			++j;
		}

		for ( int i = enemyGOs.Length - 1; i >= 0; --i)
		{
			GameObject go = enemyGOs[i];
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

	
	public void PlayBookPickup()
	{
		FMODUnity.RuntimeManager.PlayOneShot(bookPickupSound);
		PlayLoreReverb();
	}

	public void PlayBookPutdown()
	{
		StopLoreReverb();
		FMODUnity.RuntimeManager.PlayOneShot(bookPutdownSound);
	}

	public void PlayLetterPutdown()
	{
		FMODUnity.RuntimeManager.PlayOneShot(letterPutdownSound);
		PlayLoreReverb();
	}

	public void PlayLetterPickup()
	{
		FMODUnity.RuntimeManager.PlayOneShot(letterPickupSound);
		StopLoreReverb();
	}

	public void PlayWrongKey(GameObject go)
	{
		FMODUnity.RuntimeManager.PlayOneShot(keyWrongLockSound, go.transform.position);
	}

	public void PlayDoorInteract(GameObject go)
	{
		FMODUnity.RuntimeManager.PlayOneShot(doorInteractSound, go.transform.position);
	}

	public void PlayKeyPickup()
	{
		keyPickupInstance = FMODUnity.RuntimeManager.CreateInstance(keyPickupSound);
		keyPickupInstance.setVolume(0.8f);
		keyPickupInstance.start();
		keyPickupInstance.release();
	}

	public void PlayKeyInsert(GameObject go)
	{
		keyInsertInstance = FMODUnity.RuntimeManager.CreateInstance(keyInsertSound);
		keyInsertInstance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(go));
		keyInsertInstance.setVolume(0.8f);
		keyInsertInstance.start();
		keyInsertInstance.release();
	}

	public void PlayThrowableWood(GameObject go, Rigidbody rb, float amplitude)
	{
		FMOD.Studio.EventInstance woodInstance = FMODUnity.RuntimeManager.CreateInstance(woodSound);
		woodInstance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(go, rb));
		woodInstance.setParameterByName("impact_strenght", Mathf.Clamp(amplitude / 600, 0f, 0.8f));
		woodInstance.start();
		woodInstance.release();
	}

	public void PlayThrowableKey(GameObject go, Rigidbody rb, float amplitude)
	{
		FMOD.Studio.EventInstance keyThrowInstance = FMODUnity.RuntimeManager.CreateInstance(keyThrowSound);
		keyThrowInstance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(go, rb));
		keyThrowInstance.setParameterByName("impact_strenght", Mathf.Clamp(amplitude / 600, 0f, 1f));
		keyThrowInstance.start();
		keyThrowInstance.release();
	}

	public void PlayThrowableMetal(GameObject go, Rigidbody rb, float amplitude)
	{
		FMOD.Studio.EventInstance metalInstance = FMODUnity.RuntimeManager.CreateInstance(metalSound);
		metalInstance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(go, rb));
		metalInstance.setParameterByName("impact_strenght", Mathf.Clamp(amplitude / 600, 0f, 1f));
		metalInstance.start();
		metalInstance.release();
	}

	public void PlayThrowableCeramic(GameObject go, Rigidbody rb, float amplitude)
	{
		FMOD.Studio.EventInstance ceramicInstance = FMODUnity.RuntimeManager.CreateInstance(ceramicSound);
		ceramicInstance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(go, rb));
		ceramicInstance.setParameterByName("impact_strenght", Mathf.Clamp(amplitude / 600, 0f, 1f));
		ceramicInstance.start();
		ceramicInstance.release();
	}

	public void PlayThrowableGlass(GameObject go, Rigidbody rb, float amplitude)
	{
		FMOD.Studio.EventInstance glassInstance = FMODUnity.RuntimeManager.CreateInstance(glassSound);
		glassInstance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(go, rb));
		glassInstance.setParameterByName("impact_strenght", Mathf.Clamp(amplitude / 600, 0f, 1f));
		glassInstance.start();
		glassInstance.release();
	}

	#endregion

	#region Music
	[Header("Music", order = 3)] //---------------------------------------------------------------------------------
	[FMODUnity.EventRef] [SerializeField] private string menuMusic;
	[FMODUnity.EventRef] [SerializeField] private string creditsMusic;
	[SerializeField] private float menuMusicFadeTime = 1f;
	private FMOD.Studio.EventInstance musicInstance;

	public void PlayCredits()
	{
		FMODUnity.RuntimeManager.PlayOneShot(creditsMusic);
	}

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
		PlayRoomTone();
		float counter = 0f;
		musicInstance.getVolume(out float startVolume);

		while (counter < menuMusicFadeTime)
		{
			counter += Time.deltaTime;
			musicInstance.setVolume(Mathf.Lerp(startVolume, 0f, 2 * (counter / menuMusicFadeTime)));

			yield return null;
		}

		musicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);

		
	}

	#endregion

	#region Player
	[Header("Player", order = 4)] //--------------------------------------------------------------------------------
	[FMODUnity.EventRef] [SerializeField] private string playerFootStep;
	[FMODUnity.EventRef] [SerializeField] private string playerTakeDamageLow;
	[FMODUnity.EventRef] [SerializeField] private string playerTakeDamageHigh;
	[FMODUnity.EventRef] [SerializeField] private string playerSXTakeDamageLow;
	[FMODUnity.EventRef] [SerializeField] private string playerSXTakeDamageHigh;
	[FMODUnity.EventRef] [SerializeField] private string playerHPHeartbeat;
	[FMODUnity.EventRef] [SerializeField] private string playerHPRoots;
	private FMOD.Studio.EventInstance playerFootStepInstance;
	private FMOD.Studio.EventInstance playerDamageLowInstance;
	private FMOD.Studio.EventInstance playerDamageHighInstance;
	private FMOD.Studio.EventInstance playerSXDamageLowInstance;
	private FMOD.Studio.EventInstance playerSXDamageHighInstance;
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

		if (!playerSXDamageLowInstance.isValid())
		{
			playerSXDamageLowInstance = FMODUnity.RuntimeManager.CreateInstance(playerSXTakeDamageLow);
		}

		playerSXDamageLowInstance.start();
	}

	public void PlayPlayerDamageHigh()
	{
		if (!playerDamageHighInstance.isValid())
		{
			playerDamageHighInstance = FMODUnity.RuntimeManager.CreateInstance(playerTakeDamageHigh);
		}

		playerSXDamageHighInstance.start();

		if (!playerSXDamageHighInstance.isValid())
		{
			playerSXDamageHighInstance = FMODUnity.RuntimeManager.CreateInstance(playerSXTakeDamageHigh);
		}

		playerSXDamageHighInstance.start();
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

	#region Reverb
	[Header("Reverb", order = 6)] //------------------------------------------------------------------------------------
	[FMODUnity.EventRef] [SerializeField] private string loreReverb;
	[FMODUnity.EventRef] [SerializeField] private string sneakReverb;

	private bool isLore = false;
	private bool isSneak = false;

	FMOD.Studio.EventInstance LoreInstance;
	FMOD.Studio.EventInstance SneakInstance;


	public void PlayLoreReverb()
	{
		if(!LoreInstance.isValid()) LoreInstance = FMODUnity.RuntimeManager.CreateInstance(loreReverb);
		if (!isLore)
		{
			isLore = true;
			LoreInstance.start();
		}
	}

	public void StopLoreReverb()
	{
		if(isLore)
		{
			isLore = false;
			LoreInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
		}
	}

	public void PlaySneakReverb()
	{
		if (!SneakInstance.isValid()) SneakInstance = FMODUnity.RuntimeManager.CreateInstance(sneakReverb);
		if (!isSneak)
		{
			isSneak = true;
			SneakInstance.start();
		}
	}

	public void StopSneakReverb()
	{
		if (isSneak)
		{
			isSneak = false;
			SneakInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
		}
	}

	#endregion

	#region VoiceOver
	[Header("UI", order = 7)] //------------------------------------------------------------------------------------
	[FMODUnity.EventRef] [SerializeField] private string startLore;
	[FMODUnity.EventRef] [SerializeField] private string coreLetter1;
	[FMODUnity.EventRef] [SerializeField] private string coreLetter2;
	[FMODUnity.EventRef] [SerializeField] private string extraLetter1;
	[FMODUnity.EventRef] [SerializeField] private string extraLetter2;
	private FMOD.Studio.EventInstance startLoreInstance;
	private FMOD.Studio.EventInstance coreLetter1Instance;
	private FMOD.Studio.EventInstance coreLetter2Instance;
	private FMOD.Studio.EventInstance extraLetter1Instance;
	private FMOD.Studio.EventInstance extraLetter2Instance;

	public void PlayLetter(int readable)
	{
		if (readable == 0)
		{
			return;
		}
		startLoreInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
		coreLetter1Instance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
		coreLetter2Instance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
		extraLetter1Instance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
		extraLetter2Instance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);

		switch (readable)
		{
			case 1:
				PlayCoreLetter1();
				break;
			case 2:
				PlayCoreLetter2();
				break;
			case 3:
				PlayerExtraLetter1();
				break;
			case 4:
				PlayerExtraLetter2();
				break;
			default:
				break;
		}
	}

	public IEnumerator PlayLoreStart()
	{
		yield return new WaitForSecondsRealtime(2);
		startLoreInstance = FMODUnity.RuntimeManager.CreateInstance(startLore);
		startLoreInstance.start();
	}

	public void StopLoreStart()
	{
		startLoreInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
		startLoreInstance.release();
	}

	public void PlayCoreLetter1()
	{
		coreLetter1Instance = FMODUnity.RuntimeManager.CreateInstance(coreLetter1);
		coreLetter1Instance.start();
	}

	public void StopCoreLetter1()
	{
		coreLetter1Instance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
		coreLetter1Instance.release();
	}

	public void PlayCoreLetter2()
	{
		coreLetter2Instance = FMODUnity.RuntimeManager.CreateInstance(coreLetter2);
		coreLetter2Instance.start();
	}

	public void StopCoreLetter2()
	{
		coreLetter2Instance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
		coreLetter2Instance.release();
	}

	public void PlayerExtraLetter1()
	{
		coreLetter1Instance = FMODUnity.RuntimeManager.CreateInstance(extraLetter1);
		coreLetter1Instance.start();
	}

	public void StopExtraLetter1()
	{
		extraLetter1Instance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
		extraLetter1Instance.release();
	}

	public void PlayerExtraLetter2()
	{
		coreLetter2Instance = FMODUnity.RuntimeManager.CreateInstance(extraLetter2);
		coreLetter2Instance.start();
	}

	public void StopExtraLetter2()
	{
		extraLetter2Instance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
		extraLetter2Instance.release();
	}

	#endregion
}
