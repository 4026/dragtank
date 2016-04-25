using UnityEngine;
using System.Collections;
using Pathfinding;
using System.Linq;
using System.Collections.Generic;

public class EnemyTankController : Tank
{
	public float NextWaypointDistance;

    /// <summary>
    /// How far the tank will roam away from where it was spawned while patrolling.
    /// </summary>
    public float PatrolRange;

    /// <summary>
    /// How long each individual leg of the tank's patrol should be.
    /// </summary>
    public float PatrolDistance;

	private GameManager gameManager;
	private EnemyTurretController turret;

	private GameObject player;                  //The player object to hunt.
	private Vector3 lastKnownPlayerPos;

	private Seeker seeker;
	private Path currentPath;                  //The chaser's current path
	private int currentWaypoint = 0;            //The waypoint we are currently moving towards

    /// <summary>
    /// Where the tank was first spawned. It will try to remain near to this area.
    /// </summary>
    private Vector3 home;

	enum AIState
	{
		Dormant,
        Patrolling,
        Attacking,
		Chasing,
        PlayerIsDead
	}
	private AIState currentAIState = AIState.Dormant;
    
	void Start ()
	{
        gameManager = GameManager.Instance;

        home = transform.position;

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
		if (gameManager.State != GameManager.GameState.Moving || currentAIState == AIState.PlayerIsDead) {
			return;
		}

		//Set state
		if (turret.IsPlayerInVision ())
        {
			//"There they are! Get them!"
			lastKnownPlayerPos = player.transform.position;
			setAIState (AIState.Attacking);
		}
        else if (currentAIState == AIState.Attacking || currentAIState == AIState.Chasing)
        {
			//"We've lost them! Get after them!"
			setAIState (AIState.Chasing);
		}
        else
        {
            //"Must've been rats."
            setAIState(AIState.Patrolling);
        }

		//Act on state
		if (currentAIState == AIState.Chasing || currentAIState == AIState.Patrolling) {
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

            case AIState.Patrolling:
                //Build a path to a patrol location.
                seeker.StartPath(transform.position, getNewPatrolLocation(), OnPathComplete);
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

    private Vector3 getNewPatrolLocation()
    {
        // If we've strayd too far from home, go back.
        if (Vector3.Distance(transform.position, home) > PatrolRange)
        {
            return home;
        }

        List<Vector3> directions = new List<Vector3>(new Vector3[] {
            Vector3.forward * PatrolDistance,
            Vector3.right * PatrolDistance,
            Vector3.back * PatrolDistance,
            Vector3.left * PatrolDistance
        });
        LayerMask layer_mask = LayerMask.GetMask("Walls");

        do
        {
            int direction_index = Random.Range(0, directions.Count);
            Vector3 patrol_destination = transform.position + directions[direction_index];

            if (!Physics.CheckSphere(patrol_destination, 1f, layer_mask))
            {
                return patrol_destination;
            }
            directions.RemoveAt(direction_index);
        } while (directions.Count > 0);

        return home;
    }

}
