using UnityEngine;
using System.Collections;

public class SelfDestruct : MonoBehaviour
{
    public float Lifetime;
    private float m_startTime;

	// Use this for initialization
	void Start ()
    {
        m_startTime = Time.time;
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (Time.time > m_startTime + Lifetime)
        {
            Destroy(gameObject);
        }
	
	}
}
