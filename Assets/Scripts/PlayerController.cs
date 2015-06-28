using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using Pathfinding;

public class PlayerController : Tank, IBeginDragHandler, IDragHandler, IEndDragHandler
{
	public PathPlanner pathPlanner;
	public float NextWaypointDistance;

	private GameManager gameManager;
	private float trail_fade_time;
	private Vector3[] currentPath;
	private int currentWaypoint;
	private Seeker seeker;
    
	void Awake ()
	{
		gameManager = GameManager.Instance;
		gameManager.NotifyStateChange += OnStateChange;
	}

	void Start ()
	{
		trail_fade_time = GetComponentInChildren<TrailRenderer> ().time;
		seeker = GetComponent<Seeker> ();
	}

	void OnDestroy ()
	{
		gameManager.NotifyStateChange -= OnStateChange;
	}

	void Update ()
	{
		if (gameManager.gameState != GameState.Moving) {
			return;
		}

		//If we've run out of path, stop.
		if (currentWaypoint >= currentPath.Length) {
			pathPlanner.ClearWaypoints ();
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

	void OnStateChange (GameState old_state, GameState new_state)
	{
		switch (new_state) {
		case GameState.Moving:
			GetComponentInChildren<TrailRenderer> ().time = trail_fade_time; //Resume trail renderer fade-out
			currentPath = pathPlanner.Path;
			currentWaypoint = 1;
			break;


		case GameState.Planning:
			GetComponentInChildren<TrailRenderer> ().time = Mathf.Infinity; //Pause trail renderer fade-out
			break;
		}
	}

	public void OnBeginDrag (PointerEventData eventData)
	{
		if (gameManager.gameState != GameState.Planning) {
			return;
		}
		
		pathPlanner.ClearWaypoints ();
		pathPlanner.AddWaypoint (transform.position);
	}
	
	public void OnDrag (PointerEventData eventData)
	{
		if (gameManager.gameState != GameState.Planning) {
			return;
		}
		
		Vector3 current_world_pos = Camera.main.ScreenToWorldPoint (new Vector3 (eventData.position.x, eventData.position.y, Camera.main.transform.position.y));
		Vector3 last_world_pos = pathPlanner.GetLastPoint ();
		Vector3 difference = current_world_pos - last_world_pos;
		
		RaycastHit hit_info;
		LayerMask layer_mask = LayerMask.GetMask ("Walls");

		if (Physics.CheckSphere (current_world_pos, 0.75f, layer_mask)) {
			//Endpoints inside walls should turn red and not extend the path.
			pathPlanner.UpdateFinalRenderPoint (current_world_pos, false);
		} else if (Physics.SphereCast ((last_world_pos), 0.75f, difference, out hit_info, difference.magnitude, layer_mask)) {
			//Paths that intersect walls but finish in a valid location should be corrected with pathfinding.

			pathPlanner.UpdateFinalRenderPoint (current_world_pos, false); // While we're pathfinding, show the point as invalid.
			if (seeker.IsDone ()) {
				seeker.StartPath (last_world_pos, current_world_pos, OnPathComplete);
			}
		} else if (difference.magnitude < 1) {
			//A short distance isn't enough to set a new waypoint
			pathPlanner.UpdateFinalRenderPoint (current_world_pos);
		} else {
			//A long enough distance registers a new waypoint.
			pathPlanner.AddWaypoint (current_world_pos);
		}
	}
	
	public void OnEndDrag (PointerEventData eventData)
	{
		if (gameManager.gameState != GameState.Planning) {
			return;
		}
		
		Vector3 current_world_pos = Camera.main.ScreenToWorldPoint (new Vector3 (eventData.position.x, eventData.position.y, Camera.main.transform.position.y));
		Vector3 last_world_pos = pathPlanner.GetLastPoint ();
		Vector3 difference = current_world_pos - last_world_pos;
		
		RaycastHit hit_info;
		LayerMask layer_mask = LayerMask.GetMask ("Walls");
		if (Physics.SphereCast (last_world_pos, 0.75f, difference, out hit_info, difference.magnitude, layer_mask)) {
			//If the final point leads the path through a wall, ignore it and remove it from the renderer.
			pathPlanner.ClearFinalRenderPoint ();
		} else {
			//Otherwise, add it to the path.
			pathPlanner.AddWaypoint (current_world_pos);
		}
	}

	public void OnPathComplete (Path path)
	{
		if (path.error) {
			Debug.Log ("Path error: " + path.errorLog);
			return;
		}
		
		foreach (Vector3 waypoint in path.vectorPath) {
			pathPlanner.AddWaypoint (waypoint);
		}
	}
}
