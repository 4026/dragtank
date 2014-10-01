using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class PathPlanner : MonoBehaviour
{
    public GameObject player;
    public List<Vector3> dragged_path = new List<Vector3>();

    private LineRenderer rendered_path;
    private static Color path_color = new Color(1f, 1f, 1f, 0.5f);
    private static Color invalid_path_color = new Color(1f, 0f, 0f, 0.5f);
    private GameManager gameManager;
    private bool is_dragging = false;
    private int next_path_point; //Which point on the path is the next one the tank is driving to.
    private Vector3 player_offset; //Vector describing the offset of points on the rendered path from points on the path the player will actually drive.
    
    void Awake()
    {
        gameManager = GameManager.Instance;
        gameManager.NotifyStateChange += OnStateChange;

        InputController.NotifyStartDrag += StartDrag;
        InputController.NotifyDragUpdate += UpdateDrag;
        InputController.NotifyStopDrag += EndDrag;
    }

    void Start()
    {
        rendered_path = GetComponent<LineRenderer>();
        player_offset = transform.position - player.transform.position;
    }

    void OnDestroy()
    {
        gameManager.NotifyStateChange -= OnStateChange;

        InputController.NotifyStartDrag -= StartDrag;
        InputController.NotifyDragUpdate -= UpdateDrag;
        InputController.NotifyStopDrag -= EndDrag;
    }

    void OnStateChange(GameState old_state, GameState new_state)
    {
        switch (new_state)
        {
            case GameState.Moving:
                next_path_point = 1;
                break;
        }
    }

    void Update()
    {
        if (gameManager.gameState != GameState.Moving)
        {
            return;
        }

        //Have the tank erase the the path as it approaches each point.
        LineRenderer rendered_path = GetComponent<LineRenderer>();
        rendered_path.SetPosition(0, player.transform.position);
        if (next_path_point < dragged_path.Count && Vector3.Distance(player.transform.position, dragged_path [next_path_point]) < 1)
        {
            //If we're close to the next point, move all the LineRenderer points down by one in the list, and shorten the list.
            int path_i = 1;
            for (int i = next_path_point + 1; i < dragged_path.Count; ++i)
            {
                rendered_path.SetPosition(path_i, dragged_path [i] + player_offset);
                ++path_i;
            }
            rendered_path.SetVertexCount(path_i);
            
            ++next_path_point;
        }
    }


    void StartDrag(Vector2 screen_pos)
    {
        if (gameManager.gameState != GameState.Planning || !screenPosIsOnPlayer(screen_pos))
        {
            return;
        }

        is_dragging = true;

        dragged_path.Clear();
        dragged_path.Add(player.transform.position);
        
        rendered_path.SetVertexCount(dragged_path.Count);
        rendered_path.SetPosition(dragged_path.Count - 1, player.transform.position + player_offset);
    }
    
    void UpdateDrag(Vector2 new_pos)
    {
        if (gameManager.gameState != GameState.Planning || !is_dragging)
        {
            return;
        }

        Vector3 world_pos = Camera.main.ScreenToWorldPoint(new Vector3(new_pos.x, new_pos.y, Camera.main.transform.position.y));
        Vector3 difference = world_pos - dragged_path.Last();
        
        RaycastHit hit_info;
        LayerMask layer_mask = LayerMask.GetMask("Walls");
        if (Physics.SphereCast(dragged_path.Last(), 0.75f, difference, out hit_info, difference.magnitude, layer_mask))
        {
            //Paths that intersect walls should turn red and not extend.
            rendered_path.SetColors(path_color, invalid_path_color);
            rendered_path.SetVertexCount(dragged_path.Count + 1);
            rendered_path.SetPosition(dragged_path.Count, world_pos + player_offset);
        }
        else if (difference.magnitude < 1)
        {
            //A short distance isn't enough to set a new waypoint
            rendered_path.SetColors(path_color, path_color);
            rendered_path.SetVertexCount(dragged_path.Count + 1);
            rendered_path.SetPosition(dragged_path.Count, world_pos + player_offset);
        }
        else
        {
            //A long enough distance registers a new waypoint.
            dragged_path.Add(dragged_path.Last() + difference.normalized); /* only add a normalized vector in the intended direction from the last position, 
                                                                            * because otherwise the tank's movement animation can go too fast.*/
            
            rendered_path.SetColors(path_color, path_color);
            rendered_path.SetVertexCount(dragged_path.Count);
            rendered_path.SetPosition(dragged_path.Count - 1, world_pos + player_offset);
        }
    }
    
    void EndDrag(Vector2 new_pos)
    {
        if (gameManager.gameState != GameState.Planning || !is_dragging)
        {
            return;
        }

        is_dragging = false;

        Vector3 world_pos = Camera.main.ScreenToWorldPoint(new Vector3(new_pos.x, new_pos.y, Camera.main.transform.position.y));
        Vector3 difference = world_pos - dragged_path.Last();

        RaycastHit hit_info;
        LayerMask layer_mask = LayerMask.GetMask("Walls");
        if (Physics.SphereCast(dragged_path.Last(), 0.75f, difference, out hit_info, difference.magnitude, layer_mask))
        {
            //If the final point leads the path through a wall, ignore it and remove it from the renderer.
            rendered_path.SetColors(path_color, path_color);
            rendered_path.SetVertexCount(dragged_path.Count);
        }
        else
        {
            //Otherwise, add it to the path.
            dragged_path.Add(world_pos);
            rendered_path.SetVertexCount(dragged_path.Count);
            rendered_path.SetPosition(dragged_path.Count - 1, world_pos + player_offset);
        }
    }



    private bool screenPosIsOnPlayer(Vector2 screen_pos)
    {
        Ray screen_ray = Camera.main.ScreenPointToRay(screen_pos);
        RaycastHit hitInfo;
        if (Physics.Raycast(screen_ray, out hitInfo, 100))
        {
            if (hitInfo.collider.gameObject.tag == "Player")
            {
                return true;
            }
        }

        return false;
    }
}
