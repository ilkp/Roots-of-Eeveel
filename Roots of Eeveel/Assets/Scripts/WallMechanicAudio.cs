using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(FMODUnity.StudioEventEmitter))]
public class WallMechanicAudio : MonoBehaviour
{
    [SerializeField] private AudioSettings audioSettings;
    FMODUnity.StudioEventEmitter studioEventEmitter;
    public float Interval = 1;
    public float IntervalSpread = 0.5f;
    private float timeTillSound = 0;

    private void Start()
    {
        studioEventEmitter = GetComponent<FMODUnity.StudioEventEmitter>();
    }

    private void Update()
    {
        timeTillSound -= Time.deltaTime;

        if (timeTillSound < 0)
        {
            float value = Random.value;

            if (value > 0.66f)
            {
                studioEventEmitter.Event = audioSettings.roomMechanics;
            }
            else if (value > 0.33f)
            {
                studioEventEmitter.Event = audioSettings.roomMechanics1;
            }
            else
            {
                studioEventEmitter.Event = audioSettings.roomMechanics2;
            }

            studioEventEmitter.Play();


            timeTillSound = Interval + IntervalSpread * Random.value * ((Random.value > 0.5f) ? 1 : -1);
        }

    }
}
