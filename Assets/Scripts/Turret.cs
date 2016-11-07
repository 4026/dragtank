using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Turret : MonoBehaviour
{
	public float TurnSpeed;
	public GameObject BulletPrefab;
	public float BulletSpeed;
    public float FirePeriod;

    public bool CanFire
    {
        get { return m_timeToNextShot <= 0; }
    }

    protected bool animating = false;

    public float m_timeToNextShot = 0;

    public virtual void Update()
    {
        if (GameManager.Instance.State == GameManager.GameState.Moving && m_timeToNextShot > 0)
        {
            m_timeToNextShot -= Time.deltaTime;
        }
    }


    /// <summary>
    /// Makes the turret rotate to point at the specified position.
    /// </summary>
    /// <returns><c>true</c>, if turret is pointing at the position after the rotation, <c>false</c> otherwise.</returns>
    protected bool turnToward (Vector3 target_position)
	{
		Vector3 target_from_self = target_position - transform.position;
		float bearing = Vector3.Angle (transform.up, target_from_self);
        
		//Create the rotation we need to be in to look at the target
		Quaternion lookRotation = Quaternion.LookRotation (target_from_self.normalized, Vector3.up) * Quaternion.Euler (90, 0, 0);
        
		if (Mathf.Abs (bearing) < Time.deltaTime * TurnSpeed) {
			transform.rotation = lookRotation;
			return true;
		} else {
			//Rotate over time according to speed until we are in the required rotation.
			transform.rotation = Quaternion.Slerp (transform.rotation, lookRotation, Time.deltaTime * (TurnSpeed / bearing));
			return false;
		}
	}
    
	protected void fireAt (GameObject target)
	{
		//Spawn bullet
		GameObject new_bullet = Instantiate (BulletPrefab, transform.position, Quaternion.identity) as GameObject;
        BulletController new_bullet_data = new_bullet.GetComponent<BulletController>();

        new_bullet_data.Target = target.transform.position;
        new_bullet_data.Speed = BulletSpeed;
        new_bullet_data.FiredBy = transform.parent.gameObject;

        //Animate recoil
        animating = true;
		Vector3 recoil_pos = Vector3.down;
		Hashtable itween_options = iTween.Hash (
            "amount", recoil_pos,
            "time", 0.5f,
            "oncomplete", "animationEnd"
		);
		iTween.PunchPosition (this.gameObject, itween_options);

        m_timeToNextShot = FirePeriod;
	}
    
	public void animationEnd ()
	{
		animating = false;
	}
}
