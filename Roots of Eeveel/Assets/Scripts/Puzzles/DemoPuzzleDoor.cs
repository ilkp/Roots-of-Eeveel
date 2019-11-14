
using UnityEngine;

/* Create a door with this component
 * Create a number of locks with the PuzzleLock component
 * Make sure the door and locks have the same puzzle identifier
 * Make sure the door knows all of the locks. Locks have to be in the wanted solving order.
 * Create keys with the Interactable_Key component
 */

public class DemoPuzzleDoor : MonoBehaviour
{
	private float doorOpeningForce = 7f;
	// The audio instance that playes the actual sounds and sound to be played
	private FMOD.Studio.EventInstance puzzleCompleteSoundInstance;
	[FMODUnity.EventRef] [SerializeField] private string puzzleCompleteSound;

	[SerializeField] private PuzzleLock[] locks;
	private ConfigurableJoint[] joints;

	private void Start()
	{
		foreach (PuzzleLock pLock in locks)
		{
			pLock.door = this;
		}

		//// Create the instance with given audiofile. only one instance, so only one sound at a time, if need for multiple, make more instances.
		puzzleCompleteSoundInstance = FMODUnity.RuntimeManager.CreateInstance(puzzleCompleteSound);
		//// Set the audio to be played from objects location, with RBs data, for some added effects?
		puzzleCompleteSoundInstance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));

		joints = GetComponentsInChildren<ConfigurableJoint>();
		foreach (ConfigurableJoint joint in joints)
		{
			joint.angularXMotion = ConfigurableJointMotion.Locked;
			joint.anchor = new Vector3(0, -0.5f, 0);
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
		puzzleCompleteSoundInstance.start();
		foreach (ConfigurableJoint joint in joints)
		{
			joint.angularXMotion = ConfigurableJointMotion.Limited;
			if (joint.GetComponent<Interactable_Door>().doorType < 2)
			{
				joint.gameObject.GetComponent<Rigidbody>().AddForce(joint.gameObject.transform.forward * doorOpeningForce, ForceMode.Impulse);
			}
		}
	}
}
