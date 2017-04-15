using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using com.kleberswf.lib.core;

[Prefab("Prefabs/Singletons/PathPlan")]
public class PathPlanner : Singleton<PathPlanner>
{
    /// <summary>
    /// Vector describing the offset of points on the rendered path from points on the path the player will actually drive.
    /// </summary>
	public Vector3 RenderOffset;

    public Vector3[] Path { get { return dragged_path.ToArray (); } }

    public delegate void PathChangeNotifier();
    public event PathChangeNotifier OnPathChange;

    private GameObject m_player;
    private List<Vector3> dragged_path = new List<Vector3> ();
	private LineRenderer m_rendered_path;
	private static Color path_color = new Color (1f, 1f, 1f, 0.5f);
	private static Color invalid_path_color = new Color (1f, 0f, 0f, 0.5f);
	private GameManager m_gameManager;
	private int next_path_point; //Which point on the path is the next one the tank is driving to.
    


    private GameObject m_continueGuide;
    private GameObject m_resetGuide;
    
	void Start ()
	{
        m_gameManager = GameManager.Instance;
        m_gameManager.NotifyStateChange += OnStateChange;
        m_gameManager.NotifyPlayerSpawn += OnPlayerSpawn;

        m_rendered_path = GetComponent<LineRenderer> ();
        m_continueGuide = transform.FindChild("ContinueGuide").gameObject;
        m_resetGuide = transform.FindChild("ResetGuide").gameObject;
    }

    void OnPlayerSpawn (GameObject player)
    {
        m_player = player;
    }

    void OnStateChange (GameManager.GameState old_state, GameManager.GameState new_state)
	{
		switch (new_state) {
		case GameManager.GameState.Moving:
			next_path_point = 1;
			break;
		}
	}

	void Update ()
	{
		if (m_gameManager.State != GameManager.GameState.Moving || m_player == null) {
			return;
		}

		//Have the tank erase the the path as it approaches each point.
		LineRenderer rendered_path = GetComponent<LineRenderer> ();
		rendered_path.SetPosition (0, m_player.transform.position);
		if (next_path_point < dragged_path.Count && Vector3.Distance (m_player.transform.position, dragged_path [next_path_point]) < 1) {
			//If we're close to the next point, move all the LineRenderer points down by one in the list, and shorten the list.
			int path_i = 1;
			for (int i = next_path_point + 1; i < dragged_path.Count; ++i) {
				rendered_path.SetPosition (path_i, dragged_path [i] + RenderOffset);
				++path_i;
			}
			rendered_path.positionCount = path_i;
            
			++next_path_point;
		}
	}

	public void ClearWaypoints ()
	{
		dragged_path.Clear ();
		m_rendered_path.positionCount = 0;

        m_resetGuide.SetActive(false);
        m_continueGuide.transform.position = m_player.transform.position;
    }

	public void AddWaypoint (Vector3 new_waypoint)
	{
		dragged_path.Add (new_waypoint);
		m_rendered_path.positionCount = dragged_path.Count;
		m_rendered_path.SetPosition (dragged_path.Count - 1, new_waypoint + RenderOffset);

        m_resetGuide.SetActive(true);

        if (OnPathChange != null) OnPathChange();
    }

	public Vector3 GetLastPoint ()
	{
		return dragged_path.Last ();
	}

    public int GetNumWaypoints()
    {
        return dragged_path.Count;
    }

	public void UpdateFinalRenderPoint (Vector3 point, bool isValid = true)
	{
		m_rendered_path.endColor = isValid ? path_color : invalid_path_color;
		m_rendered_path.positionCount = dragged_path.Count + 1;
		m_rendered_path.SetPosition (dragged_path.Count, point + RenderOffset);
	}

	public void ClearFinalRenderPoint ()
	{
		m_rendered_path.endColor = path_color;
		m_rendered_path.positionCount = dragged_path.Count;
	}
}
