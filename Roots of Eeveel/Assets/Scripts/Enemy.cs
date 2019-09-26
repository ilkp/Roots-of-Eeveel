using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    Material defaultMaterial;
    public Material alertMaterial;

    public bool _soundHeard = false;
    public Transform[] _route;
    public int _destination;
    public NavMeshAgent _agent;

    private Vector3 _soundLocation;

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
        Debug.Log("Stay Still: Enter");
        while (state == State.StayStill)
        {
            // Change state to Investigate if sound is heard
            if (_soundHeard)
            {
                state = State.Investigate;
            }

            yield return 0;
        }
        Debug.Log("Stay Still: Exit");
        NextState();
    }

    IEnumerator WanderState()
    {
        Debug.Log("Wander: Enter");
        _agent.destination = _route[_destination].position;
        while (state == State.Wander)
        {
            // Change destination if at current destination

            if (_agent.remainingDistance < 1)
            {
                if (_destination == _route.Length - 1)
                {
                    _destination = 0;
                }
                else
                {
                    _destination++;
                }

                _agent.destination = _route[_destination].position;
            }

            // Head to destination

            // Change state to Investigate if sound is heard

            if (_soundHeard)
            {
                state = State.Investigate;
            }

            yield return 0;
        }
        Debug.Log("Wander: Exit");
        NextState();
    }

    IEnumerator InvestigateState()
    {
        Debug.Log("Investigate: Enter");
        _soundHeard = false;
        _agent.destination = _soundLocation;
        while (state == State.Investigate)
        {
            if (_agent.remainingDistance < 2)
            {
                Debug.Log("Close enough");
                state = State.Wander;
            }

            yield return 0;
        }
        Debug.Log("Investigate: Exit");
        NextState();
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
        NextState();
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

    public void alert(Vector3 source)
    {
        GetComponent<MeshRenderer>().material = alertMaterial;
        _soundHeard = true;
        _soundLocation = source;
    }
}
