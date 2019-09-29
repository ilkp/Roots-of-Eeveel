using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    /// <summary>
	/// Boolean to indicate if the enemy has heard an alarming sound
	/// </summary>
	[Tooltip("Boolean to indicate if the enemy has heard an alarming sound")]
    [SerializeField] private bool _soundHeard = false;

    /// <summary>
	/// Boolean to indicate if the enemy has heard a player unique sound
	/// </summary>
	[Tooltip("Boolean to indicate if the enemy has heard a player unique sound")]
    [SerializeField] private bool _playerSoundHeard = false;

    /// <summary>
	/// Array to store the route of the enemy
	/// </summary>
	[Tooltip("Array to store the route of the enemy")]
    [SerializeField] private Transform[] _route;

    /// <summary>
	/// Number value to indicate the current destination from the route list
	/// </summary>
	[Tooltip("Number value to indicate the current destination index from the route list")]
    [SerializeField] private int _destination;

    /// <summary>
	/// Unity navmesh agent component of the enemy
	/// </summary>
	[Tooltip("Unity navmesh agent component of the enemy")]
    [SerializeField] private NavMeshAgent _agent;

    /// <summary>
    /// Player object
    /// </summary>
    [Tooltip("Player object")]
    [SerializeField] private Transform _player;

    /// <summary>
	/// Location of heard sound
	/// </summary>
	[Tooltip("Location of heard sound")]
    [SerializeField] private Vector3 _soundLocation;

    // Define possible enemy behaviour states
    public enum State
    {
        StayStill,
        Wander,
        Investigate,
        LookAround,
        AttackPlayer,
    }

    /// <summary>
	/// Current enemy behaviour state
	/// </summary>
	[Tooltip("Current enemy behaviour state")]
    [SerializeField] private State state;

    // Behaviour states

    // State where the enemy stays still and listens to the environment
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

    // State where the enemy follows a predetermined route and listens to the environment
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

                Debug.Log("Destination Reached");
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

    // State where the enemy has heard an anomylous sound and is investigating the location from where the sound came from
    IEnumerator InvestigateState()
    {
        Debug.Log("Investigate: Enter");
        _soundHeard = false;
        _agent.destination = _soundLocation;
        while (state == State.Investigate)
        {
            
            if (_agent.remainingDistance < 3)
            {
                Debug.Log("Close enough");
                state = State.LookAround;
            }

            if (_soundHeard)
            {
                _soundHeard = false;
                _agent.destination = _soundLocation;
            }

            if (_playerSoundHeard)
            {
                state = State.AttackPlayer;
            }


            yield return 0;
        }
        Debug.Log("Investigate: Exit");
        NextState();
    }

    IEnumerator LookAroundState()
    {
        Debug.Log("Looking Around: Enter");
        double timer = 0;
        _agent.isStopped = true;
        while (state == State.LookAround)
        {

            if (_soundHeard)
            {
                state = State.Investigate;
            }

            if (_playerSoundHeard)
            {
                state = State.AttackPlayer;
            }

            if (timer >= 5)
            {
                state = State.Wander;
            }
            else
            {
                timer += Time.deltaTime;
            }

            yield return 0;
        }
        Debug.Log("Looking Around: Exit");
        _agent.isStopped = false;
        NextState();
    }

    // State where the enemy has found the player and is following them
    IEnumerator AttackPlayerState()
    {
        Debug.Log("Attack Player: Enter");
        //RaycastHit hit;
        while (state == State.AttackPlayer)
        {
            // Use unity pathfinder to follow player and attack if close enough
            //Physics.Raycast(transform.position, Vector3.Normalize(_player.position - transform.position), out hit);
            //if (hit.collider.CompareTag("Player"))
            //{
            //    _agent.destination = _player.position;
            //}

            if (_soundHeard)
            {
                _agent.destination = _soundLocation;
            }

            if (Vector3.Distance(transform.position, _player.position) < 1)
            {
                // Make player die here
            }

            // Change state to investigate if at player's last known location and player can't be seen.

            if (_agent.remainingDistance < 2)
            {
                state = State.Wander;
            }

            yield return 0;
        }
        Debug.Log("Attack Player: Exit");
        NextState();
    }

    // Set the first behaviour state when the game starts
    void Start()
    {
        NextState();
    }

    // Method to change the behaviour state
    void NextState()
    {
        string methodName = state.ToString() + "State";
        System.Reflection.MethodInfo info =
            GetType().GetMethod(methodName,
                                System.Reflection.BindingFlags.NonPublic |
                                System.Reflection.BindingFlags.Instance);
        StartCoroutine((IEnumerator)info.Invoke(this, null));
    }

    // When a sound is heard 
    public void alert(Vector3 source)
    {
        _soundHeard = true;
        _soundLocation = source;
    }

    // If enemy comes in contact with the player, the player is killed
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            collision.collider.GetComponent<PlayerMovement>().Die();
        }
    }
}
