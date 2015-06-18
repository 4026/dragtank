using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class TurretController : Turret
{

    public GameObject player;
    public GameObject targetPrefab;

    private List<GameObject> targets = new List<GameObject>();
    private GameObject dragged_target;
    private GameObject current_target;
    private bool is_dragging = false;
    private GameManager gameManager;

    void Awake()
    {
        gameManager = GameManager.Instance;

        InputController.NotifyStartDrag += StartDrag;
        InputController.NotifyDragUpdate += UpdateDrag;
        InputController.NotifyStopDrag += EndDrag;
    }

    void OnDestroy()
    {
        InputController.NotifyStartDrag -= StartDrag;
        InputController.NotifyDragUpdate -= UpdateDrag;
        InputController.NotifyStopDrag -= EndDrag;
    }

    void Update()
    {
        if (gameManager.gameState != GameState.Moving)
        {
            return;
        }

        //Follow tank around
        transform.position = new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z);

        if (!animating)
        {
            //If the turret isn't still shaking from the last shot, turn it.

            if (targets.Count > 0 || dragged_target != null)
            {
                //If we have an active target, rotate to face it.
                GameObject target = (targets.Count > 0) ? targets.First() : dragged_target;
                
                if (turnToward(target.transform.position) && targets.Count > 0)
                {
                    fireAt(target);
                    //Remove target indicator
                    targets.Remove(target);
                    GameObject.Destroy(target);
                }
            }
            else
            {
                //If no target, turn to face front of tank.
                turnToward(player.transform.position + player.transform.up);
            }
        }
    }

    void StartDrag(Vector2 screen_pos)
    {
        if (gameManager.gameState != GameState.Moving)
        {
            return;
        }

        is_dragging = true;

        Vector3 world_pos = Camera.main.ScreenToWorldPoint(new Vector3(screen_pos.x, screen_pos.y, Camera.main.transform.position.y));
        current_target = Instantiate(targetPrefab, world_pos, Quaternion.AngleAxis(90, Vector3.right)) as GameObject;
        
        dragged_target = current_target;
    }
    
    void UpdateDrag(Vector2 screen_pos)
    {
        if (gameManager.gameState != GameState.Moving || !is_dragging)
        {
            return;
        }

        Vector3 world_pos = Camera.main.ScreenToWorldPoint(new Vector3(screen_pos.x, screen_pos.y, Camera.main.transform.position.y));
        current_target.transform.position = world_pos;
    }
    
    void EndDrag(Vector2 screen_pos)
    {
        if (gameManager.gameState != GameState.Moving || !is_dragging)
        {
            return;
        }

        is_dragging = false;

        Vector3 world_pos = Camera.main.ScreenToWorldPoint(new Vector3(screen_pos.x, screen_pos.y, Camera.main.transform.position.y));
        current_target.transform.position = world_pos;
        
        dragged_target = null;
        targets.Add(current_target);
    }

}
