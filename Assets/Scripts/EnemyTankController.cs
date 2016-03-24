using UnityEngine;
using System.Collections;
using Pathfinding;
using System.Linq;

public class EnemyTankController : Tank
{
	public float NextWaypointDistance;

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
		Chasing,
        PlayerIsDead
	}
	private AIState currentAIState = AIState.Dormant;
    
	void Awake ()
	{
		gameManager = GameManager.Instance;
	}

	void Start ()
	{
		player = GameObject.FindGameObjectWithTag ("Player");
        player.GetComponent<Destructible>().OnDeath += OnPlayerDeath;

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
		if (gameManager.gameState != GameState.Moving || currentAIState == AIState.PlayerIsDead) {
			return;
		}

		//Set state
		if (turret.IsPlayerInVision ()) {
			//"There they are! Get them!"
			lastKnownPlayerPos = player.transform.position;
			setAIState (AIState.Attacking);
		} else if (currentAIState != AIState.Dormant) {
			//"We've lost them! Get after them!"
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
				SlowToStop ();
				return;
			}
			
			//Get the direction to the next waypoint on the current path.
			Vector3 waypoint = currentPath.vectorPath [currentWaypoint];
			waypoint.y = 0; //Confine movement to the horizontal plane.
			MoveToward (waypoint);

			//Check if we are close enough to the next waypoint
			//If we are, proceed to follow the next waypoint
			if (Vector3.Distance (transform.position, waypoint) < NextWaypointDistance) {
				currentWaypoint++;
				return;
			}
		} else {
			SlowToStop ();
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

            case AIState.Dormant:
            case AIState.PlayerIsDead:
                turret.IsDormant = true;
                break;
        }
	}

    public void OnPlayerDeath()
    {
        setAIState(AIState.PlayerIsDead);
    }
}
