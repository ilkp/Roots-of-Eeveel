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
    [SerializeField] private bool _attacking;
    public bool _playerHit;

	/// <summary>
	/// Aggro number effects the movement speed and hearing sensitivity of the enemy.
	/// </summary>
	private int _aggro = 0;

    // Define possible enemy behaviour states
    public enum State
    {
		Dormant,
        StayStill,
        Patrol,
        Investigate,
        Chase,
        AttackPlayer
    }

    /// <summary>
	/// Current enemy behaviour state
	/// </summary>
	[Tooltip("Current enemy behaviour state")]
    [SerializeField] public State state;

    // Behaviour states

	// Start state where the enemy is not yet active. Waits for the first lock to be solved.
	IEnumerator DormantState()
	{
		while (state == State.Dormant)
		{
			state = State.Patrol;
			yield return 0;
		}
		NextState();
	}

    // State where the enemy stays still and listens to the environment
    IEnumerator StayStillState()
    {
        audioSettings.PlayEnemyIdle(gameObject);
        _anim.SetBool("moving", false);
        float waitTime = 5f;
        while (state == State.StayStill)
        {
            // Change state to Investigate if sound is heard
            if (_soundHeard)
            {
                _soundHeard = false;
                state = State.Investigate;
            }

			// Tick timer and go to patrol if it expires
			waitTime -= Time.deltaTime;
			if (waitTime <= 0)
			{
				state = State.Patrol;
			}

			yield return 0;
        }
        NextState();
    }

    // State where the enemy follows a predetermined route and listens to the environment
    IEnumerator PatrolState()
    {
        audioSettings.PlayEnemyFootStep(gameObject);
        _anim.SetBool("moving", true);
        _agent.destination = _route[_destination].position;
        while (state == State.Patrol)
        {
            // Change destination if at current destination
            if (_agent.remainingDistance < _agent.stoppingDistance)
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

            if (_agent.remainingDistance < _agent.stoppingDistance)
            {
                state = State.StayStill;
            }

            if (_soundHeard)
            {
                _soundHeard = false;
                _agent.destination = _soundLocation;
				state = State.Investigate;
            }

            if (_playerSoundHeard)
            {
                state = State.Chase;
            }


            yield return 0;
        }
        NextState();
    }

	// State where the enemy has heard the player
	IEnumerator ChaseState()
	{
		_anim.SetBool("moving", true);
		_playerSoundHeard = false;
		_agent.destination = _soundLocation;
		while (state == State.Chase)
		{
			if (_playerSoundHeard)
			{
				_agent.destination = _soundLocation;
			}

			if (_agent.remainingDistance < _agent.stoppingDistance)
			{
				state = State.AttackPlayer;
			}

			yield return 0;
		}
		NextState();
	}

    // State where the enemy has found the player and is following them
    IEnumerator AttackPlayerState()
    {
        _anim.SetBool("moving", false);
		_anim.SetBool("attacking", true);
        _playerSoundHeard = false;
		_soundHeard = false;
        while (state == State.AttackPlayer)
        {
            if (_anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f) // animation has ended
            {
                if (_playerHit) // player was hit and player still should be in range
                {
                    _anim.SetBool("attackingAgain", true);
                    // hit again maybe the other animation here
                }
                else
                {
                    // resume moving if needed

                    // Disable Attacking
                    _anim.SetBool("attacking", false);
                    _anim.SetBool("attackingAgain", false);
                    _attacking = false;
					state = State.Investigate;
                }
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
