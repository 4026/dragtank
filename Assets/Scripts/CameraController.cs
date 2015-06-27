using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{
	public GameObject Target;
	public float PlanningZoom;
	public float MovementZoom;
	public float MoveCountdownDuration;
	public float DragMomentumFriction;

	public bool IsMomentumApplied { get; set; }
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
		Hashtable tweenOptions;

		switch (new_state) {
		case GameState.MoveCountdown:
			Vector3 targetPosition = new Vector3 (Target.transform.position.x, transform.position.y, Target.transform.position.z);
			iTween.MoveTo (gameObject, targetPosition, MoveCountdownDuration);
			iTween.RotateTo (gameObject, Target.transform.rotation.eulerAngles, MoveCountdownDuration);
			tweenOptions = iTween.Hash (
				"from", Camera.main.orthographicSize, 
				"to", MovementZoom,
				"time", MoveCountdownDuration,
				"onupdate", "SetZoom",
				"oncomplete", "MoveComplete"
			);
			iTween.ValueTo (gameObject, tweenOptions);

			break;
			
			
		case GameState.Planning:
			iTween.RotateTo (gameObject, new Vector3 (90, 0, 0), MoveCountdownDuration);
			tweenOptions = iTween.Hash (
				"from", Camera.main.orthographicSize, 
				"to", PlanningZoom,
				"time", MoveCountdownDuration,
				"onupdate", "SetZoom"
			);
			iTween.ValueTo (gameObject, tweenOptions);

			break;
		}
	}

	public void SetZoom (float newZoom)
	{
		Camera.main.orthographicSize = newZoom;
	}

	//Called when the camera is in position.
	public void MoveComplete ()
	{
		gameManager.SetGameState (GameState.Moving);
	}

	public void Pan (Vector3 direction)
	{
		transform.position += direction;
		dragMomentum = direction;
	}
}
