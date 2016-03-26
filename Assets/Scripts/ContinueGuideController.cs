using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using Pathfinding;

public class ContinueGuideController : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{

    private GameManager m_gameManager;
    private PathPlanner m_pathPlanner;
    private Seeker m_seeker;
    private GameObject m_player;
    private EnvironmentController m_environment;

    void Awake()
    {
        m_gameManager = GameManager.Instance;
        m_gameManager.NotifyStateChange += OnStateChange;
    }

    void Start ()
    {
        m_pathPlanner = transform.parent.GetComponent<PathPlanner>();
        m_seeker = GetComponent<Seeker>();
        m_player = GameObject.Find("Player");
        m_environment = FindObjectOfType<EnvironmentController>();
	}

    void OnDestroy()
    {
        m_gameManager.NotifyStateChange -= OnStateChange;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (m_gameManager.gameState != GameState.Planning)
        {
            return;
        }

        if (m_pathPlanner.GetNumWaypoints() == 0)
        {
            m_pathPlanner.AddWaypoint(transform.position);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (m_gameManager.gameState != GameState.Planning)
        {
            return;
        }

        

        Vector3 current_world_pos = Camera.main.ScreenToWorldPoint(new Vector3(eventData.position.x, eventData.position.y, Camera.main.transform.position.y));
        transform.position = current_world_pos;

        Vector3 last_world_pos = m_pathPlanner.GetLastPoint();
        Vector3 difference = current_world_pos - last_world_pos;

        RaycastHit hit_info;
        LayerMask wall_mask = LayerMask.GetMask("Walls");

        if (!m_environment.PointIsInBounds(current_world_pos) ||  Physics.CheckSphere(current_world_pos, 0.75f, wall_mask))
        {
            //Endpoints inside walls (or outside the map) should turn red and not extend the path.
            m_pathPlanner.UpdateFinalRenderPoint(current_world_pos, false);
        }
        else if (Physics.SphereCast((last_world_pos), 0.75f, difference, out hit_info, difference.magnitude, wall_mask))
        {
            //Paths that intersect walls but finish in a valid location should be corrected with pathfinding.

            m_pathPlanner.UpdateFinalRenderPoint(current_world_pos, false); // While we're pathfinding, show the point as invalid.
            if (m_seeker.IsDone())
            {
                m_seeker.StartPath(last_world_pos, current_world_pos, OnPathComplete);
            }
        }
        else if (difference.magnitude < 1)
        {
            //A short distance isn't enough to set a new waypoint
            m_pathPlanner.UpdateFinalRenderPoint(current_world_pos);
        }
        else
        {
            //A long enough distance registers a new waypoint.
            m_pathPlanner.AddWaypoint(current_world_pos);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (m_gameManager.gameState != GameState.Planning)
        {
            return;
        }

        Vector3 current_world_pos = Camera.main.ScreenToWorldPoint(new Vector3(eventData.position.x, eventData.position.y, Camera.main.transform.position.y));
        Vector3 last_world_pos = m_pathPlanner.GetLastPoint();
        Vector3 difference = current_world_pos - last_world_pos;

        RaycastHit hit_info;
        LayerMask layer_mask = LayerMask.GetMask("Walls");
        if (Physics.SphereCast(last_world_pos, 0.75f, difference, out hit_info, difference.magnitude, layer_mask))
        {
            //If the final point leads the path through a wall, ignore it and remove it from the renderer.
            m_pathPlanner.ClearFinalRenderPoint();
            transform.position = last_world_pos;
        }
        else
        {
            //Otherwise, add it to the path.
            m_pathPlanner.AddWaypoint(current_world_pos);
        }
    }

    //When pathfinding is done, add the waypoints on the path to the plan.
    public void OnPathComplete(Path path)
    {
        if (path.error)
        {
            Debug.Log("Path error: " + path.errorLog);
            return;
        }

        foreach (Vector3 waypoint in path.vectorPath)
        {
            m_pathPlanner.AddWaypoint(waypoint);
        }
    }

    void OnStateChange(GameState old_state, GameState new_state)
    {
        switch (new_state)
        {
            case GameState.Moving:
                GetComponent<SpriteRenderer>().enabled = false;
                break;


            case GameState.Planning:
                transform.position = m_player.transform.position;
                GetComponent<SpriteRenderer>().enabled = true;
                break;
        }
    }
}
