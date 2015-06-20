using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Turret : MonoBehaviour
{
	public float turnSpeed;
	public GameObject bulletPrefab;
	public float bulletSpeed;

	protected bool animating = false;

	/// <summary>
	/// Makes the turret rotate to point at the specified position.
	/// </summary>
	/// <returns><c>true</c>, if turret is pointing at the position after the rotation, <c>false</c> otherwise.</returns>
	/// <param name="position">Position.</param>
	protected bool turnToward (Vector3 target_position)
	{
		Vector3 target_from_self = target_position - transform.position;
		float bearing = Vector3.Angle (transform.up, target_from_self);
        
		//Create the rotation we need to be in to look at the target
		Quaternion lookRotation = Quaternion.LookRotation (target_from_self.normalized, Vector3.up) * Quaternion.Euler (90, 0, 0);
        
		if (Mathf.Abs (bearing) < Time.deltaTime * turnSpeed) {
			transform.rotation = lookRotation;
			return true;
		} else {
			//Rotate over time according to speed until we are in the required rotation.
			transform.rotation = Quaternion.Slerp (transform.rotation, lookRotation, Time.deltaTime * (turnSpeed / bearing));
			return false;
		}
	}
    
	protected void fireAt (GameObject target)
	{
		//Spawn bullet
		GameObject new_bullet = Instantiate (bulletPrefab, transform.position, Quaternion.identity) as GameObject;
		new_bullet.GetComponent<BulletController> ().target = target.transform.position;
		new_bullet.GetComponent<BulletController> ().speed = bulletSpeed;
        
		//Animate recoil
		animating = true;
		Vector3 recoil_pos = Vector3.down;
		Hashtable itween_options = iTween.Hash (
            "amount", recoil_pos,
            "time", 0.5f,
            "oncomplete", "animationEnd"
		);
		iTween.PunchPosition (this.gameObject, itween_options);
	}
    
	public void animationEnd ()
	{
		animating = false;
	}
}
