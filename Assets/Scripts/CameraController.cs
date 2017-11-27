using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{
    public float PlanningY;
    public float MovementY;
    public float SceneEndY;
    public float DragMomentumFriction;

    private int momentumLocks = 0;
	public bool IsMomentumApplied { 
		get { return momentumLocks == 0; } 
		set { 
			if (!value) {
				++momentumLocks;
			} else {
				momentumLocks = Mathf.Max (0, momentumLocks - 1);
			}
		}
	}
	public bool RotationFollowsTarget { get; set; }

	private GameManager m_gameManager;
    private GameObject m_playerTank;
    private EnvironmentController m_environment;
    private Vector3 m_dragMomentum;

	void Awake ()
	{
		RotationFollowsTarget = true;
		IsMomentumApplied = true;
	}

    void Start()
    {
        m_gameManager = GameManager.Instance;
        m_gameManager.NotifyStateChange += OnStateChange;
        m_gameManager.NotifyPlayerSpawn += OnPlayerSpawn;

        m_environment = FindObjectOfType<EnvironmentController>();
    }
	
	void OnDestroy ()
	{
		m_gameManager.NotifyStateChange -= OnStateChange;
        m_gameManager.NotifyPlayerSpawn -= OnPlayerSpawn;
    }
		
	void Update ()
	{
		switch (m_gameManager.State) {
            case GameManager.GameState.Moving:
                if (m_playerTank == null)
                {
                    break;
                }

                //Determine where camera should focus.
			    transform.position = new Vector3 (m_playerTank.transform.position.x, transform.position.y, m_playerTank.transform.position.z);

			    if (RotationFollowsTarget) {
				    iTween.RotateUpdate (gameObject, m_playerTank.transform.rotation.eulerAngles, 2.0f);
			    }
			    break;

		    case GameManager.GameState.Planning:
			    if (m_dragMomentum.magnitude > 0) {
				    m_dragMomentum = iTween.Vector3Update (m_dragMomentum, Vector3.zero, DragMomentumFriction);
				    if (IsMomentumApplied) {
                        Translate(m_dragMomentum, false);
				    }
			    }
			    break;
		}
	}

    void OnPlayerSpawn(GameObject player)
    {
        m_playerTank = player.transform.Find("Tank").gameObject;
        moveIntoPositionForState(m_gameManager.State);
    }

    void OnStateChange(GameManager.GameState old_state, GameManager.GameState new_state)
    {
        moveIntoPositionForState(new_state);
    }


    private void moveIntoPositionForState (GameManager.GameState state)
	{
        //Animate camera into position for state.
        Vector3 targetPosition;
        Hashtable tween_options;

        switch (state) {
            case GameManager.GameState.SceneStarting:
                transform.position = GameObject.FindObjectOfType<PlayerSpawner>().transform.position + (transform.position.y * Vector3.up);
                break;

            case GameManager.GameState.MoveCountdown:
                if (m_playerTank == null) { return; }

                targetPosition = new Vector3 (m_playerTank.transform.position.x, MovementY, m_playerTank.transform.position.z);

                tween_options = iTween.Hash(
                    "position", targetPosition,
                    "time", m_gameManager.MoveCountdownDuration,
                    "oncomplete", "MoveCountdownComplete",
                    "easetype", iTween.EaseType.easeInOutQuad
                );
                iTween.MoveTo(gameObject, tween_options);

                tween_options = iTween.Hash(
                    "rotation", m_playerTank.transform.rotation.eulerAngles,
                    "time", m_gameManager.MoveCountdownDuration,
                    "easetype", iTween.EaseType.easeInOutQuad
                );
                iTween.RotateTo(gameObject, tween_options);
                break;
			
			
		    case GameManager.GameState.PlanCountdown:
                if (m_playerTank == null) { return; }
                targetPosition = new Vector3(m_playerTank.transform.position.x, PlanningY, m_playerTank.transform.position.z);

                tween_options = iTween.Hash(
                    "position", targetPosition,
                    "time", m_gameManager.PlanningCountdownDuration,
                    "oncomplete", "PlanCountdownComplete",
                    "easetype", iTween.EaseType.easeInOutQuad
                );
                iTween.MoveTo(gameObject, tween_options);

                tween_options = iTween.Hash(
                    "rotation", new Vector3(90, 0, 0),
                    "time", m_gameManager.PlanningCountdownDuration,
                    "easetype", iTween.EaseType.easeInOutQuad
                );
                iTween.RotateTo (gameObject, tween_options);

                       
			    break;

            case GameManager.GameState.SceneEnding:
                targetPosition = new Vector3(transform.position.x, SceneEndY, transform.position.z);

                tween_options = iTween.Hash(
                    "position", targetPosition,
                    "time", m_gameManager.SceneEndDuration,
                    "oncomplete", "SceneEndComplete",
                    "easetype", iTween.EaseType.easeInOutQuad
                );
                iTween.MoveTo(gameObject, tween_options);

                break;
        }
	}

	//Called when the camera is in position at the end of the move countdown.
	public void MoveCountdownComplete()
	{
		m_gameManager.State = GameManager.GameState.Moving;
	}

    //Called when the camera is in position at the end of the planning countdown.
    public void PlanCountdownComplete()
    {
        m_gameManager.State = GameManager.GameState.Planning;
    }

    //Called when the camera has finished animating at the end of the scene.
    public void SceneEndComplete()
    {
        m_gameManager.OnSceneEndComplete();
    }

    public void SetY(float new_y)
    {
        transform.position = new Vector3(transform.position.x, new_y, transform.position.z);
    }

    /// <summary>
    /// Move the camera within its XZ bounds, optionally applying momentum.
    /// </summary>
    public void Translate (Vector3 direction, bool set_momentum = true)
	{
        Vector3 new_position = transform.position + direction;
        if (m_environment.Bounds.Contains(new_position))
        {
            //If the translation keeps us within our bounds, great.
            transform.position = new_position;
        }
        else
        {
            //If it would take us outside the bounds, put us at the edge of the bounds instead.
            transform.position = m_environment.Bounds.ClosestPoint(new_position);
        }

        if (set_momentum)
        {
            m_dragMomentum = direction;
        }
    }

	public Vector3 screenToGroundPoint (Vector2 screen_pos)
	{
		return Camera.main.ScreenToWorldPoint (new Vector3 (screen_pos.x, screen_pos.y, Camera.main.transform.position.y));
	}
}
