using UnityEngine;
using System.Collections;

public class ExitController : MonoBehaviour
{
    /// <summary>
    /// The number of objectives that the player must complete on this level to open this exit.
    /// </summary>
    public int ObjectivesRequired = 3;

    /// <summary>
    /// The number of objectives that the player has currently completed in this level.
    /// </summary>
    public int ObjectivesCompleted { get; private set; }

    /// <summary>
    /// Whether the player has completed enough objectives to open this exit.
    /// </summary>
    public bool IsOpen {
        get { return ObjectivesCompleted >= ObjectivesRequired; }
    }

    public delegate void ExitNotifier();
    /// <summary>
    /// Event that can be subscribed to to receive notification of the player leaving the level via this exit.
    /// </summary>
    public event ExitNotifier OnExit;

    private ParticleSystem m_particles;

	void Start ()
    {
        ObjectivesCompleted = 0;
        m_particles = transform.FindChild("Particle System").GetComponent<ParticleSystem>();
    }

    public void NewObjectiveComplete()
    {
        ++ObjectivesCompleted;

        //Check if the newly-completed objective causes the exit to open.
        if (IsOpen)
        {
            m_particles.Play();
        }
    }
	
	public void OnTriggerEnter(Collider other)
    {
        //Unless the exit is open and being driven into by the player, it has no effect.
        if (!IsOpen || other.tag != "Player")
        {
            return;
        }

        if (OnExit != null)
        {
            OnExit();
        }
    }
}
