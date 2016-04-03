using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{
	public GameObject Target;
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
        OnStateChange(m_gameManager.State, m_gameManager.State);
    }
	
	void OnDestroy ()
	{
		m_gameManager.NotifyStateChange -= OnStateChange;
	}
		
	void Update ()
	{
		switch (m_gameManager.State) {
		    case GameManager.GameState.Moving:
                if (Target == null)
                {
                    break;
                }

                //Determine where camera should focus.
			    transform.position = new Vector3 (Target.transform.position.x, transform.position.y, Target.transform.position.z);

			    if (RotationFollowsTarget) {
				    iTween.RotateUpdate (gameObject, Target.transform.rotation.eulerAngles, 2.0f);
			    }
			    break;

		    case GameManager.GameState.Planning:
			    if (m_dragMomentum.magnitude > 0) {
				    m_dragMomentum = iTween.Vector3Update (m_dragMomentum, Vector3.zero, DragMomentumFriction);
				    if (IsMomentumApplied) {
					    transform.position += m_dragMomentum;
				    }
			    }
			    break;
		}
	}

	void OnStateChange (GameManager.GameState old_state, GameManager.GameState new_state)
	{
        //Animate camera into position for state.
        Vector3 targetPosition;
        Hashtable tween_options;

        switch (new_state) {
		    case GameManager.GameState.MoveCountdown:
                targetPosition = new Vector3 (Target.transform.position.x, MovementY, Target.transform.position.z);

                tween_options = iTween.Hash(
                    "position", targetPosition,
                    "time", m_gameManager.MoveCountdownDuration,
                    "oncomplete", "MoveCountdownComplete",
                    "easetype", iTween.EaseType.easeInOutQuad
                );
                iTween.MoveTo(gameObject, tween_options);

                tween_options = iTween.Hash(
                    "rotation", Target.transform.rotation.eulerAngles,
                    "time", m_gameManager.MoveCountdownDuration,
                    "easetype", iTween.EaseType.easeInOutQuad
                );
                iTween.RotateTo(gameObject, tween_options);
                break;
			
			
		    case GameManager.GameState.PlanCountdown:
                targetPosition = new Vector3(Target.transform.position.x, PlanningY, Target.transform.position.z);

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
		m_gameManager.SetGameState (GameManager.GameState.Moving);
	}

    //Called when the camera is in position at the end of the planning countdown.
    public void PlanCountdownComplete()
    {
        m_gameManager.SetGameState(GameManager.GameState.Planning);
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

    public void Pan (Vector3 direction)
	{
		transform.position += direction;
		m_dragMomentum = direction;
	}

	public Vector3 screenToGroundPoint (Vector2 screen_pos)
	{
		return Camera.main.ScreenToWorldPoint (new Vector3 (screen_pos.x, screen_pos.y, Camera.main.transform.position.y));
	}
}
