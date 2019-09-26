using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

public class StateMachine : MonoBehaviour
{
    public bool SoundHeard { get; set;}
    public Transform[] _route;
    public int _destination;
    public NavMeshAgent agent;

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
            // Change state to Investigate if sound is heard
            if (SoundHeard)
            {
                state = State.Investigate;
            }

            yield return 0;
        }
        Debug.Log("Crawl: Exit");
        NextState();
    }

    IEnumerator WanderState()
    {
        Debug.Log("Walk: Enter");
        agent.destination = _route[_destination].position;
        while (state == State.Wander)
        {
            // Change destination if at current destination

            if (agent.remainingDistance < 1)
            {
                if (_destination == _route.Length - 1)
                {
                    _destination = 0;
                }
                else
                {
                    _destination++;
                }

                agent.destination = _route[_destination].position;
            }
            
            // Head to destination

            // Change state to Investigate if sound is heard

            if (SoundHeard)
            {
                state = State.Investigate;
            }

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
            // Move to sound location

            // Change state to wander if can't "see" player

            // Change state to attack player if can "see" player

            // 

            yield return 0;
        }
        Debug.Log("Die: Exit");
    }

    IEnumerator AttackPlayerState()
    {
        Debug.Log("Attack Player: Enter");
        while (state == State.AttackPlayer)
        {
            // Use unity pathfinder to follow player and attack if close enough

            // Follow the player's last known location if player can't be seen. Possibly can be combined with just following the player

            // Change state to investigate if at player's last known location and player can't be seen.

            // Change state to wander if player is killed.

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
