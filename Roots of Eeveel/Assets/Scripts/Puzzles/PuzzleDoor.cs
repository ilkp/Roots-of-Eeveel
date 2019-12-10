using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleDoor : MonoBehaviour
{
    FMODUnity.StudioEventEmitter studioEventEmitter;
    [SerializeField] private PuzzleLock[] locks;
    public Interactable_Door2[] doors;

    private void Start()
    {
        studioEventEmitter = GetComponent<FMODUnity.StudioEventEmitter>();

        foreach (PuzzleLock pLock in locks)
        {
            pLock.door = this;
        }
    }

    public void checkLocks()
    {
        bool puzzleFailed = false;
        bool puzzleCompleted = true;
        for (int i = 0; i < locks.Length; i++)
        {
            // there is uncompleted lock before a solved one -> locks not solved in order
            if (!puzzleCompleted && locks[i].Solved)
            {
                puzzleFailed = true;
                break;
            }
            if (!locks[i].Solved)
            {
                puzzleCompleted = false;
            }
        }
        if (puzzleFailed)
        {
            foreach (PuzzleLock pLock in locks)
            {
                if (pLock.Solved)
                {
                    pLock.Unsolve();
                }
            }
        }
        else if (puzzleCompleted)
        {
            unlock();
        }
    }

    private void unlock()
    {
        //studioEventEmitter.Play();
        foreach (Interactable_Door2 door in doors)
        {
            door.Unlock();
        }
    }
}
