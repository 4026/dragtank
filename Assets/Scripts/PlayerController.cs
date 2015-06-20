using UnityEngine;
using System.Collections;

public class PlayerController : Tank
{
	public PathPlanner pathPlanner;
	public float NextWaypointDistance;

	private GameManager gameManager;
	private float trail_fade_time;
	private Vector3[] currentPath;
	private int currentWaypoint;
    
	void Awake ()
	{
		gameManager = GameManager.Instance;
		gameManager.NotifyStateChange += OnStateChange;
	}

	void Start ()
	{
		trail_fade_time = GetComponentInChildren<TrailRenderer> ().time;
	}

	void Update ()
	{
		if (gameManager.gameState != GameState.Moving) {
			return;
		}

		//If we've run out of path, stop.
		if (currentWaypoint >= currentPath.Length) {
			pathPlanner.dragged_path.Clear ();
			gameManager.SetGameState (GameState.Planning);
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

	void OnDestroy ()
	{
		gameManager.NotifyStateChange -= OnStateChange;
	}

	void OnStateChange (GameState old_state, GameState new_state)
	{
		switch (new_state) {
		case GameState.Moving:
			GetComponentInChildren<TrailRenderer> ().time = trail_fade_time; //Resume trail renderer fade-out
			currentPath = pathPlanner.dragged_path.ToArray ();
			currentWaypoint = 0;
			break;


		case GameState.Planning:
			GetComponentInChildren<TrailRenderer> ().time = Mathf.Infinity; //Pause trail renderer fade-out
			break;
		}
	}
}
