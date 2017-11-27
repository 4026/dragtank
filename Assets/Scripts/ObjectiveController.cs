using UnityEngine;
using System.Collections;

public class ObjectiveController : MonoBehaviour
{
    public Vector3 AngularVelocity;
    public Vector3 AngularVelocityAfterCollection;

    private bool m_collected = false;
    private bool m_arrivedAtExit = false;

    private Transform m_cube;
    private ParticleSystem m_explosion;
    private ParticleSystem m_trail;

    private GameObject m_exit;

    void Start()
    {
        m_cube = transform.Find("Cube");
        m_trail = transform.Find("Trail").GetComponent<ParticleSystem>();
        m_explosion = transform.Find("Explosion").GetComponent<ParticleSystem>();

        m_exit = GameObject.FindGameObjectWithTag("Exit");
    }

	void Update ()
    {
        if (!m_arrivedAtExit)
        {
            //As long as the objective cube hasn't actually reached the exit yet, continue to rotate the cube.
            m_cube.Rotate((m_collected ? AngularVelocityAfterCollection : AngularVelocity) * Time.deltaTime);
        }
        else if (m_trail.particleCount == 0)
        {
            //If we're at the exit, and all the particles in the trail have expired, our work here is done.
            Destroy(gameObject);
        }
	}

    void OnTriggerEnter(Collider other)
    {
        if (other.tag != "Player" || m_collected)
        {
            return;
        }

        m_collected = true;

        //Start particles
        m_explosion.Play();
        m_trail.Play();

        //Rise up in the air before moving to the exit.
        Hashtable tween_options = iTween.Hash(
            "y", 2.0f,
            "oncomplete", "MoveToExit"
        );
        iTween.MoveAdd(gameObject, tween_options);
    }

    public void MoveToExit()
    {
        Hashtable tween_options = iTween.Hash(
            "position", m_exit.transform.position + new Vector3(0, 2.0f, 0),
            "speed", 20.0f,
            "easetype", iTween.EaseType.easeInCubic,
            "oncomplete", "OnArriveAtExit"
        );
        iTween.MoveTo(gameObject, tween_options);
    }

    public void OnArriveAtExit()
    {
        m_arrivedAtExit = true;
        m_cube.gameObject.SetActive(false);

        m_exit.GetComponent<ExitController>().NewObjectiveComplete();
    }
}
