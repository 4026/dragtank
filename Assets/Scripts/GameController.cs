using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class GameController : MonoBehaviour 
{

	public GameObject player;

    public TurretController player_turret;

    public GameObject targetPrefab;
    private GameObject current_target;
	
	public enum GameState
	{
		Planning,
		Moving
	}
	
	private GameState _currentState = GameState.Planning;
	public GameState currentState 
	{
		get
		{
			return _currentState;
		}
		set
		{
            //Implicit setState method calls setup methods for each state.
			_currentState = value;
            switch (value)
            {
                case GameState.Moving:
                    startMoving();
                    break;
            }
		}
	}

    //Explicit setState() method required for e.g. animation callbacks.
    public void setState(GameState new_state)
    {
        currentState = new_state;
    }
	
	private bool is_dragging = false;
	List<Vector3> dragged_path = new List<Vector3>();
	
	
	
	void Update () 
	{
		switch (currentState)
		{
			case GameState.Planning:
				PlanningUpdate();
				break;
			case GameState.Moving:
				MovingUpdate();
				break;
		}
	}
	
	void OnGUI () 
	{
		switch (currentState)
		{
			case GameState.Planning:
				if (GUI.Button(new Rect(20, 20, 100, 40), "Go"))
				{
					currentState = GameState.Moving;
				}
				break;
			case GameState.Moving:
				break;
		}
	}
	
	
	//Update() method when in the "Planning" state.
	private void PlanningUpdate()
	{
		#if UNITY_EDITOR
			Vector2 screen_pos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
		
			if (Input.GetMouseButtonDown(0) && screenPosIsOnPlayer(screen_pos))
			{
				startDrag(screen_pos);
			}
			else if (is_dragging)
			{
				if (Input.GetMouseButton(0))
				{
					updateDrag(screen_pos);
				}
				else if (Input.GetMouseButtonUp(0))
				{
					endDrag(screen_pos);
				}
			}
		
		#endif
		
		
		#if UNITY_ANDROID
			if (Input.touchCount > 0)
			{
				Touch touch = Input.GetTouch(0);
				if (touch.phase == TouchPhase.Began && screenPosIsOnPlayer(touch.position))
				{
					startDrag(touch.position);
				}
				else if (is_dragging)
				{
					if (touch.phase == TouchPhase.Moved)
					{
						updateDrag(touch.position);
					}
					else if (touch.phase == TouchPhase.Ended)
					{
						endDrag(touch.position);
					}
				}
			}
		#endif
		
		
	}

    //Called whenever the Moving state is entered.
    private void startMoving()
    {
        Hashtable itween_options = iTween.Hash(
            "path", dragged_path.ToArray(),
            "orienttopath", true,
            "axis", "y",
            "speed", 1,
            "easetype", iTween.EaseType.linear, 
            "oncompletetarget", this.gameObject,
            "oncomplete", "setState",
            "oncompleteparams", GameState.Planning
        );
        iTween.MoveTo(player, itween_options);
    }
	
	//Update() method when in the "Moving" state.
	private void MovingUpdate()
	{
        #if UNITY_EDITOR
            Vector2 screen_pos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            
            if (Input.GetMouseButtonDown(0))
            {
                startTargetDrag(screen_pos);
            }
            else if (is_dragging)
            {
                if (Input.GetMouseButton(0))
                {
                    updateTargetDrag(screen_pos);
                }
                else if (Input.GetMouseButtonUp(0))
                {
                    endTargetDrag(screen_pos);
                }
            }
        
        #endif
        
        
        #if UNITY_ANDROID
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Began)
                {
                    startTargetDrag(touch.position);
                }
                else if (is_dragging)
                {
                    if (touch.phase == TouchPhase.Moved)
                    {
                        updateTargetDrag(touch.position);
                    }
                    else if (touch.phase == TouchPhase.Ended)
                    {
                        endTargetDrag(touch.position);
                    }
                }
            }
        #endif
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
	
	private void startDrag(Vector2 start_pos)
	{
		is_dragging = true;
		
		Vector3 world_pos = Camera.main.ScreenToWorldPoint(new Vector3(start_pos.x, start_pos.y, Camera.main.transform.position.y));
		
        dragged_path.Clear();
        dragged_path.Add(world_pos);

		GetComponent<LineRenderer>().SetVertexCount(dragged_path.Count);
		GetComponent<LineRenderer>().SetPosition(dragged_path.Count - 1, world_pos + 0.5f * Vector3.down);
	}
	
	private void updateDrag(Vector2 new_pos)
	{
        Vector3 world_pos = Camera.main.ScreenToWorldPoint(new Vector3(new_pos.x, new_pos.y, Camera.main.transform.position.y));
		Vector3 difference = world_pos - dragged_path.Last();
		if (difference.magnitude > 1)
		{	
            dragged_path.Add(world_pos);

			GetComponent<LineRenderer>().SetVertexCount(dragged_path.Count);
            GetComponent<LineRenderer>().SetPosition(dragged_path.Count - 1, world_pos + 0.5f * Vector3.down);
		}
		else
		{
            GetComponent<LineRenderer>().SetVertexCount(dragged_path.Count + 1);
            GetComponent<LineRenderer>().SetPosition(dragged_path.Count, world_pos + 0.5f * Vector3.down);
		}
	}
	
    private void endDrag(Vector2 new_pos)
	{
		is_dragging = false;

        Vector3 world_pos = Camera.main.ScreenToWorldPoint(new Vector3(new_pos.x, new_pos.y, Camera.main.transform.position.y));
        dragged_path.Add(world_pos);
        
        GetComponent<LineRenderer>().SetVertexCount(dragged_path.Count);
        GetComponent<LineRenderer>().SetPosition(dragged_path.Count - 1, world_pos + 0.5f * Vector3.down);
	}



    private void startTargetDrag(Vector2 screen_pos)
    {
        is_dragging = true;
        Vector3 world_pos = Camera.main.ScreenToWorldPoint(new Vector3(screen_pos.x, screen_pos.y, Camera.main.transform.position.y));
        current_target = Instantiate(targetPrefab, world_pos, Quaternion.AngleAxis(90, Vector3.right)) as GameObject;
    }

    private void updateTargetDrag(Vector2 screen_pos)
    {
        Vector3 world_pos = Camera.main.ScreenToWorldPoint(new Vector3(screen_pos.x, screen_pos.y, Camera.main.transform.position.y));
        current_target.transform.position = world_pos;
    }

    private void endTargetDrag(Vector2 screen_pos)
    {
        is_dragging = false;
        Vector3 world_pos = Camera.main.ScreenToWorldPoint(new Vector3(screen_pos.x, screen_pos.y, Camera.main.transform.position.y));
        current_target.transform.position = world_pos;

        player_turret.addTarget(current_target);
    }
}
