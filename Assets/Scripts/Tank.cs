using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Tank : MonoBehaviour
{
	public float MaxForwardSpeed;
	public float MaxReverseSpeed;
	public float Acceleration;
	public float Deceleration;
	public float TurnSpeed;
	public float TurnAcceleration;
	public float ReverseWhenTargetBearingGreaterThan;

	private float lastSpeed = 0;
	private float lastTurnSpeed = 0;

	public void MoveToward (Vector3 targetPosition)
	{
		Vector3 targetDirection = targetPosition - transform.position;
		float targetBearing = Vector3.Angle (transform.forward, targetDirection);

		//Decide which way we want to face.
		Quaternion faceRotation;
        if (targetDirection == Vector3.zero)
        {
            //If we're on top of the target, don't change facing at all.
            faceRotation = transform.rotation;
        } else if (targetBearing <= ReverseWhenTargetBearingGreaterThan) {
			//If the target isn't directly behind us, we want to aim to face it.
			faceRotation = Quaternion.LookRotation (targetDirection.normalized, Vector3.up);
		} else {
			//Otherwise, we want to face directly away from it, and reverse to it.
			faceRotation = Quaternion.LookRotation (-targetDirection.normalized, Vector3.up);
		}

		//Turn to face desired direction.
		float maxPossibleTurnSpeed = Mathf.Clamp (lastTurnSpeed + (TurnAcceleration * Time.deltaTime), 0, TurnSpeed);
		if (Mathf.Abs (targetBearing) < Time.deltaTime * maxPossibleTurnSpeed) {
			transform.rotation = faceRotation;
			lastTurnSpeed = 0;
		} else {
			//Rotate over time according to speed until we are in the required rotation.
			transform.rotation = Quaternion.Slerp (transform.rotation, faceRotation, Time.deltaTime * (maxPossibleTurnSpeed / targetBearing));
			lastTurnSpeed = maxPossibleTurnSpeed;
		}


		//Decide how to move...
		if (targetBearing < 60) {
			// Move forwards, if the target is roughly ahead of us.
			float maxPossibleSpeed = Mathf.Clamp (lastSpeed + (Acceleration * Time.deltaTime), -MaxReverseSpeed, MaxForwardSpeed);
			transform.position += transform.forward * maxPossibleSpeed * Time.deltaTime;
			lastSpeed = maxPossibleSpeed;
		} else if (targetBearing > 120) {
			//Move backwards, if the target is roughly behind us.
			float minPossibleSpeed = lastSpeed > 0 ? lastSpeed - (Deceleration * Time.deltaTime) : lastSpeed - (Acceleration * Time.deltaTime);
			minPossibleSpeed = Mathf.Clamp (minPossibleSpeed, -MaxReverseSpeed, MaxForwardSpeed);
			transform.position += transform.forward * minPossibleSpeed * Time.deltaTime;
			lastSpeed = minPossibleSpeed;
		} else {
			//Slow to a stop if the target is to our side, while we turn to face it.
			SlowToStop ();
		}
	}

	public void SlowToStop ()
	{
		//Slow the tank to a stop.
		float minPossibleSpeed = lastSpeed > 0 ? lastSpeed - (Deceleration * Time.deltaTime) : lastSpeed + (Deceleration * Time.deltaTime);
		transform.position += transform.forward * minPossibleSpeed * Time.deltaTime;
		lastSpeed = minPossibleSpeed;
	}
}

