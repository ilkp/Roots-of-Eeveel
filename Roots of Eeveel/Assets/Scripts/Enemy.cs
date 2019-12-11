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
    /// Location of last heard sound
    /// </summary>
    [Tooltip("Location of heard sound")]
    [SerializeField] private Vector3 _lastSoundLocation;

    /// <summary>
    /// Minimum movement speed of the enemy
    /// </summary>
    [Tooltip("Minimum movement speed of the enemy")]
    [SerializeField] private float[] _moveSpeeds;

    /// <summary>
    /// Color of enemy emission
    /// </summary>
    [Tooltip("Color of enemy emission")]
    [SerializeField] private Color _emissionColor;

    /// <summary>
    /// The intensity of emission depending on state
    /// </summary>
    [Tooltip("The intensity of emission depending on state")]
    [SerializeField] private float[] _emissionIntensities;

    /// <summary>
    /// Enemy Body Renderer
    /// </summary>
    [Tooltip("Enemy body renderer")]
    [SerializeField] private Renderer _bodyRend;

    /// <summary>
    /// Enemy Root Renderer
    /// </summary>
    [Tooltip("Enemy root renderer")]
    [SerializeField] private Renderer _rootRend;

    /// <summary>
    /// Animation Controller
    /// </summary>
    [Tooltip("Animation controller of the enemy")]
    [SerializeField] private Animator _anim;

    /// <summary>
    /// Attack Collider
    /// </summary>
    [Tooltip("Attack Collider")]
    [SerializeField] public BoxCollider _hand;

    /// <summary>
    /// A boolean to check if the enemy can lunge again yet.
    /// </summary>
    [Tooltip("A boolean to check if the enemy can lunge again yet.")]
    [SerializeField] private bool _attacking;
    public bool _playerHit;

    [Tooltip("Puzzle condition which when solved wakes up the enemy. Requires component with IPuzzleCondition")]
    [SerializeField] private PuzzleLock wakeUpTrigger;

    /// <summary>
    /// Aggro number effects the movement speed and hearing sensitivity of the enemy.
    /// </summary>
    private int _aggro = 0;

    [Tooltip("How long steps the enemy takes. Sound related.")]
    [SerializeField] private float stepLength = 1;

    private const float seeRangeShort = 3f;
    private const float seeRangeLong = seeRangeShort * 2f;

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
        wakeUpTrigger.ConditionMet += WakeUp;
        _anim.speed = 0.01f;
        do
        {
            yield return 0;
        } while (state == State.Dormant);
        _anim.speed = 1f;
        _playerSoundHeard = false;
        _soundHeard = false;
        NextState();
    }

    IEnumerator WalkSound()
    {
        Vector3 lastPosition = transform.position;
        float distanceFromLastSound = 0;
        //Debug.Log("EnemyWalkSoundStart");

        while (true)
        {
            distanceFromLastSound += Vector3.Distance(lastPosition, transform.position);
            lastPosition = transform.position;

            //Debug.Log("EnemyWalkSoundLoop");

            if (distanceFromLastSound >= stepLength)
            {
                //Debug.Log("EnemyWalkSoundLoop if:" + distanceFromLastSound);
                distanceFromLastSound = 0;
                StartCoroutine(audioSettings.PlayEnemyFootStep(this.gameObject));
            }

            yield return null;
        }
    }

    // State where the enemy stays still and listens to the environment
    IEnumerator StayStillState()
    {
        audioSettings.PlayEnemyIdle(gameObject);
        audioSettings.SetEnemyState(gameObject, 0);
        _anim.SetFloat("forward", _agent.speed);
        float waitTime = 5f;
        do
        {
            // Change state to Investigate if sound is heard
            if (_soundHeard)
            {
                Debug.Log((_lastSoundLocation - _soundLocation).magnitude);
                _soundHeard = false;
                if ((_lastSoundLocation - _soundLocation).magnitude > 2)
                {
                    state = State.Investigate;
                }
            }

            // If the player comes too close and is infront of enemy, start chasing
            Vector3 enemyToPlayer = _player.position - transform.position;
            if (enemyToPlayer.magnitude <= seeRangeShort &&
                Vector3.Angle(transform.forward, enemyToPlayer) < 45)
            {
                state = State.Chase;
            }

            // Tick timer and go to patrol if it expires
            waitTime -= Time.deltaTime;
            if (waitTime <= 0)
            {
                state = State.Patrol;
            }

            yield return 0;
        } while (state == State.StayStill);
        NextState();
    }

    // State where the enemy follows a predetermined route and listens to the environment
    IEnumerator PatrolState()
    {
        audioSettings.PlayEnemyFootStep(gameObject);
        audioSettings.SetEnemyState(gameObject, 0);
        _anim.SetFloat("forward", _agent.speed);
        _agent.destination = _route[_destination].position;
        do
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

            // If the player comes too close and is infront of enemy, start chasing
            Vector3 enemyToPlayer = _player.position - transform.position;
            if (enemyToPlayer.magnitude < seeRangeShort &&
                Vector3.Angle(transform.forward, enemyToPlayer) < 45)
            {
                state = State.Chase;
            }

            // When in light, stop for a moment
            // if (_route.Length <= 0)
            // {
            //     _agent.destination = _route[0].position;
            //     state = State.StayStill;
            // }

            yield return 0;
        } while (state == State.Patrol);
        NextState();
    }

    // State where the enemy has heard an anomylous sound and is investigating the location from where the sound came from
    IEnumerator InvestigateState()
    {
        audioSettings.PlayEnemyFootStep(gameObject);
        audioSettings.SetEnemyState(gameObject, 1);
        _agent.destination = _soundLocation;
        _anim.SetFloat("forward", _agent.speed);
        do
        {
            if (_playerSoundHeard || (_player.position - gameObject.transform.position).magnitude <= seeRangeLong)
            {
                state = State.Chase;
            }
            else if (_soundHeard)
            {
                _soundHeard = false;
                _agent.destination = _soundLocation;
                Debug.Log(_agent.remainingDistance);
            }
            else if (_agent.remainingDistance <= _agent.stoppingDistance)
            {
                state = State.StayStill;
            }
            yield return 0;
        } while (state == State.Investigate);
        NextState();
    }

    // State where the enemy has heard the player
    IEnumerator ChaseState()
    {
        //StartCoroutine(audioSettings.PlayEnemyNoticeSound());
        audioSettings.SetEnemyState(gameObject, 2);
        audioSettings.AddEnemyToChase(this.gameObject);
        _anim.SetFloat("forward", _agent.speed);
        _playerSoundHeard = false;
        _agent.destination = _soundLocation;
        float playerDistance;
        do
        {
            playerDistance = PlayerHorizontalDistance();
            if (playerDistance <= seeRangeLong)
            {
                if (playerDistance <= _agent.stoppingDistance)
                {
                    state = State.AttackPlayer;
                }
                else
                {
                    _agent.destination = _player.position;
                }
            }
            else if (_playerSoundHeard)
            {
                _agent.destination = _soundLocation;
                _playerSoundHeard = false;
            }
            else if (_agent.remainingDistance <= _agent.stoppingDistance)
            {
                _playerSoundHeard = false;
                _soundHeard = false;
                state = State.Investigate;
            }
            yield return 0;
        } while (state == State.Chase);
        audioSettings.RemoveEnemyFromChase(this.gameObject);
        NextState();
    }

    // A state for hitting the player (Or the air)
    IEnumerator AttackPlayerState()
    {
        audioSettings.AddEnemyToChase(this.gameObject);
        audioSettings.SetEnemyState(gameObject, 2);
        StartCoroutine(audioSettings.PlayEnemyAttackSound(gameObject));
        _anim.SetFloat("forward", 0f);
        _anim.SetBool("attacking", true);
        _anim.speed = 1.5f;
        _playerSoundHeard = false;
        _soundHeard = false;
        _agent.isStopped = true;
        float playerDistance;
        do
        {
            // Check if the current animation playing is an attack
            if (_anim.GetCurrentAnimatorStateInfo(0).IsTag("attack"))
            {
                if (!_playerHit && _anim.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.2f)
                {
                    _playerHit = false;
                }
                // Enable the hand collider if the hand is raised.
                if (_anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.2f &&
                    _anim.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.3f &&
                    !_hand.enabled)
                {
                    _hand.enabled = true;
                }
                // Check if the attack animation has ended
                if (_anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.9f)
                {
                    playerDistance = PlayerHorizontalDistance();
                    // Hit again if player should still be close enough.
                    if (_playerHit && playerDistance <= _agent.stoppingDistance) // player was hit and player still should be in range
                    {
                        _anim.SetBool("attackingAgain", true);
                    }
                    else
                    {
                        // Disable Attacking
                        _anim.SetBool("attacking", false);
                        _anim.SetBool("attackingAgain", false);
                        _hand.enabled = false;

                        // Go to chase if player is in seeRange, otherwise to investigate
                        state = playerDistance <= seeRangeLong ? State.Chase : State.Investigate;
                    }
                }
            }

            yield return 0;
        } while (state == State.AttackPlayer);
        _anim.speed = 1.0f;
        _agent.isStopped = false;
        audioSettings.RemoveEnemyFromChase(this.gameObject);
        NextState();
    }


    // Set the first behaviour state when the game starts
    void Start()
    {
        NextState();
        audioSettings.PlayEnemyState(gameObject);
        StartCoroutine(WalkSound());
        _agent.autoRepath = true;
    }

    // Method to change the behaviour state
    void NextState()
    {
        _agent.speed = _moveSpeeds[(int)state];
        audioSettings.StopEnemySound(gameObject);
        Color newColor = Color.red;
        _bodyRend.material.SetVector("_EmissiveColor", _emissionColor * _emissionIntensities[(int)state]);
        _rootRend.material.SetVector("_EmissiveColor", _emissionColor * _emissionIntensities[(int)state]);
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
        _lastSoundLocation = _soundLocation;
        _soundLocation = source;
        _playerSoundHeard = isPlayer;
    }

    // Used to wake enemy from dormant state
    public void WakeUp(object sender, System.EventArgs args)
    {
        // Maybe some scary sound?

        // Change state
        state = State.Patrol;

        // Unsubscribe from lock
        wakeUpTrigger.ConditionMet -= WakeUp;
    }

    private float PlayerHorizontalDistance()
    {
        return Vector3.Distance(new Vector3(_player.transform.position.x, 0f, _player.transform.position.z),
            new Vector3(transform.position.x, 0, transform.position.z));
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (state == State.Dormant)
        {
            wakeUpTrigger.ConditionMet -= WakeUp;
            state = State.Patrol;
        }
    }
}
