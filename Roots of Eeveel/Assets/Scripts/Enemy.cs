using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [SerializeField] private AudioSettings audioSettings;

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

    /// <summary>
    /// Minimum movement speed of the enemy
    /// </summary>
    [Tooltip("Minimum movement speed of the enemy")]
    [SerializeField] private float[] _moveSpeeds;

    /// <summary>
    /// Animation Controller
    /// </summary>
    [Tooltip("Animation controller of the enemy")]
    [SerializeField] private Animator _anim;

    /// <summary>
    /// A boolean to check if the enemy can lunge again yet.
    /// </summary>
    [Tooltip("A boolean to check if the enemy can lunge again yet.")]
    [SerializeField] private bool _canLunge;

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
    [SerializeField] public State state;

    // Behaviour states

    // State where the enemy stays still and listens to the environment
    IEnumerator StayStillState()
    {
        audioSettings.PlayEnemyIdle(gameObject);
        _anim.SetBool("moving", false);
        //float waitTime = 0;
        while (state == State.StayStill)
        {
            // Change state to Investigate if sound is heard
            if (_soundHeard)
            {
                _soundHeard = false;
                state = State.Investigate;
            }

            // if (waitTime <= 0)
            // {
            //     state = State.Investigate;
            // }
            // else
            // {
            //     waitTime -= Time.deltaTime;
            // }

            yield return 0;
        }
        NextState();
    }

    // State where the enemy follows a predetermined route and listens to the environment
    IEnumerator WanderState()
    {
        audioSettings.PlayEnemyFootStep(gameObject);
        _anim.SetBool("moving", true);
        _agent.destination = _route[_destination].position;
        while (state == State.Wander)
        {
            // Change destination if at current destination

            if (_agent.remainingDistance < 1)
            {
                if (_destination >= _route.Length - 1)
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

            // Change state to Investigate if a sound is heard
            if (_soundHeard)
            {
                _soundHeard = false;
                state = State.Investigate;
            }

            // When in light, stop for a moment
            // if (_route.Length <= 0)
            // {
            //     _agent.destination = _route[0].position;
            //     state = State.StayStill;
            // }

            yield return 0;
        }
        NextState();
    }

    // State where the enemy has heard an anomylous sound and is investigating the location from where the sound came from
    IEnumerator InvestigateState()
    {
        audioSettings.PlayEnemyFootStep(gameObject);
        _agent.destination = _soundLocation;
        _anim.SetBool("moving", true);
        while (state == State.Investigate)
        {

            if (_agent.remainingDistance < 3)
            {
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
        NextState();
    }

    // State where the enemy stays still for a while and "looks around them"
    IEnumerator LookAroundState()
    {
        audioSettings.PlayEnemyIdle(gameObject);
        double timer = 0;
        _anim.SetBool("moving", false);
        while (state == State.LookAround)
        {

            if (_soundHeard)
            {
                _soundHeard = false;
                if (_playerSoundHeard)
                {
                    state = State.AttackPlayer;
                }
                else
                {
                    state = State.Investigate;
                }
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
        NextState();
    }

    // State where the enemy has found the player and is following them
    IEnumerator AttackPlayerState()
    {
        _anim.SetBool("moving", true);
        _playerSoundHeard = false;
        _canLunge = true;
        while (state == State.AttackPlayer)
        {

            if (_soundHeard && _playerSoundHeard)
            {
                _soundHeard = false;
                _agent.destination = _soundLocation;
            }

            // Apparently something something new attacks or something

            // if (_agent.remainingDistance < 5 && _canLunge)
            // {
            //     Debug.Log("Boop");
            //     _canLunge = false;
            //     // Stop the enemy for a moment to wind up the lunge
            //     _agent.speed = 0;
            //     // Set lunge animation active here
            //     // Wait for however long the lunge animation winds up
            //     yield return new WaitForSeconds(.5f);
            //     //Set speed to something that feels good
            //     _agent.speed = _moveSpeeds[(int)state];
            //     Debug.Log("Gasp");
            //     yield return new WaitWhile(() => _agent.remainingDistance > .5);
            //     Debug.Log("Bonk");
            // }
            // else 

            // Change state to investigate if at player's last known location and player can't be seen.

            if (_agent.remainingDistance < .5)
            {
                state = State.LookAround;
            }

            yield return 0;
        }
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
        _agent.speed = _moveSpeeds[(int)state];
        audioSettings.StopEnemySound(gameObject);
        string methodName = state.ToString() + "State";
        System.Reflection.MethodInfo info =
            GetType().GetMethod(methodName,
                                System.Reflection.BindingFlags.NonPublic |
                                System.Reflection.BindingFlags.Instance);
        StartCoroutine((IEnumerator)info.Invoke(this, null));
    }

    // Set appropriate booleans to whatever when a sound is heard
    public void alert(Vector3 source, bool isPlayer)
    {
        _soundHeard = true;
        _soundLocation = source;
        _playerSoundHeard = isPlayer;
    }

    // The player is hurt if an enemy comes in contact with the player
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player") && state == State.AttackPlayer)
        {
            collision.collider.GetComponent<PlayerMovement>().GetHurt();
        }
    }
}
