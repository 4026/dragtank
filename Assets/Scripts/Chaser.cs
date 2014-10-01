using UnityEngine;
using System.Collections;
using Pathfinding;
using System.Linq;

public class Chaser : MonoBehaviour
{
    //The player object to chase.
    public GameObject player;

    //The AI's speed per second
    public float speed;
    
    //The max distance from the AI to a waypoint for it to continue to the next waypoint
    public float nextWaypointDistance;

    //The max distance that the player may get away from the Chaser's current destination before the Chaser calculates a new path.
    public float recalculateDistance;

    private Seeker seeker;
    private CharacterController controller;
    //The chaser's current path
    private Path current_path;
    //The waypoint we are currently moving towards
    private int currentWaypoint = 0;
    private Vector3 current_path_destination;
    private GameManager gameManager;
    
    void Awake()
    {
        gameManager = GameManager.Instance;
    }

    public void Start()
    {
        seeker = GetComponent<Seeker>();
        controller = GetComponent<CharacterController>();

        //Set up an initial plan of attack.
        seeker.StartPath(transform.position, player.transform.position, OnPathComplete);
    }

    public void OnPathComplete(Path path)
    {
        if (path.error)
        {
            Debug.Log("Path error: " + path.errorLog);
            return;
        }

        current_path = path;
        current_path_destination = current_path.vectorPath.Last();
        currentWaypoint = 0; //Reset the waypoint counter
    }
    
    public void FixedUpdate()
    {
        //Do nothing if the game is in the planning phase, or if we have no path to move along yet.
        if (current_path == null || gameManager.gameState != GameState.Moving)
        {
            return;
        }
        
        if (currentWaypoint >= current_path.vectorPath.Count || Vector3.Distance(player.transform.position, current_path_destination) > recalculateDistance)
        {
            //If we've run out of path, or if the player has moved sufficiently far away from the end of our current path, recalculate.
            current_path = null;
            seeker.StartPath(transform.position, player.transform.position, OnPathComplete);
            return;
        }
        
        //Direction to the next waypoint
        Vector3 waypoint = current_path.vectorPath [currentWaypoint];
        waypoint.y = 0; //Confine movement to the horizontal plane.
        Vector3 dir = (waypoint - transform.position).normalized;
        dir *= speed * Time.fixedDeltaTime;
        controller.Move(dir);

        //Face sprite toward direction of travel
        transform.rotation = Quaternion.FromToRotation(Vector3.forward, dir);
        
        //Check if we are close enough to the next waypoint
        //If we are, proceed to follow the next waypoint
        if (Vector3.Distance(transform.position, waypoint) < nextWaypointDistance)
        {
            currentWaypoint++;
            return;
        }
    }

    public void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.tag == "Player")
        {
            Destroy(gameObject);
            iTween.PunchPosition(Camera.main.gameObject, Vector3.left, 1.0f);
        }
    }

} 