using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class TurretController : Turret
{

	public GameObject player;
	public GameObject targetPrefab;

	private List<GameObject> m_targets = new List<GameObject> ();
	private GameObject m_draggedTarget;
	private GameObject m_currentTarget;
	private bool m_isDragging = false;
	private GameManager m_gameManager;

	void Start ()
	{
		m_gameManager = GameManager.Instance;
        m_gameManager.NotifyStateChange += OnStateChange;
	}

    void OnDestroy()
    {
        if (m_gameManager != null)
        {
            m_gameManager.NotifyStateChange -= OnStateChange;
        }
    }

    public override void Update ()
	{
        base.Update();

        if (m_gameManager.State != GameManager.GameState.Moving) {
			return;
		}

		if (!animating) {
			//If the turret isn't still shaking from the last shot, turn it.

			if (m_targets.Count > 0 || m_draggedTarget != null) {
				//If we have an active target, rotate to face it.
				GameObject target = (m_targets.Count > 0) ? m_targets.First () : m_draggedTarget;
                
				if (turnToward (target.transform.position) && CanFire && m_targets.Count > 0) {
					fireAt (target);
					//Remove target indicator
					m_targets.Remove (target);
					GameObject.Destroy (target);
				}
			} else {
				//If no target, turn to face front of tank.
				turnToward (player.transform.position + player.transform.up);
			}
		}
	}

    private void OnStateChange(GameManager.GameState old_state, GameManager.GameState new_state)
    {
        //Clear up any unplaced targets at the end of the move phase.
        if (old_state == GameManager.GameState.Moving && m_isDragging)
        {
            m_isDragging = false;
            Destroy(m_currentTarget);
        }
    }

    public void PlaceTarget(Vector2 screen_pos)
    {
        if (m_gameManager.State != GameManager.GameState.Moving || m_isDragging)
        {
            return;
        }

        Vector3 world_pos = Camera.main.ScreenToWorldPoint(new Vector3(screen_pos.x, screen_pos.y, Camera.main.transform.position.y));
        m_targets.Add(Instantiate(targetPrefab, world_pos, Quaternion.AngleAxis(90, Vector3.right)));
    }

	public void StartTargetDrag (Vector2 screen_pos)
	{
		if (m_gameManager.State != GameManager.GameState.Moving)
        {
			return;
		}

		m_isDragging = true;

		Vector3 world_pos = Camera.main.ScreenToWorldPoint (new Vector3 (screen_pos.x, screen_pos.y, Camera.main.transform.position.y));
		m_currentTarget = Instantiate (targetPrefab, world_pos, Quaternion.AngleAxis (90, Vector3.right)) as GameObject;
        
		m_draggedTarget = m_currentTarget;
	}

    public void UpdateTargetDrag (Vector2 screen_pos)
	{
		if (m_gameManager.State != GameManager.GameState.Moving || !m_isDragging)
        {
			return;
		}

		Vector3 world_pos = Camera.main.ScreenToWorldPoint (new Vector3 (screen_pos.x, screen_pos.y, Camera.main.transform.position.y));
		m_currentTarget.transform.position = world_pos;
	}

    public void EndTargetDrag (Vector2 screen_pos)
	{
		if (m_gameManager.State != GameManager.GameState.Moving || !m_isDragging)
        {
			return;
		}

		m_isDragging = false;

		Vector3 world_pos = Camera.main.ScreenToWorldPoint (new Vector3 (screen_pos.x, screen_pos.y, Camera.main.transform.position.y));
		m_currentTarget.transform.position = world_pos;
        
		m_draggedTarget = null;
		m_targets.Add (m_currentTarget);
	}

}
