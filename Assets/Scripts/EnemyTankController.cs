using UnityEngine;
using System.Collections;
using Pathfinding;
using System.Linq;

public class EnemyTankController : MonoBehaviour
{
	public float MoveSpeed;
	public float TurnSpeed;
	public float nextWaypointDistance;

	private GameManager gameManager;
	private EnemyTurretController turret;

	private GameObject player;                  //The player object to hunt.
	private Vector3 lastKnownPlayerPos;

	private Seeker seeker;
	private Path currentPath;                  //The chaser's current path
	private int currentWaypoint = 0;            //The waypoint we are currently moving towards

	enum AIState
	{
		Dormant,
		Attacking,
		Chasing
	}
	private AIState currentAIState = AIState.Dormant;
    
	void Awake ()
	{
		gameManager = GameManager.Instance;
	}

	void Start ()
	{
		player = GameObject.FindGameObjectWithTag ("Player");
		turret = GetComponentInChildren<EnemyTurretController> ();
		seeker = GetComponent<Seeker> ();
	}

	public void OnPathComplete (Path path)
	{
		if (path.error) {
			Debug.Log ("Path error: " + path.errorLog);
			return;
		}
		
		currentPath = path;
		currentWaypoint = 0; //Reset the waypoint counter
	}
	
	void Update ()
	{
		if (gameManager.gameState != GameState.Moving) {
			return;
		}

		//Set state
		if (turret.IsPlayerInVision ()) {
			Debug.Log ("There they are! Get them!");
			lastKnownPlayerPos = player.transform.position;
			setAIState (AIState.Attacking);
		} else if (currentAIState != AIState.Dormant) {
			Debug.Log ("We've lost them! Get after them!");
			setAIState (AIState.Chasing);
		}

		//Act on state
		if (currentAIState == AIState.Chasing) {
			//Chase toward the player's last known position.

			//If we have no path to move along yet, wait until we do.
			if (currentPath == null) {
				return;
			}

			//If we've run out of path, stop.
			if (currentWaypoint >= currentPath.vectorPath.Count) {
				currentPath = null;
				setAIState (AIState.Dormant);
				Debug.Log ("No idea lol.");
				return;
			}
			
			//Get the direction to the next waypoint on the current path.
			Vector3 waypoint = currentPath.vectorPath [currentWaypoint];
			waypoint.y = 0; //Confine movement to the horizontal plane.
			moveToward (waypoint);

			//Check if we are close enough to the next waypoint
			//If we are, proceed to follow the next waypoint
			if (Vector3.Distance (transform.position, waypoint) < nextWaypointDistance) {
				currentWaypoint++;
				return;
			}
		}
	}

	public void moveToward (Vector3 targetPosition)
	{
		Vector3 targetDirection = targetPosition - transform.position;
		float targetBearing = Vector3.Angle (transform.forward, targetDirection);

		//Create the rotation we need to be in to be facing the target location
		Quaternion lookRotation = Quaternion.LookRotation (targetDirection.normalized, Vector3.up);

		//Turn to face direction of travel
		if (Mathf.Abs (targetBearing) < Time.fixedDeltaTime * TurnSpeed) {
			transform.rotation = lookRotation;
		} else {
			//Rotate over time according to speed until we are in the required rotation.
			transform.rotation = Quaternion.Slerp (transform.rotation, lookRotation, Time.deltaTime * (TurnSpeed / targetBearing));
		}

		//Move forwards, if the target is roughly ahead of us.
		if (targetBearing < 90) {
			transform.position += Vector3.ClampMagnitude (transform.forward * MoveSpeed * Time.fixedDeltaTime, targetDirection.magnitude);
		}		
	}


	private void setAIState (AIState new_state)
	{
		if (new_state == currentAIState) {
			return;
		}

		currentAIState = new_state;

		switch (new_state) {
		
		case AIState.Attacking:
			turret.IsDormant = false;
			break;

		case AIState.Chasing:
			//Build a path to the player's last known position.
			seeker.StartPath (transform.position, lastKnownPlayerPos, OnPathComplete);
			break;
		}


	}
}
