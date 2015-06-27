using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class MapDragController : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
	public CameraController ControlledCamera;

	private GameManager gameManager;
	private Vector3 dragOrigin;

	void Awake ()
	{
		gameManager = GameManager.Instance;
	}
	
	public void OnBeginDrag (PointerEventData eventData)
	{
		dragOrigin = Camera.main.ScreenToWorldPoint (new Vector3 (eventData.position.x, eventData.position.y, Camera.main.transform.position.y));
		ControlledCamera.IsMomentumApplied = false;
		ControlledCamera.RotationFollowsTarget = false;
	}
	
	public void OnDrag (PointerEventData eventData)
	{
		switch (gameManager.gameState) {
		case GameState.Planning:
			Vector3 newDragWorldPos = Camera.main.ScreenToWorldPoint (new Vector3 (eventData.position.x, eventData.position.y, Camera.main.transform.position.y));
			Vector3 dragDirection = newDragWorldPos - dragOrigin;
			ControlledCamera.Pan (-dragDirection);
			break;

		case GameState.Moving:
			break;
		}


	}
	
	public void OnEndDrag (PointerEventData eventData)
	{
		ControlledCamera.IsMomentumApplied = true;
		ControlledCamera.RotationFollowsTarget = true;
	}
}
