using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class TurretController : MonoBehaviour
{

    public GameObject player;
    public float turnSpeed;
    public GameObject bulletPrefab;
    public float bulletSpeed;

    private List<GameObject> targets = new List<GameObject>();
    private GameObject dragged_target;
    private bool animating = false;


    void Update()
    {

        //Follow tank around
        transform.position = new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z);

        if (!animating && (targets.Count > 0 || dragged_target != null))
        {
            GameObject target = (targets.Count > 0) ? targets.First() : dragged_target;

            Vector3 target_from_self = target.transform.position - transform.position;

            float bearing = Vector3.Angle(transform.up, target_from_self);

            //Create the rotation we need to be in to look at the target
            Quaternion lookRotation = Quaternion.LookRotation(target_from_self.normalized, Vector3.up) * Quaternion.Euler(90, 0, 0);


            if (Mathf.Abs(bearing) < Time.deltaTime * turnSpeed)
            {
                transform.rotation = lookRotation;
                if (targets.Count > 0)
                {
                    fireAt(target);
                }
            }
            else
            {
                //rotate us over time according to speed until we are in the required rotation
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * (turnSpeed / bearing));
            }
        }

    }

    public void setDraggedTarget(GameObject target)
    {
        dragged_target = target;
    }

    public void addTarget(GameObject target)
    {
        targets.Add(target);
    }

    private void fireAt(GameObject target)
    {
        //Spawn bullet
        GameObject new_bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity) as GameObject;
        new_bullet.GetComponent<BulletController>().target = target.transform.position;
        new_bullet.GetComponent<BulletController>().speed = bulletSpeed;

        //Animate recoil
        animating = true;
        Vector3 recoil_pos = Vector3.down;
        Hashtable itween_options = iTween.Hash(
            "amount", recoil_pos,
            "time", 0.5f,
            "oncomplete", "animationEnd"
        );
        iTween.PunchPosition(this.gameObject, itween_options);

        //Remove target indicator
        targets.Remove(target);
        GameObject.Destroy(target);
    }

    public void animationEnd()
    {
        animating = false;
    }
}
