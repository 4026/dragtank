using UnityEngine;
using Pathfinding;
using UnityEngine.EventSystems;
using System;

public class PlayerController : Tank
{
	public float NextWaypointDistance;

    private PathPlanner m_pathPlanner;
    private GameManager m_gameManager;
	private float trail_fade_time;
	private Vector3[] currentPath;
	private int currentWaypoint;
	private Seeker seeker;
   
	void Start ()
	{
        m_pathPlanner = PathPlanner.Instance;

        m_gameManager = GameManager.Instance;
        m_gameManager.NotifyStateChange += OnStateChange;
        m_gameManager.OnPlayerSpawn(gameObject);

        trail_fade_time = GetComponentInChildren<TrailRenderer> ().time;
		seeker = GetComponent<Seeker> ();
	}

	void OnDestroy ()
	{
		m_gameManager.NotifyStateChange -= OnStateChange;
	}

	void Update ()
	{
		if (m_gameManager.State != GameManager.GameState.Moving) {
			return;
		}

		//If we've run out of path, stop.
		if (currentWaypoint >= currentPath.Length) {
			m_pathPlanner.ClearWaypoints ();
			m_gameManager.State = GameManager.GameState.PlanCountdown;
			return;
		}
		
		//Get the direction to the next waypoint on the current path.
		Vector3 waypoint = currentPath [currentWaypoint];
		waypoint.y = 0; //Confine movement to the horizontal plane.
		MoveToward (waypoint);
		
		//Check if we are close enough to the next waypoint
		//If we are, proceed to follow the next waypoint
		if (Vector3.Distance (transform.position, waypoint) < NextWaypointDistance) {
			currentWaypoint++;
			return;
		}
	}

	void OnStateChange (GameManager.GameState old_state, GameManager.GameState new_state)
	{
		switch (new_state) {
		    case GameManager.GameState.Moving:
			    GetComponentInChildren<TrailRenderer> ().time = trail_fade_time; //Resume trail renderer fade-out
			    currentPath = m_pathPlanner.Path;
			    currentWaypoint = 1;
			    break;


		    case GameManager.GameState.Planning:
			    GetComponentInChildren<TrailRenderer> ().time = Mathf.Infinity; //Pause trail renderer fade-out
			    break;
		}
	}
}
