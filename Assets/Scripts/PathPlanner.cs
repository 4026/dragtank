using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class PathPlanner : MonoBehaviour
{
	public GameObject player;

	public Vector3[] Path { get { return dragged_path.ToArray (); } }

	private List<Vector3> dragged_path = new List<Vector3> ();
	private LineRenderer rendered_path;
	private static Color path_color = new Color (1f, 1f, 1f, 0.5f);
	private static Color invalid_path_color = new Color (1f, 0f, 0f, 0.5f);
	private GameManager gameManager;
	private int next_path_point; //Which point on the path is the next one the tank is driving to.
	private Vector3 render_offset; //Vector describing the offset of points on the rendered path from points on the path the player will actually drive.

    private GameObject m_continueGuide;
    private GameObject m_resetGuide;

	void Awake ()
	{
		gameManager = GameManager.Instance;
		gameManager.NotifyStateChange += OnStateChange;
	}

	void Start ()
	{
		rendered_path = GetComponent<LineRenderer> ();
		render_offset = transform.position - player.transform.position;

        m_continueGuide = transform.FindChild("ContinueGuide").gameObject;
        m_resetGuide = transform.FindChild("ResetGuide").gameObject;
    }

	void OnDestroy ()
	{
		gameManager.NotifyStateChange -= OnStateChange;
	}

	void OnStateChange (GameState old_state, GameState new_state)
	{
		switch (new_state) {
		case GameState.Moving:
			next_path_point = 1;
			break;
		}
	}

	void Update ()
	{
		if (gameManager.gameState != GameState.Moving || player == null) {
			return;
		}

		//Have the tank erase the the path as it approaches each point.
		LineRenderer rendered_path = GetComponent<LineRenderer> ();
		rendered_path.SetPosition (0, player.transform.position);
		if (next_path_point < dragged_path.Count && Vector3.Distance (player.transform.position, dragged_path [next_path_point]) < 1) {
			//If we're close to the next point, move all the LineRenderer points down by one in the list, and shorten the list.
			int path_i = 1;
			for (int i = next_path_point + 1; i < dragged_path.Count; ++i) {
				rendered_path.SetPosition (path_i, dragged_path [i] + render_offset);
				++path_i;
			}
			rendered_path.SetVertexCount (path_i);
            
			++next_path_point;
		}
	}

	public void ClearWaypoints ()
	{
		dragged_path.Clear ();
		rendered_path.SetVertexCount (0);

        m_resetGuide.SetActive(false);
        m_continueGuide.transform.position = player.transform.position;
    }

	public void AddWaypoint (Vector3 new_waypoint)
	{
		dragged_path.Add (new_waypoint);
		rendered_path.SetVertexCount (dragged_path.Count);
		rendered_path.SetPosition (dragged_path.Count - 1, new_waypoint + render_offset);

        m_resetGuide.SetActive(true);
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
		rendered_path.SetColors (path_color, isValid ? path_color : invalid_path_color);
		rendered_path.SetVertexCount (dragged_path.Count + 1);
		rendered_path.SetPosition (dragged_path.Count, point + render_offset);
	}

	public void ClearFinalRenderPoint ()
	{
		rendered_path.SetColors (path_color, path_color);
		rendered_path.SetVertexCount (dragged_path.Count);
	}
}
