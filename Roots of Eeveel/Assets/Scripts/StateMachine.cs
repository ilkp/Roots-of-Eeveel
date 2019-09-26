using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine : MonoBehaviour
{
    public enum State
    {
        StayStill,
        Wander,
        Investigate,
        AttackPlayer,
    }

    public State state;

    IEnumerator StayStillState()
    {
        Debug.Log("Crawl: Enter");
        while (state == State.StayStill)
        {
            yield return 0;
        }
        Debug.Log("Crawl: Exit");
        NextState();
    }

    IEnumerator WanderState()
    {
        Debug.Log("Walk: Enter");
        while (state == State.Wander)
        {
            yield return 0;
        }
        Debug.Log("Walk: Exit");
        NextState();
    }

    IEnumerator InvestigateState()
    {
        Debug.Log("Die: Enter");
        while (state == State.Investigate)
        {
            yield return 0;
        }
        Debug.Log("Die: Exit");
    }

    IEnumerator AttackPlayerState()
    {
        Debug.Log("Attack Player: Enter");
        while (state == State.AttackPlayer)
        {
            yield return 0;
        }
        Debug.Log("Attack Player: Exit");
    }

    void Start()
    {
        NextState();
    }

    void NextState()
    {
        string methodName = state.ToString() + "State";
        System.Reflection.MethodInfo info =
            GetType().GetMethod(methodName,
                                System.Reflection.BindingFlags.NonPublic |
                                System.Reflection.BindingFlags.Instance);
        StartCoroutine((IEnumerator)info.Invoke(this, null));
    }
}
