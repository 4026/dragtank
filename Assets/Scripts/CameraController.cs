using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{
	public GameObject Target;
	public float PlanningZoom;
	public float MovementZoom;
	public float MoveCountdownDuration;
	public float DragMomentumFriction;

	private GameManager gameManager;
	private bool isDragging;
	private Vector3 dragOrigin;
	private Vector3 dragMomentum;
	
	void Awake ()
	{
		gameManager = GameManager.Instance;
		gameManager.NotifyStateChange += OnStateChange;
		OnStateChange (gameManager.gameState, gameManager.gameState);

		InputController.NotifyStartDrag += StartDrag;
		InputController.NotifyDragUpdate += UpdateDrag;
		InputController.NotifyStopDrag += EndDrag;
	}
	
	void OnDestroy ()
	{
		InputController.NotifyStartDrag -= StartDrag;
		InputController.NotifyDragUpdate -= UpdateDrag;
		InputController.NotifyStopDrag -= EndDrag;
	}
		
	void Update ()
	{
		switch (gameManager.gameState) {
		case GameState.Moving:
			//Determine where camera should focus.
			transform.position = new Vector3 (Target.transform.position.x, transform.position.y, Target.transform.position.z);

			if (!isDragging) {
				iTween.RotateUpdate (gameObject, Target.transform.rotation.eulerAngles, 2.0f);
			}
			break;

		case GameState.Planning:
			if (!isDragging) {
				if (dragMomentum.magnitude > 0) {
					dragMomentum = iTween.Vector3Update (dragMomentum, Vector3.zero, DragMomentumFriction);
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

	void StartDrag (Vector2 screen_pos)
	{
		isDragging = true;

		if (gameManager.gameState == GameState.Planning) {
			dragOrigin = Camera.main.ScreenToWorldPoint (new Vector3 (screen_pos.x, screen_pos.y, Camera.main.transform.position.y));
		}
	}
	
	void UpdateDrag (Vector2 screen_pos)
	{
		if (gameManager.gameState == GameState.Planning) {
			Vector3 newDragWorldPos = Camera.main.ScreenToWorldPoint (new Vector3 (screen_pos.x, screen_pos.y, Camera.main.transform.position.y));
			Vector3 dragDirection = newDragWorldPos - dragOrigin;
			transform.position -= dragDirection;
			dragMomentum = -dragDirection;
		}
	}
	
	void EndDrag (Vector2 screen_pos)
	{
		isDragging = false;
	}
}
