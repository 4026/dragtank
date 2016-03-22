using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{
	public GameObject Target;
    public float PlanningY;
    public float MovementY;
	public float MoveCountdownDuration;
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

	private GameManager gameManager;
	private Vector3 dragMomentum;

	void Awake ()
	{
		gameManager = GameManager.Instance;
		gameManager.NotifyStateChange += OnStateChange;
		OnStateChange (gameManager.gameState, gameManager.gameState);

		RotationFollowsTarget = true;
		IsMomentumApplied = true;
	}
	
	void OnDestroy ()
	{
		gameManager.NotifyStateChange -= OnStateChange;
	}
		
	void Update ()
	{
		switch (gameManager.gameState) {
		case GameState.Moving:
			//Determine where camera should focus.
			transform.position = new Vector3 (Target.transform.position.x, transform.position.y, Target.transform.position.z);

			if (RotationFollowsTarget) {
				iTween.RotateUpdate (gameObject, Target.transform.rotation.eulerAngles, 2.0f);
			}
			break;

		case GameState.Planning:
			if (dragMomentum.magnitude > 0) {
				dragMomentum = iTween.Vector3Update (dragMomentum, Vector3.zero, DragMomentumFriction);
				if (IsMomentumApplied) {
					transform.position += dragMomentum;
				}
			}
			break;
		}
	}

	void OnStateChange (GameState old_state, GameState new_state)
	{
        //Animate camera into position for state.
        Vector3 targetPosition;
        Hashtable tween_options;

        switch (new_state) {
		    case GameState.MoveCountdown:
                targetPosition = new Vector3 (Target.transform.position.x, MovementY, Target.transform.position.z);

                tween_options = iTween.Hash(
                    "position", targetPosition,
                    "time", MoveCountdownDuration,
                    "oncomplete", "MoveComplete",
                    "easetype", iTween.EaseType.easeInOutQuad
                );
                iTween.MoveTo(gameObject, tween_options);

                tween_options = iTween.Hash(
                    "rotation", Target.transform.rotation.eulerAngles,
                    "time", MoveCountdownDuration,
                    "easetype", iTween.EaseType.easeInOutQuad
                );
                iTween.RotateTo(gameObject, tween_options);
                break;
			
			
		    case GameState.Planning:
                targetPosition = new Vector3(Target.transform.position.x, PlanningY, Target.transform.position.z);

                tween_options = iTween.Hash(
                    "position", targetPosition,
                    "time", MoveCountdownDuration,
                    "easetype", iTween.EaseType.easeInOutQuad
                );
                iTween.MoveTo(gameObject, tween_options);

                tween_options = iTween.Hash(
                    "rotation", new Vector3(90, 0, 0),
                    "time", MoveCountdownDuration,
                    "easetype", iTween.EaseType.easeInOutQuad
                );
                iTween.RotateTo (gameObject, tween_options);

                       
			    break;
		}
	}

	//Called when the camera is in position.
	public void MoveComplete ()
	{
		gameManager.SetGameState (GameState.Moving);
	}

    public void SetY(float new_y)
    {
        transform.position = new Vector3(transform.position.x, new_y, transform.position.z);
    }

    public void Pan (Vector3 direction)
	{
		transform.position += direction;
		dragMomentum = direction;
	}

	public Vector3 screenToGroundPoint (Vector2 screen_pos)
	{
		return Camera.main.ScreenToWorldPoint (new Vector3 (screen_pos.x, screen_pos.y, Camera.main.transform.position.y));
	}
}
