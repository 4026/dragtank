using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class MapDragController : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
	public CameraController ControlledCamera;
	public float MinZoom = 3;        // The rate of change of the field of view in perspective mode.
	public float MaxZoom = 30;        // The rate of change of the orthographic size in orthographic mode.

	private GameManager gameManager;

	private Vector3 dragOrigin;
	private bool dragIsSingleTouch;

	private bool isPinching = false;
	private float startingPinchScreenDistance;
	private Vector3 pinchWorldCenter;

	private float cameraSizeOrigin;
	private Vector3 cameraPosOrigin;

	void Awake ()
	{
		gameManager = GameManager.Instance;
	}

	public void Update ()
	{
		if (Input.touchCount != 2 || gameManager.gameState != GameState.Planning) {
			if (isPinching) {
				isPinching = false;
				ControlledCamera.IsMomentumApplied = true;
			}
		} else {
			// If there are two touches on the device...
			Touch touchZero = Input.GetTouch (0);
			Touch touchOne = Input.GetTouch (1);

			if (!isPinching) {
				isPinching = true;
				ControlledCamera.IsMomentumApplied = false;

				//Get the starting distance between the two touches in screen co-ordinates.
				startingPinchScreenDistance = Vector2.Distance (touchZero.position, touchOne.position);

				//Find the starting center of the pinch in world co-ordinates. We're aiming to arrange the camera to that this remains the centre of the pinch, regardless of how the touches move.
				pinchWorldCenter = Vector3.Lerp (ControlledCamera.screenToGroundPoint (touchZero.position), ControlledCamera.screenToGroundPoint (touchOne.position), 0.5f);

				cameraSizeOrigin = Camera.main.orthographicSize;
				cameraPosOrigin = Camera.main.transform.position;
			}

			// Find the the distance between the touches in this frame.
			float pinchScreenDistance = Vector2.Distance (touchZero.position, touchOne.position);
			
			// Find the ratio of the starting pinch distance to the current pinch distance.
			float invDistanceMultiplier = startingPinchScreenDistance / pinchScreenDistance;

			// Change the orthographic size based on the change in distance between the touches.
			Camera.main.orthographicSize = Mathf.Clamp (cameraSizeOrigin * invDistanceMultiplier, MinZoom, MaxZoom);

			//Get the world positions of the two touches after zooming.
			Vector3 touchZeroWorldPos = ControlledCamera.screenToGroundPoint (touchZero.position);
			Vector3 touchOneWorldPos = ControlledCamera.screenToGroundPoint (touchOne.position);

			//Calulate the new pinch center in world space.
			Vector3 newPinchCenter = Vector3.Lerp (touchZeroWorldPos, touchOneWorldPos, 0.5f);

			// Pan the camera to compensate for any movement of the pinch center.
			ControlledCamera.Pan (pinchWorldCenter - newPinchCenter);
		}
	}
	
	public void OnBeginDrag (PointerEventData eventData)
	{
		if (Input.touchCount > 1) {
			return;
		}
		dragIsSingleTouch = true;
		dragOrigin = ControlledCamera.screenToGroundPoint (eventData.position);
		ControlledCamera.IsMomentumApplied = false;
		ControlledCamera.RotationFollowsTarget = false;
	}
	
	public void OnDrag (PointerEventData eventData)
	{
		if (Input.touchCount > 1 || !dragIsSingleTouch) {
			dragIsSingleTouch = false;
			ControlledCamera.IsMomentumApplied = true;
			ControlledCamera.RotationFollowsTarget = true;
			return;
		}

		switch (gameManager.gameState) {
		case GameState.Planning:
			Vector3 newDragWorldPos = ControlledCamera.screenToGroundPoint (eventData.position);
			Vector3 dragDirection = newDragWorldPos - dragOrigin;
			ControlledCamera.Pan (-dragDirection);
			break;

		case GameState.Moving:
			break;
		}


	}
	
	public void OnEndDrag (PointerEventData eventData)
	{
		if (dragIsSingleTouch) {
			ControlledCamera.IsMomentumApplied = true;
			ControlledCamera.RotationFollowsTarget = true;
		}
	}
}
